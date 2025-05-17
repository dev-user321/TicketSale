namespace TicketSales.Models
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public bool SoftDelete { get; set; } = false;
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
    }
}
