using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.DTOs.AdressesDTOs;
using Application.DTOs.SettingsDTOs;
using E_Shopping.Domain.Entities;
using E_Shopping.Domain.Interfaces.Repositories;
using E_Shopping.Infrastructure.Persistence.Repositories;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebUI.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IRepository<AppUser> _userRepository;
        public SettingsController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IRepository<AppUser> userRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
        }

        [HttpGet("UserProfile")]
        public async Task<IActionResult> UserProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var model = new UserProfileDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };
            return View(model);
        }

        [HttpPost("UserProfile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserProfileAsync(UserProfileDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;


            await _userManager.UpdateAsync(user);


            return View("UserProfile", model);
        }

        [HttpGet("Change-Password")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost("Change-Password")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePasswordAsync(UserChangePassword model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            var currentPasswordValid = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (!currentPasswordValid)
            {
                ModelState.AddModelError("", "Mevcut şifre yanlış.");
                return View(model);
            }
            if (model.NewPassword != model.ConfirmNewPassword)
            {
                ModelState.AddModelError("", "Yeni şifreler eşleşmiyor.");
                return View(model);
            }
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            return RedirectToAction("UserProfile");
        }




        [HttpGet("Addresses")]
        public async Task<IActionResult> Addresses()
        {
            var user = await _userManager.GetUserAsync(User);
            var result = await _userRepository.GetSingleWithIncludeAsync(u => u.Id == user.Id, u => u.Address);

            var allDto = new AdressesAllDto();

            if (result.Address != null)
            {
                allDto.ViewDto = new AdressesViewDto
                {
                    City = result.Address.City,
                    District = result.Address.District,
                    Neighborhood = result.Address.Neighborhood,
                    Title = result.Address.Title,
                    FullAddress = result.Address.FullAddress
                };

                allDto.UpdateDto = new AdressesUpdateDto
                {
                    City = result.Address.City,
                    District = result.Address.District,
                    Neighborhood = result.Address.Neighborhood,
                    Title = result.Address.Title,
                    FullAddress = result.Address.FullAddress
                };
            }
            else
            {
                allDto.CreateDto = new AdressesCreateDto();
            }

            return View(allDto);
        }
        [HttpPost("Addresses")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> UpdateAddress(AdressesAllDto model)
        {
            if (model.UpdateDto == null)
            {
                return RedirectToAction(nameof(Addresses));
            }

            var user = await _userManager.GetUserAsync(User);
            var result = await _userRepository.GetSingleWithIncludeAsync(u => u.Id == user.Id, u => u.Address);

            if (result == null) return NotFound();

            if (result.Address == null)
            {
                result.Address = new Address();
            }

            result.Address.City = model.UpdateDto.City;
            result.Address.District = model.UpdateDto.District;
            result.Address.Neighborhood = model.UpdateDto.Neighborhood;
            result.Address.Title = model.UpdateDto.Title;
            result.Address.FullAddress = model.UpdateDto.FullAddress;

            _userRepository.Update(result);
            await _userRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Addresses));
        }
        [HttpPost("Create-Address")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAddress(AdressesAllDto model)
        {
            if (model.CreateDto == null)
            {
                return RedirectToAction(nameof(Addresses));
            }

            var user = await _userManager.GetUserAsync(User);
            var result = await _userRepository.GetSingleWithIncludeAsync(u => u.Id == user.Id, u => u.Address);

            if (result == null) return NotFound();

         
            if (result.Address != null)
            {
                return RedirectToAction(nameof(Addresses));
            }

            result.Address = new Address
            {
                UserId = user.Id,
                City = model.CreateDto.City,
                District = model.CreateDto.District,
                Neighborhood = model.CreateDto.Neighborhood,
                Title = model.CreateDto.Title,
                FullAddress = model.CreateDto.FullAddress
            };

            _userRepository.Update(result);
            await _userRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Addresses));
        }


        [HttpGet("Orders")]
        public IActionResult Orders()
        {
            return View();
        }
        [HttpGet("DeleteMyAccount")]
        public IActionResult DeleteMyAccount()
        {
            return View();
        }
        [HttpPost("DeleteMyAccount")]
        public async Task<ActionResult> DeleteMyAccountAsync(UserDeleteDto model)
        {
            var user = await _userManager.GetUserAsync(User);

            var passwordValid = await _userManager.CheckPasswordAsync(
                user,
                model.Password);

            if (!passwordValid)
            {
                ModelState.AddModelError("", "Şifre yanlış.");
                return View(model);
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}