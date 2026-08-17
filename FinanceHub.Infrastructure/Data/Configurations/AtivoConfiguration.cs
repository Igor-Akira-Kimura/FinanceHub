using FinanceHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceHub.Infrastructure.Data.Configurations;

public class AtivoConfiguration : IEntityTypeConfiguration<Ativo>
{
    public void Configure(EntityTypeBuilder<Ativo> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Ticker)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Tipo)
            .IsRequired();

        builder.Property(x => x.Preco)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.HasOne(x => x.Bolsa)
            .WithMany(x => x.Ativos)
            .HasForeignKey(x => x.BolsaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.Ticker)
            .IsUnique();
    }
}