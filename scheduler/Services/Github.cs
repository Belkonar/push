using scheduler.Models;

namespace scheduler.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

// lol so I yoinked this from my first (second?) POC.
public class Github
{
    private readonly IConfiguration _configuration;

    public Github(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<byte[]> GetReference(string source, string reference)
    {
        var githubRoot = _configuration["githubApiRoot"] ?? "https://api.github.com";
        var token = await GetToken();

        var org = "Troll-Cave";
        var repo = "";

        var parts = source.Split("/");
        var partsCount = parts.Length;
        org = parts[partsCount - 2];
        repo = parts[partsCount - 1];
        
        var tokenUrl = await GetTokenUrl(token, org);

        var tokenResponse = await GetTokenResponse(tokenUrl, token);

        using var client = new HttpClient();
        using var request = new HttpRequestMessage()
        {
            RequestUri = new Uri($"{githubRoot}/repos/{org}/{repo}/zipball/{reference}"),
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("token", tokenResponse.Token),
                UserAgent = {new ProductInfoHeaderValue("Deployer", "1.0")}
            }
        };

        using var response = await client.SendAsync(request);
        
        return await response.Content.ReadAsByteArrayAsync();
    }

    private async Task<GithubToken?> GetTokenResponse(string? tokenUrl, string token)
    {
        using var client = new HttpClient();
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

        using var response = await client.SendAsync(request);

        var stringContent = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<GithubToken>(stringContent);
        return tokenResponse;
    }

    private async Task<string?> GetTokenUrl(string token, string org)
    {
        using var client = new HttpClient();
        using var request = new HttpRequestMessage()
        {
            RequestUri = new Uri($"{_configuration["githubApiRoot"]}/app/installations"),
            Headers =
            {
                Accept = {new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json")},
                Authorization = new AuthenticationHeaderValue("bearer", token),
                UserAgent = {new ProductInfoHeaderValue("Deployer", "1.0")}
            }
        };

        using var response = await client.SendAsync(request);

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
            issuer: _configuration["githubAppId"],
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