using TicketSales.Models;

namespace TicketSales.ViewModels
{
    public class BuyTicketVm
    {
        public int EventId { get; set; }
        public List<string> SelectedSeats { get; set; }
    }
}
