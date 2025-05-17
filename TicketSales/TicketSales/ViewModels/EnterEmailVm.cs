using System.ComponentModel.DataAnnotations;

namespace TicketSales.ViewModels
{
    public class EnterEmailVm
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
