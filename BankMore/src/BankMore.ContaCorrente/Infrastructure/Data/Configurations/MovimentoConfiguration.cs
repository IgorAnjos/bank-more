using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BankMore.ContaCorrente.Domain.Entities;

namespace BankMore.ContaCorrente.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração do Entity Framework para a entidade Movimento
/// </summary>
public class MovimentoConfiguration : IEntityTypeConfiguration<Movimento>
{
    public void Configure(EntityTypeBuilder<Movimento> builder)
    {
        builder.ToTable("movimento");

        builder.HasKey(m => m.IdMovimento);

        builder.Property(m => m.IdMovimento)
            .HasColumnName("idmovimento")
            .HasColumnType("TEXT(37)")
            .IsRequired();

        builder.Property(m => m.IdContaCorrente)
            .HasColumnName("idcontacorrente")
            .HasColumnType("TEXT(37)")
            .IsRequired();

        builder.Property(m => m.DataMovimento)
            .HasColumnName("datamovimento")
            .HasColumnType("TEXT(25)")
            .IsRequired();

        builder.Property(m => m.TipoMovimento)
            .HasColumnName("tipomovimento")
            .HasColumnType("TEXT(1)")
            .IsRequired();

        builder.Property(m => m.Valor)
            .HasColumnName("valor")
            .HasColumnType("REAL")
            .IsRequired()
            .HasPrecision(18, 2); // Sempre duas casas decimais

        // Relacionamento com ContaCorrente
        builder.HasOne(m => m.ContaCorrente)
            .WithMany(c => c.Movimentos)
            .HasForeignKey(m => m.IdContaCorrente)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
