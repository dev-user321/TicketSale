using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketSales.Areas.AdminPanel.Repositories.Interfaces;
using TicketSales.Data;
using TicketSales.Models;
using TicketSales.ViewModels;

namespace TicketSales.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = "Admin")]
    public class EventController : Controller
    {
        private readonly IEventRepository _eventRepo;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EventController(IEventRepository eventRepo, AppDbContext context, IWebHostEnvironment env)
        {
            _eventRepo = eventRepo;
            _context = context; // Still needed for ViewBag.Categories
            _env = env;
        }

        public IActionResult Index()
        {
            var events = _eventRepo.GetAll();
            return View(events);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories.Where(c => !c.SoftDelete), "Id", "CategoryName");
            return View();
        }

        [HttpPost]
        public IActionResult Create(EventCreateVm model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.Categories.Where(c => !c.SoftDelete), "Id", "CategoryName");
                ModelState.AddModelError("", "Zəhmət olmasa bütün sahələri doldurun.");
                return View(model);
            }

            string fileName = Guid.NewGuid() + Path.GetExtension(model.Image.FileName);
            string filePath = Path.Combine(_env.WebRootPath, "img", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                model.Image.CopyTo(stream);
            }

            var newEvent = new Event
            {
                Title = model.Title,
                Description = model.Description,
                Location = model.Location,
                ImageLink = fileName,
                CategoryId = model.CategoryId,
                CreatedDate = model.Created
            };

            _eventRepo.Add(newEvent);
            _eventRepo.Save();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var evt = _eventRepo.GetById(id);
            if (evt == null) return NotFound();

            _eventRepo.SoftDelete(evt);
            _eventRepo.Save();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var evt = _eventRepo.GetById(id);
            if (evt == null) return NotFound();

            ViewBag.Categories = new SelectList(_context.Categories.Where(c => !c.SoftDelete), "Id", "CategoryName");

            var model = new EventCreateVm
            {
                Title = evt.Title,
                Description = evt.Description,
                Location = evt.Location,
                CategoryId = evt.CategoryId
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(int id, EventCreateVm model)
        {
            var evt = _eventRepo.GetById(id);
            if (evt == null) return NotFound();

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

            _eventRepo.Update(evt);
            _eventRepo.Save();

            return RedirectToAction("Index");
        }

    }
}
