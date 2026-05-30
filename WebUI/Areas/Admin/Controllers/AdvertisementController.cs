using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.AdvertisementDTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class AdvertisementController : Controller
    {
        private readonly IAdvertisementService _advertisementService;

        public AdvertisementController(IAdvertisementService advertisementService)
        {
            _advertisementService = advertisementService;
        }
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var advertisements = await _advertisementService.GetAllAdvertisements();
            return View(advertisements);
        }
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(AdvertisementCreateDto model)
        {
            await _advertisementService.CreateAdvertisement(model);
            return RedirectToAction("Index");
        }
        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _advertisementService.DeleteAdvertisement(id);
            return RedirectToAction("Index");
        }
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var advertisement = await _advertisementService.GetAdvertisementById(id);
            if (advertisement == null)
            {
                return NotFound();
            }
            var model = new AdvertisementUpdateDto
            {
                Id = advertisement.Id,
                Title = advertisement.Title,
                Description = advertisement.Description,
            };
            return View(model);
        }
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdvertisementUpdateDto model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }
            await _advertisementService.UpdateAdvertisement(id, model);
            return RedirectToAction("Index");
        }
        [HttpPost("Remove/{id}")]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(int id)
        {
            var advertisement = _advertisementService.GetAdvertisementById(id);
            if (advertisement == null)
            {
                return NotFound();
            }
            return View(advertisement);
        }
    }
}