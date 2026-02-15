using GamePortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GamePortal.Pages.Games
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Game Game { get; set; } = new();

        //Получение данных для изменения!
        // Еще один READ по id
        public IActionResult OnGet(int id)
        {
            var gameFromDb = _context.Games.Find(id);

            if (gameFromDb == null)
                return NotFound();

            Game = gameFromDb;
            return Page();
        }

        // Реализация операции UPDATE
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Attach(Game).State = Microsoft.EntityFrameworkCore.EntityState.Modified; // Изменить уже существующий объект (UPDATE)

            // Вывод сообщения об изменении сущности в консоль (до изменений)
            foreach (var entry in _context.ChangeTracker.Entries())
            {
                Console.WriteLine($"BEFORE SaveChanges → Entity: {entry.Entity.GetType().Name}, State: {entry.State}");
            }


            /*
             Почему я использую Attach + Modified, а не _context.Update(Game):
            Update() рекурсивно помечает ВСЕ связанные сущности как Modified.
            Attach + Modified позволяет контролировать и вносить изменения только в помеченную сущность.
             */
            _context.SaveChanges();

            // Вывод сообщения об изменении сущности в консоль (после изменений)
            foreach (var entry in _context.ChangeTracker.Entries())
            {
                Console.WriteLine($"AFTER SaveChanges → Entity: {entry.Entity.GetType().Name}, State: {entry.State}");
            }


            return RedirectToPage("Index");
        }
    }
}