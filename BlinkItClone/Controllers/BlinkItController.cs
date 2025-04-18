using System.Security.Claims;
using BlinkItClone.Model;
using BlinkItClone.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BlinkItClone.Controllers
{
    [Route("api/[controller]")]
    public class BlinkItController : ControllerBase
    {
        private readonly IBlinkItService _serviceCall;

        public BlinkItController(IBlinkItService serviceCall)
        {
            _serviceCall = serviceCall;
        }

        /*[HttpGet("category")]
        public IActionResult GetCategories()
        {
            var products = _serviceCall.GetCategory();
            if (products == null)
            {
                return NotFound("No products found for the given category.");
            }
            return Ok(products);
        }*/

        /*[HttpGet("products")]
        public IActionResult GetProductsByCategory()
        {
            var products = _serviceCall.GetProductByCategory();

            if (products == null)
            {
                return NotFound("Product not found.");
            }
            return Ok(products);
        }*/
        /*[HttpGet("product")]
        public IActionResult GetProductDetails([FromQuery] string productName)
        {
            var product = _serviceCall.GetProductDetails(productName);
            if (product == null)
            {
                return NotFound("Product not found.");
            }
            return Ok(product);
        }*/
        
        //not neccessary
        [HttpPost("addCategory")]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            if (string.IsNullOrEmpty(category.category_name))
            {
                return BadRequest("Category name is required.");
            }

            try
            {
                await _serviceCall.AddCategoryAsync(category);
                return Ok("Category added successfully.");
            }
            catch (Exception ex)
            {
                // Log exception if needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("addProduct")]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            if (string.IsNullOrEmpty(product.product_name) ||  product.price <= 0 || product.stock < 0)
            {
                return BadRequest("Invalid product data.");
            }

            try
            {
                var res = await _serviceCall.AddProductAsync(product);
                if (res)
                {
                    return Ok("Product added successfully.");
                }
                else
                {
                    return Unauthorized("Product Exists");
                }
                
            }
            catch (Exception ex)
            {
                // Log exception if needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("user/addProducts")]
        [Authorize]
        public async Task<IActionResult> AddProductsToUser([FromBody] AddProductsRequest request)
        {

            var claims = User.Claims.ToList();
            var mobileNumber = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;

            if (string.IsNullOrEmpty(mobileNumber))
            {
                return Unauthorized("Invalid or missing mobile number in token.");
            }

            if (request.Products == null || request.Products.Count == 0)
            {
                return BadRequest("Invalid product list.");
            }

            await _serviceCall.AddProductsToUserAsync(mobileNumber, request.Products);

            return Ok("Products added successfully.");
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.MobileNumber) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest("Mobile number and password are required.");
            }

            var (success, message, token) = await _serviceCall.ValidateCredentialsAsync(loginRequest.MobileNumber, loginRequest.Password);

            if (success)
            {
                return Ok(new { Message = message, Token = token });
            }

            return Unauthorized(new { Message = message });
        }
        
        [HttpPost("signUp")]
        public async Task<IActionResult> SignUp([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.MobileNumber) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest("Mobile number and password are required.");
            }

            var (success, message, token) = await _serviceCall.CreateNewUser(loginRequest.MobileNumber, loginRequest.Password);

            if (success)
            {
                return Ok(new { Message = message, Token = token });
            }

            return Unauthorized(new { Message = message });
        }

        [HttpPost("login/seller")]
        public async Task<IActionResult> SellerLogin([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.MobileNumber) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest("UserName and password are required.");
            }

            var (success, message, token) = await _serviceCall.ValidateCredentialsSellerAsync(loginRequest.MobileNumber, loginRequest.Password);

            if (success)
            {
                return Ok(new { Message = message, Token = token });
            }

            return Unauthorized(new { Message = message });
        }

        [HttpPost("login/seller/signup")]
        public async Task<IActionResult> SignUpSeller([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.MobileNumber) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var (success, message, token) = await _serviceCall.CreateNewSeller(loginRequest.MobileNumber, loginRequest.Password);

            if (success)
            {
                return Ok(new { Message = message, Token = token });
            }

            return Unauthorized(new { Message = message });
        }


        [HttpPost("seller/addProducts")]
        [Authorize] 
        public async Task<IActionResult> AddProducts([FromBody] AddProductsRequestBySeller request)
        {

            var claims = User.Claims.ToList(); 
            
            var mobileNumber = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;

            if (string.IsNullOrEmpty(mobileNumber))
            {
                return Unauthorized("Invalid or missing mobile number in token.");
            }

            if (request == null)
            {
                return BadRequest("Invalid product list.");
            }

            await _serviceCall.AddProductsAsync(mobileNumber, request.Products);

            return Ok("Products added successfully.");
        }

        [HttpGet("products-bought")]
        public async Task<IActionResult> GetProductsBought()
        {
            // Extract mobile number from JWT token claims
            var mobileNumber = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(mobileNumber))
            {
                return Unauthorized("Invalid or missing mobile number in token.");
            }

            var productsBought = await _serviceCall.GetProductsBoughtAsync(mobileNumber);

            if (productsBought == null)
            {
                return NotFound("No products found for this user.");
            }

            return Ok(productsBought);
        }

        [HttpPost("changePassword")]
        [Authorize] // Ensure the user is authorized and has a valid JWT token
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            // Extract mobile number from JWT token claims
            var mobileNumber = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(mobileNumber))
            {
                return Unauthorized("Invalid or missing mobile number in token.");
            }

            if (string.IsNullOrEmpty(request.CurrentPassword) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest("Current password and new password cannot be empty.");
            }

            var success = await _serviceCall.ChangePasswordAsync(mobileNumber, request.CurrentPassword, request.NewPassword);

            if (success)
            {
                return Ok("Password changed successfully.");
            }
            else
            {
                return BadRequest("Current password is incorrect or could not change password.");
            }
        }

        [HttpPost("getProductsBySeller")]
        [Authorize]
        public async Task<IActionResult> GetProductsBySeller()
        {

            var sellerName = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;


            // Check if sellerName was retrieved from the token
            if (string.IsNullOrEmpty(sellerName))
            {
                return BadRequest("Seller name not found in token.");
            }

            // Get products by seller
            var products = await _serviceCall.GetProductsBySellerAsync(sellerName);

            // Check if any products were found
            if (products == null || products.Count == 0)
            {
                return NotFound("No products found for the given seller.");
            }

            // Return the products
            return Ok(products);
        }

        [HttpPost("updateStock")]
        public async Task<IActionResult> UpdateStock([FromBody] UpdateStockRequest request)
        {
            if (string.IsNullOrEmpty(request.Product_name) && request.NewStock < 0)
            {
                return BadRequest("Invalid product ID or stock.");
            }

            await _serviceCall.UpdateStockByProductIdAsync(request.Product_name, request.NewStock);

            return Ok("Stock updated successfully.");
        }
        [HttpGet("getAllUserDetails")]
        public async Task<IActionResult> GetAllUserDetails()
        {
            // Fetch all users from the database
            var users = await _serviceCall.GetAllUsersAsync();

            if (users == null || !users.Any())
            {
                return NotFound("No users found.");
            }

            // Prepare the response with mobile numbers and products bought
            var userDetailsList = users.Select(user => new
            {
                MobileNumber = user.MobileNumber,
                ProductsBought = user.ProductsBought
            }).ToList();

            return Ok(userDetailsList);
        }

    }
}

