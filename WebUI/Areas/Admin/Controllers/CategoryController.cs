using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.CategoryDTOs;
using Application.DTOs.ProductsDTOs.CategoryDTOs;
using Application.Interfaces;
using E_Shopping.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.Extensions.Logging;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isExist = (await _categoryService.GetAllCategoriesAsync())
                .Any(c => c.Name.ToLower() == model.Name.ToLower());

            if (isExist)
            {
                ModelState.AddModelError("Name", "Bu isimde bir kategori zaten mevcut.");
                return View(model);
            }

            await _categoryService.CategoryCreateAsync(model);
            return RedirectToAction("Index");
        }
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryUpdateAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            bool isExist = (await _categoryService.GetAllCategoriesAsync())
                            .Any(c => c.Name.ToLower() == model.Name.ToLower());

            if (isExist)
            {
                ModelState.AddModelError("Name", "Bu isimde bir kategori zaten mevcut.");
                return View(model);
            }
            await _categoryService.CategoryUpdateAsync(id, model);
            return RedirectToAction("Index");
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return RedirectToAction("Index");
        }

    }

}
