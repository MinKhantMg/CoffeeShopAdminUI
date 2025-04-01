using CoffeeShopAdmin.Models.SubCategoryM;

namespace CoffeeShopAdmin.Services.SubCategoryS
{
    public interface ISubCategoryService
    {
        Task<List<SubCategoryRequestModel>> GetSubCategories();
        Task<SubCategoryRequestModel> GetSubCategoryById(string id);
        Task<bool> CreateSubCategory(SubCategoryRequestModel category);
        Task<bool> UpdateSubCategory(string id, SubCategoryRequestModel category);
        Task<bool> DeleteSubCategory(string id);
    }
}
