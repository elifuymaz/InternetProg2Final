using System.Net.Http.Headers;
using System.Text.Json;
using emlak.api.DTOs;
using emlak.api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace emlak.api.Services
{
    public interface IGitHubAuthService
    {
        Task<AuthResponseDto> AuthenticateGitHubUser(string code);
    }

    public class GitHubAuthService : IGitHubAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthService _authService;
        private readonly HttpClient _httpClient;

        public GitHubAuthService(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            IAuthService authService)
        {
            _configuration = configuration;
            _userManager = userManager;
            _authService = authService;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "EmlakPortal");
        }

        public async Task<AuthResponseDto> AuthenticateGitHubUser(string code)
        {
            try
            {
                var clientId = _configuration["GitHub:ClientId"];
                var clientSecret = _configuration["GitHub:ClientSecret"];
                var callbackUrl = _configuration["GitHub:CallbackUrl"];

                // Exchange code for access token
                var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = clientId,
                    ["client_secret"] = clientSecret,
                    ["code"] = code,
                    ["redirect_uri"] = callbackUrl
                });

                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var tokenResponse = await _httpClient.PostAsync("https://github.com/login/oauth/access_token", tokenRequest);
                var tokenContent = await tokenResponse.Content.ReadAsStringAsync();

                if (!tokenResponse.IsSuccessStatusCode)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = $"GitHub token exchange failed: {tokenContent}"
                    };
                }

                var tokenData = JsonSerializer.Deserialize<Dictionary<string, string>>(tokenContent);
                if (!tokenData.ContainsKey("access_token"))
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "No access token received from GitHub"
                    };
                }

                var accessToken = tokenData["access_token"];

                // Get user email
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var emailResponse = await _httpClient.GetAsync("https://api.github.com/user/emails");
                var emailContent = await emailResponse.Content.ReadAsStringAsync();
                var emails = JsonSerializer.Deserialize<List<GitHubEmail>>(emailContent);
                var primaryEmail = emails.FirstOrDefault(e => e.primary)?.email;

                if (string.IsNullOrEmpty(primaryEmail))
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "No primary email found in GitHub account"
                    };
                }

                // Find or create user
                var user = await _userManager.FindByEmailAsync(primaryEmail);
                if (user == null)
                {
                    // Generate username from email
                    var username = primaryEmail.Split('@')[0];
                    var counter = 1;
                    while (await _userManager.FindByNameAsync(username) != null)
                    {
                        username = $"{primaryEmail.Split('@')[0]}{counter}";
                        counter++;
                    }

                    user = new ApplicationUser
                    {
                        UserName = username,
                        Email = primaryEmail,
                        EmailConfirmed = true,
                        FirstName = "GitHub",
                        LastName = "User"
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        return new AuthResponseDto
                        {
                            IsSuccess = false,
                            Message = "Failed to create user: " + string.Join(", ", result.Errors.Select(e => e.Description))
                        };
                    }

                    await _userManager.AddToRoleAsync(user, "User");
                }

                // Generate JWT token
                var token = await _authService.CreateToken(user);
                var userDto = await _authService.GetUserDtoAsync(user);

                return new AuthResponseDto
                {
                    IsSuccess = true,
                    Token = token,
                    User = userDto,
                    Message = user == null ? "GitHub hesabınız basarıyla sisteme kaydedildi. Giris yapabilirsiniz." : "GitHub ile giris basarili!"
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = $"GitHub authentication failed: {ex.Message}"
                };
            }
        }
    }

    public class GitHubEmail
    {
        public string email { get; set; }
        public bool primary { get; set; }
        public bool verified { get; set; }
    }
} 