using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestifyServer.Models;
using RestifyServer.Models.Enums;

namespace RestifyServer.Repository.Configurations;

public class AdminConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> b)
    {
        b.ToTable("T_ADMIN");

        b.HasKey(x => x.Id);

        b.Property(x => x.Username)
            .HasMaxLength(100)
            .IsRequired();

        b.HasIndex(x => x.Username)
            .IsUnique();

        b.Property(x => x.Password)
            .HasMaxLength(255)
            .IsRequired();

        b.Property(x => x.AccessLevel)
            .HasConversion<int>()
            .IsRequired()
            .HasDefaultValue(Permission.Read);
    }
}
