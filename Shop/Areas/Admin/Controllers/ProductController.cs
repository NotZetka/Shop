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
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductController(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            this.dbContext = dbContext;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var products = dbContext.Products.ToList();
            return View(products);
        }
        public IActionResult Create()
        {
            var categories = dbContext.Categories.Select(c=>new SelectListItem()
            {
                Value = c.Id.ToString(),
                Text = c.Name,
            });
            var productVM = new ProductVM() { Product = new() ,Categories = categories };
            return View(productVM);
        }
        [HttpPost]
        public IActionResult Create(ProductVM productVM, IFormFile? file)
        {
            var product = productVM.Product;
            
            if (ModelState.IsValid)
            {
                var wwwRootPath = webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath,@"images\products");
                    var extention = Path.GetExtension(file.FileName);
                    using (var stream = new FileStream(Path.Combine(uploads, fileName + extention),FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    product.ImageUrl = @"\images\products\" + fileName + extention;
                    dbContext.Products.Add(product);
                    dbContext.SaveChanges();
                    TempData["Success"] = "Product created successfully";
                }
                return RedirectToAction("Index");
            }
            return View(productVM);
        }
        public IActionResult Delete(int id)
        {
            var product = dbContext.Products.FirstOrDefault(p => p.Id == id);
            if(product != null)
            {
                var wwwRootPath = webHostEnvironment.WebRootPath;
                var imagePath = Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                dbContext.Products.Remove(product);
                dbContext.SaveChanges(true);
                TempData["success"] = "Product deleted successfully";
            }
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int id)
        {
            var categories = dbContext.Categories.Select(c => new SelectListItem()
            {
                Value = c.Id.ToString(),
                Text = c.Name,
            });
            var productVM = new ProductVM() { Product = dbContext.Products.FirstOrDefault(x=>x.Id==id), Categories = categories };
            return View(productVM);
        }
        [HttpPost]
        public IActionResult Edit(ProductVM productVM, IFormFile? file)
        {
            var product = productVM.Product;
            if (file != null)
            {
                var wwwRootPath = webHostEnvironment.WebRootPath;
                var oldImagePath = Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images\products");
                var extention = Path.GetExtension(file.FileName);
                using (var stream = new FileStream(Path.Combine(uploads, fileName + extention), FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                product.ImageUrl = @"\images\products\" + fileName + extention;
            }
            dbContext.Products.Update(product);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
