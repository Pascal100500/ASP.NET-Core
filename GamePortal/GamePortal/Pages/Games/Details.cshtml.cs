using GamePortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Security.Claims;

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
        public bool IsInCart { get; set; } // Флаг для проверки добавлена игра в корзину или нет
        public Game? Game { get; set; }

        // Операция READ по id
        public IActionResult OnGet(int id)
        {
            Game = _context.Games.AsNoTracking().FirstOrDefault(g => g.Id == id); //Добавил метод AsNoTracking() чтобы не отслеживать обект Game и уксорить работу программы

            if (Game == null)
            {
                return NotFound();
            }

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                IsPurchased = _context.Purchases
                    .Any(p => p.GameId == id && p.UserId == userId);

                IsInCart = _context.CartItems
                     .Any(c => c.GameId == id && c.UserId == userId);
            }

            return Page();
        }

        public IActionResult OnPost(int id)
        {
            Console.WriteLine("POST triggered");// для логов
            Console.WriteLine($"id = {id}");// для логов
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"userId = {userId}");// для логов

            var game = _context.Games.FirstOrDefault(g => g.Id == id);

            if (game == null || !game.IsActive)
            {
                TempData["ErrorMessage"] = "Игра недоступна.";
                Console.WriteLine("Game null or inactive");// лог
                return RedirectToPage();
            }


            // Проверка, куплена ли игра ранее
            var alreadyPurchased = _context.Purchases.Any(p => p.GameId == id && p.UserId == userId);
            if (alreadyPurchased)
            {
                TempData["ErrorMessage"] = "Игра уже куплена.";
                Console.WriteLine("Game already buy"); // лог
                return RedirectToPage();
            }

            //Проверка, есть ли игра в корзине
            var alreadyInCart = _context.CartItems.Any(c => c.GameId == id && c.UserId == userId);
            if (alreadyInCart)
            {
                TempData["ErrorMessage"] = "Игра уже добавлена в корзину.";
                Console.WriteLine("Game already in Purchase");//лог
                return RedirectToPage();
            }

            var cartItem = new CartItem
            {
                GameId = id,
                UserId = userId!,
            };
            

            try
            {
                _context.CartItems.Add(cartItem);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Игра успешно добавлена в корзину!";
                Console.WriteLine("Game buy!");//лог
                using (LogContext.PushProperty("LogType", "User"))
                {
                    _logger.LogInformation("Пользователь {UserId} добавил игру {GameId} в корзину", userId, id);
                }
            }
            catch (DbUpdateException) // ЭТО ИСКЛЮЧЕНИЕ НЕ ДОЛЖНО ПРОИСХОДИТЬ, так как кнопка "уже в корзине" не отправляет POST
            //но возможен race-condition при одновременных запросах
            {
                using (LogContext.PushProperty("LogType", "User"))
                {
                    _logger.LogWarning("Попытка повтороного добавления в корзину пользователем {UserId} игры {GameId}", userId, id);
                }

                TempData["ErrorMessage"] = "Игра уже находится в корзине";
            }
            Console.WriteLine($"alreadyPurchased = {alreadyPurchased}");// лог
            Console.WriteLine($"alreadyInCart = {alreadyInCart}");// лог
            Console.WriteLine($"GameId = {id}");
            return RedirectToPage(new { id }); // Добавил переход по id, чтобы после покупки точно вернуться к страницы игры, которую купили
            //Чтобы избежать случае с получением методом OnGet(int id) значения id=0, иначе игра не найдется
        }

    }
}
