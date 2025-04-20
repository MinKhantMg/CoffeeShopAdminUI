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

        public async Task<string?> RegisterUserAsync(RegisterRequestModel dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Email))
                {
                    return "Email cannot be empty.";
                }

                dto.Id = Guid.NewGuid().ToString().ToUpper();

                var result = await _apiClient.PostAsync<RegisterResponse, RegisterRequestModel>("/user/register", dto);

                if (result?.Result == true)
                {
                    return null; // Success
                }

                return result?.Message ?? "Email is already in used.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering user: {ex.Message}");
                return "An error occurred while registering. Please try again.";
            }
        }

    }
}

