using System.ComponentModel.DataAnnotations;

namespace GamePortal.Models
{
    public class Game
    {
        public int Id { get; set; }

        [Display(Name = "Название игры")]
        [Required(ErrorMessage = "Название игры обязательно")]       
        public string Title { get; set; } = "";

        [Display(Name = "Краткое описание")]
        [Required(ErrorMessage = "Краткое описание обязательно")]     
        public string ShortDescription { get; set; } = "";

        [Display(Name = "Полное описание")]
        [Required(ErrorMessage = "Полное описание обязательно")]
        public string FullDescription { get; set; } = "";

        [Display(Name = "Цена")]
        [Range(0, 100000, ErrorMessage = "Цена должна быть больше или равна 0")]
        public decimal Price { get; set; }

        [Display(Name = "Возраст")]
        [Range(3, 18, ErrorMessage = "Возраст от 3 до 18")]
        public int AgeLimit { get; set; }

        [Display(Name = "Логотип игры")]
        public string? ImageUrl { get; set; } = "";

        [Display(Name = "Скриншот 1")]
        public string? Screenshot1 { get; set; }

        [Display(Name = "Скриншот 2")]
        public string? Screenshot2 { get; set; }

        [Display(Name = "Скриншот 3")]
        public string? Screenshot3 { get; set; }

        public bool IsOnSale { get; set; }
        public bool IsTopGame { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true; //Если игра будет удалена админом из продажи, но потом вновь появится.

        // Связь с категорией
        [Display(Name = "Категория")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите категорию")] // Сделал поле обязательным. При этом категория не может начинаться с 0 (0 это пусто)
        public int CategoryId { get; set; } // теперь поле CategoryId не должно быть пустым
        public Category? Category { get; set; } // А категория для навигации пусть остается nullable, иначе SQLite ругается
    }
}
