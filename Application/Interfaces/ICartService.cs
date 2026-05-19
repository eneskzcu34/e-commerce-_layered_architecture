using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.CartDTOs;

namespace Application.Interfaces
{
    public interface ICartService
    {
        Task AddToCartAsync(int productId, int quantity = 1);
        Task RemoveFromCartAsync(int cartItemId);
        Task UpdateCartItemAsync(int cartItemId, int quantity);
        Task ClearCartAsync();
        Task<CartDto> GetCartByUserIdAsync(string userId);
    }
}