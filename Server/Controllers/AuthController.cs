using Microsoft.AspNetCore.Mvc;
using Prog.Model;
using Prog.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Prog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IDBManager _dbManager;
        private readonly IAuthJwt _authJwt;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IDBManager dbManager, IAuthJwt authJwt, ILogger<AuthController> logger)
        {
            _dbManager = dbManager;
            _authJwt = authJwt;
            _logger = logger;
        }

        [HttpPost("signup")]
        public IActionResult SignUp([FromBody] SignUpRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid input data.", Errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            try
            {
                if (_dbManager.AddUser(request.Login, request.Password))
                    return Ok(new { Message = $"User {request.Login} registered successfully" });
                else
                    return BadRequest(new { Message = $"Failed to register user {request.Login}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration.");
                return StatusCode(500, new { Message = "An error occurred while processing the request.", Error = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid input data.", Errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            try
            {
                if (!_dbManager.CheckUser(request.Login, request.Password))
                    return Unauthorized(new { Message = "Invalid login or password" });

                var token = _authJwt.LogIn(request.Login, request.Password);
                if (token == null)
                    return Unauthorized(new { Message = "Failed to generate token. Please try again." });

                return Ok(new LoginResponse { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login.");
                return StatusCode(500, new { Message = "An error occurred while processing the request.", Error = ex.Message });
            }
        }

        [HttpPost("deleteuser")]
        public IActionResult DeleteUser([FromBody] DeleteUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid input data.", Errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            try
            {
                if (_dbManager.DeleteUser(request.Login))
                    return Ok(new { Message = $"User {request.Login} deleted successfully" });
                else
                    return BadRequest(new { Message = $"Failed to delete user {request.Login}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user deletion.");
                return StatusCode(500, new { Message = "An error occurred while processing the request.", Error = ex.Message });
            }
        }

        [HttpPost("changepassword")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid input data.", Errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            try
            {
                if (request.OldPassword == request.NewPassword)
                {
                    return BadRequest(new { Message = "New password must be different from the old password." });
                }

                if (!_dbManager.CheckUser(request.Login, request.OldPassword))
                    return Unauthorized(new { Message = "Invalid login or old password" });

                if (_dbManager.UpdatePassword(request.Login, request.NewPassword))
                    return Ok(new { Message = $"Password for user {request.Login} updated successfully" });
                else
                    return BadRequest(new { Message = $"Failed to update password for user {request.Login}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password change.");
                return StatusCode(500, new { Message = "An error occurred while processing the request.", Error = ex.Message });
            }
        }

    }
}