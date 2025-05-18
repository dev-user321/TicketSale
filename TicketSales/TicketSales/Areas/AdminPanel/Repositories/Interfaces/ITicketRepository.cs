using TicketSales.Models;

namespace TicketSales.Areas.AdminPanel.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        IEnumerable<Ticket> GetAll();
        Ticket? GetById(int id);
        void Add(Ticket ticket);
        void Update(Ticket ticket);
        void SoftDelete(Ticket ticket);
        void Save();
    }
}
