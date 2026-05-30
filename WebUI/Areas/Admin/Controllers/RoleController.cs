using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.RoleDTOs;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[controller]")]
    public class RoleController : Controller
    {
        private readonly RoleManager<AppRole> _roleManager;

        public RoleController(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }
        [HttpGet("/Roles")]
        public async Task<IActionResult> Index()
        {
            var result = await _roleManager.Roles
                    .Select(r => new RoleViewDto
                    {
                        Id = r.Id,
                        Name = r.Name
                    }).ToListAsync();
            return View(result);
        }
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleCreateDto model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var role = new AppRole
            {
                Name = model.Name
            };
            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
            return RedirectToAction("Index");
        }
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            var result = new RoleEditDto
            {
                Id = role.Id,
                Name = role.Name
            };
            return View(result);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, RoleEditDto model)
        {
            if (id == null)
                return View(model);
            var role = await _roleManager.FindByIdAsync(id);

            role.Name = model.Name;

            await _roleManager.UpdateAsync(role);
            return RedirectToAction("Index");
        }
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
                return RedirectToAction("Index");

            var role = await _roleManager.FindByIdAsync(id);
            if (id == role.Id)
            {
                var result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            return null;
        }
    }
}