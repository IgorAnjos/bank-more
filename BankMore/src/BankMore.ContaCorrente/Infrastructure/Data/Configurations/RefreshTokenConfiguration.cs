using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BankMore.ContaCorrente.Domain.Entities;

namespace BankMore.ContaCorrente.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração do Entity Framework para a entidade RefreshToken
/// </summary>
public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_token");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Id)
            .HasColumnName("id")
            .HasColumnType("TEXT(37)")
            .IsRequired();

        builder.Property(rt => rt.IdContaCorrente)
            .HasColumnName("id_conta_corrente")
            .HasColumnType("TEXT(37)")
            .IsRequired();

        builder.Property(rt => rt.TokenHash)
            .HasColumnName("token_hash")
            .HasColumnType("TEXT(64)")
            .IsRequired();

        builder.Property(rt => rt.DataCriacao)
            .HasColumnName("data_criacao")
            .HasColumnType("TEXT")
            .IsRequired();

        builder.Property(rt => rt.DataExpiracao)
            .HasColumnName("data_expiracao")
            .HasColumnType("TEXT")
            .IsRequired();

        builder.Property(rt => rt.Revogado)
            .HasColumnName("revogado")
            .HasColumnType("INTEGER")
            .IsRequired();

        builder.Property(rt => rt.DataRevogacao)
            .HasColumnName("data_revogacao")
            .HasColumnType("TEXT");

        builder.Property(rt => rt.MotivoRevogacao)
            .HasColumnName("motivo_revogacao")
            .HasColumnType("TEXT(500)");

        // Índices para performance
        builder.HasIndex(rt => rt.TokenHash)
            .IsUnique();

        builder.HasIndex(rt => rt.IdContaCorrente);

        builder.HasIndex(rt => rt.DataExpiracao);
    }
}
