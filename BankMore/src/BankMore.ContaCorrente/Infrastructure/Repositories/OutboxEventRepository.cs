using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.ContaCorrente.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankMore.ContaCorrente.Infrastructure.Repositories;

public class OutboxEventRepository : IOutboxEventRepository
{
    private readonly ContaCorrenteDbContext _context;

    public OutboxEventRepository(ContaCorrenteDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<string> AdicionarAsync(OutboxEvent outboxEvent)
    {
        await _context.OutboxEvents.AddAsync(outboxEvent);
        await _context.SaveChangesAsync();
        return outboxEvent.Id;
    }

    public async Task<IEnumerable<OutboxEvent>> ObterNaoProcessadosAsync(int limit = 100)
    {
        return await _context.OutboxEvents
            .Where(x => !x.Processed)
            .OrderBy(x => x.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task AtualizarAsync(OutboxEvent outboxEvent)
    {
        _context.OutboxEvents.Update(outboxEvent);
        await _context.SaveChangesAsync();
    }

    public async Task MarcarComoProcessadoAsync(string id)
    {
        var outboxEvent = await _context.OutboxEvents.FindAsync(id);
        if (outboxEvent != null)
        {
            outboxEvent.MarkAsProcessed();
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> RemoverProcessadosAntigosAsync(int diasRetencao = 7)
    {
        var dataLimite = DateTime.UtcNow.AddDays(-diasRetencao);
        
        var eventosAntigos = await _context.OutboxEvents
            .Where(x => x.Processed && x.ProcessedAt < dataLimite)
            .ToListAsync();

        _context.OutboxEvents.RemoveRange(eventosAntigos);
        await _context.SaveChangesAsync();

        return eventosAntigos.Count;
    }
}
