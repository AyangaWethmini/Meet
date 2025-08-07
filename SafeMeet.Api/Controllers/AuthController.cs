using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SafeMeet.Api.Models;
using SafeMeet.Api.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SafeMeet.Api.Controllers
{

    // Debug: Check if environment variables are loaded


    [ApiController]
    [Route("/api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;

        public AuthController(UserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            // Debug: Log the client configuration
            var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ?? _configuration["Authentication:Google:ClientId"];
            var clientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET") ?? _configuration["Authentication:Google:ClientSecret"];

            Console.WriteLine($"Google Client ID: {clientId}");
            Console.WriteLine($"Google Client Secret: {(string.IsNullOrEmpty(clientSecret) ? "NOT SET" : "SET")}");

            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("Callback", "Auth")
            };

            return Challenge(properties, "Google");
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback()
        {
            // The authentication middleware will have already processed the callback
            var authenticateResult = await HttpContext.AuthenticateAsync("Cookies");

            if (!authenticateResult.Succeeded)
            {
                return Unauthorized("Authentication failed");
            }

            try
            {
                // Attempt to get or create user in the database
                var user = await _userService.GetOrCreateUserAsync(authenticateResult.Principal);
                Console.WriteLine($"User successfully saved/retrieved from database: {user.Email}");

                // Generate JWT TOKEN 
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? _configuration["Authentication:Google:JwtSecret"];
                if (string.IsNullOrEmpty(jwtSecret))
                {
                    return StatusCode(500, "JWT secret not configured");
                }
                var key = Encoding.UTF8.GetBytes(jwtSecret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id ?? ""),
                        new Claim(ClaimTypes.Email, user.Email ?? ""),
                        new Claim(ClaimTypes.Name, user.Name ?? "")
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new
                {
                    Token = tokenString,
                    User = user,
                    Message = "User successfully authenticated and saved to database"
                });
            }
            catch (Exception ex)
            {
                // Log detailed error information
                Console.WriteLine($"Database error during user authentication: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                // Return proper error response instead of fallback
                return StatusCode(500, new
                {
                    Error = "Failed to save user to database",
                    Details = ex.Message,
                    Suggestion = "Please check database connection and try again"
                });
            }
        }
    }
}