using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _ProductRepo;
        public ProductsController(IProductRepository ProductRepo)
        {
            _ProductRepo = ProductRepo;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string? brand, string? type, string? sort)
        {
            return Ok(await _ProductRepo.GetProductsAsync(brand, type, sort));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var product = await _ProductRepo.GetProductById(id);
            if (product == null)
                return NotFound();
            else 
                return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> AddProduct(Product product)
        {
             _ProductRepo.Add(product);
            if (await _ProductRepo.SaveChangesAsync())
                return CreatedAtAction("GetProductById", new { id = product.Id }, product);
            else
                return BadRequest("Couldnt create the product");
        }

        [HttpPost("{id:int}")]
        public async Task<ActionResult<Product>> EditProduct(int id, Product product)
        {
            if (id != product.Id || !_ProductRepo.ProductExists(id))
                return BadRequest("Product mismatch");

            _ProductRepo.Update(product);
            if (await _ProductRepo.SaveChangesAsync())
                return NoContent();

            return BadRequest("Couldn't edit product data");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _ProductRepo.GetProductById(id);

            if (product == null)
                return NotFound("Product not found");
            _ProductRepo.Delete(product);
            if (await _ProductRepo.SaveChangesAsync())
                return NoContent();
            return BadRequest("Failed to delete");
        }
    }
}

