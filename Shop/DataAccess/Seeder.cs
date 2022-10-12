using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shop.DataAccess.Models;
using Shop.Utility;

namespace Shop.DataAccess
{
    public class Seeder 
    {
        private readonly ApplicationDbContext dbContext;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;

        public Seeder(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            this.dbContext = dbContext;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        public void seed()
        {
            getMigrations();
            seedCategories();
            seedProducts();
            seedRoles();
            seedUsers();
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
        private void seedRoles()
        {
            if (!roleManager.RoleExistsAsync(SD.Role_Admin).Result)
            {
                roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
            }        
            if (!roleManager.RoleExistsAsync(SD.Role_User).Result)
            {
                roleManager.CreateAsync(new IdentityRole(SD.Role_User)).GetAwaiter().GetResult();
            }        
        }
        private void seedUsers()
        {
            if(dbContext.Users.FirstOrDefault(x => x.UserName == "admin@test.com") == null)
            {
                userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@test.com",
                    Email = "admin@test.com",
                    Name = "admin",
                    PhoneNumber = "123456789",
                    StreetAddress = "test",
                    PostalCode = "12345",
                    City = "Warsaw"
                }, "Test123.").GetAwaiter().GetResult();
                var admin = dbContext.Users.FirstOrDefault(x => x.UserName == "admin@test.com");
                userManager.AddToRoleAsync(admin, SD.Role_Admin).GetAwaiter().GetResult();
            }

            if (dbContext.Users.FirstOrDefault(x => x.UserName == "user@test.com") == null)
            {
                userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "user@test.com",
                    Email = "user@test.com",
                    Name = "user",
                    PhoneNumber = "123456789",
                    StreetAddress = "test",
                    PostalCode = "12345",
                    City = "Warsaw"
                }, "Test123.").GetAwaiter().GetResult();
                var user = dbContext.Users.FirstOrDefault(x => x.UserName == "user@test.com");
                userManager.AddToRoleAsync(user, SD.Role_User).GetAwaiter().GetResult();
            }
            if (dbContext.Users.FirstOrDefault(x => x.UserName == "user2@test.com") == null)
            {
                userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "user2@test.com",
                    Email = "user2@test.com",
                    Name = "user2",
                }, "Test123.").GetAwaiter().GetResult();
                var user2 = dbContext.Users.FirstOrDefault(x => x.UserName == "user2@test.com");
                userManager.AddToRoleAsync(user2, SD.Role_User).GetAwaiter().GetResult();
            }
            
        }
    }
}
