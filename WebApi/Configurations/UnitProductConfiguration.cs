using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalogAPI.Models;

namespace ProductCatalogAPI.Configurations;

public class UnitProductConfiguration : IEntityTypeConfiguration<UnitProduct>
{
    public void Configure(EntityTypeBuilder<UnitProduct> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.SerialNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.ProductId)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");
    
        builder.HasIndex(p => p.SerialNumber)
                .IsUnique();

        builder.HasIndex(u => u.ProductId);

        builder.HasIndex(u => u.UserId);

        builder.HasOne(u => u.Product)
            .WithMany(p => p.UnitProducts)
            .HasForeignKey(u => u.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.User)
            .WithMany(a => a.UnitProducts)
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.SetNull);

    }
}