using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Shopping.Domain.Entities;

namespace Domain.Entities
{
    public class Advertisement
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}