using CoffeeShopAdmin.Models;
using CoffeeShopAdmin.Models.OrderM;

namespace CoffeeShopAdmin.Services.OrderS
{
    public interface IOrderService
    {
        Task<List<OrderModel>> GetPendingOrdersAsync();

        Task<List<OrderModel>> GetConfirmOrdersAsync();

        Task<OrderResponse> ConfirmOrderAsync(string orderId);
    }
}
