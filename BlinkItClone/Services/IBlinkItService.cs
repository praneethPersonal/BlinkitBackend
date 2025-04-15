using BlinkItClone.Model;

namespace BlinkItClone.Services
{
    public interface IBlinkItService
    {
        public List<Category> GetCategory();
        public List<Product> GetProductByCategory();

        public Product GetProductDetails(string productId);

        public Task UpdateCart(string userId, string productId, string action);

        public Task<List<Cart>> GetCartItems(string userId);

        public Task<(bool Success, string Message, string Token)> ValidateCredentialsAsync(string mobileNumber, string password);
        public Task<(bool Success, string Message, string Token)> CreateNewUser(string mobileNumber, string password);
        public Task<(bool Success, string Message, string Token)> CreateNewSeller(string mobileNumber, string password);
        public Task<(bool Success, string Message, string Token)> ValidateCredentialsSellerAsync(string userName, string password);
        public Task AddProductsAsync(string mobileNumber, string products);
        Task<List<string>> GetProductsBoughtAsync(string mobileNumber);
        public Task<bool> ChangePasswordAsync(string mobileNumber, string currentPassword, string newPassword);
        public Task AddCategoryAsync(Category category);
        public Task<bool> AddProductAsync(Product product);
        public Task<List<String>> GetProductsBySellerAsync(string sellerName);
        public  Task UpdateStockByProductIdAsync(string productId, int newStock);
        public Task<List<User>> GetAllUsersAsync();
        public Task AddProductsToUserAsync(string mobileNumber, List<string> products);
    }
}
