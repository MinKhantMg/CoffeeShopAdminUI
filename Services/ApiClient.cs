using CoffeeShopAdmin.Models.LoginM;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using CoffeeShopAdmin.Auth;
using Blazored.LocalStorage;
using CoffeeShopAdmin.Models.CategoryM;
using CoffeeShopAdmin.Models;

namespace CoffeeShopAdmin.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
       // private readonly ProtectedLocalStorage _localStorage;
        private readonly NavigationManager _navigationManager;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorageService;

        public ApiClient(HttpClient httpClient, ILocalStorageService localStorageService, NavigationManager navigationManager, AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
            _navigationManager = navigationManager;
            _authStateProvider = authStateProvider;
        }

        public async Task SetAuthorizeHeader()
        {

            var sessionState = (await _localStorageService.GetItemAsync<LoginResponseModel>("sessionState"));

            if (sessionState == null || string.IsNullOrEmpty(sessionState.AccessToken))
            {
                Console.WriteLine("[ERROR] No token found in local storage. User is not authenticated.");
                return;
            }

            Console.WriteLine($"[DEBUG] Token found: {sessionState.AccessToken.Substring(0, 10)}..."); // Print part of the token

            var identity = GetClaimsIdentity(sessionState.AccessToken);
            var tokenExpiredClaim = identity.FindFirst("exp");  // Use standard 'exp' claim

            if (tokenExpiredClaim != null && long.TryParse(tokenExpiredClaim.Value, out long tokenExpiry))
            {
                long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                Console.WriteLine($"[DEBUG] Token Expiry Time: {tokenExpiry}, Current Time: {currentTime}");

                if (tokenExpiry < currentTime)
                {
                    Console.WriteLine("[ERROR] Token is expired. Logging out user.");
                    await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsLoggedOut();
                    _navigationManager.NavigateTo("/login");
                    return;
                }
                else if (tokenExpiry < currentTime + 600) // Refresh token if expiring in 10 mins
                {
                    Console.WriteLine("[INFO] Token is about to expire, refreshing token...");
                    var res = await _httpClient.GetFromJsonAsync<LoginResponseModel>($"/api/auth/loginByRefreshToken?refreshToken={sessionState.RefreshToken}");

                    if (res != null)
                    {
                        Console.WriteLine("[SUCCESS] Token refreshed.");
                        await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsAuthenticated(res);
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", res.AccessToken);
                    }
                    else
                    {
                        Console.WriteLine("[ERROR] Failed to refresh token, logging out.");
                        await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsLoggedOut();
                        _navigationManager.NavigateTo("/login");
                    }
                }
                else
                {
                    Console.WriteLine("[INFO] Token is valid, setting Authorization header.");
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessionState.AccessToken);
                }
            }
        }

        // This method will extract claims from the JWT token
        private ClaimsIdentity GetClaimsIdentity(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return new ClaimsIdentity();
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claims = jwtToken.Claims.ToList();

            // Extract username from claims (support different claim types)
            var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
                         ?? claims.FirstOrDefault(c => c.Type == "name")?.Value;

            if (!string.IsNullOrEmpty(nameClaim))
            {
                claims.Add(new Claim(ClaimTypes.Name, nameClaim)); // Ensure it is present
            }

            return new ClaimsIdentity(claims, "jwt");
        }

        public async Task<T1> PostAsync<T1, T2>(string path, T2 postModel)
        {

            await SetAuthorizeHeader();

            // Log headers
            Console.WriteLine("[DEBUG] Sending request with headers:");
            foreach (var header in _httpClient.DefaultRequestHeaders)
            {
                Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
            }

            var res = await _httpClient.PostAsJsonAsync(path, postModel);

            if (res == null)
            {
                Console.WriteLine("[ERROR] Response is null.");
                return default;
            }

            if (!res.IsSuccessStatusCode)
            {
                Console.WriteLine($"[ERROR] API returned {res.StatusCode} for {path}");
                return default;
            }

            return await res.Content.ReadFromJsonAsync<T1>();
        }

        public async Task<T> GetFromJsonAsync<T>(string path)
        {
            await SetAuthorizeHeader();
            return await _httpClient.GetFromJsonAsync<T>(path);
        }

        public async Task<T> GetByIdAsync<T>(string path, string id)
        {
            await SetAuthorizeHeader();
            return await _httpClient.GetFromJsonAsync<T>($"{path}/{id}");
        }

        public async Task<T1> PutAsync<T1, T2>(string path, T2 putModel)
        {
            await SetAuthorizeHeader();

            // Log headers
            Console.WriteLine("[DEBUG] Sending request with headers:");
            foreach (var header in _httpClient.DefaultRequestHeaders)
            {
                Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
            }

            var res = await _httpClient.PutAsJsonAsync(path, putModel);

            if (res == null)
            {
                Console.WriteLine("[ERROR] Response is null.");
                return default;
            }

            if (!res.IsSuccessStatusCode)
            {
                Console.WriteLine($"[ERROR] API returned {res.StatusCode} for {path}");
                return default;
            }

            return await res.Content.ReadFromJsonAsync<T1>();
        }

        //public async Task<bool> UpdateCategory(string id, CategoryRequestModel category)
        //{
        //    var result = await PutAsync<ApiResponse, CategoryRequestModel>($"/category/{id}", category);
        //    return result?.Result ?? false;
        //}

      

        public async Task<ApiResponse> DeleteAsync(string path)
        {
            await SetAuthorizeHeader();

            Console.WriteLine($"[DEBUG] DELETE request to: {path}");

            var res = await _httpClient.DeleteAsync(path);

            if (!res.IsSuccessStatusCode)
            {
                Console.WriteLine($"[ERROR] API returned {res.StatusCode} for {path}");
                return new ApiResponse { Result = false};
            }

            return await res.Content.ReadFromJsonAsync<ApiResponse>() ?? new ApiResponse { Result = true };
        }

    }
}

 