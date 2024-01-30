using LoginApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using LoginApi.Helpers;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel.DataAnnotations;

namespace LoginApi.Controllers
{
    public class CustomErrorResponse
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string TraceId { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationLOGController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RegistrationLOGController> _logger;
        private readonly JWT _jwt;
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RegistrationLOGController(
            IConfiguration configuration,
            AppDbContext dbContext,
            ILogger<RegistrationLOGController> logger,
            IOptions<JWT> jwt,
            IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _logger = logger;
            _jwt = jwt.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("registration")]
        public async Task<IActionResult> Registration([FromBody] Registration registration)
        {
            try
            {
                if (registration == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { message = "Invalid registration data", Status = 400 });

                }
                if (await _dbContext.RegistrationTable.AnyAsync(u => u.UserEmail == registration.UserEmail))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { message = "Email is already registered!", Status = 400 });

                }

                if (await _dbContext.RegistrationTable.AnyAsync(u => u.UserName == registration.UserName))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { message = "Username is already registered!", Status = 400 });
                }
                if (!IsValidEmail(registration.UserEmail))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { message = "Invalid email address", Status = 400 });

                }

                if (!IsStrongPassword(registration.UserPassword))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { message = "Weak password. Password must contain at least 6 characters including uppercase, lowercase, numbers, and symbols.", Status = 400 });

                }


                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registration.UserPassword);

                var newUser = new Registration
                {
                    UserName = registration.UserName,
                    UserEmail = registration.UserEmail,
                    IsActive = registration.IsActive,
                    UserPassword = hashedPassword
                };

                // Generate token
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

                await _dbContext.RegistrationTable.AddAsync(newUser).ConfigureAwait(false);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);

                //_httpContextAccessor.HttpContext.Session.SetString("AuthToken", tokenString);

                return Ok(new AuthenticatedResponse { Token = tokenString, Email = registration.UserEmail, Message = "Registration successful" });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error during registration.");

                var errorMessage = ex.GetBaseException()?.Message;

                return BadRequest(new { error = errorMessage });
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false; 
            }
        }
        private bool IsStrongPassword(string password)
        {

            return password.Length >= 6 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit);
                  
        }
    }
}
