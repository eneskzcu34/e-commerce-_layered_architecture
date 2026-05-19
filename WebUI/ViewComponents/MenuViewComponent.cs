using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.CategoryDTOs;
using E_Shopping.Domain.Entities;
using E_Shopping.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;

namespace WebUI.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IRepository<Category> _categoryRepository;

        public MenuViewComponent(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _categoryRepository.GetAllWithIncludeAsync(x => x.Products);
            var result = categories.Select(p => new CategoryViewDto
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();
            return View(result);
        }
    }
}