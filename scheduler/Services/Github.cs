using System.Net.Http.Json;
using scheduler.Models;

namespace scheduler.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

// TODO: Refactor this.
// It'll currently support multiple github's at the same time, but I need it cleaner
// for the later integrations.
public class Github
{
    private readonly HttpClient _client;
    private readonly Dictionary<string, GithubConfig> _configs;
    private GithubConfig? _config;

    public Github(IConfiguration configuration, IHttpClientFactory factory)
    {
        _configs = configuration.GetSection("Github").Get<Dictionary<string, GithubConfig>>()!;
        _client = factory.CreateClient("github");
    }

    public void Setup(string source)
    {
        var uri = new Uri(source);
        if (!_configs.ContainsKey(uri.Host))
        {
            throw new Exception($"missing `Github` key {uri.Host}");
        }

        _config = _configs[uri.Host];
    }
    
    public async Task<byte[]> GetReference(string source, string reference)
    {
        var githubRoot = _config!.ApiRoot;
        var token = await GetToken();

        var org = "Troll-Cave";
        var repo = "";

        var parts = source.Split("/");
        var partsCount = parts.Length;
        org = parts[partsCount - 2];
        repo = parts[partsCount - 1];
        
        var tokenUrl = await GetTokenUrl(token, org);

        var tokenResponse = await GetTokenResponse(tokenUrl, token);

        string actualReference = await GetActualReference(reference, tokenResponse?.Token ?? "", org, repo);
        
        Console.WriteLine($"ref: {actualReference}");

        using var request = new HttpRequestMessage()
        {
            RequestUri = new Uri($"{githubRoot}/repos/{org}/{repo}/zipball/{actualReference}"),
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("token", tokenResponse.Token),
                UserAgent = {new ProductInfoHeaderValue("Deployer", "1.0")}
            }
        };

        using var response = await _client.SendAsync(request);
        
        return await response.Content.ReadAsByteArrayAsync();
    }

    private async Task<string> GetActualReference(string reference, string token, string owner, string repo)
    {
        using var request = new HttpRequestMessage()
        {
            RequestUri = new Uri($"{_config!.ApiRoot}/repos/{owner}/{repo}/commits/{reference}"),
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

    private async Task<GithubToken?> GetTokenResponse(string? tokenUrl, string token)
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
        var tokenResponse = JsonSerializer.Deserialize<GithubToken>(stringContent);
        return tokenResponse;
    }

    private async Task<string?> GetTokenUrl(string token, string org)
    {
        using var request = new HttpRequestMessage()
        {
            RequestUri = new Uri($"{_config!.ApiRoot}/app/installations"),
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
    private async Task<string> GetToken()
    {
        var key = await ParsePem(Path.GetFullPath("github.pem"));
        
        using RSA rsa = RSA.Create();
        
        rsa.ImportRSAPrivateKey(key, out _);

        var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
        };
        
        var now = DateTime.Now;
        var unixTimeSeconds = new DateTimeOffset(now).ToUnixTimeSeconds();

        var jwt = new JwtSecurityToken(
            issuer: _config!.AppId,
            claims: new Claim[] {
                new Claim(JwtRegisteredClaimNames.Iat, (unixTimeSeconds - 60).ToString(), ClaimValueTypes.Integer64),
            },
            expires: now.AddMinutes(10),
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

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