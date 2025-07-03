using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Inventory.Models.ProductModel
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Product code is required.")]
        public string? Code { get; set; }
        [Required(ErrorMessage = "Product name is required.")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Product description is required.")]
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string? ImageUrl { get; set; }
    }
}
