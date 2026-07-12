using AtlasCommerce.BuildingBlocks.Common.Results;
using Identity.Application.Common.Interfaces;
using Identity.Application.Features.Auth.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Auth.Commands.RefreshToken
{
    public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<TokenResponseDto>>
    {
        private readonly IKeycloakTokenService _tokenService;

        public RefreshTokenCommandHandler(IKeycloakTokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<Result<TokenResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var tokenResult = await _tokenService.RefreshAsync(request.RefreshToken, cancellationToken);

            if (!tokenResult.IsSuccess)
            {
                return Result.Failure<TokenResponseDto>(
                    Error.Unauthorized("Auth.InvalidRefreshToken", "Oturum süresi dolmuş, lütfen tekrar giriş yapın."));
            }

            var dto = new TokenResponseDto(
                tokenResult.AccessToken!,
                tokenResult.RefreshToken!,
                tokenResult.ExpiresInSeconds);

            return Result.Success(dto);
        }
    }
}
