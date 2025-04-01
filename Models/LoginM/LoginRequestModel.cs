using System.ComponentModel.DataAnnotations;

namespace CoffeeShopAdmin.Models.LoginM;

public class LoginRequestModel
{

    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;


    //public class AuthResponse
    //{
    //    public string Token { get; set; } = string.Empty;
    //}
}
