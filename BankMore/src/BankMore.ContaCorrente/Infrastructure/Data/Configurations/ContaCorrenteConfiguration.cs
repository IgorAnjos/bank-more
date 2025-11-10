using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankMore.ContaCorrente.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração do Entity Framework para a entidade ContaCorrente
/// Mapeia para a tabela conforme especificação SQL (TEXT, INTEGER, etc)
/// </summary>
public class ContaCorrenteConfiguration : IEntityTypeConfiguration<Domain.Entities.ContaCorrente>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.ContaCorrente> builder)
    {
        builder.ToTable("contacorrente");

        builder.HasKey(c => c.IdContaCorrente);

        builder.Property(c => c.IdContaCorrente)
            .HasColumnName("idcontacorrente")
            .HasColumnType("TEXT(37)")
            .IsRequired();

        builder.Property(c => c.Numero)
            .HasColumnName("numero")
            .HasColumnType("INTEGER(10)")
            .IsRequired();

        builder.HasIndex(c => c.Numero)
            .IsUnique();

        builder.Property(c => c.Cpf)
            .HasColumnName("cpf")
            .HasColumnType("TEXT(200)")
            .IsRequired();

        builder.HasIndex(c => c.Cpf)
            .IsUnique(); // CPF único por conta

        builder.Property(c => c.Nome)
            .HasColumnName("nome")
            .HasColumnType("TEXT(100)")
            .IsRequired();

        builder.Property(c => c.Ativo)
            .HasColumnName("ativo")
            .HasColumnType("INTEGER(1)")
            .IsRequired()
            .HasDefaultValue(1); // 1 = ativo por padrão

        builder.Property(c => c.Senha)
            .HasColumnName("senha")
            .HasColumnType("TEXT(100)")
            .IsRequired();

        builder.Property(c => c.Salt)
            .HasColumnName("salt")
            .HasColumnType("TEXT(100)")
            .IsRequired();

        // Relacionamento com Movimento (1:N)
        builder.HasMany(c => c.Movimentos)
            .WithOne(m => m.ContaCorrente)
            .HasForeignKey(m => m.IdContaCorrente)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
