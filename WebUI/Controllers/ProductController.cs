using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.ProductDTOs;
using Application.Interfaces;
using E_Shopping.Domain.Entities;
using E_Shopping.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.Extensions.Logging;

namespace WebUI.Controllers
{
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IRepository<Product> _repositoryProducts;

        public ProductController(IProductService productService, IRepository<Product> repositoryProducts)
        {
            _productService = productService;
            _repositoryProducts = repositoryProducts;
        }

        // GET: /Product/List
        [HttpGet("/Products/{name?}")]
        public async Task<IActionResult> Index(string? name)
        {
            var products = await _productService.GetAllProducts();
            var categories = await _productService.GetAllCategories();

            if (!string.IsNullOrEmpty(name))
            {
                products = products
                    .Where(x => x.CategoryName == name)
                    .ToList();
            }

            var model = new ProductPageViewModel
            {
                Products = products,
                Categories = categories
            };

            return View(model);
        }

        // GET: /Product/Detail/5
        [HttpGet("Detail/{name}")]
        public async Task<IActionResult> Detail(string name)
        {
            var product = await _productService.DetailProductAsync(name);
            return View(product);
        }

        [HttpGet("/Product/Search")]
        public async Task<IActionResult> Search(string searchTerm)
        {
            var products = await _productService.GetProductsBySearchAsync(searchTerm);

            var model = new ProductPageViewModel
            {
                Products = products,
                Categories = await _productService.GetAllCategories()
            };

            return View("Index", model);
        }
        [HttpGet("/Product/Filter")]
        public async Task<IActionResult> Filter(List<string> categories, string price, string sort)
        {
            var products = await _productService.FilterProductsAsync(categories, price, sort);

            var model = new ProductPageViewModel
            {
                Products = products,
                Categories = await _productService.GetAllCategories()
            };

            return View("Index", model);
        }
    }
}