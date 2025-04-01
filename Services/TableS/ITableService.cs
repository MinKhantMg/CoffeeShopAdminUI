using CoffeeShopAdmin.Models.TableM;

namespace CoffeeShopAdmin.Services.TableS
{
    public interface ITableService
    {
        Task<List<TableRequestModel>> GetTables();
        Task<TableRequestModel> GetTableById(string id);
        Task<bool> CreateTable(TableRequestModel table);
        Task<bool> UpdateTable(string id, TableRequestModel table);
        Task<bool> DeleteTable(string id);
    }
}
