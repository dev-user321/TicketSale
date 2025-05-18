using System.Net.Sockets;

namespace TicketSales.Models
{
    public class Event : BaseEntity
    {
        public int CategoryId { get; set; }
        public string ImageLink { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location  { get; set; }
        public DateTime CreatedDate { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public Category Category { get; set; }
    }
}
