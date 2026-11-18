using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StorageApi.DTOs;
using StorageApi.Models;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(StorageApiContext context) : ControllerBase
{
    private readonly StorageApiContext _context = context;

    // GET: api/Product
    [HttpGet] //Mitt egna försök med querys
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsDtoQuery(
        [FromQuery] string? category,
        [FromQuery] string? name
    )
    {
        var hasCategory = !string.IsNullOrWhiteSpace(category);
        var hasName = !string.IsNullOrWhiteSpace(name);
        Task<List<ProductDto>> products;

        if (hasCategory && hasName)
        {
            products = _context
                .Product.Where(p => p.Category == category)
                .Where(p => p.Name == name)
                .Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Count))
                .ToListAsync();
        }
        else if (hasCategory)
        {
            products = _context
                .Product.Where(p => p.Category == category)
                .Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Count))
                .ToListAsync();
        }
        else if (hasName)
        {
            products = _context
                .Product.Where(p => p.Name == name)
                .Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Count))
                .ToListAsync();
        }
        else
        {
            products = _context
                .Product.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Count))
                .ToListAsync();
        }

        return await products;
    }

    // GET: api/Product/getCopilot
    [HttpGet("getCopilot")] //Efter lite hjälp från Copolot
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsDtoQuery2(
        [FromQuery] string? category,
        [FromQuery] string? name
    )
    {
        var products = _context.Product.AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            products = products.Where(p => p.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            products = products.Where(p => p.Name.Contains(name));
        }

        return await products
            .Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Count))
            .ToListAsync();
    }

    // GET: api/Product/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProductDto(int id)
    {
        var product = await _context
            .Product.Where(p => p.Id == id)
            .Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Count))
            .FirstOrDefaultAsync();

        if (product == null)
        {
            return NotFound();
        }

        return new ProductDto(product.Id, product.Name, product.Price, product.Count);
    }

    // GET: api/Product/stats
    [HttpGet("stats")] //Mitt första sätt
    public async Task<ActionResult<ProductStatsDto>> GetProductStats()
    {
        var totalProducts = await _context.Product.SumAsync(p => p.Count);
        var totalInventoryValue = await _context.Product.SumAsync(p => (decimal)p.Price * p.Count);
        var averagePrice =
            totalProducts == 0 ? 0m : await _context.Product.AverageAsync(p => (decimal)p.Price);

        var stats = new ProductStatsDto(totalProducts, totalInventoryValue, averagePrice);

        return Ok(stats ?? new ProductStatsDto(0, 0m, 0m));
    }

    // GET: api/Product/stats2
    [HttpGet("stats2")] //Mitt andra sätt
    public async Task<ActionResult<ProductStatsDto>> GetProductStats2()
    {
        var products = await _context.Product.Select(p => new { p.Price, p.Count }).ToListAsync();

        var totalProducts = products.Sum(p => p.Count);

        var totalInventoryValue = products.Sum(p => (decimal)p.Price * p.Count);

        var averagePrice = products.Count == 0 ? 0m : products.Average(p => (decimal)p.Price);

        var stats = new ProductStatsDto(totalProducts, totalInventoryValue, averagePrice);

        return Ok(stats);
    }

    // GET: api/Product/stats3
    [HttpGet("stats3")] //Copilots sätt
    public async Task<ActionResult<ProductStatsDto>> GetProductStats3()
    {
        var stats = await _context
            .Product.GroupBy(p => 1)
            .Select(g => new ProductStatsDto(
                g.Sum(p => p.Count),
                g.Sum(p => (decimal)p.Price * p.Count),
                g.Average(p => (decimal)p.Price)
            ))
            .SingleOrDefaultAsync();

        return Ok(stats ?? new ProductStatsDto(0, 0m, 0m));
    }

    // PUT: api/Product/5
    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> PutProductDto(int id, Product product)
    {
        if (id != product.Id)
        {
            return BadRequest();
        }

        _context.Entry(product).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return Ok(new ProductDto(product.Id, product.Name, product.Price, product.Count));
    }

    // POST: api/Product
    [HttpPost]
    public async Task<ActionResult<ProductDto>> PostProductDto(CreateProductDto productDto)
    {
        var product = new Product
        {
            Name = productDto.Name,
            Price = productDto.Price,
            Category = productDto.Category,
            Shelf = productDto.Shelf,
            Count = productDto.Count,
            Description = productDto.Description,
        };

        _context.Product.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            "GetProductDto",
            new { id = product.Id },
            new ProductDto(product.Id, product.Name, product.Price, product.Count)
        );
    }

    // DELETE: api/Product/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int? id)
    {
        var product = await _context.Product.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        _context.Product.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProductExists(int? id)
    {
        return _context.Product.Any(e => e.Id == id);
    }
}
