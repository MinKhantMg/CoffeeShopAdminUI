using CoffeeShopAdmin.Models.ProductM;
using CoffeeShopAdmin.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace CoffeeShopAdmin.Services.ProductS
{
    public class ProductService : IProductService
    {
        private readonly ApiClient _apiClient;
        private readonly AuthenticationStateProvider _authStateProvider;

        public ProductService(ApiClient apiClient, AuthenticationStateProvider authStateProvider)
        {
            _apiClient = apiClient;
            _authStateProvider = authStateProvider;
        }

        public async Task<List<ProductRequestModel>> GetProducts()
        {
            var response = await _apiClient.GetFromJsonAsync<ProductResponseModel>("/product/all");

            return response?.Product ?? new List<ProductRequestModel>();
        }

        public async Task<ProductRequestModel> GetProductById(string id)
        {
            try
            {
                var response = await _apiClient.GetByIdAsync<ProductRequestModel>("/product", id);

                if (response == null)
                {
                    Console.WriteLine("Product not found.");
                    return null;
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Product: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateProduct(ProductRequestModel product)
        {

            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                Console.WriteLine("User is not authenticated");
                return false;
            }
            product.Id = Guid.NewGuid().ToString().ToUpper();
            product.CreatedBy = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Admin";
            product.Status = true;
            var result = await _apiClient.PostAsync<ApiResponse, ProductRequestModel>("/product", product);
            return result?.Result ?? false;
        }

        public async Task<bool> UpdateProduct(string id, ProductRequestModel product)
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                Console.WriteLine("User is not authenticated");
                return false;
            }
            product.LastModifiedBy = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Admin";
            product.LastModifiedOn = DateTime.UtcNow;

            var result = await _apiClient.PutAsync<ApiResponse, ProductRequestModel>($"/product/{id}", product);
            return result?.Result ?? false;
        }

        public async Task<bool> DeleteProduct(string id)
        {

            string apiUrl = $"product/{id}";
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
