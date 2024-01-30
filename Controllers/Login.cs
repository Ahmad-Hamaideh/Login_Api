using LoginApi.Models;
using LoginApi.Moudls;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LoginApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<LoginController> _logger;

        public LoginController(AppDbContext dbContext, ILogger<LoginController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginClass _login)
        {
            try
            {
                if (string.IsNullOrEmpty(_login.UserEmail) || string.IsNullOrEmpty(_login.UserPassword))
                {
                    return BadRequest("Invalid login data.");
                }

                var existingUser = _dbContext.RegistrationTable
                     .FirstOrDefault(u => u.UserEmail == _login.UserEmail);

                if (existingUser != null)
                {
                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(_login.UserPassword, existingUser.UserPassword);

                    if (isPasswordValid)
                    {
                        return Ok("Login successful");
                    }
                    else
                    {
                        return BadRequest("Invalid email or password.");
                    }
                }
                else
                {
                    return BadRequest("Invalid email or password.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { StatusCode = StatusCodes.Status500InternalServerError, Message = "An error occurred during login." });
            }
        }


        [HttpGet("all-users")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var allUsers = _dbContext.RegistrationTable.ToList(); 

                return Ok(allUsers);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "An error occurred while retrieving all users.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { StatusCode = StatusCodes.Status500InternalServerError, Message = "An error occurred while retrieving all users." });
            }
        }
    }
}



