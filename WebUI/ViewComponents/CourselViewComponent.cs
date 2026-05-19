using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.AdvertisementDTOs;
using Domain.Entities;
using E_Shopping.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.ViewComponents
{
    public class CourselViewComponent : ViewComponent
    {
        private readonly IRepository<Advertisement> _repository;

        public CourselViewComponent(IRepository<Advertisement> repository)
        {
            this._repository = repository;
        }
        [HttpGet]
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var advertisements = await _repository.GetAllWithIncludeAsync(x => x.Product);
            var result = advertisements.Select(x => new AdvertisementsViewDto
            {
                ImageUrl = x.ImageUrl,
                Description = x.Description,
                Title = x.Title,
                Id = x.Id,
                ProductId = x.ProductId,
                ProductName = x.Product?.Name

            }).ToList();
            return View(result);
        }
    }
}