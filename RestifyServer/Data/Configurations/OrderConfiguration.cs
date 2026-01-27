using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestifyServer.Models;

namespace RestifyServer.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> b)
    {
        b.ToTable("T_ORDER");

        b.HasKey(x => x.Id);

        b.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        b.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey("ProductId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        b.HasOne(x => x.Invoice)
            .WithMany(i => i.Orders)
            .HasForeignKey("InvoiceId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex("ProductId");
        b.HasIndex("InvoiceId");
    }
}
