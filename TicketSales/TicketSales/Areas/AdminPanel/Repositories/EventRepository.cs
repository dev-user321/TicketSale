using Microsoft.EntityFrameworkCore;
using TicketSales.Areas.AdminPanel.Repositories.Interfaces;
using TicketSales.Data;
using TicketSales.Models;

namespace TicketSales.Areas.AdminPanel.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Event> GetAll()
        {
            return _context.Events
                .Where(e => !e.SoftDelete)
                .Include(e => e.Category)
                .ToList();
        }

        public Event? GetById(int id)
        {
            return _context.Events.FirstOrDefault(e => e.Id == id && !e.SoftDelete);
        }

        public void Add(Event evt)
        {
            _context.Events.Add(evt);
        }

        public void Update(Event evt)
        {
            _context.Events.Update(evt);
        }

        public void SoftDelete(Event evt)
        {
            evt.SoftDelete = true;
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
