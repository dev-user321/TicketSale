namespace TicketSales.Models
{
    public class TicketOrder
    {
        public int Id { get; set; }
        public string SeatInfo { get; set; } 
        public decimal Price { get; set; }
        public DateTime PurchaseDate { get; set; }

        public int UserId { get; set; }
        public AppUser User { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
