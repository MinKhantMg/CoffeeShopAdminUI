using System.Text.Json.Serialization;

namespace CoffeeShopAdmin.Models.LoginM
{
    public class LoginResponseModel
    {
        public string Email { get; set; }

        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        public long TokenExpired { get; set; }


        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }

        public string Message { get; set; }

        public LoginResponseModel() { }

    }
}
