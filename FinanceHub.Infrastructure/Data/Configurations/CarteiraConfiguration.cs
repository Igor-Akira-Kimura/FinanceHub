using FinanceHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceHub.Infrastructure.Data.Configurations
{
    public class CarteiraConfiguration : IEntityTypeConfiguration<Carteira>
    {
        public void Configure(EntityTypeBuilder<Carteira> builder)
        {
            builder.ToTable("Carteiras");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Saldo)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(c => c.Ativa)
                .IsRequired();

            builder.Property(c => c.DataCriacao)
                .IsRequired();

            builder.Property(c => c.DataAtualizacao);

            builder.HasOne(c => c.Usuario)
                .WithMany(u => u.Carteiras)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Posicoes)
                .WithOne(p => p.Carteira)
                .HasForeignKey(p => p.CarteiraId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => new { c.UsuarioId, c.Nome })
                .IsUnique();
        }
    }
}
