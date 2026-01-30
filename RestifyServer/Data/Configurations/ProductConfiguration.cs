using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestifyServer.Models;

namespace RestifyServer.Repository.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.ToTable("T_PRODUCT");

        b.HasKey(x => x.Id);

        b.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired();

        b.Property(x => x.Price)
            .HasPrecision(12, 2)
            .IsRequired();

        b.HasOne(x => x.Category)
            .WithMany(c => c.Products)
            .HasForeignKey("CategoryId")
            .OnDelete(DeleteBehavior.SetNull);

        b.HasIndex("CategoryId");

    }
}
