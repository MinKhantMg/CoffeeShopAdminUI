using Blazored.LocalStorage;
using CoffeeShopAdmin.Models.LoginM;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CoffeeShopAdmin.Auth;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorageService;

    public CustomAuthStateProvider(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
      
        var sessionModel = (await _localStorageService.GetItemAsync<LoginResponseModel>("sessionState"));

        if (sessionModel == null || string.IsNullOrEmpty(sessionModel.AccessToken))
        {
            await MarkUserAsLoggedOut();
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var identity = GetClaimsIdentity(sessionModel.AccessToken);

        var tokenExpiredClaim = identity.FindFirst("TokenExpired");
        if (tokenExpiredClaim != null && long.TryParse(tokenExpiredClaim.Value, out long tokenExpiry))
        {
            if (tokenExpiry < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                await MarkUserAsLoggedOut();
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        var user = new ClaimsPrincipal(identity);
        return new AuthenticationState(user);
    }

    public async Task MarkUserAsAuthenticated(LoginResponseModel model)
    {
        if (model == null || string.IsNullOrEmpty(model.AccessToken))
        {
            return;
        }

       // await _localStorage.SetAsync("AccessToken", model.AccessToken);
        //await _localStorage.SetAsync("RefrehToken", model.RefreshToken);

        await _localStorageService.SetItemAsync("sessionState", model);
        Console.WriteLine($"Session saved: {model.AccessToken}");

        var identity = GetClaimsIdentity(model.AccessToken);
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));

        Console.WriteLine($"User logged in as: {user.Identity?.Name}");
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _localStorageService.RemoveItemAsync("sessionState");
        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
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

        var expClaim = jwtToken.Payload.Exp;
        long tokenExpiry = expClaim.HasValue ? expClaim.Value : 0;

        claims.Add(new Claim("TokenExpired", tokenExpiry.ToString()));

        var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
                     ?? claims.FirstOrDefault(c => c.Type == "name")?.Value;

        if (!string.IsNullOrEmpty(nameClaim))
        {
            claims.Add(new Claim(ClaimTypes.Name, nameClaim)); 
        }

        return new ClaimsIdentity(claims, "jwt");
    }

}
