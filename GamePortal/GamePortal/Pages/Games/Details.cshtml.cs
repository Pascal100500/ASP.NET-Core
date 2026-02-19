using GamePortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace GamePortal.Pages.Games
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(ApplicationDbContext context, ILogger<DetailsModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public bool IsPurchased { get; set; } // Флаг для проверки куплена ли игра или нет
        public Game? Game { get; set; }

        // Операция READ по id
        public IActionResult OnGet(int id)
        {
            Game = _context.Games.FirstOrDefault(g => g.Id == id);

            if (Game == null)
            {
                return NotFound();
            }

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                IsPurchased = _context.Purchases
                    .Any(p => p.GameId == id && p.UserId == userId);
            }

            return Page();
        }

        public IActionResult OnPost(int id)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                //return RedirectToPage("/Identity/Account/Login", new { area = "Identity" });
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Проверка, куплена ли игра или еще нет
            // Сейчас реализовано на базе уровне базы данных через [Index(nameof(UserId), nameof(GameId), IsUnique = true)]
                                 
            // При покупке кнопка просто изменится на "Куплено", а она не привязана к методу OnPost
            var purchase = new Purchase
            {
                GameId = id,
                UserId = userId!,
                PurchaseDate = DateTime.Now
            };
            

            try
            {
                _context.Purchases.Add(purchase);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Игра успешно куплена!";

                _logger.LogInformation("Пользователь {UserId} совершил покупку игры {GameId}", userId, id);
            }
            catch (DbUpdateException) // ЭТО ИСКЛЮЧЕНИЕ НЕ ДОЛЖНО ПРОИСХОДИТЬ, так как кнопка куплено не отправляет POST
            {
                _logger.LogWarning ("Ошибка покупки пользователем {UserId} игры {GameId}", userId, id);

                TempData["SuccessMessage"] = "Игра уже куплена";
            }

            return RedirectToPage();
        }

    }
}
