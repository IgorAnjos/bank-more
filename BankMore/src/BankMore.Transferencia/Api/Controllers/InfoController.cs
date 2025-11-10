using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Asp.Versioning;

namespace BankMore.Transferencia.Api.Controllers;

/// <summary>
/// Controller para informações da API
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class InfoController : ControllerBase
{
    /// <summary>
    /// Retorna informações sobre a versão da API
    /// </summary>
    /// <returns>Informações da API</returns>
    /// <response code="200">Informações retornadas com sucesso</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiInfoResponse), StatusCodes.Status200OK)]
    public IActionResult GetInfo()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;
        var buildDate = System.IO.File.GetLastWriteTime(assembly.Location);

        var info = new ApiInfoResponse
        {
            Name = "BankMore - API Transferência",
            Version = "1.0.0",
            BuildVersion = version?.ToString() ?? "1.0.0.0",
            BuildDate = buildDate,
            Framework = ".NET 9.0",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            Features = new List<string>
            {
                "Transferências com validação de saldo",
                "Compensação automática com retry",
                "Idempotência via Redis (24h TTL)",
                "Publicação de eventos no Kafka",
                "DLQ para transferências falhadas",
                "Outbox Pattern para consistência",
                "Versionamento de API",
                "Swagger/OpenAPI 3.0"
            },
            Endpoints = new EndpointInfo
            {
                Swagger = "/swagger",
                Health = "/health",
                Metrics = "/metrics"
            }
        };

        return Ok(info);
    }

    /// <summary>
    /// Health check básico
    /// </summary>
    /// <returns>Status da API</returns>
    [HttpGet("health")]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
    public IActionResult GetHealth()
    {
        return Ok(new HealthResponse
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        });
    }
}

public class ApiInfoResponse
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string BuildVersion { get; set; } = string.Empty;
    public DateTime BuildDate { get; set; }
    public string Framework { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public List<string> Features { get; set; } = new();
    public EndpointInfo Endpoints { get; set; } = new();
}

public class EndpointInfo
{
    public string Swagger { get; set; } = string.Empty;
    public string Health { get; set; } = string.Empty;
    public string Metrics { get; set; } = string.Empty;
}

public class HealthResponse
{
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Version { get; set; } = string.Empty;
}
