namespace GamePortal.Models
{
    public class Purchase
    {
        public int Id { get; set; }

        public string UserId { get; set; } = "";

        public int GameId { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        public Game? Game { get; set; }
    }
}