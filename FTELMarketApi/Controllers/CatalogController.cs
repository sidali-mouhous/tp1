using FTELMarketApi.Data;
using FTELMarketApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FTELMarketApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly AppDbContext _context;

    public CatalogController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts()
    {
        var products = await _context.Products
            .Include(product => product.Category)
            .OrderBy(product => product.Id)
            .Select(product => new ProductResponse(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.QuantityInStock,
                product.CreatedAt,
                product.CategoryId,
                product.Category.Name))
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductResponse>> GetProduct(int id)
    {
        var product = await _context.Products
            .Include(product => product.Category)
            .Where(product => product.Id == id)
            .Select(product => new ProductResponse(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.QuantityInStock,
                product.CreatedAt,
                product.CategoryId,
                product.Category.Name))
            .SingleOrDefaultAsync();

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> CreateProduct([FromBody] ProductCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Le nom du produit est obligatoire.");
        }

        if (request.Price <= 0)
        {
            return BadRequest("Le prix doit être strictement positif.");
        }

        var categoryExists = await _context.Categories.AnyAsync(category => category.Id == request.CategoryId);
        if (!categoryExists)
        {
            return BadRequest("La catégorie sélectionnée est introuvable.");
        }

        var product = new Product
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Price = request.Price,
            QuantityInStock = request.QuantityInStock,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var response = await _context.Products
            .Include(item => item.Category)
            .Where(item => item.Id == product.Id)
            .Select(item => new ProductResponse(
                item.Id,
                item.Name,
                item.Description,
                item.Price,
                item.QuantityInStock,
                item.CreatedAt,
                item.CategoryId,
                item.Category.Name))
            .SingleAsync();

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductResponse>> UpdateProduct(int id, [FromBody] ProductUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Le nom du produit est obligatoire.");
        }

        if (request.Price <= 0)
        {
            return BadRequest("Le prix doit être strictement positif.");
        }

        var existing = await _context.Products
            .Include(product => product.Category)
            .SingleOrDefaultAsync(product => product.Id == id);

        if (existing is null)
        {
            return NotFound();
        }

        var categoryExists = await _context.Categories.AnyAsync(category => category.Id == request.CategoryId);
        if (!categoryExists)
        {
            return BadRequest("La catégorie sélectionnée est introuvable.");
        }

        existing.Name = request.Name.Trim();
        existing.Description = request.Description?.Trim();
        existing.Price = request.Price;
        existing.QuantityInStock = request.QuantityInStock;
        existing.CategoryId = request.CategoryId;

        await _context.SaveChangesAsync();

        return Ok(new ProductResponse(
            existing.Id,
            existing.Name,
            existing.Description,
            existing.Price,
            existing.QuantityInStock,
            existing.CreatedAt,
            existing.CategoryId,
            existing.Category.Name));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.SingleOrDefaultAsync(product => product.Id == id);
        if (product is null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
