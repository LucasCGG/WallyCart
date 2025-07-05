using Microsoft.EntityFrameworkCore;
using WallyCart.Models;
using WallyCart.Enums;

public class ListService
{
    private readonly WallyCartDbContext _context;

    public ListService(WallyCartDbContext context)
    {
        _context = context;
    }

    public async Task<List> GetOrCreateListAsync(Guid groupId)
    {
        var list = await _context.Lists.FirstOrDefaultAsync(l => l.GroupId == groupId);
        
        if(list == null)
        {
            list = new List { GroupId = groupId, CreatedAt = DateTime.UtcNow };
            _context.Lists.Add(list);
            await _context.SaveChangesAsync();
        }
        return list;
    }
    
    public async Task<Product> GerOrCreateProductAsync(Guid productId, string? name = null, string source = "custom")
    {
        var product = await _context.Products.FindAsync(productId);

        if(product == null)
        {
            product = new Product
            {
                id          = productId,
                Name        = name ?? "Unnamed",
                Source      = source,
                CreatedAt   = DateTime.UtcNow
            };
            _context.Product.Add(product)
            await _context.SaveChangesAsync();
        }
        return product;
    }
}