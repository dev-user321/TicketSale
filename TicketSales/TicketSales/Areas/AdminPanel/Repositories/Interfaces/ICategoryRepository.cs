using TicketSales.Models;

namespace TicketSales.Areas.AdminPanel.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAll();
        Category? GetById(int id);
        void Add(Category category);
        void Update(Category category);
        void SoftDelete(Category category);
        void Save();
    }
}
