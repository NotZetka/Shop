using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shop.DataAccess;
using Shop.DataAccess.Models;
using Shop.DataAccess.ViewModels;
using Shop.Utility;

namespace Shop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IWebHostEnvironment webHostEnvironment;

        public CategoryController(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            this.dbContext = dbContext;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var categories = dbContext.Categories.ToList();
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
                dbContext.Categories.Add(category);
                dbContext.SaveChanges();
                TempData["success"] = "Category created successfully";
            }

            return RedirectToAction("Index");
            
        }
        public IActionResult Delete(int id)
        {
            var category = dbContext.Categories.FirstOrDefault(p => p.Id == id);
            if(category != null)
            {
                dbContext.Categories.Remove(category);
                dbContext.SaveChanges(true);
                TempData["success"] = "Category deleted successfully";
            }
            return RedirectToAction("Index");
        }
    }
}
