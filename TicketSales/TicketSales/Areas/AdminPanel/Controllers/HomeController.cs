using Microsoft.AspNetCore.Mvc;

namespace TicketSales.Areas.AdminPanel.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
