using CampusEats.Api.Dtos;
using CampusEats.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusEats.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _svc;

    public MenuController(IMenuService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await _svc.GetAll(cancellationToken));

    [HttpGet("{id}")]
    public async Task<ActionResult<MenuItemDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _svc.GetById(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<MenuItemDto>> Create(
        [FromBody] CreateMenuItemDto dto,
        CancellationToken cancellationToken)
    {
        var created = await _svc.Create(dto, cancellationToken);
        if (created is null)
        {
            return BadRequest(new { message = $"Menu category '{dto.Category}' does not exist." });
        }

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] CreateMenuItemDto dto,
        CancellationToken cancellationToken)
    {
        bool ok;
        try
        {
            ok = await _svc.Update(id, dto, cancellationToken);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }

        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var ok = await _svc.Delete(id, cancellationToken);
        return ok ? NoContent() : NotFound();
    }
}