using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.Commands.UploadProductImage
{
    public sealed record UploadProductImageCommand(
        Guid ProductId,
        Stream FileStream,
        string FileName,
        string ContentType,
        bool IsMain) : IRequest<Result<string>>;
}
