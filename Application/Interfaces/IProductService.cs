using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.CategoryDTOs;
using Application.DTOs.ProductDTOs;
using Application.DTOs.ProductsDTOs;

namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<List<CategoryViewDto>> GetAllCategories();
        Task CreateProductAsync(ProductCreateDto model);
        Task<List<ProductViewDto>> GetAllProducts();
        Task<ProductUpdateDto> GetUpdateProductAsync(int id);
        Task UpdateProductAsync(int id, ProductUpdateDto model);
        Task<ProductViewDto> DetailProductAsync(string name);
        Task<List<ProductViewDto>> GetProductsBySearchAsync(string searchTerm);
        Task<List<ProductViewDto>> FilterProductsAsync(List<string> categories, string price, string sort);
    }
}