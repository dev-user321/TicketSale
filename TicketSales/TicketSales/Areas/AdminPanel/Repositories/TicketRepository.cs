using Microsoft.EntityFrameworkCore;
using TicketSales.Areas.AdminPanel.Repositories.Interfaces;
using TicketSales.Data;
using TicketSales.Models;

namespace TicketSales.Areas.AdminPanel.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AppDbContext _context;

        public TicketRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Ticket> GetAll()
        {
            return _context.Tickets
                .Where(t => !t.SoftDelete)
                .Include(t => t.Event)
                .ToList();
        }

        public Ticket? GetById(int id)
        {
            return _context.Tickets
                .Include(t => t.Event)
                .FirstOrDefault(t => t.Id == id && !t.SoftDelete);
        }

        public void Add(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
        }

        public void Update(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
        }

        public void SoftDelete(Ticket ticket)
        {
            ticket.SoftDelete = true;
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
