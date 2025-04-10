using CoffeeShopAdmin.Models;
using System.Security.Claims;
using CoffeeShopAdmin.Models.ProductVariantM;
using Microsoft.AspNetCore.Components.Authorization;

namespace CoffeeShopAdmin.Services.ProductVariantS
{
    public class ProductVariantService: IProductVariantService
    {
        private readonly ApiClient _apiClient;
        private readonly AuthenticationStateProvider _authStateProvider;

        public ProductVariantService(ApiClient apiClient, AuthenticationStateProvider authStateProvider)
        {
            _apiClient = apiClient;
            _authStateProvider = authStateProvider;
        }

        public async Task<List<ProductVariantRequestModel>> GetProductVariants()
        {
            var response = await _apiClient.GetFromJsonAsync<ProductVariantResponseModel>("/productvariant/all");

            return response?.ProductVariant ?? new List<ProductVariantRequestModel>();
        }

        public async Task<ProductVariantRequestModel> GetProducVariantById(string id)
        {
            try
            {
                var response = await _apiClient.GetByIdAsync<ProductVariantRequestModel>("/productvariant", id);

                if (response == null)
                {
                    Console.WriteLine("ProductVariant not found.");
                    return null;
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching ProductVariant: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateProductVariant(ProductVariantRequestModel productVariant)
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                Console.WriteLine("User is not authenticated");
                return false;
            }
            productVariant.Id = Guid.NewGuid().ToString().ToUpper();
            productVariant.CreatedBy = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Admin";
            productVariant.Status = true;
            var result = await _apiClient.PostAsync<ApiResponse, ProductVariantRequestModel>("/productvariant", productVariant);
            return result?.Result ?? false;
        }

        public async Task<bool> UpdateProductVariant(string id, ProductVariantRequestModel productVariant)
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                Console.WriteLine("User is not authenticated");
                return false;
            }
            productVariant.LastModifiedBy = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Admin";
            productVariant.LastModifiedOn = DateTime.UtcNow;

            var result = await _apiClient.PutAsync<ApiResponse, ProductVariantRequestModel>($"/productvariant/{id}", productVariant);
            return result?.Result ?? false;
        }

        public async Task<bool> DeleteProductVariant(string id)
        {
            string apiUrl = $"productvariant/{id}";
            Console.WriteLine($"[DEBUG] Sending DELETE request to: {apiUrl}");

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
