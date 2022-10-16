
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shop.DataAccess.Models;
using System.ComponentModel.DataAnnotations;

namespace Shop.DataAccess.ViewModels
{
    public class ProductVM
    {
        [Range(1,100)]
        public int Amount { get; set; } = 1;
        public Product Product { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> Categories { get; set; }

    }
}
