using System.ComponentModel.DataAnnotations;

namespace GamePortal.Models
{
    public class Game
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название игры обязательно")]
        public string Title { get; set; } = "";

        [Required(ErrorMessage = "Краткое описание обязательно")]
        public string ShortDescription { get; set; } = "";

        [Required(ErrorMessage = "Полное описание обязательно")]
        public string FullDescription { get; set; } = "";

        [Range(0, 100000, ErrorMessage = "Цена должна быть больше или равна 0")]
        public decimal Price { get; set; }

        [Range(3, 18, ErrorMessage = "Возраст от 3 до 18")]
        public int AgeLimit { get; set; }

        public string? ImageUrl { get; set; } = "";

        public string? Screenshot1 { get; set; }
        public string? Screenshot2 { get; set; }
        public string? Screenshot3 { get; set; }

        public bool IsOnSale { get; set; }
        public bool IsTopGame { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Связь с категорией
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}