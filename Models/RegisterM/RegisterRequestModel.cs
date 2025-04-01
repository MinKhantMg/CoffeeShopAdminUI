using System.ComponentModel.DataAnnotations;
using System.Data;
using CoffeeShopAdmin.Enums;

namespace CoffeeShopAdmin.Models.RegisterM
{
    public class RegisterRequestModel
    {
        public string Id { get; set; }


        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        public string? PhoneNumber { get; set; } = default!;


        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Password is required")]
        public string? PasswordHash { get; set; } = default!;

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = Enum.GetName(typeof(Role), Enums.Role.None);

    }
}
