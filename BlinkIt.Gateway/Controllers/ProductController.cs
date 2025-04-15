using Microsoft.AspNetCore.Mvc;
using BlinkIt.Service.Interfaces;

namespace BlinkIt.Gateway.Controllers
{
    [ApiController]
    [Route("api/blinkit/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            var products = _productService.GetProductByCategory();

            if (products == null)
            {
                return NotFound("Product not found.");
            }
            return Ok(products);
        }
        [HttpGet("product")]
        public IActionResult GetProductDetails([FromQuery] string productName)
        {
            var product = _productService.GetProductDetails(productName);
            if (product is null)
            {
                return NotFound("Product not found.");
            }
            return Ok(product);
        }

        [HttpGet("product/:keyword")]
        public IActionResult GetProductByKeyword(string keyword)
        {
            if (keyword == null || keyword.Length < 2)
            {
                return BadRequest("Keyword is empty or less than 2 characters.");
            }
            
            var products = _productService.GetAllProductsWithKeyWord(keyword);
            return Ok(products);
        }
    }
}
