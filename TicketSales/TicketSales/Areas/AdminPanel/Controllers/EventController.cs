using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketSales.Data;
using TicketSales.Models;
using TicketSales.ViewModels;

namespace TicketSales.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class EventController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public EventController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            IEnumerable<Event> events = _context.Events.Where(m=>!m.SoftDelete).Include(m=>m.Category);
            return View(events);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories.Where(c => !c.SoftDelete), "Id", "CategoryName");
            return View();
        }
        [HttpPost]
        public IActionResult Create(EventCreateVm eventCreate)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Zəhmət olmasa bütün sahələri doldurun.");
                return View(eventCreate);
            }

            string fileName = Guid.NewGuid().ToString() + eventCreate.Image.FileName;
            string path = Path.Combine(_env.WebRootPath, "img", fileName);
            using(FileStream stream = new FileStream(path, FileMode.Create))
            {
                eventCreate.Image.CopyTo(stream);
            };

            var newEvent = new Event()
            {
                Title = eventCreate.Title,
                Description = eventCreate.Description,
                Location = eventCreate.Location,
                ImageLink = fileName,
                CategoryId = eventCreate.CategoryId
            };

            _context.Events.Add(newEvent);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
