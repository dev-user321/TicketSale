using System.ComponentModel.DataAnnotations;

namespace TicketSales.ViewModels
{
    public class CategoryCreateVm
    {
        [Required]
        public string CategoryName { get; set; }    
    }
}
