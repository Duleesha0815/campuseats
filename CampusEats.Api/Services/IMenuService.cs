using CampusEats.Api.Dtos;

namespace CampusEats.Api.Services;

public interface IMenuService
{
    Task<IReadOnlyList<MenuItemDto>> GetAll(CancellationToken cancellationToken);
    Task<MenuItemDto?> GetById(int id, CancellationToken cancellationToken);
    Task<MenuItemDto?> Create(CreateMenuItemDto dto, CancellationToken cancellationToken);
    Task<bool> Update(int id, CreateMenuItemDto dto, CancellationToken cancellationToken);
    Task<bool> Delete(int id, CancellationToken cancellationToken);
}