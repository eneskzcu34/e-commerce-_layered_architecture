using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Shopping.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; }

        public ICollection<OrderItem> Items { get; set; }
    }
}