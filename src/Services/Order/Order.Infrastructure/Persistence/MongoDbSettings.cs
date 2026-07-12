using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Infrastructure.Persistence
{
    public sealed class MongoDbSettings
    {
        public string ConnectionStrings { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string OrdersCollectionName { get; set; } = "orders";
    }
}
