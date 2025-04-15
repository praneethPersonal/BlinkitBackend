using BlinkItClone.Model;

namespace BlinkItClone.Repository
{
    public interface IBlinkItRepository
    {
        public List<Category> GetCategoryFromRepo();
        public List<Product> GetProductByCategoryFromRepo();

        public Product GetProduct(string productId);
        public Task AddProductToCart(string userId, string productId);
        public Task RemoveProductFromCart(string userId, string productId);

        public Task<List<Cart>> GetCartItemsByUserId(string userId);
        Task<User> GetUserByMobileNumberAsync(string mobileNumber);
        public  Task<Seller> GetSellerByUserNameAsync(string userName);
        Task CreateUserAsync(User newUser);
      
        public Task CreateSellerAsync(Seller newUser);
        public Task AddProductsToUser(string mobileNumber, List<string> products);
        public Task AddProductsToSeller(string mobileNumber, string products);
        Task<List<string>> GetProductsBoughtByMobileNumberAsync(string mobileNumber);
        Task<bool> ChangePasswordAsync(string mobileNumber, string currentPassword, string newPassword);
        public Task AddCategoryAsync(Category category);
        public Task AddProductAsync(Product product);
        public Task UpdateStockByProductIdAsync(string productId, int newStock);
        public Task<List<String>> GetProductsBySellerAsync(string sellerName);
        public Task<List<User>> GetAllUsersAsync();
    }
}
