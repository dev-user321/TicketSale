using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using TicketSales.Areas.AdminPanel.Repositories.Interfaces;
using TicketSales.Data;
using TicketSales.Models;
using TicketSales.ViewModels;

namespace TicketSales.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            var categories = _categoryRepository.GetAll();
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

            var newCategory = new Category
            {
                CategoryName = categoryCreate.CategoryName
            };

            _categoryRepository.Add(newCategory);
            _categoryRepository.Save();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var category = _categoryRepository.GetById(id);
            if (category == null)
                return NotFound();

            _categoryRepository.SoftDelete(category);
            _categoryRepository.Save();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _categoryRepository.GetById(id);
            if (category == null) return NotFound();

            var oldCategory = new CategoryCreateVm
            {
                CategoryName = category.CategoryName
            };
            return View(oldCategory);
        }

        [HttpPost]
        public IActionResult Edit(int id, CategoryCreateVm categoryCreate)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Zəhmət olmasa bütün sahələri doldurun.");
                return View(categoryCreate);
            }

            var category = _categoryRepository.GetById(id);
            if (category == null)
            {
                ModelState.AddModelError("", "Category Tapilmadi!.");
                return View(categoryCreate);
            }

            category.CategoryName = categoryCreate.CategoryName;
            _categoryRepository.Update(category);
            _categoryRepository.Save();

            return RedirectToAction("Index");
        }
    }
}
