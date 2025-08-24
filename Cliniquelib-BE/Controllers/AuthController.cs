using Microsoft.AspNetCore.Mvc;
using Cliniquelib_BE.Services;
using Cliniquelib_BE.DTOs.Auth;

namespace Cliniquelib_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Sign in a user
        /// </summary>
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SigninRequestDto request)
        {
            try
            {
                var result = await _authService.SignInAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignupRequestDto request)
        {
            try
            {
                var result = await _authService.SignUpAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Refresh JWT token
        /// </summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Sign out a user (invalidate refresh tokens)
        /// </summary>
        [HttpPost("signout")]
        public async Task<IActionResult> SignOut([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                var userId = await _authService.GetUserIdByRefreshTokenAsync(request.RefreshToken);
                if (userId == null)
                    return NotFound(new { message = "Refresh token not found or expired" });

                await _authService.SignOutAsync(userId.Value);

                return Ok(new { message = "Signed out successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //[HttpPost("me")]
        //public async Task <IActionResult> 
    }
}
