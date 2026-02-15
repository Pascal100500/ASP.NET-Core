using GamePortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GamePortal.Pages.Games
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Game? Game { get; set; }

        // Операция READ по id
        public IActionResult OnGet(int id)
        {
            Game = _context.Games.FirstOrDefault(g => g.Id == id);

            if (Game == null)
            {
                return NotFound();
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
            /*
            var alreadyPurchased = _context.Purchases
            .Any(p => p.GameId == id && p.UserId == userId);

            if (alreadyPurchased)
            {
                TempData["SuccessMessage"] = "Вы уже купили эту игру!";
                return RedirectToPage();
            }
            */

            // Сама покупка игры
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
            }
            catch (DbUpdateException)
            {
                TempData["SuccessMessage"] = "Вы уже купили эту игру!";
            }

            return RedirectToPage();
        }

    }
}
