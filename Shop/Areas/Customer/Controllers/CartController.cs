using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.DataAccess;
using Shop.DataAccess.Models;
using Stripe.Checkout;
using System.Security.Claims;

namespace Shop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IWebHostEnvironment webHostEnvironment;

        public CartController(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            this.dbContext = dbContext;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                TempData["error"] = "User not logged in";
                return RedirectToAction("Index", "Product");
            }
            var list = dbContext.ShoppingCarts.
                Include(x => x.ApplicationUser).
                Include(x => x.Product).
                Where(x => x.ApplicationUserId == claim.Value).ToList();
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
                return RedirectToAction("Index", "Home");
            }
            var cart = dbContext.ShoppingCarts.FirstOrDefault(x => x.ApplicationUserId == claim.Value && x.ProductId == productId);
            if (cart != null)
            {
                cart.Quantity += 1;
                dbContext.ShoppingCarts.Update(cart);
            }
            else if (product != null)
            {
                cart = new ShoppingCart()
                {
                    Quantity = 1,
                    Product = product,
                    ApplicationUser = dbContext.ApplicationUsers.FirstOrDefault(x => x.Id == claim.Value)
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
                Carts = orderProducts,
                TotalPrice = orderProducts.Select(x => x.Price * x.Quantity).Sum()
            };
            return View(order);
        }
        [ActionName("Summary")]
        [HttpPost]
        public IActionResult SummaryPost(Order order)
        {
            order.ApplicationUser = dbContext.ApplicationUsers.FirstOrDefault(x => x.Id == order.ApplicationUserId);
            List<OrderProduct> orderProducts = dbContext.ShoppingCarts.Include(x => x.Product).Where(x => x.ApplicationUserId == order.ApplicationUserId).Select(x => new OrderProduct()
            {
                Price = x.Product.Price,
                ProductName = x.Product.Name,
                Quantity = x.Quantity
            }).ToList();
            order.Date = DateTime.Now;
            order.Carts = orderProducts;

            dbContext.Orders.Add(order);
            dbContext.SaveChanges();
            //stripe
            var domain = $"{Request.Scheme}://{Request.Host}";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),            
                Mode = "payment",
                SuccessUrl = domain+$"/Customer/Cart/OrderConfirmation?id={order.Id}",
                CancelUrl = domain+$"/Customer/Cart",
            };

            foreach (var item in orderProducts)
            {
                options.LineItems.Add(
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price*100),
                        Currency = "pln",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.ProductName,
                        },

                    },
                    Quantity = item.Quantity,
                });
            
            }

            var service = new SessionService();
            Session session = service.Create(options);
            order.SessionId = session.Id;
            order.PaymentIntendId = session.PaymentIntentId;
            dbContext.Orders.Update(order);
            dbContext.SaveChanges();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

            
        }
        public IActionResult OrderConfirmation(int id)
        {
            var order = dbContext.Orders.SingleOrDefault(x => x.Id == id);
            var service = new SessionService();
            Session session = service.Get(order.SessionId);
            if (session.PaymentStatus.ToLower() == "paid")
            {
                var cart = dbContext.ShoppingCarts.Where(x => x.ApplicationUserId == order.ApplicationUserId).ToList();
                dbContext.ShoppingCarts.RemoveRange(cart);
                dbContext.SaveChanges();
                return View(id);
            }
            return RedirectToAction("Index","Home");
        }
    }
}
