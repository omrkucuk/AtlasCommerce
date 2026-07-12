using MongoDB.Driver;
using Order.Application.Interfaces;
using Order.Domain.Entities;
using Order.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Infrastructure.Persistence.Repository
{
    public sealed class OrderRepository : IOrderRepository
    {
        private readonly MongoDbContext _context;

        public OrderRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Domain.Entities.Order order, CancellationToken cancellationToken)
        {
            await _context.Orders.InsertOneAsync(order, new InsertOneOptions(), cancellationToken);
        }

        public async Task<(IReadOnlyList<Domain.Entities.Order> Items, long TotalCount)> GetAllAsync(int page, int pageSize, string? status, CancellationToken cancellationToken)
        {
            var filterBuilder = Builders<Order.Domain.Entities.Order>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
            {
                filter = filterBuilder.Eq(o => o.Status, orderStatus);
            }

            var sort = Builders<Order.Domain.Entities.Order>.Sort.Descending(o => o.CreatedAt);
            var totalCount = await _context.Orders.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

            var items = await _context.Orders
                .Find(filter)
                .Sort(sort)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<Domain.Entities.Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Orders.Find(o => o.Id == id).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Domain.Entities.Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken)
        {
            return await _context.Orders.Find(o => o.OrderNumber == orderNumber).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<(IReadOnlyList<Domain.Entities.Order> Items, long TotalCount)> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken)
        {
            var filter = Builders<Order.Domain.Entities.Order>.Filter.Eq(o => o.UserId, userId);
            var sort = Builders<Order.Domain.Entities.Order>.Sort.Descending(o => o.CreatedAt);

            var totalCount = await _context.Orders.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

            var items = await _context.Orders
                .Find(filter)
                .Sort(sort)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task UpdateAsync(Domain.Entities.Order order, CancellationToken cancellationToken)
        {
            await _context.Orders.ReplaceOneAsync(
                o => o.Id == order.Id, order, new ReplaceOptions { IsUpsert = false }, cancellationToken);
        }
    }
}
