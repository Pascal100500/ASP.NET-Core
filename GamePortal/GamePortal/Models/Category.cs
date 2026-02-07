using System.ComponentModel.DataAnnotations;

namespace GamePortal.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public List<Game> Games { get; set; } = new();
    }
}