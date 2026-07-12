using AtlasCommerce.BuildingBlocks.Common.Results;
using Catalog.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.Commands.UploadProductImage
{
    public sealed class UploadProductImageCommandHandler : IRequestHandler<UploadProductImageCommand, Result<string>>
    {
        private readonly ICatalogDbContext _context;
        private readonly IFileStorageService _fileStorage;

        public UploadProductImageCommandHandler(ICatalogDbContext context, IFileStorageService fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        public async Task<Result<string>> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

            if (product is null)
                return Result.Failure<string>(Error.NotFound("Product.NotFound", "Ürün bulunamadı."));

            // Benzersiz dosya adı oluştur
            var extension = Path.GetExtension(request.FileName);
            var uniqueFileName = $"{request.ProductId}/{Guid.NewGuid()}{extension}";

            var imageUrl = await _fileStorage.UploadAsync(
                request.FileStream,
                request.FileName,
                request.ContentType,
                bucketName: "products",
                cancellationToken);

            product.AddImage(imageUrl, request.IsMain);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(imageUrl);
        }
    }
}
