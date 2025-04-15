
using System.Text.Json.Serialization;

namespace CoffeeShopAdmin.Models.ProductVariantM
{
    public class ProductVariantResponseModel
    {
        [JsonPropertyName("productVariant")]
        public List<ProductVariantRequestModel> ProductVariant { get; set; }

        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }
    }
}
