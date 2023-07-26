﻿using Identity.Model.Constants;
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

        [AllowAnonymous]
        [HttpPost("verify")]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<IdentityError>), StatusCodes.Status400BadRequest)]
        [SwaggerOperation("Confirm email address")]
        public async Task<ActionResult<IdentityResult>> VerifyEmailAsync([FromBody] EmailVerificationRequest request)
        {
            try
            {
                var result = await _userService.VerifyEmailAsync(request)
                    .ConfigureAwait(false);

                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, nameof(VerifyEmailAsync));
                throw ex;
            }
        }

        [AllowAnonymous]
        [HttpPost("token")]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(AccessTokenResponse), StatusCodes.Status200OK)]
        [SwaggerOperation("Login method")]
        public async Task<ActionResult<AccessTokenResponse>> Token([FromBody] TokenRequest request)
        {
            try
            {
                var result = await _userService.TokenAsync(request).ConfigureAwait(false);

                if (result is null)
                    return BadRequest(new ApiErrorResponse(ErrorCodes.InvalidGrant, ErrorDescriptions.InvalidGrantType));

                if (result.IsError)
                    return BadRequest(new ApiErrorResponse(result.Error, result.ErrorDescription));

                return Ok(new AccessTokenResponse(
                    result.AccessToken,
                    result.RefreshToken,
                    result.ExpiresIn));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "POST:/token");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
