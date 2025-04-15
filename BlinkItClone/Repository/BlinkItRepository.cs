using BlinkItClone.Model;
using MongoDB.Driver;

namespace BlinkItClone.Repository
{
    public class BlinkItRepository:IBlinkItRepository
    {
        private IMongoDatabase _database; 
        public BlinkItRepository() 
        {
            var client = new MongoClient("mongodb://praneeth:blinkit@localhost:27017/mydatabase?authSource=admin");
            _database = client.GetDatabase("blinkit");
        }
        public List<Category> GetCategoryFromRepo()
        {
            var categories = _database.GetCollection<Category>("categories");
            return categories.Find(_ => true).ToList();
        }
        public async Task AddCategoryAsync(Category category)
        {
            var categories = _database.GetCollection<Category>("categories");
            // Insert the new category into the collection
            await categories.InsertOneAsync(category);
        }

        public List<Product> GetProductByCategoryFromRepo()
        {
            var products = _database.GetCollection<Product>("products");
            var productsByCategory = products.Find(iter => true).ToList(); 
            return productsByCategory;
        }
        public Product GetProduct(string productName)
        {
            var products = _database.GetCollection<Product>("products");
            var product = products.Find(iter => iter.product_name == productName).ToList();
            if (product.Any())
            {
                var firstProduct = product.First();
                return firstProduct;
            }
          
            //var product = products.Find(iter => iter._id == productId).ToList().First();
            return null;
        }
        public async Task AddProductAsync(Product product)
        {
            var products = _database.GetCollection<Product>("products");
            await products.InsertOneAsync(product);
        }

        public async Task AddProductToCart(string userId, string productId)
        {
            var cartCollection = _database.GetCollection<Cart>("Cart");

            var compositeId = $"{userId}_{productId}";

  
            var filter = Builders<Cart>.Filter.Eq("_id", compositeId);

            
            var update = Builders<Cart>.Update.Inc("Quantity", 1)
                                .SetOnInsert("UserId", userId)
                                .SetOnInsert("ProductId", productId);

           
            await cartCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }


        public async Task RemoveProductFromCart(string userId, string productId)
        {
            var cartCollection = _database.GetCollection<Cart>("Cart");


            var compositeId = $"{userId}_{productId}";

           
            var filter = Builders<Cart>.Filter.Eq("_id", compositeId);

            var cartItem = await cartCollection.Find(filter).FirstOrDefaultAsync();

            if (cartItem != null && cartItem.Quantity > 1)
            {
        
                var update = Builders<Cart>.Update.Inc("Quantity", -1);
                await cartCollection.UpdateOneAsync(filter, update);
            }
            else
            {
            
                await cartCollection.DeleteOneAsync(filter);
            }
        }

        public async Task<List<Cart>> GetCartItemsByUserId(string userId)
        {
            var cartCollection = _database.GetCollection<Cart>("Cart");

          
            var filter = Builders<Cart>.Filter.Eq("UserId", userId);

            var cartItems = await cartCollection.Find(filter).ToListAsync();
            return cartItems;
        }

        public async Task<User> GetUserByMobileNumberAsync(string mobileNumber)
        {
            var _users = _database.GetCollection<User>("UserInformation");
            return await _users.Find(user => user.MobileNumber == mobileNumber).FirstOrDefaultAsync();
        }

        public async Task<Seller> GetSellerByUserNameAsync(string userName)
        {
            var _users = _database.GetCollection<Seller>("SellerInformation");
            return await _users.Find(user => user.UserName == userName).FirstOrDefaultAsync();
        }

        public async Task CreateUserAsync(User newUser)
        {
            var _users = _database.GetCollection<User>("UserInformation");
            await _users.InsertOneAsync(newUser);
        }
        public async Task CreateSellerAsync(Seller newUser)
        {
            var _users = _database.GetCollection<Seller>("SellerInformation");
            await _users.InsertOneAsync(newUser);
        }

        public async Task AddProductsToUser(string mobileNumber, List<string> products)
        {
            var _usersCollection = _database.GetCollection<User>("UserInformation");
            var filter = Builders<User>.Filter.Eq(u => u.MobileNumber, mobileNumber);
            var update = Builders<User>.Update.AddToSetEach(u => u.ProductsBought, products);
            await _usersCollection.UpdateOneAsync(filter, update);

        }

        public async Task AddProductsToSeller(string mobileNumber, string products)
        {
            var _usersCollection = _database.GetCollection<Seller>("SellerInformation");

            // Create a filter to match the seller by mobileNumber (UserName)
            var filter = Builders<Seller>.Filter.Eq(u => u.UserName, mobileNumber);

            // Use $push to add products to the Products array
            var update = Builders<Seller>.Update.Push(u => u.Products, products);

            // Update the document
            await _usersCollection.UpdateOneAsync(filter, update);
        }



        public async Task<List<string>> GetProductsBoughtByMobileNumberAsync(string mobileNumber)
        {
            var _usersCollection = _database.GetCollection<User>("UserInformation");
            var user = await _usersCollection
                .Find(u => u.MobileNumber == mobileNumber)
                .FirstOrDefaultAsync();

            return user?.ProductsBought; // Return the ProductsBought list, or null if user not found
        }

        public async Task<bool> ChangePasswordAsync(string mobileNumber, string currentPassword, string newPassword)
        {
            var _usersCollection = _database.GetCollection<User>("UserInformation");
            // Find the user based on the mobile number
            var user = await _usersCollection.Find(u => u.MobileNumber == mobileNumber).FirstOrDefaultAsync();

            // Check if the user exists and the current password matches
            if (user != null && user.Password == currentPassword)
            {
         
                var update = Builders<User>.Update.Set(u => u.Password, newPassword);

                var result = await _usersCollection.UpdateOneAsync(
                    u => u.MobileNumber == mobileNumber,
                    update
                );

                return result.ModifiedCount > 0;
            }

            return false;
        }
        public async Task<List<String>> GetProductsBySellerAsync(string sellerName)
        {
            // Get the SellerInformation collection
            var sellersCollection = _database.GetCollection<Seller>("SellerInformation");

            // Create a filter to match the seller by userName (which is passed as sellerName)
            var filter = Builders<Seller>.Filter.Eq(s => s.UserName, sellerName);

            // Find the seller document
            var seller = await sellersCollection.Find(filter).FirstOrDefaultAsync();

            // If the seller document is found, return the Products list; otherwise, return an empty list
            return seller?.Products ?? new List<String>();
        }

        public async Task UpdateStockByProductIdAsync(string productName, int newStock)
        {

            var products = _database.GetCollection<Product>("Products");
            var filter = Builders<Product>.Filter.Eq(p => p.product_name, productName);
            var update = Builders<Product>.Update.Set(p => p.stock, newStock);
            await products.UpdateOneAsync(filter, update);
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            var _usersCollection = _database.GetCollection<User>("UserInformation");
            return await _usersCollection.Find(_ => true).ToListAsync(); // Fetch all users
        }


    }

}
