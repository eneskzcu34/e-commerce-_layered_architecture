using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.CategoryDTOs
{
    public class CategoryUpdateDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir.")]
        [RegularExpression(@"^\p{L}+( \p{L}+)*$", ErrorMessage = "Kategori adı sadece harf ve boşluk içerebilir.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string Description { get; set; }

        public DateTime UpdateDate { get; set; } = DateTime.Now;
    }
}