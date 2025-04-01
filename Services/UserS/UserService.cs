using CoffeeShopAdmin.Models;
using CoffeeShopAdmin.Models.RegisterM;

namespace CoffeeShopAdmin.Services.UserS
{
    public class UserService : IUserService
    {
        private readonly ApiClient _apiClient;

        public UserService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<bool> RegisterUserAsync(RegisterRequestModel dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Email))
                {
                    throw new ArgumentException("Email cannot be empty.", nameof(dto.Email));
                }

                // Hash the password before sending to the API
                dto.Id = Guid.NewGuid().ToString().ToUpper();

                var result = await _apiClient.PostAsync<ApiResponse, RegisterRequestModel>("/user/register", dto);

                return result?.Result ?? false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering user: {ex.Message}");
                return false;
            }
        }
    }
}

