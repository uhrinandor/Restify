using RestifyServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RestifyServer.Repository.Configurations;

public class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> b)
    {
        b.ToTable("T_TABLE");

        b.HasKey(x => x.Id);

        b.Property(x => x.Number)
            .IsRequired();

        b.HasIndex(x => x.Number)
            .IsUnique();
    }
}

