using AtlasCommerce.BuildingBlocks.Common.Results;
using Identity.Application.Features.Auth.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Auth.Commands.Register
{
    public sealed record RegisterCommand(
        string Username,
        string Email,
        string Password,
        string FirstName,
        string LastName) : IRequest<Result<TokenResponseDto>>;
}
