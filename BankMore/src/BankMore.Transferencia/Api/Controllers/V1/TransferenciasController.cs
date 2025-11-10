using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Security.Claims;
using Asp.Versioning;
using BankMore.Transferencia.Application.Commands;
using BankMore.Transferencia.Application.Queries;
using BankMore.Transferencia.Application.DTOs;

namespace BankMore.Transferencia.Api.Controllers.V1;

/// <summary>
/// Endpoints RESTful para gerenciar transferências entre contas
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/transferencias")]
[Authorize]
[Produces("application/json")]
public class TransferenciasController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TransferenciasController> _logger;

    public TransferenciasController(IMediator mediator, ILogger<TransferenciasController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Realiza uma nova transferência entre contas
    /// </summary>
    /// <param name="request">Dados da transferência</param>
    /// <returns>Detalhes da transferência criada</returns>
    /// <response code="201">Transferência realizada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="403">Token inválido ou expirado</response>
    /// <response code="422">Erro de validação de negócio</response>
    [HttpPost]
    [ProducesResponseType(typeof(TransferenciaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<TransferenciaDto>> RealizarTransferencia([FromBody] RealizarTransferenciaRequest request)
    {
        try
        {
            var idContaCorrente = User.FindFirstValue("idcontacorrente")
                ?? throw new UnauthorizedAccessException("Token não contém identificador da conta");

            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var command = new RealizarTransferenciaCommand
            {
                ChaveIdempotencia = request.ChaveIdempotencia ?? Guid.CreateVersion7().ToString(),
                IdContaCorrenteOrigem = idContaCorrente,
                NumeroContaDestino = request.NumeroContaDestino,
                Valor = request.Valor,
                Token = token
            };

            var transferencia = await _mediator.Send(command);

            // Adicionar HATEOAS links
            transferencia.Links = new List<Link>
            {
                new Link
                {
                    Rel = "self",
                    Href = Url.Action(nameof(ObterTransferencia), new { id = transferencia.Id }) ?? string.Empty,
                    Method = "GET",
                    Description = "Obter detalhes desta transferência"
                },
                new Link
                {
                    Rel = "list",
                    Href = Url.Action(nameof(ListarTransferencias)) ?? string.Empty,
                    Method = "GET",
                    Description = "Listar todas as transferências"
                },
                new Link
                {
                    Rel = "create",
                    Href = Url.Action(nameof(RealizarTransferencia)) ?? string.Empty,
                    Method = "POST",
                    Description = "Realizar nova transferência"
                }
            };

            return CreatedAtAction(
                nameof(ObterTransferencia),
                new { id = transferencia.Id },
                transferencia
            );
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("INVALID_VALUE"))
        {
            var problem = new ApiProblemDetails
            {
                Type = "https://exemplo.com/errors/invalid-value",
                Title = "Valor Inválido",
                Status = StatusCodes.Status422UnprocessableEntity,
                Detail = "O valor da transferência deve ser maior que zero",
                Instance = HttpContext.Request.Path,
                ErrorCode = "INVALID_VALUE"
            };
            return UnprocessableEntity(problem);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("INVALID_ACCOUNT"))
        {
            var problem = new ApiProblemDetails
            {
                Type = "https://exemplo.com/errors/invalid-account",
                Title = "Conta Inválida",
                Status = StatusCodes.Status422UnprocessableEntity,
                Detail = "Conta origem ou destino não encontrada ou inválida",
                Instance = HttpContext.Request.Path,
                ErrorCode = "INVALID_ACCOUNT"
            };
            return UnprocessableEntity(problem);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("INACTIVE_ACCOUNT"))
        {
            var problem = new ApiProblemDetails
            {
                Type = "https://exemplo.com/errors/inactive-account",
                Title = "Conta Inativa",
                Status = StatusCodes.Status422UnprocessableEntity,
                Detail = "A conta origem ou destino está inativa",
                Instance = HttpContext.Request.Path,
                ErrorCode = "INACTIVE_ACCOUNT"
            };
            return UnprocessableEntity(problem);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("DUPLICATE_REQUEST"))
        {
            var problem = new ApiProblemDetails
            {
                Type = "https://exemplo.com/errors/duplicate-request",
                Title = "Requisição Duplicada",
                Status = StatusCodes.Status409Conflict,
                Detail = "Esta transferência já foi processada anteriormente",
                Instance = HttpContext.Request.Path,
                ErrorCode = "DUPLICATE_REQUEST"
            };
            return Conflict(problem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao realizar transferência");
            var problem = new ApiProblemDetails
            {
                Type = "https://exemplo.com/errors/internal-error",
                Title = "Erro Interno",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "Ocorreu um erro ao processar a transferência",
                Instance = HttpContext.Request.Path,
                ErrorCode = "INTERNAL_ERROR"
            };
            return StatusCode(StatusCodes.Status500InternalServerError, problem);
        }
    }

    /// <summary>
    /// Obtém detalhes de uma transferência específica
    /// </summary>
    /// <param name="id">ID da transferência</param>
    /// <returns>Detalhes da transferência</returns>
    /// <response code="200">Transferência encontrada</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="403">Token inválido ou expirado</response>
    /// <response code="404">Transferência não encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TransferenciaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransferenciaDto>> ObterTransferencia(string id)
    {
        try
        {
            var idContaCorrente = User.FindFirstValue("idcontacorrente")
                ?? throw new UnauthorizedAccessException("Token não contém identificador da conta");

            var query = new ObterTransferenciaQuery
            {
                IdTransferencia = id,
                IdContaCorrente = idContaCorrente
            };

            var transferencia = await _mediator.Send(query);

            if (transferencia == null)
            {
                var problem = new ApiProblemDetails
                {
                    Type = "https://exemplo.com/errors/not-found",
                    Title = "Transferência Não Encontrada",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"Não foi encontrada transferência com ID {id} para esta conta",
                    Instance = HttpContext.Request.Path,
                    ErrorCode = "NOT_FOUND"
                };
                return NotFound(problem);
            }

            // Adicionar HATEOAS links
            transferencia.Links = new List<Link>
            {
                new Link
                {
                    Rel = "self",
                    Href = Url.Action(nameof(ObterTransferencia), new { id = transferencia.Id }) ?? string.Empty,
                    Method = "GET",
                    Description = "Obter detalhes desta transferência"
                },
                new Link
                {
                    Rel = "list",
                    Href = Url.Action(nameof(ListarTransferencias)) ?? string.Empty,
                    Method = "GET",
                    Description = "Listar todas as transferências"
                },
                new Link
                {
                    Rel = "create",
                    Href = Url.Action(nameof(RealizarTransferencia)) ?? string.Empty,
                    Method = "POST",
                    Description = "Realizar nova transferência"
                }
            };

            return Ok(transferencia);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter transferência {Id}", id);
            var problem = new ApiProblemDetails
            {
                Type = "https://exemplo.com/errors/internal-error",
                Title = "Erro Interno",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "Ocorreu um erro ao buscar a transferência",
                Instance = HttpContext.Request.Path,
                ErrorCode = "INTERNAL_ERROR"
            };
            return StatusCode(StatusCodes.Status500InternalServerError, problem);
        }
    }

    /// <summary>
    /// Lista transferências da conta autenticada com filtros e paginação
    /// </summary>
    /// <param name="tipo">Tipo de transferências: enviadas, recebidas ou todas (padrão)</param>
    /// <param name="dataInicio">Data inicial do filtro (formato: DD/MM/YYYY)</param>
    /// <param name="dataFim">Data final do filtro (formato: DD/MM/YYYY)</param>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 20, máximo: 100)</param>
    /// <returns>Lista paginada de transferências</returns>
    /// <response code="200">Lista de transferências</response>
    /// <response code="400">Parâmetros inválidos</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="403">Token inválido ou expirado</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<TransferenciaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedList<TransferenciaDto>>> ListarTransferencias(
        [FromQuery] string? tipo = "todas",
        [FromQuery] string? dataInicio = null,
        [FromQuery] string? dataFim = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var idContaCorrente = User.FindFirstValue("idcontacorrente")
                ?? throw new UnauthorizedAccessException("Token não contém identificador da conta");

            // Validar pageSize
            if (pageSize > 100) pageSize = 100;
            if (pageSize < 1) pageSize = 20;
            if (page < 1) page = 1;

            // Validar tipo
            var tipoLower = tipo?.ToLower() ?? "todas";
            if (tipoLower != "enviadas" && tipoLower != "recebidas" && tipoLower != "todas")
            {
                var problem = new ApiProblemDetails
                {
                    Type = "https://exemplo.com/errors/invalid-parameter",
                    Title = "Parâmetro Inválido",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "O parâmetro 'tipo' deve ser: enviadas, recebidas ou todas",
                    Instance = HttpContext.Request.Path,
                    ErrorCode = "INVALID_PARAMETER"
                };
                return BadRequest(problem);
            }

            // Parsear datas
            DateTime? dtInicio = null;
            DateTime? dtFim = null;

            if (!string.IsNullOrWhiteSpace(dataInicio))
            {
                if (!DateTime.TryParseExact(dataInicio, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var parsedInicio))
                {
                    var problem = new ApiProblemDetails
                    {
                        Type = "https://exemplo.com/errors/invalid-date",
                        Title = "Data Inválida",
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "O parâmetro 'dataInicio' deve estar no formato DD/MM/YYYY",
                        Instance = HttpContext.Request.Path,
                        ErrorCode = "INVALID_DATE"
                    };
                    return BadRequest(problem);
                }
                dtInicio = parsedInicio;
            }

            if (!string.IsNullOrWhiteSpace(dataFim))
            {
                if (!DateTime.TryParseExact(dataFim, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var parsedFim))
                {
                    var problem = new ApiProblemDetails
                    {
                        Type = "https://exemplo.com/errors/invalid-date",
                        Title = "Data Inválida",
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "O parâmetro 'dataFim' deve estar no formato DD/MM/YYYY",
                        Instance = HttpContext.Request.Path,
                        ErrorCode = "INVALID_DATE"
                    };
                    return BadRequest(problem);
                }
                dtFim = parsedFim;
            }

            var query = new ListarTransferenciasQuery
            {
                IdContaCorrente = idContaCorrente,
                Tipo = tipoLower,
                DataInicio = dtInicio,
                DataFim = dtFim,
                Page = page,
                PageSize = pageSize
            };

            var resultado = await _mediator.Send(query);

            // Adicionar HATEOAS links em cada item
            foreach (var item in resultado.Items)
            {
                item.Links = new List<Link>
                {
                    new Link
                    {
                        Rel = "self",
                        Href = Url.Action(nameof(ObterTransferencia), new { id = item.Id }) ?? string.Empty,
                        Method = "GET",
                        Description = "Obter detalhes desta transferência"
                    }
                };
            }

            // Adicionar links de navegação à lista paginada
            resultado.Links = new List<Link>
            {
                new Link
                {
                    Rel = "self",
                    Href = Url.Action(nameof(ListarTransferencias), new { tipo, dataInicio, dataFim, page, pageSize }) ?? string.Empty,
                    Method = "GET",
                    Description = "Página atual"
                },
                new Link
                {
                    Rel = "create",
                    Href = Url.Action(nameof(RealizarTransferencia)) ?? string.Empty,
                    Method = "POST",
                    Description = "Realizar nova transferência"
                }
            };

            if (resultado.HasPreviousPage)
            {
                resultado.Links.Add(new Link
                {
                    Rel = "previous",
                    Href = Url.Action(nameof(ListarTransferencias), new { tipo, dataInicio, dataFim, page = page - 1, pageSize }) ?? string.Empty,
                    Method = "GET",
                    Description = "Página anterior"
                });
            }

            if (resultado.HasNextPage)
            {
                resultado.Links.Add(new Link
                {
                    Rel = "next",
                    Href = Url.Action(nameof(ListarTransferencias), new { tipo, dataInicio, dataFim, page = page + 1, pageSize }) ?? string.Empty,
                    Method = "GET",
                    Description = "Próxima página"
                });
            }

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar transferências");
            var problem = new ApiProblemDetails
            {
                Type = "https://exemplo.com/errors/internal-error",
                Title = "Erro Interno",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "Ocorreu um erro ao listar as transferências",
                Instance = HttpContext.Request.Path,
                ErrorCode = "INTERNAL_ERROR"
            };
            return StatusCode(StatusCodes.Status500InternalServerError, problem);
        }
    }
}

/// <summary>
/// Request para realizar transferência
/// </summary>
public class RealizarTransferenciaRequest
{
    /// <summary>
    /// Chave de idempotência para evitar duplicações (opcional, será gerado se não fornecido)
    /// </summary>
    public string? ChaveIdempotencia { get; set; }

    /// <summary>
    /// Número da conta destino
    /// </summary>
    public int NumeroContaDestino { get; set; }

    /// <summary>
    /// Valor da transferência (deve ser maior que zero)
    /// </summary>
    public decimal Valor { get; set; }
}
