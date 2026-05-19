using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Shopping.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<ProductImages> Images { get; set; } = new();
        public List<CartItem> CartItems { get; set; } = new();
    }
}