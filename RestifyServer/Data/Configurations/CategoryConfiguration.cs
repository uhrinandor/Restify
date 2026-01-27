using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestifyServer.Models;

namespace RestifyServer.Repository.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> b)
    {
        b.ToTable("T_CATEGORY");

        b.HasKey(x => x.Id);

        b.Property(x => x.Name)
            .HasMaxLength(120)
            .IsRequired();

        b.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey("ParentId")
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.Products)
            .WithOne(p => p.Category)
            .HasForeignKey("CategoryId")
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex("ParentId");
    }
}
