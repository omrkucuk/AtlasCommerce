using AtlasCommerce.BuildingBlocks.Common.Results;
using Catalog.Application.IntegrationEvents;
using Catalog.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.Commands.DeleteProduct
{
    public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result>
    {
        private readonly ICatalogDbContext _context;
        private readonly IEventBus _eventBus;

        public DeleteProductCommandHandler(ICatalogDbContext context, IEventBus eventBus)
        {
            _context = context;
            _eventBus = eventBus;
        }

        public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (product is null)
                return Result.Failure(Error.NotFound("Product.NotFound", "Ürün bulunamadı."));

            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);

            await _eventBus.PublishAsync(new ProductDeletedEvent(product.Id),cancellationToken);

            return Result.Success();
        }
    }
}
