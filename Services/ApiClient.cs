﻿using CoffeeShopAdmin.Models.LoginM;
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
using System.Text.Json;

namespace CoffeeShopAdmin.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
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

            Console.WriteLine($"[DEBUG] Token found: {sessionState.AccessToken.Substring(0, 10)}..."); 

            var identity = GetClaimsIdentity(sessionState.AccessToken);
            var tokenExpiredClaim = identity.FindFirst("exp");  

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

       
        private ClaimsIdentity GetClaimsIdentity(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return new ClaimsIdentity();
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claims = jwtToken.Claims.ToList();

    
            var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
                         ?? claims.FirstOrDefault(c => c.Type == "name")?.Value;

            if (!string.IsNullOrEmpty(nameClaim))
            {
                claims.Add(new Claim(ClaimTypes.Name, nameClaim)); 
            }

            return new ClaimsIdentity(claims, "jwt");
        }

        public async Task<T1?> PostAsync<T1, T2>(string path, T2 postModel)
        {
            await SetAuthorizeHeader();

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

            var responseContent = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
            {
                Console.WriteLine($"[ERROR] API returned {res.StatusCode} for {path}");
                Console.WriteLine($"[ERROR] Response content: {responseContent}");

                try
                {
                    // Try to deserialize the error body to T1, assuming it has a Message property
                    var errorObj = JsonSerializer.Deserialize<T1>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return errorObj;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to parse error response: {ex.Message}");
                    return default;
                }
            }

            return JsonSerializer.Deserialize<T1>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }


        public async Task<T> GetFromJsonAsync<T>(string path)
        {
            await SetAuthorizeHeader();

            var res = await _httpClient.GetAsync(path);

            if (!res.IsSuccessStatusCode)
            {
                Console.WriteLine($"[ERROR] API returned {res.StatusCode} for {path}");
                if (res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("[ERROR] Unauthorized - redirecting to login.");
                    await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsLoggedOut();
                    _navigationManager.NavigateTo("/login");
                }
                return default;
            }

            return await res.Content.ReadFromJsonAsync<T>();
        }

        public async Task<T> GetByIdAsync<T>(string path, string id)
        {
            T rtn = default;
            await SetAuthorizeHeader();
            try
            {
               rtn   =  await _httpClient.GetFromJsonAsync<T>($"{path}/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return rtn;
        }

        public async Task<T1> PutAsync<T1, T2>(string path, T2 putModel)
        {
            await SetAuthorizeHeader();

    
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

 