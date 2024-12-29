using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FormsApp.Models
{
    // [Bind("Name","Price")] //bind attribute is used to bind the model to the view
    public class Product
    {
        [Display(Name = "Ürün Id")]
        [BindNever] //bind never attribute is used to prevent the model from binding to the view
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Ürün Adı")]
        public string? Name { get; set; }

        [Required]
        [Range(0, 100000)]
        [Display(Name = "Fiyat")]
        public decimal? Price { get; set; }

        [Display(Name = "Resim")]
        public string? Image { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        [Display(Name="Category")]
        [Required]
        public int CategoryId { get; set; }
    }
}

// 1 iphone 14 1
// 2 iphone 15 1