using Blazored.LocalStorage;
using CoffeeShopAdmin.Models.LoginM;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers;

namespace CoffeeShopAdmin.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    //private readonly ProtectedLocalStorage _localStorage;
    private readonly ILocalStorageService _localStorageService;

    public ApiService(HttpClient httpClient, ILocalStorageService localStorageService)
    {
        _httpClient = httpClient;
        _localStorageService = localStorageService;
    }

    private async Task SetAuthorizationHeader()
    {
        var sessionModel = (await _localStorageService.GetItemAsync<LoginResponseModel>("sessionState"));

        if (sessionModel != null && !string.IsNullOrEmpty(sessionModel.AccessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", sessionModel.AccessToken);
        }
        else
        {

            Console.WriteLine("Session data has not found");
        }
    }

    public async Task<HttpResponseMessage> GetDataFromApi(string endpoint)
    {
        await SetAuthorizationHeader();
        return await _httpClient.GetAsync(endpoint);
    }

    public async Task<HttpResponseMessage> PostDataToApi<T>(string endpoint, T data)
    {
        await SetAuthorizationHeader();
        return await _httpClient.PostAsJsonAsync(endpoint, data);
    }
}

