using System.ComponentModel.DataAnnotations;

namespace TicketSales.ViewModels
{
    public class RegisterVm
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; }
    }
}
