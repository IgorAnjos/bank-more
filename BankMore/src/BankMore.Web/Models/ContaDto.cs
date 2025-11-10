namespace BankMore.Web.Models;

public class ContaDto
{
    public string Id { get; set; } = string.Empty;
    public int NumeroContaCorrente { get; set; }
    public string Cpf { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public decimal Saldo { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
    public List<Link>? Links { get; set; }
}

public class SaldoDto
{
    public decimal Saldo { get; set; }
    public DateTime DataConsulta { get; set; }
}

public class MovimentoDto
{
    public string Id { get; set; } = string.Empty;
    public char TipoMovimento { get; set; }
    public decimal Valor { get; set; }
    public decimal SaldoAnterior { get; set; }
    public decimal SaldoAtualizado { get; set; }
    public DateTime DataMovimento { get; set; }
    public List<Link>? Links { get; set; }
}

public class PaginatedList<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
    public List<Link>? Links { get; set; }
}

public class Link
{
    public string Rel { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string? Description { get; set; }
}
