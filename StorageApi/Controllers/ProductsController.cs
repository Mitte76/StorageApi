using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StorageApi.DTOs;
using StorageApi.Models;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly StorageApiContext _context;

    public ProductsController(StorageApiContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
    {
        return await _context.Product.ToListAsync();
    }

    // GET: api/Product
    [HttpGet("dto")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProduct2()
    {
        return await _context
            .Product.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Count))
            .ToListAsync();
    }

    // GET: api/Product/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Product.FindAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return product;
    }

    [HttpGet("dto/{id}")]
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

    // PUT: api/Product/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<ActionResult<Product>> PutProduct(int? id, Product product)
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

        return Ok(product);
    }

    [HttpPut("dto/{id}")]
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
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Product>> PostProduct(Product product)
    {
        _context.Product.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetProduct", new { id = product.Id }, product);
    }

    [HttpPost("dto")]
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
