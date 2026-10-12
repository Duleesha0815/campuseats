using CampusEats.Api.Data;
using CampusEats.Api.Dtos;
using CampusEats.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Api.Services;

public class MenuService(MenuDbContext db) : IMenuService
{
    private static MenuItemDto ToDto(MenuItem item) =>
        new(item.Id, item.Name, item.Price, item.Category.Name, item.Available);

    public async Task<IReadOnlyList<MenuItemDto>> GetAll(CancellationToken cancellationToken) =>
        await db.MenuItems
            .AsNoTracking()
            .Include(item => item.Category)
            .OrderBy(item => item.Id)
            .Select(item => new MenuItemDto(
                item.Id,
                item.Name,
                item.Price,
                item.Category.Name,
                item.Available))
            .ToListAsync(cancellationToken);

    public async Task<MenuItemDto?> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await db.MenuItems
            .AsNoTracking()
            .Include(menuItem => menuItem.Category)
            .FirstOrDefaultAsync(menuItem => menuItem.Id == id, cancellationToken);
        return item is null ? null : ToDto(item);
    }

    public async Task<MenuItemDto?> Create(CreateMenuItemDto dto, CancellationToken cancellationToken)
    {
        var category = await FindCategory(dto.Category, cancellationToken);
        if (category is null)
        {
            return null;
        }

        var item = new MenuItem
        {
            Name = dto.Name.Trim(),
            Price = dto.Price,
            CategoryId = category.Id,
            Available = true
        };
        db.MenuItems.Add(item);
        await db.SaveChangesAsync(cancellationToken);
        return new MenuItemDto(item.Id, item.Name, item.Price, category.Name, item.Available);
    }

    public async Task<bool> Update(int id, CreateMenuItemDto dto, CancellationToken cancellationToken)
    {
        var item = await db.MenuItems.FirstOrDefaultAsync(
            menuItem => menuItem.Id == id, cancellationToken);
        if (item is null)
        {
            return false;
        }

        var category = await FindCategory(dto.Category, cancellationToken);
        if (category is null)
        {
            throw new ArgumentException($"Menu category '{dto.Category}' does not exist.", nameof(dto));
        }

        item.Name = dto.Name.Trim();
        item.Price = dto.Price;
        item.CategoryId = category.Id;
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> Delete(int id, CancellationToken cancellationToken)
    {
        var item = await db.MenuItems.FirstOrDefaultAsync(
            menuItem => menuItem.Id == id, cancellationToken);
        if (item is null)
        {
            return false;
        }

        db.MenuItems.Remove(item);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private Task<MenuCategory?> FindCategory(string name, CancellationToken cancellationToken)
    {
        var normalizedName = name.Trim().ToUpper();
        return db.Categories.FirstOrDefaultAsync(
            category => category.Name.ToUpper() == normalizedName, cancellationToken);
    }
}
