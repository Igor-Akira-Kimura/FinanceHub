using FinanceHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceHub.Infrastructure.Data.Configurations;

public class CompraConfiguration : IEntityTypeConfiguration<Compra>
{
    public void Configure(EntityTypeBuilder<Compra> builder)
    {
        builder.ToTable("Compras");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CarteiraId)
            .IsRequired();

        builder.Property(c => c.AtivoId)
            .IsRequired();

        builder.Property(c => c.Quantidade)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(c => c.Preco)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(c => c.IdempotencyKey)
            .IsRequired()
            .HasMaxLength(450);

        builder.HasOne<Carteira>()
            .WithMany()
            .HasForeignKey(c => c.CarteiraId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Ativo>()
            .WithMany()
            .HasForeignKey(c => c.AtivoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.CarteiraId);

        builder.HasIndex(c => c.AtivoId);

        builder
            .HasIndex(c => c.IdempotencyKey)
            .IsUnique()
            .HasDatabaseName("UX_Compras_IdempotencyKey");
    }
}