using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.DataAccess;
using Shop.DataAccess.Models;
using System.Security.Claims;

namespace Shop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public CartController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim == null)
            {
                TempData["error"] = "User not logged in";
                return RedirectToAction("Index", "Product");
            }
            var list = dbContext.ShoppingCarts.
                Include(x=>x.ApplicationUser).
                Include(x=>x.Product).
                Where(x => x.ApplicationUserId==claim.Value).ToList();
            return View(list);
        }
        public IActionResult Add(int productId)
        {
            var product = dbContext.Products.FirstOrDefault(x => x.Id == productId);
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                TempData["error"] = "User not logged in";
                return RedirectToAction("Index","Home");
            }
            var cart = dbContext.ShoppingCarts.FirstOrDefault(x => x.ApplicationUserId == claim.Value && x.ProductId == productId);
            if(cart != null)
            {
                cart.Quantity += 1;
                dbContext.ShoppingCarts.Update(cart);
            }
            else if(product != null)
            {
                cart = new ShoppingCart() { 
                    Quantity = 1, 
                    Product = product,
                    ApplicationUser = dbContext.ApplicationUsers.FirstOrDefault(x=>x.Id==claim.Value)
                };
                dbContext.ShoppingCarts.Add(cart);
            }
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Subtract(int productId)
        {
            var product = dbContext.Products.FirstOrDefault(x => x.Id == productId);
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var cart = dbContext.ShoppingCarts.FirstOrDefault(x => x.ApplicationUserId == claim.Value && x.ProductId == productId);
            if (cart != null)
            {
                cart.Quantity -= 1;
                if (cart.Quantity <= 0)
                {
                    dbContext.Remove(cart);
                }
                else
                {
                    dbContext.ShoppingCarts.Update(cart);
                }
                dbContext.SaveChanges();
            }
            else
            {
                TempData["error"] = "item not in cart";
            }
            return RedirectToAction("Index");
        }
        public IActionResult Remove(int productId)
        {
            var product = dbContext.Products.FirstOrDefault(x => x.Id == productId);
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var cart = dbContext.ShoppingCarts.FirstOrDefault(x => x.ApplicationUserId == claim.Value && x.ProductId == productId);
            if (cart != null)
            {
                dbContext.Remove(cart);
                dbContext.SaveChanges();
            }
            else
            {
                TempData["error"] = "item not in cart";
            }

            return RedirectToAction("Index");
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                TempData["error"] = "User not logged in";
                return RedirectToAction("Index", "Product");
            }
            var user = dbContext.ApplicationUsers.FirstOrDefault(x => x.Id == claim.Value.ToString());
            List<OrderProduct> orderProducts = dbContext.ShoppingCarts.Include(x => x.Product).Where(x => x.ApplicationUserId == claim.Value).Select(x => new OrderProduct()
            {
                Price = x.Product.Price,
                ProductName = x.Product.Name,
                Quantity = x.Quantity
            }).ToList();
            var order = new Order()
            {
                ApplicationUser = user,
                ApplicationUserId = claim.Value,
                Name = user.Name,
                StreetAddress = user.StreetAddress,
                City = user.City,
                PostalCode = user.PostalCode,
                PhoneNumber = user.PhoneNumber,
                Carts = orderProducts
            };
            return View(order);
        }
        [ActionName("Summary")]
        [HttpPost]
        public IActionResult SummaryPost(Order order)
        {
            order.ApplicationUser = dbContext.ApplicationUsers.FirstOrDefault(x=>x.Id == order.ApplicationUserId);
            List<OrderProduct> orderProducts = dbContext.ShoppingCarts.Include(x => x.Product).Where(x => x.ApplicationUserId == order.ApplicationUserId).Select(x => new OrderProduct()
            {
                Price = x.Product.Price,
                ProductName = x.Product.Name,
                Quantity = x.Quantity
            }).ToList();
            order.Date = DateTime.Today;
            order.Carts = orderProducts;
            dbContext.Orders.Add(order);
            var cart = dbContext.ShoppingCarts.Where(x => x.ApplicationUserId == order.ApplicationUserId).ToList();
            dbContext.ShoppingCarts.RemoveRange(cart);
            dbContext.SaveChanges();
            TempData["success"] = "purchase completed";
            return RedirectToAction("Index");
        }
    }
}
