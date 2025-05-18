using System.ComponentModel.DataAnnotations;

namespace TicketSales.ViewModels
{
    public class EventCreateVm
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public IFormFile Image { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
}
