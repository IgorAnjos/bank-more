using System.Text.Json;

namespace BankMore.Tarifas.Services;

/// <summary>
/// Serviço para comunicação com API Conta Corrente
/// Debita a tarifa automaticamente na conta origem
/// </summary>
public interface IContaCorrenteService
{
    Task<bool> DebitarTarifaAsync(string idContaCorrente, string idTransferencia, decimal valor);
}

public class ContaCorrenteService : IContaCorrenteService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ContaCorrenteService> _logger;
    private readonly string _apiUrl;

    public ContaCorrenteService(HttpClient httpClient, ILogger<ContaCorrenteService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _apiUrl = configuration["ApiContaCorrente:BaseUrl"] ?? "http://localhost:5003";
    }

    public async Task<bool> DebitarTarifaAsync(string idContaCorrente, string idTransferencia, decimal valor)
    {
        try
        {
            var requestBody = new
            {
                ChaveIdempotencia = $"tarifa-{idTransferencia}",
                TipoMovimento = 'D',
                Valor = valor
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            // TODO: Obter token JWT do sistema (em produção usar service account)
            // Por ora, não envia token (endpoint de tarifa seria interno/service-to-service)

            var response = await _httpClient.PostAsync($"{_apiUrl}/api/movimentacao", httpContent);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Tarifa de R$ {Valor} debitada da conta {IdConta}", valor, idContaCorrente);
                return true;
            }

            _logger.LogWarning("Falha ao debitar tarifa. Status: {StatusCode}", response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao debitar tarifa da conta {IdConta}", idContaCorrente);
            return false;
        }
    }
}
