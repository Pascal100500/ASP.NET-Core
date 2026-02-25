using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GamePortal.Models;

namespace GamePortal.Pages.News
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public Models.News? NewsItem { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            NewsItem = await _context.News
                .FirstOrDefaultAsync(n => n.Id == id && n.IsPublished);

            if (NewsItem == null)
                return NotFound();

            return Page();
        }
    }
}
