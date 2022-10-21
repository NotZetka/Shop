using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop.DataAccess;
using Shop.DataAccess.Models;

namespace Shop.Services
{
    public class ShopProductService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ShopProductService(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            this.dbContext = dbContext;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IEnumerable<Product> getAllProducts()
        {
            var products = dbContext.Products.ToList();
            return products;
        }

        public IEnumerable<SelectListItem> getCategoriesList()
        {
            var categories = dbContext.Categories.Select(c => new SelectListItem()
            {
                Value = c.Id.ToString(),
                Text = c.Name,
            });
            return categories;
        }

        public void createProduct(IFormFile file, Product product)
        {
            var wwwRootPath = webHostEnvironment.WebRootPath;
            string fileName = Guid.NewGuid().ToString();
            var uploads = Path.Combine(wwwRootPath, @"images\products");
            var extention = Path.GetExtension(file.FileName);
            using (var stream = new FileStream(Path.Combine(uploads, fileName + extention), FileMode.Create))
            {
                file.CopyTo(stream);
            }
            product.ImageUrl = @"\images\products\" + fileName + extention;
            dbContext.Products.Add(product);
            dbContext.SaveChanges();
        }

        public void deleteProduct(Product product)
        {
            var wwwRootPath = webHostEnvironment.WebRootPath;
            var imagePath = Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            dbContext.Products.Remove(product);
            dbContext.SaveChanges(true);
        }

        public Product getProductById(int id)
        {
            var product = dbContext.Products.Include(x => x.Category).FirstOrDefault(p => p.Id == id);
            return product;
        }

        public void updateImage(IFormFile file, Product product)
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

        internal void updateProduct(Product product)
        {
            dbContext.Products.Update(product);
            dbContext.SaveChanges();
        }
    }
}
