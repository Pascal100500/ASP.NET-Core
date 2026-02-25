using GamePortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog.Context;

[Authorize(Roles = "Admin")] // Ограничиваю возможность работы с данной страницей. С ней может работать только Администратор
public class AddGameModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AddGameModel> _logger;


    public AddGameModel(ApplicationDbContext context, ILogger<AddGameModel> logger)
    {
        _context = context;
        _logger = logger;
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
        var adminEmail = User.Identity?.Name;
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknow"; // IP может быть null!!!

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

        // Лог о добавлении игры. Теперь уже использую Serilog ILogger
        using (LogContext.PushProperty("LogType", "Admin"))
        {
            _logger.LogInformation(
        "Добавлена игра GameId: {GameId} название: {Title} администратором: {Admin} | IP: {IP}",
        Game.Id,
        Game.Title,
        adminEmail,
        ip);
        }

        return RedirectToPage("/Games/Index");
    }
}
