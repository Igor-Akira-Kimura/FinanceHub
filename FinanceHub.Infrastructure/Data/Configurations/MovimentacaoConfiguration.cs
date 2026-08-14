using FinanceHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceHub.Infrastructure.Data.Configurations;

public class MovimentacaoConfiguration : IEntityTypeConfiguration<Movimentacao>
{
    public void Configure(EntityTypeBuilder<Movimentacao> builder)
    {
        builder.ToTable("Movimentacoes");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Tipo)
            .IsRequired();

        builder.Property(m => m.Quantidade)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(m => m.Preco)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(m => m.DataMovimentacao)
            .IsRequired();

        builder.HasOne(m => m.Posicao)
            .WithMany(p => p.Movimentacoes)
            .HasForeignKey(m => m.PosicaoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}