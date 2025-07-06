using Microsoft.EntityFrameworkCore;
using WallyCart.Models;
using WallyCart.Dtos.Lists;

public class ListService
{
    private readonly WallyCartDbContext _context;

    public ListService(WallyCartDbContext context)
    {
        _context = context;
    }

    # region Helpers

    public async Task<List> GetOrCreateListAsync(Guid groupId)
    {
        var list = await _context.Lists.FirstOrDefaultAsync(l => l.GroupId == groupId);

        if (list == null)
        {
            list = new List { GroupId = groupId, CreatedAt = DateTime.UtcNow };
            _context.Lists.Add(list);
            await _context.SaveChangesAsync();
        }
        return list;
    }

    public async Task<Product> GetOrCreateProductAsync(Guid productId, string? name = null, string source = "custom")
    {
        var product = await _context.Products.FindAsync(productId);

        if (product == null)
        {
            product = new Product
            {
                Id = productId,
                Name = name ?? "Unnamed",
                Source = source,
                CreatedAt = DateTime.UtcNow
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
        return product;
    }

    #endregion

    #region Public APi

    public async Task<IEnumerable<ListItemDto>> GetItemsAsync(Guid groupId)
    {
        return await _context.ListItems
            .Include(li => li.Product)
            .Where(li => li.List.GroupId == groupId)
            .OrderBy(li => li.AddedAt)
            .Select(li => new ListItemDto
            {
                Id = li.Id,
                ProductId = li.ProductId,
                ProductName = li.Product.Name,
                Quantity = li.Quantity,
                AddedAt = li.AddedAt,
                AddedBy = li.AddedBy
            })
            .ToListAsync();
    }


    public async Task<ListItemDto> AddItemAsync(
    Guid groupId,
    Guid productId,
    Guid addedByUserId,
    int quantity = 1,
    string? productNameIfCustom = null)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than 0");

        var list = await GetOrCreateListAsync(groupId);
        var prod = await GetOrCreateProductAsync(productId, productNameIfCustom);

        var existing = await _context.ListItems
            .Include(li => li.Product)
            .FirstOrDefaultAsync(li => li.ListId == list.Id && li.ProductId == productId);

        if (existing != null)
        {
            existing.Quantity += quantity;
            await _context.SaveChangesAsync();
            return new ListItemDto
            {
                Id = existing.Id,
                ProductId = existing.ProductId,
                ProductName = existing.Product.Name,
                Quantity = existing.Quantity,
                AddedAt = existing.AddedAt,
                AddedBy = existing.AddedBy
            };
        }

        var item = new ListItem
        {
            ListId = list.Id,
            ProductId = productId,
            AddedBy = addedByUserId,
            Quantity = quantity,
            AddedAt = DateTime.UtcNow
        };

        _context.ListItems.Add(item);
        await _context.SaveChangesAsync();

        return new ListItemDto
        {
            Id = item.Id,
            ProductId = productId,
            ProductName = prod.Name,
            Quantity = item.Quantity,
            AddedAt = item.AddedAt,
            AddedBy = addedByUserId
        };
    }

    public async Task<bool> UpdateQuantityAsync(Guid listItemId, int newQuantity)
    {
        var item = await _context.ListItems.FindAsync(listItemId);

        if (item == null) return false;

        if (newQuantity <= 0)
        {
            _context.ListItems.Remove(item);
        }
        else
        {
            item.Quantity = newQuantity;
            _context.ListItems.Update(item);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveItemAsync(Guid listItemId)
    {
        var item = await _context.ListItems.FindAsync(listItemId);
        if (item == null) return false;

        _context.ListItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> ClearListAsync(Guid groupId)
    {
        var list = await _context.Lists.FirstOrDefaultAsync(l => l.GroupId == groupId);
        if (list == null) return 0;

        var items = _context.ListItems.Where(li => li.ListId == list.Id);
        if (items == null) return 0;

        _context.ListItems.RemoveRange(items);
        await _context.SaveChangesAsync();

        return 1;
    }
    
    #endregion

}