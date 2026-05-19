using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.CategoryDTOs;

namespace Application.DTOs.ProductDTOs
{
    public class ProductPageViewModel
    {
        public List<ProductViewDto> Products { get; set; }
        public List<CategoryViewDto> Categories { get; set; }
    }
}