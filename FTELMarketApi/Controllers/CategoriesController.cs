using FTELMarketApi.Data;
using FTELMarketApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FTELMarketApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategories()
    {
        var categories = await _context.Categories
            .OrderBy(category => category.Id)
            .Select(category => new CategoryResponse(category.Id, category.Name))
            .ToListAsync();

        return Ok(categories);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryResponse>> GetCategory(int id)
    {
        var category = await _context.Categories
            .Where(category => category.Id == id)
            .Select(category => new CategoryResponse(category.Id, category.Name))
            .SingleOrDefaultAsync();

        if (category is null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> CreateCategory([FromBody] CategoryCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Le nom de la catégorie est obligatoire.");
        }

        var normalizedName = request.Name.Trim();
        var exists = await _context.Categories.AnyAsync(category => category.Name == normalizedName);
        if (exists)
        {
            return BadRequest("Une catégorie portant ce nom existe déjà.");
        }

        var category = new Category { Name = normalizedName };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, new CategoryResponse(category.Id, category.Name));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryResponse>> UpdateCategory(int id, [FromBody] CategoryCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Le nom de la catégorie est obligatoire.");
        }

        var category = await _context.Categories.FindAsync(id);
        if (category is null)
        {
            return NotFound();
        }

        var normalizedName = request.Name.Trim();
        var duplicateExists = await _context.Categories
            .AnyAsync(existing => existing.Id != id && existing.Name == normalizedName);
        if (duplicateExists)
        {
            return BadRequest("Une catégorie portant ce nom existe déjà.");
        }

        category.Name = normalizedName;
        await _context.SaveChangesAsync();

        return Ok(new CategoryResponse(category.Id, category.Name));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories
            .Include(category => category.Products)
            .SingleOrDefaultAsync(category => category.Id == id);

        if (category is null)
        {
            return NotFound();
        }

        if (category.Products.Count > 0)
        {
            return BadRequest("Impossible de supprimer une catégorie qui contient encore des produits.");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
