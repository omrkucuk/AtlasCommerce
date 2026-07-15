using AtlasCommerce.BuildingBlocks.Common.Results;
using AtlasCommerce.BuildingBlocks.EventBus.Events;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Brands.Commands.CreateBrand
{
    public sealed class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, Result<Guid>>
    {
        private readonly ICatalogDbContext _context;
        private readonly IEventBus _eventBus;

        public CreateBrandCommandHandler(ICatalogDbContext context, IEventBus eventBus)
        {
            _context = context;
            _eventBus = eventBus;
        }

        public async Task<Result<Guid>> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            var nameExists = await _context.Brands
                .AnyAsync(b => b.Name == request.Name, cancellationToken);

            if (nameExists)
                return Result.Failure<Guid>(Error.Conflict("Brand.NameExists", $"'{request.Name}' adında bir marka zaten var."));

            var brand = Brand.Create(request.Name, request.Description);
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync(cancellationToken);

            await _eventBus.PublishAsync(new BrandCreatedEvent(brand.Id, brand.Name, brand.Description, brand.LogoUrl, brand.IsActive), cancellationToken);

            return Result.Success(brand.Id);
        }
    }
}
