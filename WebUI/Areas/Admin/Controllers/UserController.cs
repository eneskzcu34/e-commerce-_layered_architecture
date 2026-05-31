using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.UserDTOs;
using Azure.Messaging;
using E_Shopping.Domain.Interfaces.Repositories;
using Infrastructure.Identity;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly AppDbContext _appDbContext;


        public UserController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, AppDbContext appDbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _appDbContext = appDbContext;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Index()
        {
            var users = await (
                                from user in _userManager.Users
                                join userRole in _appDbContext.UserRoles
                                    on user.Id equals userRole.UserId into userRoles
                                from userRole in userRoles.DefaultIfEmpty()
                                join role in _appDbContext.Roles
                                    on userRole.RoleId equals role.Id into roles
                                from role in roles.DefaultIfEmpty()
                                select new UserViewDto
                                {
                                    Id = user.Id,
                                    UserName = user.UserName,
                                    FirstName = user.FirstName,
                                    LastName = user.LastName,
                                    EMail = user.Email,
                                    PhoneNumber = user.PhoneNumber,
                                    Role = role.Name
                                }
                                ).ToListAsync();

            return View(users);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var model = new UserCreateDto();
            model.SelectLists = await GetRoles();
            return View(model);
        }
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _userManager.FindByEmailAsync(model.EMail);
            if (existingUser != null)
            {
                ModelState.AddModelError("EMail", "Bu email zaten kayıtlı.");
                model.SelectLists = await GetRoles();
                return View(model);
            }
            var user = new AppUser
            {
                FirstName = model.FirstName.ToLower(),
                LastName = model.LastName.ToLower(),
                UserName = model.UserName.ToLower(),
                Email = model.EMail.ToLower(),
            };


            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                model.SelectLists = await GetRoles();
                return View(model);
            }

            var role = await _roleManager.FindByIdAsync(model.RoleId);

            if (role != null)
            {
                await _userManager.AddToRoleAsync(user, role.Name);
            }

            return RedirectToAction("Index");

        }
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            var userRoleName = (await _userManager.GetRolesAsync(user))
                .FirstOrDefault();


            var role = userRoleName is null
                ? null
                : await _roleManager.FindByNameAsync(userRoleName);

            var result = new UserEditDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                EMail = user.Email,
                RoleId = role?.Id,
                SelectLists = new SelectList(
                    await _roleManager.Roles.ToListAsync(),
                    "Id",
                    "Name",
                    role?.Id
                )
            };

            return View(result);
        }
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(string id, UserEditDto model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.UserName = model.UserName;
            user.Email = model.EMail;


            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                model.SelectLists = await GetRoles();
                return View(model);
            }

            var currentRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(model.RoleId))
            {
                if (!string.IsNullOrWhiteSpace(currentRole))
                {
                    await _userManager.RemoveFromRoleAsync(user, currentRole);
                }
            }
            else
            {
                var newRole = await _roleManager.FindByIdAsync(model.RoleId);

                if (newRole != null && currentRole != newRole.Name)
                {
                    if (!string.IsNullOrWhiteSpace(currentRole))
                    {
                        await _userManager.RemoveFromRoleAsync(user, currentRole);
                    }
                    await _userManager.AddToRoleAsync(user, newRole.Name);
                }
            }

            if (!string.IsNullOrWhiteSpace(model.OldPassword)
                && !string.IsNullOrWhiteSpace(model.NewPassword))
            {
                var passwordResult = await _userManager.ChangePasswordAsync(
                    user,
                    model.OldPassword,
                    model.NewPassword
                );

                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    model.SelectLists = await GetRoles();
                    return View(model);
                }
            }

            return RedirectToAction("Index");
        }

        private async Task<SelectList> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return new SelectList(roles, "Id", "Name");
        }
    }
}