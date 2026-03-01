using Microsoft.EntityFrameworkCore;

namespace GamePortal.Models
{
    [Index(nameof(UserId), nameof(GameId), IsUnique = true)]
    public class Purchase
    {
        public int Id { get; set; }

        public string UserId { get; set; } = "";

        public int GameId { get; set; }
        public decimal PriceAtPurchase { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        public Game? Game { get; set; }
    }
}
