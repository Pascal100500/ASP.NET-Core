using GamePortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace GamePortal.Pages.Games
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        //public List<Purchase> Purchases { get; set; } = new(); Если буду добаввлять покупку со страницы с играми

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Game> Games { get; set; } = new();

        public void OnGet()
        {
            Games = _context.Games.ToList(); // Операция SELECT для операции READ

            //==Возможная покупка==
           /* var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Purchases = _context.Purchases
                .Where(p => p.UserId == userId)
                .Include(p => p.Game)
                .ToList();
           */
        }
    }
}
