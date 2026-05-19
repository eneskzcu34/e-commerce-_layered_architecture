using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.CategoryDTOs;
using Application.DTOs.ProductsDTOs.CategoryDTOs;
using Application.Interfaces;
using E_Shopping.Domain.Entities;
using E_Shopping.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Managers
{
    public class CategoryManager : ICategoryService
    {
        private readonly IRepository<Category> _repository;

        public CategoryManager(IRepository<Category> repository)
        {
            _repository = repository;
        }
        public async Task<List<CategoryViewDto>> GetAllCategoriesAsync()
        {
            var categories = await _repository.GetQueryable().Include(c => c.Products).OrderByDescending(c => c.Id).Select(c => new CategoryViewDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CreateDate = c.CreateDate,
                UpdateDate = c.UpdateDate,
                ProductCount = c.Products.Count
            }).ToListAsync();

            return categories;
        }

        public async Task CategoryCreateAsync(CategoryCreateDto model)
        {
            bool isExist = await _repository.GetQueryable().AnyAsync(c => c.Name.ToLower() == model.Name.ToLower());
            if (isExist)
            {
                throw new Exception("Bu isimde bir kategori zaten mevcut.");
            }
            var category = new Category
            {
                Name = model.Name,
                Description = model.Description,
                CreateDate = model.CreateDate,
            };
            await _repository.AddAsync(category);
            await _repository.SaveChangesAsync();
        }


        public async Task CategoryUpdateAsync(int id, CategoryUpdateDto model)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
            {
                throw new Exception("Kategori bulunamadı.");
            }
            category.Name = model.Name;
            category.Description = model.Description;
            category.UpdateDate = model.UpdateDate;
            _repository.Update(category);
            await _repository.SaveChangesAsync();
        }

        public async Task<CategoryUpdateDto> GetCategoryUpdateAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
            {
                throw new Exception("Kategori bulunamadı.");
            }
            return new CategoryUpdateDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
            };
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
            {
                throw new Exception("Kategori bulunamadı.");
            }
            _repository.Delete(category);
            await _repository.SaveChangesAsync();
        }
    }
}