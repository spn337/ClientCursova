
namespace ServiceDll.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string PhotoName { get; set; }
    }
    public class ProductAddModel
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Photo { get; set; }
    }
    public class ProductDeleteModel
    {
        public int Id { get; set; }
    }
}