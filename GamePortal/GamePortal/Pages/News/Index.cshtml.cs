using GamePortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GamePortal.Pages.News
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }
       public List<Models.News> NewsList { get; set; } = new();
         public async Task OnGetAsync()
        {
            var news = await _context.News
               .Where(n => n.IsPublished)
               .ToListAsync();   

            NewsList = news
                .OrderBy(n => Guid.NewGuid()) // Перенес метод, чтобы перемешивание новостей происходило в память
                .Take(6)
                .ToList();
        }
    }
}
