using FinanceHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceHub.Infrastructure.Data.Configurations;

public class BolsaConfiguration : IEntityTypeConfiguration<Bolsa>
{
    public void Configure(EntityTypeBuilder<Bolsa> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Pais)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Moeda)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasData(
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Nome = "B3",
                Pais = "Brasil",
                Moeda = "BRL",
                Ativa = true,
                DataCriacao = new DateTime(2026, 1, 1),
                DataAtualizacao = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Nome = "NASDAQ",
                Pais = "Estados Unidos",
                Moeda = "USD",
                Ativa = true,
                DataCriacao = new DateTime(2026, 1, 1),
                DataAtualizacao = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Nome = "NYSE",
                Pais = "Estados Unidos",
                Moeda = "USD",
                Ativa = true,
                DataCriacao = new DateTime(2026, 1, 1),
                DataAtualizacao = (DateTime?)null
            });
    }
}