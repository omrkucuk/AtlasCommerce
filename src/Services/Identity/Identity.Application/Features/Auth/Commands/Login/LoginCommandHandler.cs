using AtlasCommerce.BuildingBlocks.Common.Results;
using Identity.Application.Common.Interfaces;
using Identity.Application.Features.Auth.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Auth.Commands.Login
{
    public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<TokenResponseDto>>
    {
        private readonly IKeycloakTokenService _tokenService;

        public LoginCommandHandler(IKeycloakTokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<Result<TokenResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var tokenResult = await _tokenService.LoginAsync(request.Username, request.Password, cancellationToken);

            if (!tokenResult.IsSuccess)
            {
                return Result.Failure<TokenResponseDto>(
                    Error.Unauthorized("Auth.InvalidCredentials", "Kullanıcı adı veya şifre hatalı."));
            }

            var dto = new TokenResponseDto(
                tokenResult.AccessToken!,
                tokenResult.RefreshToken!,
                tokenResult.ExpiresInSeconds);

            return Result.Success(dto);
        }
    }
}
