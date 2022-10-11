using System.ComponentModel.DataAnnotations;

namespace Shop.DataAccess.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }
        public Category? Category { get; set; }

    }
}
