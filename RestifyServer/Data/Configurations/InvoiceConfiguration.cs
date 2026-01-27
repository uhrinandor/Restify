using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestifyServer.Models;

namespace RestifyServer.Repository.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> b)
    {
        b.ToTable("T_INVOICE");

        b.HasKey(x => x.Id);

        b.Property(x => x.Tip)
            .HasPrecision(10, 2)
            .IsRequired();

        b.Property(x => x.Payment)
            .HasConversion<int>()
            .IsRequired();

        b.HasOne(x => x.Table)
            .WithMany()
            .HasForeignKey("TableId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Waiter)
            .WithMany(w => w.Invoices)
            .HasForeignKey("WaiterId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.Orders)
            .WithOne(o => o.Invoice)
            .HasForeignKey("InvoiceId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
