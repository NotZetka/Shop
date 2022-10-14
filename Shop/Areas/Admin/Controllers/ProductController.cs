using Microsoft.AspNetCore.Mvc;
using Shop.DataAccess;
using Shop.DataAccess.Models;

namespace Shop.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product product, IFormFile? file)
        {
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
            return View(product);
        }
    }
}
