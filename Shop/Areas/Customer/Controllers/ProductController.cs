using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.DataAccess;
using Shop.DataAccess.ViewModels;
using Shop.Services;

namespace Shop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        private readonly ShopProductService productService;

        public ProductController(ShopProductService productService)
        {
            this.productService = productService;
        }
        public IActionResult Index()
        {
            var products = productService.getAllProducts();
            return View(products);
        }
        public IActionResult Details(int productId)
        {
            var product = productService.getProductById(productId);
            var productVM = new ProductVM { Product = product };
            return View(productVM);
        }
    }
}
