using GamePortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GamePortal.Pages.Games
{
    [Authorize(Roles = "Admin")] // Ограничиваю возможность работы с данной страницей. С ней может работать только Администратор
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Game? Game { get; set; }
        public string? SuccessMessage { get; set; } //Сообщение об успешном удалении игры

        // SELECT нужной игры ( по id)
        public IActionResult OnGet(int id)
        {
            Game = _context.Games.Find(id); // Поиск нужной игры (SELECT)

            if (Game == null)
                return NotFound();

            return Page();
        }

        // Реализация операции DELETE
        public IActionResult OnPost(int id)
        {
            var game = _context.Games.Find(id);

            if (game == null)
                return NotFound();

            /*
             Важно:
             Title сохраняется до удаления, иначе после удаления объекта его уже не будет.
            Потому что после удаления:
            объект может стать Detached
            а после Redirect у нас уже нет доступа к этому объекту
            и при попытке вывода сообщения, что выбранная игра удалена, 
            то нужного названия игры не будет
             */
            string deletedTitle = game.Title; 
            
            _context.Games.Remove(game); //Пометили выбранную игру к удалению (операция DELETE)
            _context.SaveChanges(); // Удаление из БД после сохранения

            SuccessMessage = $"Игра \"{deletedTitle}\" успешно удалена.";

            return RedirectToPage("Index");
        }
    }
}
