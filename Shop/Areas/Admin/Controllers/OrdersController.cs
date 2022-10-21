using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.DataAccess;
using Shop.Services;
using Shop.Utility;
using System.Security.Claims;

namespace Shop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class OrdersController : Controller
    {
        private readonly OrderService service;

        public OrdersController(OrderService service)
        {
            this.service = service;
        }

        public IActionResult Index()
        {
            var orders = service.getAllOrders();
            return View(orders);
        } 
    }
}
