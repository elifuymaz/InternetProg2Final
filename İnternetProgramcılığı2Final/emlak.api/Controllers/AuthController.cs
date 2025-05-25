using Microsoft.AspNetCore.Mvc;
using emlak.api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using emlak.api.Extensions;
using AutoMapper;
using emlak.api.Models;
using Microsoft.Extensions.Configuration;
using emlak.api.DTOs;

namespace emlak.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IGitHubAuthService _gitHubAuthService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthController(
            IAuthService authService,
            IGitHubAuthService gitHubAuthService,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IConfiguration configuration)
        {
            _authService = authService;
            _gitHubAuthService = gitHubAuthService;
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="model">User registration details</param>
        /// <returns>Registration result with user details</returns>
        /// <response code="200">Returns the newly created user</response>
        /// <response code="400">If the registration fails</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var result = await _authService.Register(model);
            
            return result.IsSuccess ? 
                Ok(result) : 
                BadRequest(result);
        }

        /// <summary>
        /// Authenticates a user
        /// </summary>
        /// <param name="model">Login credentials</param>
        /// <returns>Authentication result with JWT token</returns>
        /// <response code="200">Returns the JWT token</response>
        /// <response code="400">If the login fails</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _authService.Login(model);
            
            return result.IsSuccess ? 
                Ok(result) : 
                BadRequest(result);
        }

        /// <summary>
        /// Logs out the current user
        /// </summary>
        /// <returns>Logout result</returns>
        /// <response code="200">Returns success message</response>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Logout()
        {
            // JWT token'lar stateless olduğu için sunucu tarafında özel bir işlem yapmaya gerek yok
            // Client tarafında token'ı silmek yeterli olacaktır
            return Ok(new { message = "Başarıyla çıkış yapıldı" });
        }
       
        [HttpGet("current-user")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userManager.FindByNameAsync(User.GetUsername());
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<UserDto>(user);

            return Ok(new
            {
                User = userDto,
                Roles = roles
            });
        }

        /// <summary>
        /// Initiates GitHub authentication
        /// </summary>
        /// <returns>GitHub authorization URL</returns>
        [HttpGet("github-login")]
        public IActionResult GitHubLogin()
        {
            var clientId = _configuration["GitHub:ClientId"];
            var callbackUrl = _configuration["GitHub:CallbackUrl"];
            var githubAuthUrl = $"https://github.com/login/oauth/authorize?client_id={clientId}&redirect_uri={callbackUrl}&scope=user:email";
            
            return Ok(new { Url = githubAuthUrl });
        }

        /// <summary>
        /// Handles GitHub OAuth callback
        /// </summary>
        /// <param name="code">Authorization code from GitHub</param>
        /// <returns>Authentication result with JWT token</returns>
        [HttpGet("github-callback")]
        public async Task<IActionResult> GitHubCallback([FromQuery] string code)
        {
            var result = await _gitHubAuthService.AuthenticateGitHubUser(code);
            
            if (result.IsSuccess)
            {
                // Popup'ı kapat ve ana pencereye mesaj gönder
                return Content($@"
                    <html>
                        <body>
                            <script>
                                window.opener.postMessage({{ 
                                    type: 'github-login-success',
                                    token: '{result.Token}',
                                    message: '{result.Message}'
                                }}, '*');
                                window.close();
                            </script>
                        </body>
                    </html>", "text/html");
            }
            
            return Content($@"
                <html>
                    <body>
                        <script>
                            window.opener.postMessage({{ 
                                type: 'github-login-error',
                                message: '{result.Message}'
                            }}, '*');
                            window.close();
                        </script>
                    </body>
                </html>", "text/html");
        }
    }
} 