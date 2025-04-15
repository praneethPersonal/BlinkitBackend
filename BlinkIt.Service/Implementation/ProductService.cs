using BlinkIt.Repository.Interfaces;
using BlinkIt.Service.Interfaces;
using BlinkIt.Repository.Models;

namespace BlinkIt.Service.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IBlinkItRepository _blinkItRepository;

        public ProductService(IBlinkItRepository blinkItRepository)
        {
            _blinkItRepository = blinkItRepository;
        }

        public List<Product> GetProductByCategory()
        {
            var product = _blinkItRepository.GetProductByCategoryFromRepo();
            return product;
        }
        public Product GetProductDetails(string productName)
        {
            var prod = _blinkItRepository.GetProduct(productName);
            return prod;
        }

        public List<Product> GetAllProductsWithKeyWord(string keyword)
        {
            var products = _blinkItRepository.GetProductsWithPrefix(keyword);
            return products;
        }

    }
}
