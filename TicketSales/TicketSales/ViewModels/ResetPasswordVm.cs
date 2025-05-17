using System.ComponentModel.DataAnnotations;

namespace TicketSales.ViewModels
{
    public class ResetPasswordVm
    {
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Parolun uzunlugu minimum 8 olmalidir.")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Yazdiginiz parolla eyni olmalidir !")]
        public string ConfirmPassword { get; set; }
    }
}
