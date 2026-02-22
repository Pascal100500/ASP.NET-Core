using GamePortal.Models;
using GamePortal.Pages.Games;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GamePortal.Pages.Cart
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ApplicationDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<CartItem> CartItems { get; set; } = new();
        public decimal Total { get; set; }

        public void OnGet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Хотя есть защита [Authorize], но дополнительно
            if (userId == null)
            {
                return;
            }

            CartItems = _context.CartItems
                .AsNoTracking() // //Добавил метод AsNoTracking() чтобы не отслеживать обект CartItems и уксорить работу программы
                .Include(c => c.Game)
                .Where(c => c.UserId == userId)
                .ToList();

            Total = CartItems
                .Where(c => c.Game!.IsActive)
                .Sum(c => c.Game!.Price);
        }

        // Для удаления игры
        public async Task<IActionResult> OnPostRemove(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCheckout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItems = await _context.CartItems
                .Include(c => c.Game)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            var activeItems = cartItems
                .Where(c => c.Game != null && c.Game.IsActive)
                .ToList();

            if (!activeItems.Any())
            {
                TempData["ErrorMessage"] = "Нет доступных для покупки игр.";
                return RedirectToPage();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Получаем уже купленные игры
                var purchasedGameIds = await _context.Purchases
                    .Where(p => p.UserId == userId)
                    .Select(p => p.GameId)
                    .ToListAsync();

                foreach (var item in activeItems)
                {
                    if (purchasedGameIds.Contains(item.GameId))
                        continue;
                    var purchase = new Purchase
                    {
                        UserId = userId!,
                        GameId = item.GameId,
                        PriceAtPurchase = item.Game!.Price,
                        PurchaseDate = DateTime.UtcNow
                    };

                    _context.Purchases.Add(purchase);
                }

                _context.CartItems.RemoveRange(activeItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Покупка успешно завершена!";
                //_logger.LogInformation("Пользователь {UserId} добавил игру {GameId} в корзину", userId, id);
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Ошибка при оформлении заказа.";
            }

            return RedirectToPage();
        }

    }
}
