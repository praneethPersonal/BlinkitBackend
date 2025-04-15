using BlinkItClone.Model;
using BlinkItClone.Repository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlinkItClone.Services
{
    public class BlinkItService:IBlinkItService
    {
        public IBlinkItRepository _detailsFromRepo;
        private readonly IConfiguration _configuration;
        public BlinkItService(IBlinkItRepository DetailsFromRepo, IConfiguration configuration) {

            _detailsFromRepo = DetailsFromRepo;
            _configuration = configuration;
        }

        public List<Category> GetCategory()
        {
            var products = _detailsFromRepo.GetCategoryFromRepo();
            return products;
        }
        public async Task AddCategoryAsync(Category category)
        {
            // Optionally add business logic here like checking for duplicate category names
            await _detailsFromRepo.AddCategoryAsync(category);
        }
        public List<Product> GetProductByCategory()
        {
            var product = _detailsFromRepo.GetProductByCategoryFromRepo();
            return product;
        }
        public Product GetProductDetails(string productName)
        {
            var prod = _detailsFromRepo.GetProduct(productName);
            return prod;
        }
        public async Task<bool> AddProductAsync(Product product)
        {
            var prod = _detailsFromRepo.GetProduct(product.product_name);
            if (prod != null)
            {
                return false;
            }
            await _detailsFromRepo.AddProductAsync(product);
            return true;
        }
        public async Task UpdateCart(string userId, string productId, string action)
        {
            if (action == "add")
            {
                await _detailsFromRepo.AddProductToCart(userId, productId);
            }
            else if (action == "remove")
            {
                await _detailsFromRepo.RemoveProductFromCart(userId, productId);
            }
        }
        public Task<List<Cart>> GetCartItems(string userId)
        {
            return  _detailsFromRepo.GetCartItemsByUserId(userId);

        }


 

        public async Task<(bool Success, string Message, string Token)> ValidateCredentialsAsync(string mobileNumber, string password)
        {
            var user = await _detailsFromRepo.GetUserByMobileNumberAsync(mobileNumber);

            if (user == null)
            {
                return (false, "inValid Mobile Number!", null);
            }

            if (user.Password == password)
            {
              
                var token = GenerateToken(mobileNumber);
                return (true, "Login successful!", token);
            }

            return (false, "Invalid password.", null);
        }

        public async Task<(bool Success, string Message, string Token)> CreateNewUser(string mobileNumber, string password)
        {
            var existingUser = await _detailsFromRepo.GetUserByMobileNumberAsync(mobileNumber);
            if (existingUser != null)
            {
                return (false, "Mobile number already exists. Cannot create new User.", null);
            }

            var newUser = new User
            {
                MobileNumber = mobileNumber,
                Password = password,
                ProductsBought = new List<string>()
            };

            await _detailsFromRepo.CreateUserAsync(newUser);

            var token = GenerateToken(mobileNumber);

            return (true, "New user created and login successful!", token);
        }
        public async Task<(bool Success, string Message, string Token)> CreateNewSeller(string mobileNumber, string password)
        {

            var existingSeller = await _detailsFromRepo.GetSellerByUserNameAsync(mobileNumber);

            if (existingSeller != null)
            {
                return (false, "Mobile number already exists. Cannot create new seller.", null);
            }


            var newUser = new Seller
            {
                UserName = mobileNumber,
                Password = password,
                Products = new List<string>()
            };

            await _detailsFromRepo.CreateSellerAsync(newUser);

            var token = GenerateToken(mobileNumber);

            return (true, "New user created and login successful!", token);
        }

        public async Task<(bool Success, string Message, string Token)> ValidateCredentialsSellerAsync(string userName, string password)
        {
            var user = await _detailsFromRepo.GetSellerByUserNameAsync(userName);

            if (user == null)
            {
                return (false, "New user please sign up!", null);
            }

            if (user.Password == password)
            {

                var token = GenerateToken(userName);
                return (true, "Login successful!", token);
            }

            return (false, "Invalid password.", null);
        }


        public async Task AddProductsAsync(string mobileNumber, string products)
        {
            await _detailsFromRepo.AddProductsToSeller(mobileNumber, products);
        }


        public async Task AddProductsToUserAsync(string mobileNumber, List<string> products)
        {
            await _detailsFromRepo.AddProductsToUser(mobileNumber, products);
        }




        public string GenerateToken(string mobileNumber)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
           
            var key = Encoding.ASCII.GetBytes("your_secret_key_here and itshould be 256 bits not lesser"); 

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, mobileNumber) 
            }),
                Expires = DateTime.UtcNow.AddHours(1),
                
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<List<string>> GetProductsBoughtAsync(string mobileNumber)
        {
            // Call repository to get the ProductsBought list
            return await _detailsFromRepo.GetProductsBoughtByMobileNumberAsync(mobileNumber);
        }
        public async Task<bool> ChangePasswordAsync(string mobileNumber, string currentPassword, string newPassword)
        {
            return await _detailsFromRepo.ChangePasswordAsync(mobileNumber, currentPassword, newPassword);
        }

        public async Task<List<String>> GetProductsBySellerAsync(string sellerName)
        {
            return await _detailsFromRepo.GetProductsBySellerAsync(sellerName);
        }
        public async Task UpdateStockByProductIdAsync(string productId, int newStock)
        {
            await _detailsFromRepo.UpdateStockByProductIdAsync(productId, newStock);
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _detailsFromRepo.GetAllUsersAsync();
        }

    }
}
