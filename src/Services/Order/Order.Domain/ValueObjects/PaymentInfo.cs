using Order.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Domain.ValueObjects
{
    public sealed record PaymentInfo(PaymentMethod Method, PaymentStatus Status, string? TransactionId, DateTime? PaidAt)
    {
        public static PaymentInfo Pending(PaymentMethod method)
            => new(method, PaymentStatus.Pending, null, null);

        public PaymentInfo MarkAsPaid(string transactionId)
            => this with { Status = PaymentStatus.Paid, TransactionId = transactionId, PaidAt = DateTime.UtcNow };

        public PaymentInfo MarkAsFailed()
            => this with { Status = PaymentStatus.Failed };

        public PaymentInfo MarkAsRefunded()
            => this with { Status = PaymentStatus.Refunded };
    }
}
 