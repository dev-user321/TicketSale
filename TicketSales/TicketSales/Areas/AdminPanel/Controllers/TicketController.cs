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
    public class TicketController : Controller
    {
        private readonly ITicketRepository _ticketRepo;
        private readonly AppDbContext _context; 
        public TicketController(ITicketRepository ticketRepo, AppDbContext context)
        {
            _ticketRepo = ticketRepo;
            _context = context;
        }

        public IActionResult Index()
        {
            var tickets = _ticketRepo.GetAll();
            return View(tickets);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Events = new SelectList(_context.Events.Where(e => !e.SoftDelete), "Id", "Title");
            return View();
        }

        [HttpPost]
        public IActionResult Create(TicketCreateVm ticketCreate)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Zəhmət olmasa bütün sahələri doldurun.");
                ViewBag.Events = new SelectList(_context.Events.Where(e => !e.SoftDelete), "Id", "Title");
                return View(ticketCreate);
            }

            var newTicket = new Ticket
            {
                SeatNumber = ticketCreate.SeatCount,
                Price = ticketCreate.Price,
                TotalCount = ticketCreate.TotalCount,
                EventId = ticketCreate.EventId
            };

            _ticketRepo.Add(newTicket);
            _ticketRepo.Save();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var ticket = _ticketRepo.GetById(id);
            if (ticket == null) return NotFound();

            ViewBag.Events = new SelectList(_context.Events.Where(e => !e.SoftDelete), "Id", "Title");

            var model = new TicketCreateVm
            {
                TotalCount = ticket.TotalCount,
                SeatCount = ticket.SeatNumber,
                Price = ticket.Price,
                EventId = ticket.EventId
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(int id, TicketCreateVm model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Events = new SelectList(_context.Events.Where(e => !e.SoftDelete), "Id", "Title");
                ModelState.AddModelError("", "Zəhmət olmasa bütün sahələri doldurun.");
                return View(model);
            }

            var ticket = _ticketRepo.GetById(id);
            if (ticket == null) return NotFound();

            ticket.TotalCount = model.TotalCount;
            ticket.SeatNumber = model.SeatCount;
            ticket.Price = model.Price;
            ticket.EventId = model.EventId;

            _ticketRepo.Update(ticket);
            _ticketRepo.Save();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var ticket = _ticketRepo.GetById(id);
            if (ticket == null) return NotFound();

            _ticketRepo.SoftDelete(ticket);
            _ticketRepo.Save();

            return RedirectToAction("Index");
        }
    }
}
