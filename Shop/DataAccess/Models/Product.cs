﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; }

    }
}
