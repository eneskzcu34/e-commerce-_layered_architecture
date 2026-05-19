using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.AdvertisementDTOs;
using Application.Interfaces;
using Domain.Entities;
using E_Shopping.Domain.Entities;
using E_Shopping.Domain.Interfaces.Repositories;

namespace Application.Managers
{
    public class AdvertisementManager : IAdvertisementService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Advertisement> _advertisementRepository;

        public AdvertisementManager(IRepository<Advertisement> advertisementRepository, IRepository<Product> productRepository)
        {
            _advertisementRepository = advertisementRepository;
            _productRepository = productRepository;
        }

        public async Task CreateAdvertisement(AdvertisementCreateDto model)
        {
            var product = await _productRepository.GetSingleWithIncludeAsync(p => p.ProductCode == model.ProductCode, p => p.Images);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            var advertisement = new Advertisement
            {
                ProductId = product.Id,
                Description = model.Description,
                ImageUrl = product.Images.FirstOrDefault(x => x.IsMain)?.ImageUrl,
                Title = model.Title,
                
            };
            await _advertisementRepository.AddAsync(advertisement);
            await _advertisementRepository.SaveChangesAsync();
        }
    }
}