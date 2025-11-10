using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Enums;

namespace BankMore.ContaCorrente.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração do Entity Framework para a entidade Idempotencia
/// </summary>
public class IdempotenciaConfiguration : IEntityTypeConfiguration<Idempotencia>
{
    public void Configure(EntityTypeBuilder<Idempotencia> builder)
    {
        builder.ToTable("idempotencia");

        builder.HasKey(i => i.ChaveIdempotencia);

        builder.Property(i => i.ChaveIdempotencia)
            .HasColumnName("chave_idempotencia")
            .HasColumnType("TEXT(37)")
            .IsRequired();

        builder.Property(i => i.Requisicao)
            .HasColumnName("requisicao")
            .HasColumnType("TEXT(1000)");

        builder.Property(i => i.Resultado)
            .HasColumnName("resultado")
            .HasColumnType("TEXT(1000)");

        builder.Property(i => i.Status)
            .HasColumnName("status")
            .HasColumnType("INTEGER")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(i => i.ResultadoHash)
            .HasColumnName("resultado_hash")
            .HasColumnType("TEXT(64)");

        builder.Property(i => i.Metadata)
            .HasColumnName("metadata")
            .HasColumnType("TEXT(2000)");

        builder.Property(i => i.DataCriacao)
            .HasColumnName("data_criacao")
            .HasColumnType("TEXT")
            .IsRequired();

        builder.Property(i => i.DataExpiracao)
            .HasColumnName("data_expiracao")
            .HasColumnType("TEXT")
            .IsRequired();
    }
}
