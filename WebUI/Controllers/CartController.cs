using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebUI.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartManager;
        public CartController(ICartService cartManager)
        {
            _cartManager = cartManager;
        }


        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartManager.GetCartByUserIdAsync(userId);

            return View(cart);
        }
        [HttpPost("AddToCart/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int Id, int quantity = 1)
        {
            await _cartManager.AddToCartAsync(Id, quantity);
            return RedirectToAction("Index", "Home");
        }
        [HttpPost("Remove/{Id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int Id)
        {
            await _cartManager.RemoveFromCartAsync(Id);
            return RedirectToAction("Index");
        }

        [HttpPost("ClearCart")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            await _cartManager.ClearCartAsync();
            return RedirectToAction("Index");
        }
    }
}