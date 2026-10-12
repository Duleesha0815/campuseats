namespace CampusEats.Api.Models;

public class MenuItem
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public MenuCategory Category { get; set; } = null!;
    public bool Available { get; set; } = true;
}