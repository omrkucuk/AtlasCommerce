using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Domain.Events
{
    public sealed record OrderCreatedEvent(Guid OrderId, Guid UserId, decimal TotalAmount) : IDomainEvent;
}
