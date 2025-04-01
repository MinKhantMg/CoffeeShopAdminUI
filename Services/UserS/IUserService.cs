using CoffeeShopAdmin.Models.RegisterM;

namespace CoffeeShopAdmin.Services.UserS
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(RegisterRequestModel dto);
    }
}
