using Order.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order.Domain.Entities.Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Order.Domain.Entities.Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken);
        Task<(IReadOnlyList<Order.Domain.Entities.Order> Items, long TotalCount)> GetByUserIdAsync(
            Guid userId, int page, int pageSize, CancellationToken cancellationToken);
        Task<(IReadOnlyList<Order.Domain.Entities.Order> Items, long TotalCount)> GetAllAsync(
            int page, int pageSize, string? status, CancellationToken cancellationToken);
        Task AddAsync(Order.Domain.Entities.Order order, CancellationToken cancellationToken);
        Task UpdateAsync(Order.Domain.Entities.Order order, CancellationToken cancellationToken);
    }
}
