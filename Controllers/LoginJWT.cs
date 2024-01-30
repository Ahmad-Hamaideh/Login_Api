using LoginApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using LoginApi.Helpers;
using LoginApi.Moudls;
using Microsoft.Extensions.Options;

namespace LoginApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginJWT : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<LoginJWT> _logger;
        private readonly JWT _jwt;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginJWT(AppDbContext dbContext, ILogger<LoginJWT> logger, IOptions<JWT> jwt, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            _jwt = jwt.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Login _login)
        {
            try
            {
                if (string.IsNullOrEmpty(_login.UserEmail) || string.IsNullOrEmpty(_login.UserPassword))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { message = "Invalid login data.", Status = 400 });

                }

                var existingUser = _dbContext.RegistrationTable
                     .FirstOrDefault(u => u.UserEmail == _login.UserEmail);

                if (existingUser != null)
                {
                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(_login.UserPassword, existingUser.UserPassword);

                    if (isPasswordValid)
                    {
                        var claims = new List<Claim>();

                        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
                        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                        var tokeOptions = new JwtSecurityToken(
                            issuer: _jwt.Issuer,
                            audience: _jwt.Audience,
                            claims: claims,
                            expires: DateTime.Now.AddMinutes(1),
                            signingCredentials: signinCredentials
                        );
                        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

                        //_httpContextAccessor.HttpContext.Session.SetString("AuthToken", tokenString);

                        Response.Headers.Add("Authorization", "Bearer " + tokenString);
                        return Ok(new AuthenticatedResponse { Token = tokenString, Email = _login.UserEmail, Message = "Login succeeded" });
                    }
                    else
                    {
                        _logger.LogWarning("Invalid email or password. Password verification failed.");

                        return StatusCode(StatusCodes.Status400BadRequest, new { message = "Invalid email or password", Status = 400 });
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { message = "Invalid email or password", Status = 400 });

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { StatusCode = StatusCodes.Status500InternalServerError, Message = "An error occurred during login." });
            }
        }
    }
}
