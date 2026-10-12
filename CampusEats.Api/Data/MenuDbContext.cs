using CampusEats.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Api.Data;

public class MenuDbContext(DbContextOptions<MenuDbContext> options) : DbContext(options)
{
    public DbSet<MenuCategory> Categories => Set<MenuCategory>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MenuCategory>(entity =>
        {
            entity.ToTable("categories");
            entity.HasKey(category => category.Id);
            entity.Property(category => category.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(category => category.Name).HasColumnName("name").HasMaxLength(40).IsRequired();
            entity.HasIndex(category => category.Name).IsUnique();
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.ToTable("menu_items");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(item => item.Name).HasColumnName("name").HasMaxLength(80).IsRequired();
            entity.Property(item => item.Price).HasColumnName("price").HasPrecision(10, 2);
            entity.Property(item => item.CategoryId).HasColumnName("category_id").IsRequired();
            entity.Property(item => item.Available).HasColumnName("available").HasDefaultValue(true);
            entity.HasOne(item => item.Category)
                .WithMany(category => category.MenuItems)
                .HasForeignKey(item => item.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
