using FinanceHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceHub.Infrastructure.Data.Configurations
{
    public class PosicaoConfiguration : IEntityTypeConfiguration<Posicao>
    {
        public void Configure(EntityTypeBuilder<Posicao> builder)
        {
            builder.ToTable("Posicoes");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.CarteiraId)
                .IsRequired();

            builder.Property(p => p.AtivoId)
                .IsRequired();

            builder.Property(p => p.Quantidade)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(p => p.PrecoMedio)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(p => p.DataCriacao)
                .IsRequired();

            builder.Property(p => p.DataAtualizacao);

            builder.HasOne(p => p.Carteira)
                .WithMany(c => c.Posicoes)
                .HasForeignKey(p => p.CarteiraId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Ativo)
                .WithMany(a => a.Posicoes)
                .HasForeignKey(p => p.AtivoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => new { p.CarteiraId, p.AtivoId })
                .IsUnique();
        }
    }
}
