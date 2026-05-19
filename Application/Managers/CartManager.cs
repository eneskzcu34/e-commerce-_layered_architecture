using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.DTOs.CartDTOs;
using Application.Interfaces;
using E_Shopping.Domain.Entities;
using E_Shopping.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Managers
{
    public class CartManager : ICartService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<CartItem> _cartItemRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartManager(
            IRepository<Product> productRepository,
            IRepository<Cart> cartRepository,
            IRepository<CartItem> cartItemRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _productRepository = productRepository;
            _cartRepository = cartRepository;
            _httpContextAccessor = httpContextAccessor;
            _cartItemRepository = cartItemRepository;
        }
        public async Task AddToCartAsync(int Id, int quantity = 1)
        {

            var product = await _productRepository.GetByIdAsync(Id);
            if (product == null)
                throw new Exception("Product not found");
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartRepository.GetSingleWithIncludeAsync(x => x.UserId == userId, x => x.CartItems);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                };

                await _cartRepository.AddAsync(cart);
            }
            var cartItem = cart.CartItems.Where(x => x.ProductId == Id).FirstOrDefault();

            if (cartItem != null)
            {
                cartItem.Quantity += 1;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = product.Id,
                    Quantity = quantity,
                    UnitPrice = product.Price
                });
            }
            await _cartRepository.SaveChangesAsync();
        }

        public async Task ClearCartAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartRepository.GetSingleWithIncludeAsync(x => x.UserId == userId, x => x.CartItems);
            if (cart == null) throw new Exception("Cart not found");
            _cartItemRepository.DeleteRange(cart.CartItems);
            await _cartItemRepository.SaveChangesAsync();
        }

        public async Task<CartDto> GetCartByUserIdAsync(string userId)
        {
            var cart = await _cartRepository.GetQueryable()
                        .Where(c => c.UserId == userId)
                        .Select(c => new CartDto
                        {
                            UserId = c.UserId,
                            TotalItemCount = c.CartItems.Sum(i => i.Quantity),
                            TotalPrice = c.CartItems.Sum(i => i.Quantity * i.UnitPrice),

                            Items = c.CartItems.Select(item => new CartItemDto
                            {
                                Id = item.Id,
                                ProductId = item.ProductId,
                                Quantity = item.Quantity,
                                UnitPrice = item.UnitPrice,
                                ProductName = item.Product.Name,
                                ImageUrl = item.Product.Images.Where(m => m.IsMain).Select(m => m.ImageUrl).FirstOrDefault()!
                            }).ToList()
                        }).FirstOrDefaultAsync();
            return cart;
        }

        public async Task RemoveFromCartAsync(int cartItemId)
        {
            var cartItem = await _cartItemRepository.GetByIdAsync(cartItemId);
            if (cartItem == null) throw new Exception("Cart item not found");
            _cartItemRepository.Delete(cartItem);
            await _cartItemRepository.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(int cartItemId, int quantity)
        {
            throw new NotImplementedException();
        }
    }
}