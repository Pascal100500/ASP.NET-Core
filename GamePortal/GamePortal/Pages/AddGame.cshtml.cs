using GamePortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GamePortal.Pages
{
    public class AddGameModel : PageModel
    {
        [BindProperty]
        public Game Game { get; set; } = new();

        public string Message { get; private set; } = "Добавление игры";

        public void OnGet()
        {
        }

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                Message = "Ошибка ввода данных";
                return;
            }

            Message = $"Добавлена игра: {Game.Title}";
        }
    }
}