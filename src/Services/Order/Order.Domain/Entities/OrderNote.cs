using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Domain.Entities
{
    public sealed class OrderNote
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Content { get; private set; } = string.Empty;
        public string AddedBy { get; private set; } = string.Empty;
        public DateTime AddedAt { get; private set; } = DateTime.UtcNow;
        public bool IsCustomerVisible { get; private set; }

        private OrderNote() { }

        internal static OrderNote Create(string content, string addedBy, bool isCustomerVisible = false)
            => new() { Content = content, AddedBy = addedBy, IsCustomerVisible = isCustomerVisible };
    }
}
