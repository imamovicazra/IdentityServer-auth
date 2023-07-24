using Identity.Model.Constants;
using Identity.Model.DTOs.Requests;
using Identity.Model.DTOs.Responses;
using Identity.Model.Responses;
using Identity.Model.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly ILogger<IdentityController> _logger;
        private readonly IUserService _userService;

        public IdentityController(ILogger<IdentityController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status201Created)]
        [SwaggerOperation("Register new user")]
        public async Task<ActionResult<IdentityResult>> Register([FromBody] ApplicationUserRequest request)
        {
            try
            {
                var result = await _userService.RegisterAsync(request).ConfigureAwait(false);

                if (!result.Succeeded)
                    return BadRequest(result);

                return CreatedAtAction(nameof(GetUser), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "POST:/register");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("users")]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [SwaggerOperation("Get information about currently logged in user")]
        public async Task<ActionResult<UserResponse>> GetUser()
        {
            try
            {
                var result = await _userService.GetUserAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, null).ConfigureAwait(false);

                if (result is null)
                    return NotFound(new ApiErrorResponse(ErrorCodes.UserNotFound, ErrorDescriptions.UserDoesNotExistWithId));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GET:/users");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
