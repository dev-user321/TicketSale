using System.ComponentModel.DataAnnotations;
using TicketSales.Models;

namespace TicketSales.ViewModels
{
    public class HomeVm
    {
        [Required]
        public IEnumerable<Category> Categories { get; set; }
        [Required]
        public IEnumerable<Event> Events { get; set; }
    }
}
