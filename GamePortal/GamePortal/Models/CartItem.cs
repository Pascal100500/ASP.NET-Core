using Microsoft.EntityFrameworkCore;

namespace GamePortal.Models
{
    [Index(nameof(UserId), nameof(GameId), IsUnique = true)]
    public class CartItem
    {     
        public int Id { get; set; }

        public string UserId { get; set; } = "";

        public int GameId { get; set; }

        public Game Game { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
