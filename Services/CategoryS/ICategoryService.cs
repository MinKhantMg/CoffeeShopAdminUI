using CoffeeShopAdmin.Models;
using CoffeeShopAdmin.Models.CategoryM;

namespace CoffeeShopAdmin.Services.CategoryS
{
    public interface ICategoryService
    {
        Task<List<CategoryRequestModel>> GetCategories();
        Task<AdminSummaryModel> GetSummary();
        Task<CategoryRequestModel> GetCategoryById(string id);
        Task<bool> CreateCategory(CategoryRequestModel category);
        Task<bool> UpdateCategory(string id, CategoryRequestModel category);
        Task<bool> DeleteCategory(string id);
    }
}
