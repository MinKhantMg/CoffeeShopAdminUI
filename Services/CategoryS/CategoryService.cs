using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using CoffeeShopAdmin.Models;
using CoffeeShopAdmin.Models.CategoryM;
using CoffeeShopAdmin.Models.TableM;
using Microsoft.AspNetCore.Components.Authorization;
using static MudBlazor.CategoryTypes;

namespace CoffeeShopAdmin.Services.CategoryS
{
    public class CategoryService : ICategoryService
    {
        private readonly ApiClient _apiClient;
        private readonly AuthenticationStateProvider _authStateProvider;

        public CategoryService(ApiClient apiClient, AuthenticationStateProvider authStateProvider)
        {
            _apiClient = apiClient;
            _authStateProvider = authStateProvider;
        }

        public async Task<List<CategoryRequestModel>> GetCategories()
        {
            var response = await _apiClient.GetFromJsonAsync<CategoryResponseModel>("/category/all");

            return response?.Category ?? new List<CategoryRequestModel>();
        }

        public async Task<CategoryRequestModel> GetCategoryById(string id)
        {
            try
            {
                var response = await _apiClient.GetByIdAsync<CategoryRequestModel>("/category", id);

                if (response == null)
                {
                    Console.WriteLine("Category not found.");
                    return null;
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching category: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateCategory(CategoryRequestModel category)
        {

            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                Console.WriteLine("User is not authenticated");
                return false;
            }
            category.Id = Guid.NewGuid().ToString().ToUpper();
            category.CreatedBy = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Admin";
            var result = await _apiClient.PostAsync<ApiResponse, CategoryRequestModel>("/category", category);
            return result?.Result ?? false;
        }

        public async Task<bool> UpdateCategory(string id, CategoryRequestModel category)
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                Console.WriteLine("User is not authenticated");
                return false;
            }
            category.LastModifiedBy = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Admin";
            category.LastModifiedOn = DateTime.UtcNow;
           
            var result = await _apiClient.PutAsync<ApiResponse, CategoryRequestModel>($"/category/{id}", category);
            return result?.Result ?? false;
        }

        public async Task<bool> DeleteCategory(string id)
        {

            string apiUrl = $"category/{id}";
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

