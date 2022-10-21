using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.DataAccess;
using Shop.DataAccess.Models;
using Shop.Services;
using Stripe.Checkout;
using System.Security.Claims;

namespace Shop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly CartService cartService;
        private readonly ShopProductService productService;
        private readonly OrderService orderService;
        private readonly UserService userService;

        public CartController(CartService cartService, ShopProductService productService, OrderService orderService,UserService userService)
        {
            this.cartService = cartService;
            this.productService = productService;
            this.orderService = orderService;
            this.userService = userService;
        }
        public IActionResult Index()
        {
            var claim = getClaim();
            
            if (claim == null)
            {
                TempData["error"] = "User not logged in";
                return RedirectToAction("Index", "Product");
            }
            var list = cartService.getUserCarts(claim);

            return View(list);
        }


        public IActionResult Add(int productId)
        {
            var product = productService.getProductById(productId);
            var claim = getClaim();
            if (claim == null)
            {
                TempData["error"] = "User not logged in";
                return RedirectToAction("Index", "Home");
            }
            var cart = cartService.getCart(productId, claim);
            cartService.addCart(cart,product,claim);
            return RedirectToAction("Index");
        }
        public IActionResult Subtract(int productId)
        {
            var claim = getClaim();
            var cart = cartService.getCart(productId, claim);
            if (cart != null)
            {
                cartService.subtract(cart);
            }
            else
            {
                TempData["error"] = "item not in cart";
            }
            return RedirectToAction("Index");
        }
        public IActionResult Remove(int productId)
        {
            var product = productService.getProductById(productId);
            var claim = getClaim();
            var cart = cartService.getCart(productId, claim);
            if (cart != null)
            {
                cartService.removeCart(cart);
            }
            else
            {
                TempData["error"] = "item not in cart";
            }

            return RedirectToAction("Index");
        }
        public IActionResult Summary()
        {
            var claim = getClaim();
            if (claim == null)
            {
                TempData["error"] = "User not logged in";
                return RedirectToAction("Index", "Product");
            }
            var user = userService.getUser(claim);

            var orderProducts = orderService.getOrderProducts(claim);
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
            order.ApplicationUser = userService.getUserById(order.ApplicationUserId);
            order.Date = DateTime.Now;
            var orderProducts = orderService.getOrderProducts(order.ApplicationUser.Id);
            order.Carts = orderProducts;
            orderService.addOrder(order);
            //stripe
            var domain = $"{Request.Scheme}://{Request.Host}";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"/Customer/Cart/OrderConfirmation?id={order.Id}",
                CancelUrl = domain + $"/Customer/Cart",
            };

            foreach (var item in orderProducts)
            {
                options.LineItems.Add(
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
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
            orderService.updateOrder(order);
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);


        }

        public IActionResult OrderConfirmation(int id)
        {
            var order = orderService.getOrderById(id);
                
            var service = new SessionService();
            Session session = service.Get(order.SessionId);
            if (session.PaymentStatus.ToLower() == "paid")
            {
                var carts = cartService.getUserCarts(order.ApplicationUserId);
                cartService.removeCarts(carts);
                return View(id);
            }
            return RedirectToAction("Index", "Home");
        }

        private Claim getClaim()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return claim;
        }
        }
    }
