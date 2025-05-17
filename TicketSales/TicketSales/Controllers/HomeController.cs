using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TicketSales.Controllers
{
    [Area("AdminPanel")]
    public class HomeController : Controller
    {   
        public IActionResult Index()
        {
            return View();
        }
    }
}
