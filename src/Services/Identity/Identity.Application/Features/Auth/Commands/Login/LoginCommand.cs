using AtlasCommerce.BuildingBlocks.Common.Results;
using Identity.Application.Features.Auth.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Auth.Commands.Login
{
    public sealed record class LoginCommand(string Username, string Password) : IRequest<Result<TokenResponseDto>>;
}
