using GamePortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GamePortal.Pages.Games
{
    public class SaleModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public SaleModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<Game> Games { get; set; } = new();
        public async Task OnGetAsync()
        {
            var games = await _context.Games
                .Where(g => g.IsOnSale && g.IsActive)
                .ToListAsync();

            Games = games
                .OrderBy(g => Guid.NewGuid())
                .Take(10)
                .ToList();
        }
    }
}
