using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.AdvertisementDTOs;
using Application.Interfaces;
using Domain.Entities;
using E_Shopping.Domain.Entities;
using E_Shopping.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

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

        public async Task DeleteAdvertisement(int id)
        {
            var advertisement = await _advertisementRepository.GetByIdAsync(id);
            if (advertisement == null)
            {
                throw new Exception("Advertisement not found");
            }
            _advertisementRepository.Delete(advertisement);
            await _advertisementRepository.SaveChangesAsync();
        }

        public async Task<AdvertisementsViewDto> GetAdvertisementById(int id)
        {
            var advertisement = await _advertisementRepository.GetSingleWithIncludeAsync(a => a.Id == id, a => a.Product);
            if (advertisement == null)
            {
                throw new Exception("Advertisement not found");
            }
            return new AdvertisementsViewDto
            {
                Id = advertisement.Id,
                Title = advertisement.Title,
                Description = advertisement.Description,
                ProductId = advertisement.ProductId,
                ProductName = advertisement.Product.Name,
                ImageUrl = advertisement.ImageUrl
            };
        }

        public async Task<List<AdvertisementsViewDto>> GetAllAdvertisements()
        {
            var advertisements = await _advertisementRepository.GetAllWithIncludeAsync(a => a.Product);
            var result = advertisements.Select(a => new AdvertisementsViewDto
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                ProductId = a.ProductId,
                ProductName = a.Product.Name,
                ImageUrl = a.ImageUrl,
                ProductCode = a.Product.ProductCode
            }).ToList();

            return result;
        }

        public async Task UpdateAdvertisement(int id, AdvertisementUpdateDto model)
        {
            var advertisement = await _advertisementRepository.GetByIdAsync(id);
            if (advertisement == null)
            {
                throw new Exception("Advertisement not found");
            }

            advertisement.Title = model.Title;
            advertisement.Description = model.Description;

            _advertisementRepository.Update(advertisement);
            await _advertisementRepository.SaveChangesAsync();
        }
    }
}