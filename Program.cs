using CoffeeShopAdmin.Auth;
using CoffeeShopAdmin.Components;
using CoffeeShopAdmin.Services.CategoryS;
using CoffeeShopAdmin.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Blazored.LocalStorage;
using CoffeeShopAdmin.Services.UserS;
using CoffeeShopAdmin.Services.SubCategoryS;
using CoffeeShopAdmin.Services.ProductS;
using CoffeeShopAdmin.Services.ProductVariantS;
using CoffeeShopAdmin.Services.OrderS;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("7b67213d4ffa66c454e722aa905ce1f078567f83bea923f4f3d332390e5680bbe5eedb962e463537a0874aef460a91ae3eee8e7212594c78d95b53b57cceda63")),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = "https://localhost:7035",
            ValidAudience = "https://localhost:7035",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

builder.Services.AddOutputCache();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISubCategoryService, SubCategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductVariantService, ProductVariantService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddCascadingAuthenticationState();


builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7035/"); 
});

builder.Services.AddLocalization();
builder.Services.AddControllers();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.UseOutputCache();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
