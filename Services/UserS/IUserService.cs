using CoffeeShopAdmin.Models.RegisterM;

namespace CoffeeShopAdmin.Services.UserS
{
    public interface IUserService
    {
        Task<string?> RegisterUserAsync(RegisterRequestModel dto);
    }
}
