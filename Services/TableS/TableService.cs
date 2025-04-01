using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using CoffeeShopAdmin.Models;
using CoffeeShopAdmin.Models.SubCategoryM;
using CoffeeShopAdmin.Models.TableM;
using Microsoft.AspNetCore.Components.Authorization;

namespace CoffeeShopAdmin.Services.TableS
{
    public class TableService : ITableService
    {
        private readonly ApiClient _apiClient;
        private readonly AuthenticationStateProvider _authStateProvider;

        public TableService(ApiClient apiClient, AuthenticationStateProvider authStateProvider)
        {
            _apiClient = apiClient;
            _authStateProvider = authStateProvider;
        }

        public async Task<List<TableRequestModel>> GetTables()
        {
            var response = await _apiClient.GetFromJsonAsync<TableResponseModel>("/table/all");

            return response?.Table ?? new List<TableRequestModel>();
        }

        public async Task<TableRequestModel> GetTableById(string id)
        {
            try
            {
                var response = await _apiClient.GetByIdAsync<TableRequestModel>("/table", id);

                if (response == null)
                {
                    Console.WriteLine("Table not found.");
                    return null;
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching table: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateTable(TableRequestModel table)
        {

            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                Console.WriteLine("User is not authenticated");
                return false;
            }
            table.Id = Guid.NewGuid().ToString().ToUpper();
            table.Status = true;
            table.CreatedBy = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Admin";
            var result = await _apiClient.PostAsync<ApiResponse, TableRequestModel>("/table", table);
            return result?.Result ?? false;
        }

        public async Task<bool> UpdateTable(string id, TableRequestModel table)
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                Console.WriteLine("User is not authenticated");
                return false;
            }

            table.Status = true;
            table.LastModifiedBy = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Admin";
            table.LastModifiedOn = DateTime.UtcNow;
       
            var result = await _apiClient.PutAsync<ApiResponse, TableRequestModel>($"/table/{id}", table);
            return result?.Result ?? false;
        }

        public async Task<bool> DeleteTable(string id)
        {

            string apiUrl = $"table/{id}";
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

