using Marketify.Contracts.Cart;
using Marketify.Contracts.CartItem;
using Marketify.Date;
using Marketify.Entites;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Digests;

namespace Marketify.Services
{//
    public class CartServices(ApplicationDbContext dbContext) : ICartServices
    {
        private readonly ApplicationDbContext _context = dbContext;

        public async Task<CartResponse> GetCartAsync(string userId)
        {
            var cart = await _context.Carts.AsNoTracking()
                .Include(c => c.Items)
                .ThenInclude(x => x.Product)
                .ThenInclude(p => p.Images) 
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return new CartResponse(0, new List<CartItemResponse>(), 0, 0);

            string baseUrl = "http://localhost:4000";

            var itemsDto = cart.Items.Select(i => {
                var baseUrl = "http://localhost:4000";
                var imgUrl = i.Product?.Images?.FirstOrDefault()?.ImageUrl ?? "";

                var cleanPath = imgUrl.Replace("/images/", "").TrimStart('/');
                var finalImageUrl = $"{baseUrl}/images/{cleanPath}";

                return new CartItemResponse(
                    i.ProductId,
                    i.Product?.Name ?? "Unknown",
                    finalImageUrl,
                    i.Price,
                    i.Quantity,
                    i.SelectedSize,
                    i.TotalItemPrice
                );
            }).ToList();

            return new CartResponse(
                cart.Id,
                itemsDto,
                itemsDto.Sum(x => x.TotalItemPrice),
                itemsDto.Sum(x => x.Quantity)
            );
        }
        public async Task<bool> AddToCartAsync(string userId, CartItemRequest dto)
        {
            var cart = await _context.Carts
                .Include(c=>c.Items).
                FirstOrDefaultAsync(c=>c.UserId==userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }
            var existingItem = cart.Items
                .FirstOrDefault(i => i.ProductId
                == dto.ProductId && i.SelectedSize == dto.SelectedSize);
            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                var productPrice = await _context.Products
                    .Where(p => p.Id == dto.ProductId)
                    .Select(p => p.Price)
                    .FirstOrDefaultAsync();

                cart.Items.Add(new CartItem
                {
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    Price = productPrice,
                    SelectedSize = dto.SelectedSize
                });
            }

            return await _context.SaveChangesAsync() > 0;
        }

        


        public async Task<bool> ClearCartAsync(string userId)
        {
            var cartItems = await _context.CartItems
                .Where(i => i.Cart.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any()) return false;

            _context.CartItems.RemoveRange(cartItems);
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<bool> RemoveFromCartAsync(string userId, int productId)
        {
            var item = await _context.CartItems
                .FirstOrDefaultAsync(i => i.Cart.UserId == userId && i.ProductId == productId);
                        
            if (item == null) return false;

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public  async Task<bool> UpdateQuantityAsync(string userId, int productId, int newQuantity)
        {
            if (newQuantity <= 0)
                return await RemoveFromCartAsync(userId, productId);

            var item = await _context.CartItems
                .FirstOrDefaultAsync(i => i.Cart.UserId == userId && i.ProductId == productId);

            if (item == null) return false;

            item.Quantity = newQuantity;
            return await _context.SaveChangesAsync() > 0;
        }

    }
}
//strinAAAg!@fverfverv231
//garexe8149@pmdeal.com