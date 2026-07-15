using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlasCommerce.BuildingBlocks.EventBus.Events
{
    // Product
    public sealed record ProductCreatedEvent(
        Guid ProductId, string Name, string Sku,
        decimal BasePrice, int StockQuantity,
        Guid CategoryId, string CategoryName,
        Guid BrandId, string BrandName,
        bool IsActive, bool IsFeatured) : IntegrationEvent;

    public sealed record ProductUpdatedEvent(
        Guid ProductId, string Name, string Sku,
        decimal BasePrice, int StockQuantity,
        Guid CategoryId, string CategoryName,
        Guid BrandId, string BrandName,
        bool IsActive, bool IsFeatured) : IntegrationEvent;

    public sealed record ProductDeletedEvent(Guid ProductId) : IntegrationEvent;

    // Category
    public sealed record CategoryCreatedEvent(
        Guid CategoryId,
        string Name,
        string? Description,
        string? ImageUrl,
        int DisplayOrder,
        bool IsActive,
        Guid? ParentId,
        string? ParentName) : IntegrationEvent;

    public sealed record CategoryUpdatedEvent(
        Guid CategoryId,
        string Name,
        string? Description,
        string? ImageUrl,
        int DisplayOrder,
        bool IsActive,
        Guid? ParentId,
        string? ParentName) : IntegrationEvent;

    public sealed record CategoryDeletedEvent(Guid CategoryId) : IntegrationEvent;

    // Brand
    public sealed record BrandCreatedEvent(
        Guid BrandId,
        string Name,
        string? Description,
        string? LogoUrl,
        bool IsActive) : IntegrationEvent;

    public sealed record BrandUpdatedEvent(
        Guid BrandId,
        string Name,
        string? Description,
        string? LogoUrl,
        bool IsActive) : IntegrationEvent;

    public sealed record BrandDeletedEvent(Guid BrandId) : IntegrationEvent;
}
