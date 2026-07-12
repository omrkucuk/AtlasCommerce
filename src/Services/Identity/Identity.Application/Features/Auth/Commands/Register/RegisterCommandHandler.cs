using AtlasCommerce.BuildingBlocks.Common.Results;
using Identity.Application.Common.Interfaces;
using Identity.Application.Features.Auth.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Auth.Commands.Register
{
    public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<TokenResponseDto>>
    {
        private readonly IKeycloakAdminService _adminService;
        private readonly IKeycloakTokenService _tokenService;

        public RegisterCommandHandler(IKeycloakAdminService adminService, IKeycloakTokenService tokenService)
        {
            _adminService = adminService;
            _tokenService = tokenService;
        }

        public async Task<Result<TokenResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var createResult = await _adminService.CreateUserAsync(
               request.Username,
               request.Email,
               request.Password,
               request.FirstName,
               request.LastName,
               cancellationToken);

            if (!createResult.IsSuccess)
            {
                return Result.Failure<TokenResponseDto>(
                    Error.Conflict("Auth.RegistrationFailed", createResult.ErrorMessage ?? "Kayıt oluşturulamadı."));
            }

            var loginResult = await _tokenService.LoginAsync(request.Username, request.Password, cancellationToken);

            if (!loginResult.IsSuccess)
            {
                return Result.Failure<TokenResponseDto>(
                Error.Failure("Auth.RegisteredButLoginFailed",
                    "Kayıt başarılı oldu ancak otomatik giriş yapılamadı, lütfen giriş yapmayı deneyin."));
            }

            var dto = new TokenResponseDto(
                loginResult.AccessToken!,
                loginResult.RefreshToken!,
                loginResult.ExpiresInSeconds);

            return Result.Success(dto);
        }
    }
}
