using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop.DataAccess;
using Shop.DataAccess.Models;
using System.Security.Claims;

namespace Shop.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public ApplicationUser getUser(Claim claim)
        {
            var user = dbContext.ApplicationUsers.FirstOrDefault(x => x.Id == claim.Value.ToString());
            return user;
        }
        public ApplicationUser getUserById(string applicationUserId)
        {
            var user = dbContext.ApplicationUsers.FirstOrDefault(x => x.Id == applicationUserId);
            return user;
        }
    }
}
