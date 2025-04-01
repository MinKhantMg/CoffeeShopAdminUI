using CoffeeShopAdmin.Models.ProductM;

namespace CoffeeShopAdmin.Services.ProductS
{
    public interface IProductService
    {
        Task<List<ProductRequestModel>> GetProducts();
        Task<ProductRequestModel> GetProductById(string id);
        Task<bool> CreateProduct(ProductRequestModel product);
        Task<bool> UpdateProduct(string id, ProductRequestModel product);
        Task<bool> DeleteProduct(string id);
    }
}
