using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Shopping.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string PaymentMethod { get; set; } // Card, PayPal
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public Order Order { get; set; }
    }
}