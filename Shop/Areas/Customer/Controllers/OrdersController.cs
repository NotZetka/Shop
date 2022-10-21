using Microsoft.AspNetCore.Mvc;
using Shop.DataAccess;
using Shop.Services;
using System.Security.Claims;

namespace Shop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrdersController : Controller
    {
        private readonly OrderService orderService;

        public OrdersController(OrderService orderService)
        {
            this.orderService = orderService;
        }
        public IActionResult Index()
        {
            var claim = getClaim();
            var orders = orderService.getUserOrders(claim);
            return View(orders);
        }
        private Claim getClaim()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return claim;
        }
    }
}
