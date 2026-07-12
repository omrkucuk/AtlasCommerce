using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Domain.Events
{
    public sealed record OrderCancelledEvent(Guid OrderId, Guid UserId, string Reason) : IDomainEvent;

}
