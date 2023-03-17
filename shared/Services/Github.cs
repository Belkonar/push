using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using shared.Models;

namespace shared.Services;

// TODO: Support caching of tokens if it becomes a problem
public class Github : IGithub
{
    private readonly HttpClient _client;
    private readonly Dictionary<string, GithubConfig> _configs;

    public Github(IConfiguration configuration, IHttpClientFactory factory)
    {
        _configs = configuration.GetSection("Github").Get<Dictionary<string, GithubConfig>>()!;
        _client = factory.CreateClient("github");
    }

    private GithubConfig GetConfig(string source)
    {
        var uri = new Uri(source);
        if (!_configs.ContainsKey(uri.Host))
        {
            throw new Exception($"missing `Github` key {uri.Host}");
        }

        return _configs[uri.Host];
    }
    
    public async Task<byte[]> GetZip(string source, string reference)
    {
        var githubConfig = GetConfig(source);
        var githubRoot = githubConfig.ApiRoot;

        var parsedSource = ParseSource(source);
        
        var org = parsedSource.Item1;
        var repo = parsedSource.Item2;

        var token = await GetToken(githubConfig, org);

        using var request = new HttpRequestMessage()
        {
            RequestUri = new Uri($"{githubRoot}/repos/{org}/{repo}/zipball/{reference}"),
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("token", token),
                UserAgent = {new ProductInfoHeaderValue("Deployer", "1.0")}
            }
        };

        using var response = await _client.SendAsync(request);
        
        return await response.Content.ReadAsByteArrayAsync();
    }

    private Tuple<string, string> ParseSource(string source)
    {
        var parts = source.Split("/");
        var partsCount = parts.Length;

        return new Tuple<string, string>(
            parts[partsCount - 2],
            parts[partsCount - 1]
        );
    }

    public async Task<string> GetReference(string source, string reference)
    {
        var githubConfig = GetConfig(source);
        var githubRoot = githubConfig.ApiRoot;

        var parsedSource = ParseSource(source);
        
        var org = parsedSource.Item1;
        var repo = parsedSource.Item2;

        var token = await GetToken(githubConfig, org);
        
        using var request = new HttpRequestMessage()
        {
            RequestUri = new Uri($"{githubRoot}/repos/{org}/{repo}/commits/{reference}"),
            Method = HttpMethod.Get,
            Headers =
            {
                Accept = {new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json")},
                Authorization = new AuthenticationHeaderValue("token", token),
                UserAgent = {new ProductInfoHeaderValue("Deployer", "1.0")}
            }
        };
    
        using var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var responseData = await response.Content.ReadFromJsonAsync<JsonDocument>();
    
        return responseData?.RootElement.GetProperty("sha").GetString() ?? "";
    }

    private async Task<string> GetToken(GithubConfig config, string org)
    {
        var token = await GetJwt(config);
        
        var tokenUrl = await GetTokenUrl(config, token, org);

        var tokenResponse = await GetTokenResponse(tokenUrl, token);

        return tokenResponse!.Token;
    }

    private async Task<GithubToken?> GetTokenResponse(string tokenUrl, string token)
    {
        using var request = new HttpRequestMessage()
        {
            RequestUri = new Uri(tokenUrl),
            Method = HttpMethod.Post,
            Headers =
            {
                Accept = {new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json")},
                Authorization = new AuthenticationHeaderValue("bearer", token),
                UserAgent = {new ProductInfoHeaderValue("Deployer", "1.0")}
            }
        };

        using var response = await _client.SendAsync(request);

        var stringContent = await response.Content.ReadAsStringAsync();
        
        Console.WriteLine(stringContent);
        
        var tokenResponse = JsonSerializer.Deserialize<GithubToken>(stringContent);
        return tokenResponse;
    }

    private async Task<string> GetTokenUrl(GithubConfig config, string token, string org)
    {
        using var request = new HttpRequestMessage()
        {
            RequestUri = new Uri($"{config.ApiRoot}/app/installations"),
            Headers =
            {
                Accept = {new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json")},
                Authorization = new AuthenticationHeaderValue("bearer", token),
                UserAgent = {new ProductInfoHeaderValue("Deployer", "1.0")}
            }
        };

        using var response = await _client.SendAsync(request);

        var stringContent = await response.Content.ReadAsStringAsync();
        var installations = JsonSerializer.Deserialize<List<GithubInstallation>>(stringContent);

        var tokenUrl = installations!.FirstOrDefault(x =>
            x.TargetType == "Organization" &&
            x.Account.Login == org)?.AccessTokenUrl;

        if (tokenUrl == null)
        {
            throw new Exception("no token url");
        }

        return tokenUrl;
    }

    /// <summary>
    /// ref https://vmsdurano.com/-net-core-3-1-signing-jwt-with-rsa/
    /// </summary>
    /// <returns></returns>
    private async Task<string> GetJwt(GithubConfig config)
    {
        var key = await ParsePem(config.PemLocation ?? Path.GetFullPath("github.pem"));
        
        using RSA rsa = RSA.Create();
        
        rsa.ImportRSAPrivateKey(key, out _);

        var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
        };
        
        var now = DateTime.Now;
        var unixTimeSeconds = new DateTimeOffset(now).ToUnixTimeSeconds();

        var jwt = new JwtSecurityToken(
            issuer: config.AppId,
            claims: new [] {
                new Claim(JwtRegisteredClaimNames.Iat, (unixTimeSeconds - 60).ToString(), ClaimValueTypes.Integer64),
            },
            expires: now.AddMinutes(10),
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    // TODO: Accept AWS secrets manager to return the file
    private async Task<byte[]> ParsePem(string getFullPath)
    {
        var contents = await File.ReadAllTextAsync(getFullPath);
        var base64Parts = contents.Split('\n')
            .ToList()
            .Select(x => x.Trim())
            .Where(x => !x.StartsWith("-"))
            .ToList();

        var base64 = string.Join("", base64Parts)
            .Trim();
        
        return Convert.FromBase64String(base64);
    }
}