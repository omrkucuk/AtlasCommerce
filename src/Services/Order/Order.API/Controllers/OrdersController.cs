using AtlasCommerce.BuildingBlocks.Common.Middleware;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Features.Orders.Commands.CancelOrder;
using Order.Application.Features.Orders.Commands.ConfirmOrder;
using Order.Application.Features.Orders.Commands.CreateOrder;
using Order.Application.Features.Orders.Commands.ShipOrder;
using Order.Application.Features.Orders.Queries.GetAllOrders;
using Order.Application.Features.Orders.Queries.GetMyOrders;
using Order.Application.Features.Orders.Queries.GetOrderById;
using System.Security.Claims;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ISender _mediator;

        public OrdersController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyOrders(
            [FromQuery] int page = 1,[FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _mediator.Send(new GetMyOrdersQuery(userId.Value, page, pageSize), cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? status = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAllOrdersQuery(page, pageSize, status), cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetOrderByIdQuery(id), cancellationToken);

            if (result.IsFailure) return result.ToActionResult();

            // Admin değilse sadece kendi siparişini görebilir
            var userId = GetUserId();
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && result.Value.UserId != userId)
                return Forbid();

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request,CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var command = new CreateOrderCommand(
                userId.Value,
                request.ShippingAddress,
                request.BillingAddress,
                request.PaymentMethod,
                request.Items,
                request.ShippingFee);

            var result = await _mediator.Send(command, cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost("{id:guid}/confirm")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Confirm(Guid id,[FromBody] ConfirmOrderRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ConfirmOrderCommand(id, request.TransactionId), cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost("{id:guid}/ship")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ship(Guid id,[FromBody] ShipOrderRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ShipOrderCommand(id, request.CargoTrackingNumber), cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id,[FromBody] CancelOrderRequest request,
        CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            // Kullanıcı kendi siparişini mi iptal ediyor?
            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin)
            {
                var order = await _mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
                if (order.IsFailure) return order.ToActionResult();
                if (order.Value.UserId != userId) return Forbid();
            }

            var cancelledBy = userId.Value.ToString();
            var result = await _mediator.Send(new CancelOrderCommand(id, request.Reason, cancelledBy), cancellationToken);
            return result.ToActionResult();
        }

        private Guid? GetUserId()
        {
            var claim = User.FindFirst("sub") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim?.Value, out var id) ? id : null;
        }
    }

    public sealed record CreateOrderRequest(
        CreateOrderRequest_AddressDto ShippingAddress,
        CreateOrderRequest_AddressDto BillingAddress,
        string PaymentMethod,
        List<CreateOrderRequest_ItemDto> Items,
        decimal ShippingFee = 0);

    public sealed record ConfirmOrderRequest(string TransactionId);
    public sealed record ShipOrderRequest(string CargoTrackingNumber);
    public sealed record CancelOrderRequest(string Reason);
}
