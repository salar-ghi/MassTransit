using MyApiOne.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MyApiOne.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize(Policy = "ApiScope")]

//[Route("identity")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ILogger<ProductsController> logger)
    {
        _logger = logger;
    }

    private static readonly List<Product> products = new()
    {
        new Product { Id = 1, Name = "Product 1", Price = 9.99m },
        new Product { Id = 2, Name = "Product 2", Price = 19.99m },
        new Product { Id = 3, Name = "Product 3", Price = 29.99m },
        new Product { Id = 4, Name = "Product 4", Price = 39.99m },
        new Product { Id = 5, Name = "Product 5", Price = 49.99m }
    };



    [HttpGet]
    //[Authorize(Policy = "ApiScope")]
    public IEnumerable<Product> GetProducts()
    {
        return products;
    }

    [HttpGet("{id}")]
    public ActionResult<Product> GetProduct(int id)
    {
        var product = products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        return product;
    }

    [HttpGet("report/cheapest")]
    public ActionResult<Product> GetCheapestProduct()
    {
        var cheapestProduct = products.OrderBy(p => p.Price).FirstOrDefault();
        return cheapestProduct;
    }

    [HttpGet("report/most-expensive")]
    public ActionResult<Product> GetMostExpensiveProduct()
    {
        var mostExpensiveProduct = products.OrderByDescending(p => p.Price).FirstOrDefault();
        return mostExpensiveProduct;
    }

    [HttpGet("report/average-price")]
    public ActionResult<decimal> GetAverageProductPrice()
    {
        var averagePrice = products.Average(p => p.Price);
        return averagePrice;
    }


    [HttpPost]
    public ActionResult<Product> CreateProduct(Product product)
    {
        product.Id = products.Count + 1;
        products.Add(product);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public ActionResult<Product> UpdateProduct(int id, Product updatedProduct)
    {
        var product = products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        product.Name = updatedProduct.Name;
        product.Price = updatedProduct.Price;
        return product;
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(int id)
    {
        var product = products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        products.Remove(product);
        return NoContent();
    }


}