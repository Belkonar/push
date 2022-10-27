using System.Text.Json;
using api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace api.Services;

public class UserService
{
    private readonly Auth0Service _auth0Service;
    public string Subject { get; set; }
    public JsonDocument Profile { get; set; }

    public UserService(Auth0Service auth0Service)
    {
        _auth0Service = auth0Service;
    }

    public async Task SetupUser(string authHeader)
    {
        Console.WriteLine($"auth token {authHeader}");
        /*
         * So here's what I need to do.
         * Extract the subject, scopes and issuer out of the header
         * if profile scope exists, hit the profile endpoint and extract
         * profile. Otherwise use access token as profile.
         */

        // TODO: Make this more robust
        var profileBase64 = authHeader?.Split('.')?[1] ?? "";
        
        if (string.IsNullOrWhiteSpace(profileBase64))
        {
            // nothing to see here
            return;
        }
        
        var profileJson = Base64UrlEncoder.Decode(profileBase64);
        var profile = JsonSerializer.Deserialize<JsonDocument>(profileJson);
        
        Subject = profile.RootElement.GetString("sub");
        var scopes = profile.RootElement.GetString("scope");
        
        if (scopes.Contains("profile"))
        {
            // TODO: Find out thwy the issuer is missing from the token
            profile = await _auth0Service.GetProfile(authHeader);
        }
        
        Profile = profile;
    }
}