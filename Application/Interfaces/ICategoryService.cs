using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.CategoryDTOs;
using Application.DTOs.ProductsDTOs.CategoryDTOs;

namespace Application.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryViewDto>> GetAllCategoriesAsync();
        Task CategoryCreateAsync(CategoryCreateDto model);
        Task CategoryUpdateAsync(int id, CategoryUpdateDto model);
        Task<CategoryUpdateDto> GetCategoryUpdateAsync(int id);
        Task DeleteCategoryAsync(int id);
    }
}