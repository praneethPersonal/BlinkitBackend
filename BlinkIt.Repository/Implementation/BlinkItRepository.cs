using BlinkIt.Repository.Interfaces;
using BlinkIt.Repository.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BlinkIt.Repository.Implementation;

public class BlinkItRepository : IBlinkItRepository
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<Product> _products;

    public BlinkItRepository()
    {
        var client = new MongoClient("mongodb://praneeth:blinkit@localhost:27017/mydatabase?authSource=admin");
        _database = client.GetDatabase("blinkit");
        _products = _database.GetCollection<Product>("products");
    }

    public List<Product> GetProductByCategoryFromRepo()
    {
        var productsByCategory = _products.Find(iter => true).ToList(); 
        return productsByCategory;
    }

    public Product GetProduct(string productName)
    {
        var product = _products.Find(iter => iter.product_name == productName).ToList();
        if (product.Any())
        {
            var firstProduct = product.First();
            return firstProduct;
        }
        
        return null;
    }
    
    public List<Category> GetCategoryFromRepo()
    {
        var categories = _database.GetCollection<Category>("categories");
        return categories.Find(_ => true).ToList();
    }

    public List<Product> GetProductsWithPrefix(string prefix)
    {
        var filter = Builders<Product>.Filter.Regex("product_name", new BsonRegularExpression($".*{prefix}.*", "i"));
        var productsWithPrefix = _products.Find(filter).Limit(50).ToList();
        return productsWithPrefix;
    }
    
}