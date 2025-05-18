using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketSales.Data;
using TicketSales.Models;
using TicketSales.ViewModels;

namespace TicketSales.Controllers
{
   
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> categories = _context.Categories.Where(m => !m.SoftDelete);
            IEnumerable<Event> events = _context.Events.Where(m => !m.SoftDelete).Include(m=>m.Tickets);
            HomeVm vm = new HomeVm()
            {
                Events = events,
                Categories = categories
            };
            return View(vm);
        }
        public IActionResult Detail(int id)
        {
            return View();
        }

    }
}
