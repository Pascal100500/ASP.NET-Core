using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GamePortal.Models;
using Microsoft.EntityFrameworkCore;

namespace GamePortal.Pages.Games
{
    public class TopModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public TopModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<Game> Games { get; set; } = new();
        public async Task OnGetAsync()
        {
            var games = await _context.Games
                .Where(g => g.IsTopGame && g.IsActive)
                .ToListAsync();

            Games = games
                .OrderBy(g => Guid.NewGuid())
                .Take(10)
                .ToList();
        }
    }
}
