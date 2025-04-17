using System.Net.Http;
using CoffeeShopAdmin.Models;
using CoffeeShopAdmin.Models.OrderM;
using Microsoft.AspNetCore.Components.Authorization;
using static System.Net.WebRequestMethods;

namespace CoffeeShopAdmin.Services.OrderS
{
    public class OrderService : IOrderService
    {
        private readonly ApiClient _client;
        private readonly AuthenticationStateProvider _authStateProvider;

        public OrderService(ApiClient client, AuthenticationStateProvider authenticationStateProvider)
        {
            _client = client;
            _authStateProvider = authenticationStateProvider;
        }

        public async Task<List<OrderModel>> GetPendingOrdersAsync()
        {
            return await _client.GetFromJsonAsync<List<OrderModel>>("/order/pending");
        }

        public async Task<List<OrderModel>> GetConfirmOrdersAsync()
        {
            return await _client.GetFromJsonAsync<List<OrderModel>>("/order/confirm");
        }

        public async Task<OrderResponse> ConfirmOrderAsync(string orderId)
        {
            var result = await _client.PutAsync<OrderResponse, object>($"/order/confirm/{orderId}", new { });

            return result ?? new OrderResponse
            {
                Message = "Order confirmed successfully."
            };
        }


    }
}
