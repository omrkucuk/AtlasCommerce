using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Domain.ValueObjects
{
    public sealed record Money(decimal Amount, string Currency)
    {
        public static Money Zero(string currency = "TRY") => new(0, currency);

        public static Money Of(decimal amount, string currency = "TRY")
        {
            if (amount < 0)
                throw new ArgumentException("Para miktarı negatif olamaz.", nameof(amount));
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Para birimi boş olamaz.", nameof(currency));

            return new Money(amount, currency);
        }

        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException($"Farkı para birimleri toplanamaz: {Currency} + {other.Currency}");

            return new Money(Amount + other.Amount, Currency);
        }

        public Money Multiply(int factor) => new(Amount * factor, Currency);

        public override string ToString() => $"{Amount:N2} {Currency}";
    }
}
