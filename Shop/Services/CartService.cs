using Microsoft.EntityFrameworkCore;
using Shop.DataAccess;
using Shop.DataAccess.Models;
using System.Security.Claims;

namespace Shop.Services
{
    public class CartService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IWebHostEnvironment webHostEnvironment;

        public CartService(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            this.dbContext = dbContext;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IEnumerable<ShoppingCart> getUserCarts(Claim claim)
        {
            var list = dbContext.ShoppingCarts.
                Include(x => x.ApplicationUser).
                Include(x => x.Product).
                Where(x => x.ApplicationUserId == claim.Value).ToList();
            return list;
        }

        public ShoppingCart getCart(int productId, Claim claim)
        {
            var cart = dbContext.ShoppingCarts.FirstOrDefault(x => x.ApplicationUserId == claim.Value && x.ProductId == productId);
            return cart;
        }

        public void addCart(ShoppingCart cart, Product product, Claim claim)
        {
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
        }

        public void subtract(ShoppingCart cart)
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

        public void removeCart(ShoppingCart cart)
        {
            dbContext.Remove(cart);
            dbContext.SaveChanges();
        }

        public IEnumerable<ShoppingCart> getUserCarts(string applicationUserId)
        {
            var carts = dbContext.ShoppingCarts.Where(x => x.ApplicationUserId == applicationUserId).ToList();
            return carts;
        }

        public void removeCarts(IEnumerable<ShoppingCart> carts)
        {
            dbContext.ShoppingCarts.RemoveRange(carts);
            dbContext.SaveChanges();
        }
    }
}
