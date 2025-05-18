using TicketSales.Models;

namespace TicketSales.ViewModels
{
    public class HomeDetailVm
    {
        public Event Event { get; set; }
        public IEnumerable<Ticket> Tickets { get; set; }
        public IEnumerable<Event> LoadEvents { get; set; }  
    }
}
