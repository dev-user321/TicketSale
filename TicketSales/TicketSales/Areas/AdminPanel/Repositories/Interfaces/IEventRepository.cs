using TicketSales.Models;

namespace TicketSales.Areas.AdminPanel.Repositories.Interfaces
{
    public interface IEventRepository
    {
        IEnumerable<Event> GetAll();
        Event? GetById(int id);
        void Add(Event evt);
        void Update(Event evt);
        void SoftDelete(Event evt);
        void Save();
    }
}
