using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shop.DataAccess;
using Shop.DataAccess.Models;
using Shop.DataAccess.ViewModels;
using Shop.Services;
using Shop.Utility;

namespace Shop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly CategoryService categoryService;

        public CategoryController(CategoryService categoryService)
        {
            this.categoryService = categoryService;
        }
        public IActionResult Index()
        {
            var categories = categoryService.getAllCategories();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                categoryService.addCategory(category);
                TempData["success"] = "Category created successfully";
            }

            return RedirectToAction("Index");
            
        }
        public IActionResult Delete(int id)
        {
            var category = categoryService.getCategoryById(id);
            if(category != null)
            {
                categoryService.removeCategory(category);
                TempData["success"] = "Category deleted successfully";
            }
            return RedirectToAction("Index");
        }
    
    }
}
