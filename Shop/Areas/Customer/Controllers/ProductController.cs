using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.DataAccess;
using Shop.DataAccess.ViewModels;

namespace Shop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public ProductController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var products = dbContext.Products.ToList();
            return View(products);
        }
        public IActionResult Details(int productId)
        {
            var product = dbContext.Products.Include(x => x.Category).SingleOrDefault(p => p.Id == productId);
            var productVM = new ProductVM { Product = product };
            return View(productVM);
        }
    }
}
