using Microsoft.EntityFrameworkCore;
using BankMore.ContaCorrente.Domain.Entities;

namespace BankMore.ContaCorrente.Infrastructure.Data;

/// <summary>
/// Contexto do Entity Framework Core para o microsserviço de Conta Corrente
/// </summary>
public class ContaCorrenteDbContext : DbContext
{
    public ContaCorrenteDbContext(DbContextOptions<ContaCorrenteDbContext> options)
        : base(options)
    {
    }

    public DbSet<Domain.Entities.ContaCorrente> ContasCorrentes { get; set; }
    public DbSet<Movimento> Movimentos { get; set; }
    public DbSet<Idempotencia> Idempotencias { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<OutboxEvent> OutboxEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar todas as configurações do assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContaCorrenteDbContext).Assembly);
    }
}
