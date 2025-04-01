using CoffeeShopAdmin.Models.CategoryM;

namespace CoffeeShopAdmin.Models.TableM
{
    public class TableResponseModel
    {
        public List<TableRequestModel> Table { get; set; }
        public int TotalItems { get; set; }
    }
}
