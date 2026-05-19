using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using E_Shopping.Application.DTOs.AccountDTOs;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WebUI.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IOptions<IdentityOptions> identityOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _identityOptions = identityOptions;
        }



        [HttpGet("[action]")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Login(LoginDTo model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "Bu email ile kayıtlı bir kullanıcı bulunamadı.");
                return View(model);
            }
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
                return RedirectToAction("Index", "Home");
            }
            else if (result.IsLockedOut)
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                var lockMinutes = (int)_identityOptions.Value.Lockout.DefaultLockoutTimeSpan.TotalMinutes;

                ModelState.AddModelError("", $"Hesap kilitlendi. {lockMinutes} dakika bekle.Kalan {(lockoutEnd.Value - DateTimeOffset.UtcNow):mm\\:ss}");
            }
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Hesap kilitlendi");
            }
            else
            {
                ModelState.AddModelError("", "Kullanıcı adı veya şifre yanlış");
            }
            return View(model);
        }
        [HttpGet("[action]")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDTo model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Bu email zaten kayıtlı.");
                return View(model);
            }
            var user = new AppUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.FirstName + model.LastName,
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {

                return RedirectToAction("Login");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}