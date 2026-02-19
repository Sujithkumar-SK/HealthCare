using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PVHealth.Common.DTOs;
using PVHealth.Data.Context;

namespace PVHealth.Business.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;
    private readonly HttpClient _httpclient;

    public AuthService(ApplicationDbContext context, IConfiguration config, IHttpClientFactory httpclient)
    {
        _config = config;
        _context = context;
        _httpclient = httpclient.CreateClient();
    }

    public async Task<AuthResponse> AuthenticateWithGoogleAsync(string idToken)
    {
        _httpclient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);
        
        var response = await _httpclient.GetStringAsync("https://www.googleapis.com/oauth2/v3/userinfo");
        var googleUser = JsonSerializer.Deserialize<GoogleUserInfo>(response);

        if (googleUser == null || string.IsNullOrEmpty(googleUser.Email))
            throw new Exception("Invalid Google token");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == googleUser.Email);

        if (user == null)
        {
            user = new Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                Email = googleUser.Email,
                Name = googleUser.Name ?? googleUser.Email,
                OAuthProvider = "Google",
                OAuthId = googleUser.Sub!,
                ProfilePicture = googleUser.Picture,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        var token = GenerateJwtToken(user.Id, user.Email, user.Name);

        return new AuthResponse
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                ProfilePicture = user.ProfilePicture
            },
            RequiresMfa = user.MfaEnabled
        };
    }

    public string GenerateJwtToken(Guid userId, string email, string name)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Name, name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<AuthResponse> AuthenticateWithGitHubAsync(string code)
    {
        var clientId = _config["GitHub:ClientId"];
        var clientSecret = _config["GitHub:ClientSecret"];

        var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
        tokenRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = clientId!,
            ["client_secret"] = clientSecret!,
            ["code"] = code
        });
        tokenRequest.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var tokenResponse = await _httpclient.SendAsync(tokenRequest);
        var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
        var tokenData = JsonSerializer.Deserialize<GitHubTokenResponse>(tokenContent);

        if (string.IsNullOrEmpty(tokenData?.AccessToken))
            throw new Exception("Failed to get GitHub access token");

        _httpclient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenData.AccessToken);
        _httpclient.DefaultRequestHeaders.UserAgent.ParseAdd("PVHealth");

        var userResponse = await _httpclient.GetStringAsync("https://api.github.com/user");
        var githubUser = JsonSerializer.Deserialize<GitHubUserInfo>(userResponse);

        if (githubUser == null || string.IsNullOrEmpty(githubUser.Email))
        {
            var emailResponse = await _httpclient.GetStringAsync("https://api.github.com/user/emails");
            var emails = JsonSerializer.Deserialize<List<GitHubEmail>>(emailResponse);
            githubUser!.Email = emails?.FirstOrDefault(e => e.Primary)?.EmailAddress ?? emails?.FirstOrDefault()?.EmailAddress;
        }

        if (string.IsNullOrEmpty(githubUser?.Email))
            throw new Exception("Unable to retrieve email from GitHub");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == githubUser.Email);

        if (user == null)
        {
            user = new Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                Email = githubUser.Email,
                Name = githubUser.Name ?? githubUser.Login ?? githubUser.Email,
                OAuthProvider = "GitHub",
                OAuthId = githubUser.Id.ToString(),
                ProfilePicture = githubUser.AvatarUrl,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        var token = GenerateJwtToken(user.Id, user.Email, user.Name);

        return new AuthResponse
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                ProfilePicture = user.ProfilePicture
            },
            RequiresMfa = user.MfaEnabled
        };
    }

    private class GitHubTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
    }

    private class GitHubUserInfo
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("login")]
        public string? Login { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("avatar_url")]
        public string? AvatarUrl { get; set; }
    }

    private class GitHubEmail
    {
        [JsonPropertyName("email")]
        public string? EmailAddress { get; set; }

        [JsonPropertyName("primary")]
        public bool Primary { get; set; }
    }

    private class GoogleUserInfo
    {
        [JsonPropertyName("sub")]
        public string? Sub { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("picture")]
        public string? Picture { get; set; }
    }
}