using TicketSales.Areas.AdminPanel.Repositories.Interfaces;
using TicketSales.Data;
using TicketSales.Models;

namespace TicketSales.Areas.AdminPanel.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Category> GetAll()
        {
            return _context.Categories.Where(c => !c.SoftDelete).ToList();
        }

        public Category? GetById(int id)
        {
            return _context.Categories.FirstOrDefault(c => c.Id == id && !c.SoftDelete);
        }

        public void Add(Category category)
        {
            _context.Categories.Add(category);
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }

        public void SoftDelete(Category category)
        {
            category.SoftDelete = true;
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
