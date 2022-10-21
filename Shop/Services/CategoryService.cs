using Shop.DataAccess;
using Shop.DataAccess.Models;

namespace Shop.Services
{
    public class CategoryService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IWebHostEnvironment webHostEnvironment;

        public CategoryService(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            this.dbContext = dbContext;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IEnumerable<Category> getAllCategories()
        {
            var categories = dbContext.Categories.ToList();
            return categories;
        }

        public void addCategory(Category category)
        {
            dbContext.Categories.Add(category);
            dbContext.SaveChanges();
        }

        public Category getCategoryById(int id)
        {
            var category = dbContext.Categories.FirstOrDefault(p => p.Id == id);
            return category;
        }

        public void removeCategory(Category category)
        {
            dbContext.Categories.Remove(category);
            dbContext.SaveChanges(true);
        }
    }
}
