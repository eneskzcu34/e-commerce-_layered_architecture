using System.Diagnostics;
using Application.DTOs.CategoryDTOs;
using Application.DTOs.ProductDTOs;
using Application.DTOs.ProductsDTOs;
using Application.Interfaces;
using E_Shopping.Domain.Entities;
using E_Shopping.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace Application.Managers
{
    public class ProductManager : IProductService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        Random rnd = new Random();
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;

        public ProductManager(IRepository<Product> productRepository, IRepository<Category> categoryRepository, IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task CreateProductAsync(ProductCreateDto model)
        {
            string code;
            bool exist;
            do
            {
                code = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();
                exist = await _productRepository.AnyAsync(p => p.ProductCode == code);
            } while (exist);

            var product = new Product
            {
                Name = model.Name,
                ProductCode = code,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                CreateDate = model.CreatedAt,
                CategoryId = model.CategoryId,
            };
            foreach (var file in model.Images)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "images/products", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                product.Images.Add(new ProductImages
                {
                    ImageUrl = "/images/products/" + fileName,
                    IsMain = !product.Images.Any(x => x.IsMain)
                });
            }
            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();
        }
        public async Task<List<ProductViewDto>> GetAllProducts()
        {
            var sw = Stopwatch.StartNew();

            var products = await _productRepository.GetAllWithIncludeAsync(p => p.Category, p => p.Images);
            return products.Select(p => new ProductViewDto
            {
                Id = p.Id,
                ProductCode = p.ProductCode,
                MainImageUrl = p.Images.FirstOrDefault(x => x.IsMain)?.ImageUrl,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                CategoryName = p.Category.Name,
                IsActive = p.IsActive

            }).ToList();

        }
        public async Task<ProductUpdateDto> GetUpdateProductAsync(int id)
        {
            var product = await _productRepository.GetSingleWithIncludeAsync(p => p.Id == id, p => p.Category, p => p.Images);
            return new ProductUpdateDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                IsActive = product.IsActive,
                Categories = new SelectList(await GetAllCategories(), "Id", "Name", product.CategoryId),

                Images = product.Images.Select(img => new ProductImagesDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl,
                    IsMain = img.IsMain
                }).ToList(),

                SelectedMainImageId = product.Images.FirstOrDefault(x => x.IsMain)?.Id
            };
        }

        public async Task UpdateProductAsync(int id, ProductUpdateDto model)
        {
            var product = await _productRepository.GetSingleWithIncludeAsync(p => p.Id == id, p => p.Category, p => p.Images);
            if (product == null) return;

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Stock = model.Stock;
            product.CategoryId = model.CategoryId;
            product.IsActive = model.IsActive;


            if (model.SelectedMainImageId.HasValue)
            {
                foreach (var img in product.Images)
                {
                    img.IsMain = img.Id == model.SelectedMainImageId.Value;
                }
            }
            string folder = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");

            for (int i = 0; i < model.Images.Count; i++)
            {
                var dtoImage = model.Images[i];

                if (dtoImage.NewImage != null && dtoImage.NewImage.Length > 0)
                {
                    var existingImage = product.Images.FirstOrDefault(x => x.Id == dtoImage.Id);

                    if (existingImage != null)
                    {
                        var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, existingImage.ImageUrl.TrimStart('/')
                        );

                        if (File.Exists(oldPath))
                            File.Delete(oldPath);

                        var fileName = Guid.NewGuid() + Path.GetExtension(dtoImage.NewImage.FileName);
                        var path = Path.Combine(folder, fileName);

                        using var stream = new FileStream(path, FileMode.Create);
                        await dtoImage.NewImage.CopyToAsync(stream);

                        existingImage.ImageUrl = "/images/products/" + fileName;
                    }
                }
            }
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
        }
        public async Task<List<CategoryViewDto>> GetAllCategories()
        {
            var categories = (await _categoryRepository.GetAllAsync()).Select(c => new CategoryViewDto
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            return categories;
        }

        public async Task<ProductViewDto> DetailProductAsync(string name)
        {
            var product = await _productRepository.GetSingleWithIncludeAsync(p => p.Name == name, p => p.Category, p => p.Images);
            if (product == null)
            {
                return null;
            }
            return new ProductViewDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryName = product.Category.Name,
                Images = product.Images.OrderByDescending(img => img.IsMain).Select(img => new ProductImagesDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl,
                    IsMain = img.IsMain
                }).ToList()
            };
        }

        public async Task<List<ProductViewDto>> GetProductsBySearchAsync(string searchTerm)
        {
            var products = _productRepository.GetQueryable();
            var result = await products.Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p => p.Name.ToLower().Contains(searchTerm.ToLower()) ||
                p.Description.ToLower().Contains(searchTerm.ToLower()) ||
                p.Category.Name.ToLower().Contains(searchTerm.ToLower()))
            .Select(p => new ProductViewDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                CategoryName = p.Category.Name,
                MainImageUrl = p.Images.Where(x => x.IsMain).Select(x => x.ImageUrl).FirstOrDefault()
            }).ToListAsync();

            return result;
        }

        public async Task<List<ProductViewDto>> FilterProductsAsync(List<string> categories, string price, string sort)
        {
            var query = _productRepository.GetQueryable();

            if (categories != null && categories.Any())
            {
                query = query.Where(p => categories.Contains(p.Category.Name));
            }

            if (!string.IsNullOrEmpty(price))
            {
                switch (price)
                {
                    case "0-500":
                        query = query.Where(p => p.Price >= 0 && p.Price <= 500);
                        break;

                    case "500-1000":
                        query = query.Where(p => p.Price >= 500 && p.Price <= 1000);
                        break;

                    case "1000+":
                        query = query.Where(p => p.Price >= 1000);
                        break;
                }
            }
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "name-asc":
                        query = query.OrderBy(p => p.Name);
                        break;

                    case "name-desc":
                        query = query.OrderByDescending(p => p.Name);
                        break;

                    case "price-asc":
                        query = query.OrderBy(p => p.Price);
                        break;

                    case "price-desc":
                        query = query.OrderByDescending(p => p.Price);
                        break;
                }
            }
            return await query
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Select(p => new ProductViewDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                    MainImageUrl = p.Images
                        .Where(x => x.IsMain)
                        .Select(x => x.ImageUrl)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }
    }
}