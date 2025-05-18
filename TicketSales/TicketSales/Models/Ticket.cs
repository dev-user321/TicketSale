namespace TicketSales.Models
{
    public class Ticket : BaseEntity
    {
        public int TotalCount { get; set; }
        public decimal Price { get; set; }
        public string SeatNumber { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
