using ServiceDll.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceDll.Interfaces
{
    public interface IProductService
    {
        //Task вертає асинхронну задачу, яка працює у вторинному потоці
        List<ProductModel> GetProducts();
        Task<List<ProductModel>> GetProductsAsync();

        int Create(ProductAddModel product);
        Task<int> CreateAsync(ProductAddModel product);
    }
}
