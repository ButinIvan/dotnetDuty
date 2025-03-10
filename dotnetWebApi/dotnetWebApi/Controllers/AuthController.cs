using dotnetWebApi.Auth;
using dotnetWebApi.AuthUsers.Services;
using dotnetWebApi.Models;
using dotnetWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace dotnetWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AccountService accountService, AuthService authService, IOptions<AuthSettings> options) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) ||
                string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "All fields are required." });
            }

            var (success, message) =
                await accountService.RegisterAsync(request.UserName, request.FirstName, request.Password);
            
            if (!success) return BadRequest(new {message});

            return Ok(new { message = "User successfully registered" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.UserName) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest(new { message = "Both username and password are required." });
            }
            
            var (success, message, user) = await accountService.LoginAsync(loginRequest.UserName, loginRequest.Password);
            if (!success || user == null)
            {
                return Unauthorized(new { message });
            }

            var token = authService.GenerateToken(user.Id, user.UserName, user.Role);
            
            Response.Cookies.Append("AuthCookie", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.Add(options.Value.Expires)
            });
            
            return Ok(new { message = "Login successful" });
        }
    }
}
