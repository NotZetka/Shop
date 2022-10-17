using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.DataAccess;
using Shop.Utility;
using System.Security.Claims;

namespace Shop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public OrdersController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var orders = dbContext.Orders.Include(x=>x.ApplicationUser).ToList();
            return View(orders);
        } 
    }
}
