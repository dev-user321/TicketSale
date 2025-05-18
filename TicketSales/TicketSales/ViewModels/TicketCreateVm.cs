using System.ComponentModel.DataAnnotations;

namespace TicketSales.ViewModels
{
    public class TicketCreateVm
    {
        [Required]
        public int EventId { get; set; }
        [Required]
        public string SeatCount { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int TotalCount { get; set; }
    }
}
