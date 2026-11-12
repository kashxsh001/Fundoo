using BuisnessLayer.CQRS.Users;
using BuisnessLayer.User.Command;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using System.Security.Claims;

namespace Fundoo2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Fundoo2Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public Fundoo2Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(
            [FromBody] RegisterationModel user)
        {
            var result = await _mediator.Send(new CreateUserCommand(user));

            if (result == null)
            {
                return Conflict(new ResponseModel<UserResponseModel>
                {
                    success = false,
                    message = "Email already registered",
                    data = null
                });
            }

            return Ok(new ResponseModel<UserResponseModel>
            {
                success = true,
                message = "User registered successfully",
                data = result
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginModel model)
        {
            var result = await _mediator.Send(new LoginUserCommand(model));

            if (result == null)
            {
                return Unauthorized(
                    new ResponseModel<UserResponseModel>
                    {
                        success = false,
                        message = "Invalid email or password",
                        data = null
                    });
            }

            return Ok(new ResponseModel<UserResponseModel>
            {
                success = true,
                message = "Login successful",
                data = result
            });
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            string? userId =User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            string? email =User.FindFirst(ClaimTypes.Email)?.Value;

            return Ok(new { userId, email });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordModel model)
        {
            await _mediator.Send(
                new ForgotPasswordCommand(model));

            return Ok(new ResponseModel<string>
            {
                success = true,
                message= "If this email is registered, a reset token has been sent",
                data = null
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordModel model)
        {
            bool isReset = await _mediator.Send(
                new ResetPasswordCommand(model));

            if (!isReset)
            {
                return BadRequest(new ResponseModel<string>
                {
                    success = false,
                    message = "Invalid or expired token",
                    data = null
                });
            }

            return Ok(new ResponseModel<string>
            {
                success = true,
                message = "Password reset successful",
                data = null
            });
        }
    }
}