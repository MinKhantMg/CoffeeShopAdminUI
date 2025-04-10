using System.ComponentModel.DataAnnotations;

namespace CoffeeShopAdmin.Models.ProductVariantM
{
    public class ProductVariantRequestModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "ProductVariant name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Image is required.")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Select Product is required.")]
        public string ProductId { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string DeletedBy { get; set; }
        public bool IsDeleted { get; set; }
        public bool Status { get; set; }
    }
}

