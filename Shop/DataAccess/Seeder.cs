using Microsoft.EntityFrameworkCore;
using Shop.DataAccess.Models;

namespace Shop.DataAccess
{
    public class Seeder 
    {
        private readonly ApplicationDbContext dbContext;

        public Seeder(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void seed()
        {
            getMigrations();
            seedCategories();
            seedProducts();
        }
        private void getMigrations()
        {
            if (dbContext.Database.GetPendingMigrations().Count() > 0)
            {
                dbContext.Database.Migrate();
            }
        }
        private void seedCategories()
        {
            var categories = dbContext.Categories.ToList().Count;
            if (categories == 0)
            {
                var list = new List<Category>() { 
                new Category(){ Name = "bikes" },
                new Category(){ Name = "balls" },
                new Category(){ Name = "books" }
            };
            dbContext.Categories.AddRange(list);
                dbContext.SaveChanges();
            }
        }
        private void seedProducts()
        {
            var products = dbContext.Products.ToList().Count;
            if (products == 0)
            {
                Category bikes = dbContext.Categories.FirstOrDefault(c => c.Name == "bikes");
                Category balls = dbContext.Categories.FirstOrDefault(c => c.Name == "balls");
                Category books = dbContext.Categories.FirstOrDefault(c => c.Name == "books");
                var list = new List<Product> {
                new() { Name = "Red Bike", Category = bikes, Price = 100.5, ImageUrl = @"\images\products\RedBike.jpg" },
                new() { Name = "Blue Bike", Category = bikes, Price = 70.25, ImageUrl = @"\images\products\BlueBike.jpeg" },
                new() { Name = "Green Bike", Category = bikes, Price = 800, ImageUrl = @"\images\products\GreenBike.jpg" },
                new() { Name = "FootBall", Category = balls, Price = 40.2, ImageUrl = @"\images\products\FootBall.png" },
                new() { Name = "BasketBall", Category = balls, Price = 21, ImageUrl = @"\images\products\Basketball.png" },
                new() { Name = "TenissBall", Category = balls, Price = 15, ImageUrl = @"\images\products\TenissBall.jpg" },
                new() { Name = "Pan Tadeusz", Category = books, Price = 10, ImageUrl = @"\images\products\PanTadeusz.jpg" }
                };
                dbContext.Products.AddRange(list);
                dbContext.SaveChanges();
            }
        }
    }
}
