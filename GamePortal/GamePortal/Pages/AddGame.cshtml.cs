using GamePortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

    // Опереция CREATE
    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Games.Add(Game); // Добавление игры операция CREATE
        _context.SaveChanges(); // Выполнение SQL запроса для реализации операции CREATE (INSERT в БД)

        SuccessMessage = $"Игра \"{Game.Title}\" успешно добавлена!";

        return RedirectToPage("/Games/Index");
    }
}