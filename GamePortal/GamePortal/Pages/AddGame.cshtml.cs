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
    public SelectList Categories { get; set; } // Теперь категории всегда должны быть заполнены
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
            //-----------------------------------------------------------
            // Проверкаверно ли добавляется категория с логированием
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                using (LogContext.PushProperty("LogType", "Admin"))
                {
                    _logger.LogError("ModelState error: {Error}", error.ErrorMessage);
                }
            }
            //---------------------------------------------------------------
            Categories = new SelectList(_context.Categories, "Id", "Name"); // Чтобы получить категорию игры и при перезагрузке Categories было заполнено
            return Page();
        }
        Game.IsActive = true; // при добавлении игра доступна для продажи
        Game.CreatedAt = DateTime.UtcNow;

        // Проверка если проблемы при добавлении игры и лог
        try
        {
            _context.Games.Add(Game); // Добавление игры операция CREATE
            _context.SaveChanges(); // Выполнение SQL запроса для реализации операции CREATE (INSERT в БД)
        }
        catch (Exception ex)
        {
            using (LogContext.PushProperty("LogType", "Admin"))
            {
                _logger.LogError(ex.Message);
                throw;
            }    
        }

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
