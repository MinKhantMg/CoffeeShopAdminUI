using CoffeeShopAdmin.Models.SubCategoryM;
using CoffeeShopAdmin.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace CoffeeShopAdmin.Services.SubCategoryS
{
    public class SubCategoryService : ISubCategoryService
    {
        private readonly ApiClient _apiClient;
        private readonly AuthenticationStateProvider _authStateProvider;

        public SubCategoryService(ApiClient apiClient, AuthenticationStateProvider authStateProvider)
        {
            _apiClient = apiClient;
            _authStateProvider = authStateProvider;
        }

        public async Task<List<SubCategoryRequestModel>> GetSubCategories()
        {
            var response = await _apiClient.GetFromJsonAsync<SubCategoryResponseModel>("/subcategory/all");

            return response?.SubCategory ?? new List<SubCategoryRequestModel>();
        }

        public async Task<SubCategoryRequestModel> GetSubCategoryById(string id)
        {
            try
            {
                var response = await _apiClient.GetByIdAsync<SubCategoryRequestModel>("/subcategory", id);

                if (response == null)
                {
                    Console.WriteLine("SubCategory not found.");
                    return null;
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching SubCategory: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateSubCategory(SubCategoryRequestModel subcategory)
        {

            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                Console.WriteLine("User is not authenticated");
                return false;
            }
            subcategory.Id = Guid.NewGuid().ToString().ToUpper();
            subcategory.CreatedBy = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Admin";
            var result = await _apiClient.PostAsync<ApiResponse, SubCategoryRequestModel>("/subcategory", subcategory);
            return result?.Result ?? false;
        }

        public async Task<bool> UpdateSubCategory(string id, SubCategoryRequestModel subcategory)
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                Console.WriteLine("User is not authenticated");
                return false;
            }
            subcategory.LastModifiedBy = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Admin";
            subcategory.LastModifiedOn = DateTime.UtcNow;
            //return await _apiClient.UpdateCategory(id, subcategory);
            var result = await _apiClient.PutAsync<ApiResponse, SubCategoryRequestModel>($"/subcategory/{id}", subcategory);
            return result?.Result ?? false;
        }

        public async Task<bool> DeleteSubCategory(string id)
        {

            string apiUrl = $"subcategory/{id}";
            Console.WriteLine($"[DEBUG] Sending DELETE request to: {apiUrl}");

            // Send DELETE request
            var response = await _apiClient.DeleteAsync(apiUrl);

            if (response == null)
            {
                Console.WriteLine("[ERROR] No response received from API");
                return false;
            }

            return response?.Result ?? false; ;
        }
    }
}
