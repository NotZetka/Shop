using Microsoft.AspNetCore.Mvc;
using Shop.DataAccess;
using System.Security.Claims;

namespace Shop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public OrdersController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var orders = dbContext.Orders.Where(x => x.ApplicationUserId == claim.Value).ToList();
            return View(orders);
        }
    }
}
