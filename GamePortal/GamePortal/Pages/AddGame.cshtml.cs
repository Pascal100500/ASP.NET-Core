using GamePortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize(Roles = "Admin")] // Ограничиваю возможность работы с данной страницей. С ней может работать только Администратор
public class AddGameModel : PageModel
{
    private readonly ApplicationDbContext _context;
    
    public AddGameModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Game Game { get; set; } = new();

    [TempData]
    public string? SuccessMessage { get; set; } // Для вывода сообщения об успешном дополнении игры!
    public SelectList? Categories { get; set; }
    public void OnGet()
    {
        Categories = new SelectList(_context.Categories, "Id", "Name");
    }
    // Опереция CREATE
    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            Categories = new SelectList(_context.Categories, "Id", "Name"); // Чтобы получить категорию игры и при перезагрузке Categories было заполнено
            return Page();
        }
        Game.IsActive = true; // при добавлении игра доступна для продажи
        Game.CreatedAt = DateTime.UtcNow;

        _context.Games.Add(Game); // Добавление игры операция CREATE
        _context.SaveChanges(); // Выполнение SQL запроса для реализации операции CREATE (INSERT в БД)

        SuccessMessage = $"Игра \"{Game.Title}\" успешно добавлена!";

        return RedirectToPage("/Games/Index");
    }
}