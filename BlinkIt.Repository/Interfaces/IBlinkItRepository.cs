using BlinkIt.Repository.Models;

namespace BlinkIt.Repository.Interfaces
{
    public interface IBlinkItRepository
    {
        public List<Product> GetProductByCategoryFromRepo();

        public Product GetProduct(string productId);
        public List<Category> GetCategoryFromRepo();
        public List<Product> GetProductsWithPrefix(string prefix);
    }
}
