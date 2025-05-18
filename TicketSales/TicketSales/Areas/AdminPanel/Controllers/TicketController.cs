using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketSales.Data;
using TicketSales.Models;
using TicketSales.ViewModels;

namespace TicketSales.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class TicketController : Controller
    {
        private readonly AppDbContext _context;
        public TicketController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Ticket> tickets = _context.Tickets.Where(m => !m.SoftDelete).Include(m=>m.Event);
            return View(tickets);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Events = new SelectList(_context.Events.Where(c => !c.SoftDelete), "Id", "Title");
            var ticket = _context.Tickets.FirstOrDefault(m => !m.SoftDelete && m.Id == id);
            var oldTicket = new TicketCreateVm()
            {
                TotalCount = ticket.TotalCount,
                SeatCount = ticket.SeatNumber,
                Price = ticket.Price,
                EventId = ticket.EventId
            };
            return View(oldTicket);
        }
        [HttpPost]
        public IActionResult Edit(int id,TicketCreateVm model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Zəhmət olmasa bütün sahələri doldurun.");
                return View(model);
            }
            var ticket = _context.Tickets.FirstOrDefault(m => !m.SoftDelete && m.Id == id);

            if(ticket != null)
            {
                ticket.TotalCount = model.TotalCount;
                ticket.SeatNumber = model.SeatCount;
                ticket.Price = model.Price;
                ticket.EventId = model.EventId; 
            };
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Events = new SelectList(_context.Events.Where(c => !c.SoftDelete), "Id", "Title");
            return View();
        }
        [HttpPost]
        public IActionResult Create(TicketCreateVm ticketCreate)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Zəhmət olmasa bütün sahələri doldurun.");
                return View(ticketCreate);
            }
            var newTicket = new Ticket()
            {
                SeatNumber = ticketCreate.SeatCount,
                Price = ticketCreate.Price,
                TotalCount = ticketCreate.TotalCount,
                EventId = ticketCreate.EventId
            };
            _context.Tickets.Add(newTicket);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var ticket = _context.Tickets.FirstOrDefault(c => c.Id == id && !c.SoftDelete);
            if (ticket == null)
            {
                return NotFound();
            }

            ticket.SoftDelete = true;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
