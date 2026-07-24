using Identity.Application.Features.Auth.Commands.Login;
using Identity.Application.Features.Auth.Commands.RefreshToken;
using Identity.Application.Features.Auth.Commands.Register;
using Identity.Application.Features.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class AuthController : ControllerBase
    {
        private readonly ISender _mediator;

        public AuthController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFailure)
                return BadRequest(new { message = result.Error.Description });

            SetRefreshTokenCookie(result.Value.RefreshToken);

            return Ok(new
            {
                accessToken = result.Value.AccessToken,
                expiresInSeconds = result.Value.ExpiresInSeconds
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new LoginCommand(request.Username, request.Password), cancellationToken);

            if (result.IsFailure)
                return Unauthorized(new { message = result.Error.Description });

            SetRefreshTokenCookie(result.Value.RefreshToken);

            return Ok(new
            {
                accessToken = result.Value.AccessToken,
                expiresInSeconds = result.Value.ExpiresInSeconds
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrWhiteSpace(refreshToken))
                return Unauthorized(new { message = "Refresh token bulunamadı." });

            var result = await _mediator.Send(new RefreshTokenCommand(refreshToken), cancellationToken);

            if (result.IsFailure) 
            {
                DeleteRefreshTokenCookie();
                return Unauthorized(new { message = result.Error.Description });
            }

            SetRefreshTokenCookie(result.Value.RefreshToken);

            return Ok(new
            {
                accessToken = result.Value.AccessToken,
                expiresInSeconds = result.Value.ExpiresInSeconds
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            DeleteRefreshTokenCookie();
            return NoContent();
        }


        private void SetRefreshTokenCookie(string refreshToken)
        {
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                Path = "/api/auth"
            });
        }

        private void DeleteRefreshTokenCookie()
        {
            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/api/auth"
            });
        }
    }
}
