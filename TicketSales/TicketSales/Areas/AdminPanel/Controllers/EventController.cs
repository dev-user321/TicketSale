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
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var evt = _context.Events.FirstOrDefault(c => c.Id == id && !c.SoftDelete);
            if (evt == null)
            {
                return NotFound();
            }

            evt.SoftDelete = true;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var evt = _context.Events.FirstOrDefault(c => c.Id == id && !c.SoftDelete);
            ViewBag.Categories = new SelectList(_context.Categories.Where(c => !c.SoftDelete), "Id", "CategoryName");
            var oldEvt = new EventCreateVm()
            {
                Title = evt.Title,
                Description = evt.Description,
                Location = evt.Location,
                CategoryId = evt.CategoryId,
                ImageLink = evt.ImageLink,
            };

            return View(oldEvt);
        }

        [HttpPost]
        public IActionResult Edit(int id, EventCreateVm model)
        {
       
            var evt = _context.Events.FirstOrDefault(c => c.Id == id && !c.SoftDelete);
            if (evt == null)
            {
                return NotFound();
            }

            evt.Title = model.Title;
            evt.Description = model.Description;
            evt.Location = model.Location;
            evt.CategoryId = model.CategoryId;

            if (model.Image != null)
            {
                string oldImagePath = Path.Combine(_env.WebRootPath, "img", evt.ImageLink ?? "");
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                string newFileName = Guid.NewGuid() + Path.GetExtension(model.Image.FileName);
                string newImagePath = Path.Combine(_env.WebRootPath, "img", newFileName);
                using (FileStream stream = new FileStream(newImagePath, FileMode.Create))
                {
                    model.Image.CopyTo(stream);
                }

                evt.ImageLink = newFileName;
            }
            else
            {
                evt.ImageLink = evt.ImageLink;
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
