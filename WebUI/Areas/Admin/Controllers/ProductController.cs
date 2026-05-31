using Application.DTOs.ProductDTOs;
using Application.DTOs.ProductsDTOs;
using Application.Interfaces;
using E_Shopping.Domain.Entities;
using E_Shopping.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Serialization;

namespace MyApp.Namespace
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IRepository<Product> _repository;

        public ProductController(IProductService productService, IRepository<Product> repository)
        {
            _productService = productService;
            _repository = repository;
        }

        // GET: /Admin/Product/Index
        [HttpGet("[action]")]
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProducts();
            return View(products);
        }


        // GET: /Admin/Product/Create
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var result = await _productService.GetAllCategories();
            var model = new ProductCreateDto();
            model.Categories = new SelectList(result, "Id", "Name");
            return View(model);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                var result = await _productService.GetAllCategories();
                model.Categories = new SelectList(result, "Id", "Name");
                return View(model);
            }

            await _productService.CreateProductAsync(model);
            return RedirectToAction("Index");
        }


        // GET: /Admin/Product/Update
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetUpdateProductAsync(id);
            return View(product);
        }


        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                var product = await _productService.GetUpdateProductAsync(id);
                return View(product);
            }

            await _productService.UpdateProductAsync(id, model);
            return RedirectToAction("Index");
        }
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            _repository.Delete(product);
            return RedirectToAction("Index");
        }

    }
}
