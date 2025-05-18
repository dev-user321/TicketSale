using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using TicketSales.Data;
using TicketSales.Models;
using TicketSales.ViewModels;

namespace TicketSales.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> categories = _context.Categories.Where(m=>!m.SoftDelete);
            return View(categories);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();  
        }
        [HttpPost]
        public IActionResult Create(CategoryCreateVm categoryCreate)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Zəhmət olmasa bütün sahələri doldurun.");
                return View(categoryCreate);
            }

            var newCategory = new Category()
            {
                CategoryName = categoryCreate.CategoryName,
            };
            _context.Categories.Add(newCategory);   
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id && !c.SoftDelete);
            if (category == null)
            {
                return NotFound();
            }

            category.SoftDelete = true;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Where(m => !m.SoftDelete).FirstOrDefault(c => c.Id == id);

            var oldCategory = new CategoryCreateVm()
            {
                CategoryName = category.CategoryName
            };
            return View(oldCategory);
        }
        [HttpPost]
        public IActionResult Edit(int id,CategoryCreateVm categoryCreate)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Zəhmət olmasa bütün sahələri doldurun.");
                return View(categoryCreate);
            }
            var category = _context.Categories.Where(m => !m.SoftDelete).FirstOrDefault(c => c.Id == id);
            if(category == null)
            {
                ModelState.AddModelError("", "Category Tapilmadi !.");
                return View(categoryCreate);
            }
            else
            {
                category.CategoryName = categoryCreate.CategoryName;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}
