using AtlasCommerce.BuildingBlocks.Common.Middleware;
using Catalog.Application.Features.Products.Commands.AddProductAttribute;
using Catalog.Application.Features.Products.Commands.AddProductVariant;
using Catalog.Application.Features.Products.Commands.CreateProduct;
using Catalog.Application.Features.Products.Commands.DeleteProduct;
using Catalog.Application.Features.Products.Commands.RemoveProductAttribute;
using Catalog.Application.Features.Products.Commands.RemoveProductVariant;
using Catalog.Application.Features.Products.Commands.UpdateProduct;
using Catalog.Application.Features.Products.Commands.UploadProductImage;
using Catalog.Application.Features.Products.Queries.GetProductById;
using Catalog.Application.Features.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ISender _mediator;

        public ProductsController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(
           [FromQuery] int page = 1,
           [FromQuery] int pageSize = 20,
           [FromQuery] string? search = null,
           [FromQuery] Guid? categoryId = null,
           [FromQuery] Guid? brandId = null,
           [FromQuery] decimal? minPrice = null,
           [FromQuery] decimal? maxPrice = null,
           [FromQuery] bool? isActive = null,
           [FromQuery] bool? isFeatured = null,
           [FromQuery] string? sortBy = null,
           [FromQuery] string? sortOrder = null,
           CancellationToken cancellationToken = default)
        {
            var query = new GetProductsQuery(
                page, pageSize, search, categoryId, brandId,minPrice, maxPrice, isActive, isFeatured, sortBy, sortOrder);

            var result = await _mediator.Send(query, cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetProductByIdQuery(id), cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(
            [FromBody] CreateProductCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.ToActionResult();
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateProductRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateProductCommand(
                id, request.Name, request.Description, request.BasePrice,
                request.StockQuantity, request.CategoryId, request.BrandId,
                request.IsActive, request.IsFeatured);
            var result = await _mediator.Send(command, cancellationToken);
            return result.ToActionResult();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id,CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteProductCommand(id), cancellationToken);
            return result.ToActionResult();
        }


        [HttpPost("{id:guid}/images")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadImage(Guid id,IFormFile file,[FromQuery] bool isMain = false,
            CancellationToken cancellationToken = default)
        {
            if (file.Length == 0)
                return BadRequest(new { message = "Dosya boş olamaz." });

            using var stream = file.OpenReadStream();

            var command = new UploadProductImageCommand(id, stream, file.FileName, file.ContentType, isMain);

            var result = await _mediator.Send(command, cancellationToken);
            return result.ToActionResult();
        }


        // Attribute
        [HttpPost("{id:guid}/attributes")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAttribute(Guid id,[FromBody] AddAttributeRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new AddProductAttributeCommand(id, request.Name, request.Value),
                cancellationToken);
            return result.ToActionResult();
        }

        [HttpDelete("{id:guid}/attributes/{attributeId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveAttribute(Guid id, Guid attributeId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RemoveProductAttributeCommand(id, attributeId), cancellationToken);
            return result.ToActionResult();
        }

        // Variant
        [HttpPost("{id:guid}/variants")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddVariant(Guid id, [FromBody] AddVariantRequest request,
            CancellationToken cancellationToken)
        {
            var command = new AddProductVariantCommand(
                id,
                request.Sku,
                request.PriceOverride,
                request.StockQuantity,
                request.Attributes
                    .Select(a => new VariantAttributeRequest(a.Name, a.Value))
                    .ToList());

            var result = await _mediator.Send(command, cancellationToken);
            return result.ToActionResult();
        }

        [HttpDelete("{id:guid}/variants/{variantId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveVariant(Guid id,Guid variantId,CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RemoveProductVariantCommand(id, variantId), cancellationToken);
            return result.ToActionResult();
        }
    }

    public sealed record UpdateProductRequest(
        string Name,
        string? Description,
        decimal BasePrice,
        int StockQuantity,
        Guid CategoryId,
        Guid BrandId,
        bool IsActive,
        bool IsFeatured);

    public sealed record AddAttributeRequest(string Name, string Value);

    public sealed record AddVariantRequest(
        string Sku,
        decimal? PriceOverride,
        int StockQuantity,
        List<AddVariantAttributeRequest> Attributes);

    public sealed record AddVariantAttributeRequest(string Name, string Value);
}
