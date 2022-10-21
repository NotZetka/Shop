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
        public IActionResult Create()
        {
            var categories = productService.getCategoriesList();
            var productVM = new ProductVM() { Product = new() ,Categories = categories };
            return View(productVM);
        }
        [HttpPost]
        public IActionResult Create(ProductVM productVM, IFormFile? file)
        {
            var product = productVM.Product;
            
            if (ModelState.IsValid && file != null)
            {
                productService.createProduct(file, product);
                TempData["Success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            return View(productVM);
        }
        public IActionResult Delete(int id)
        {
            var product = productService.getProductById(id);
            if(product != null)
            {
                productService.deleteProduct(product);
                TempData["success"] = "Product deleted successfully";
            }
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int id)
        {
            var categories = productService.getCategoriesList();
            var productVM = new ProductVM() { Product = productService.getProductById(id), Categories = categories };
            return View(productVM);
        }
        [HttpPost]
        public IActionResult Edit(ProductVM productVM, IFormFile? file)
        {
            var product = productVM.Product;
            if (file != null)
            {
                productService.updateImage(file,product);
            }
            productService.updateProduct(product);
            return RedirectToAction("Index");
        }
    }
}
