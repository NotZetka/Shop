using Microsoft.EntityFrameworkCore;
using Shop.DataAccess;
using Shop.DataAccess.Models;
using System.Security.Claims;

namespace Shop.Services { 
    public class OrderService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IWebHostEnvironment webHostEnvironment;

        public OrderService(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            this.dbContext = dbContext;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IEnumerable<Order> getAllOrders()
        {
            var orders = dbContext.Orders.Include(x => x.ApplicationUser).ToList();
            return orders;
        }

        public IEnumerable<Order> getUserOrders(Claim claim)
        {
            var orders = dbContext.Orders.Where(x => x.ApplicationUserId == claim.Value).ToList();
            return orders;
        }

        public List<OrderProduct> getOrderProducts(Claim claim)
        {
             return getOrderProducts(claim.Value);
        }
        public List<OrderProduct> getOrderProducts(string applicationUserId)
        {

            List<OrderProduct> orderProducts = dbContext.ShoppingCarts.Include(x => x.Product).Where(x => x.ApplicationUserId == applicationUserId).Select(x => new OrderProduct()
            {
                Price = x.Product.Price,
                ProductName = x.Product.Name,
                Quantity = x.Quantity
            }).ToList();
            return orderProducts;
        }

        public void addOrder(Order order)
        {
            dbContext.Orders.Add(order);
            dbContext.SaveChanges();
        }

        public void updateOrder(Order order)
        {
            dbContext.Orders.Update(order);
            dbContext.SaveChanges();
        }

        public Order getOrderById(int id)
        {
            var order = dbContext.Orders.SingleOrDefault(x => x.Id == id);
            return order;
        }
    }
}
