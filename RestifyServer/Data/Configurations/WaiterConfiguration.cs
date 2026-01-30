using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestifyServer.Models;

namespace RestifyServer.Repository.Configurations;

public class WaiterConfiguration : IEntityTypeConfiguration<Waiter>
{
    public void Configure(EntityTypeBuilder<Waiter> b)
    {
        b.ToTable("T_WAITER");

        b.HasKey(x => x.Id);

        b.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        b.Property(x => x.Username)
            .HasMaxLength(100)
            .IsRequired();

        b.HasIndex(x => x.Username)
            .IsUnique();

        b.Property(x => x.Password)
            .HasMaxLength(255)
            .IsRequired();

        b.HasMany(x => x.Invoices)
            .WithOne(i => i.Waiter)
            .HasForeignKey("WaiterId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
