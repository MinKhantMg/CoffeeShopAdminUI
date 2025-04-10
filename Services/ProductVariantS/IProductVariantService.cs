using CoffeeShopAdmin.Models.ProductVariantM;

namespace CoffeeShopAdmin.Services.ProductVariantS
{
    public interface IProductVariantService
    {
        Task<List<ProductVariantRequestModel>> GetProductVariants();
        Task<ProductVariantRequestModel> GetProducVariantById(string id);
        Task<bool> CreateProductVariant(ProductVariantRequestModel productVariant);
        Task<bool> UpdateProductVariant(string id, ProductVariantRequestModel productVariant);
        Task<bool> DeleteProductVariant(string id);
    }
}
