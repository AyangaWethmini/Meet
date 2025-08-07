using Mocrosoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SafeMeet.Api.Models;
using SafeMeet.Api.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;


namespace Safemeet.Api.Controllers{
    [ApiController]
    [Route("/api/auth")]
    public class AuthController : ControllerBase {
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;

        public AuthController(UserService userService, IConfiguration configuration){
            _userService = userService;
            _configuration = configuration;
        }

        [HttpGet("login")]
        public IActionResult Login(){
            return Challange(new AuthnticationProperties {RedirectUri = "/api/auth/callback"}, "Google");

        }

        var user = await _userService.GetOrCreateUserAsync(authenticateResult.Principal);

        //Generate JWT TOKEN 
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Authentication:Google:JwtSecret"]);
        var tokenDescriptor = new SecurityTokenDescriptor
                 {
                     Subject = new ClaimsIdentity(new[]
                     {
                         new Claim(ClaimTypes.NameIdentifier, user.Id),
                         new Claim(ClaimTypes.Email, user.Email),
                         new Claim(ClaimTypes.Name, user.Name)
                     }),
                     Expires = DateTime.UtcNow.AddHours(1),
                     SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                 };

                 var token = tokenHandler.CreateToken(tokenDescriptor);
                 var tokenString = tokenHandler.WriteToken(token);

                 return Ok(new { Token = tokenString, User = user });
    }
}