using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.DTOs.SettingsDTOs;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebUI.Controllers
{
    [Route("[controller]")]
    public class SettingsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public SettingsController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
        [HttpGet("Addresses")]
        public IActionResult Addresses()
        {
            return View();
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