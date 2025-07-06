using Microsoft.EntityFrameworkCore;
using WallyCart.Dtos.Products;
using WallyCart.Models;

public class ProductService
{
    private readonly WallyCartDbContext _context;

    public ProductService(WallyCartDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto?> GetIdByAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Source = product.Source
        };
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Source = "custom",
            Barcode = dto.Barcode,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Source = product.Source,
        };
    }

    public async Task<IEnumerable<ProductDto>> SearchByNameAsync(string query)
    {
        return await _context.Products
            .Where(p => p.Name.ToLower().Contains(query.ToLower()))
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Source = p.Source
            }).ToListAsync();
    }

    public async Task<ProductDto?> GetByBarcodeAsync(string barcode)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Barcode == barcode);
        return product == null ? null : new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Source = product.Source
        };
    }
}