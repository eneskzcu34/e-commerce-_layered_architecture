using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Application.DTOs.ProductDTOs
{
    public class ProductUpdateDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Açıklama zorunludur.")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Fiyat zorunludur.")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Stok zorunludur.")]
        public int Stock { get; set; }
        public bool IsActive { get; set; }

        public int CategoryId { get; set; }
        public SelectList? Categories { get; set; }
        public List<ProductImagesDto> Images { get; set; } = new();
        public int? SelectedMainImageId { get; set; }
    }
}