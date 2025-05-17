namespace TicketSales.Models
{
    public class AppUser : BaseEntity
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool EmailConfirm { get; set; } = false;
        public string Role { get; set; }
    }
}
