using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Asp.Versioning;
using BankMore.ContaCorrente.Application.Commands;
using BankMore.ContaCorrente.Application.DTOs;
using BankMore.ContaCorrente.Application.Queries;
using System.Security.Claims;

namespace BankMore.ContaCorrente.Api.Controllers.V1;

/// <summary>
/// Controller RESTful para gerenciamento de Contas Correntes (v1)
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/contas")]
[Produces("application/json", "application/problem+json")]
public class ContasController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ContasController> _logger;

    public ContasController(IMediator mediator, ILogger<ContasController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Valida se uma conta existe e está ativa (endpoint público para outros microsserviços)
    /// </summary>
    /// <param name="numeroConta">Número da conta corrente</param>
    /// <returns>200 OK se conta existe e está ativa, 404 caso contrário</returns>
    /// <response code="200">Conta existe e está ativa</response>
    /// <response code="404">Conta não encontrada ou inativa</response>
    [HttpGet("validar/{numeroConta}")]
    [AllowAnonymous] // Endpoint público para validação entre microsserviços
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ValidarConta(int numeroConta)
    {
        var query = new ValidarContaQuery { NumeroConta = numeroConta };
        var contaValida = await _mediator.Send(query);

        if (contaValida)
        {
            return Ok(); // 200 - Conta existe e está ativa
        }

        return NotFound(); // 404 - Conta não existe ou está inativa (não expõe o motivo)
    }

    /// <summary>
    /// Busca ID da conta pelo número (endpoint público para microsserviços)
    /// </summary>
    /// <param name="numeroConta">Número da conta corrente</param>
    /// <returns>Objeto contendo o ID da conta</returns>
    /// <response code="200">Retorna o ID da conta</response>
    /// <response code="404">Conta não encontrada</response>
    [HttpGet("numero/{numeroConta}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BuscarIdPorNumero(int numeroConta)
    {
        var query = new ObterContaPorNumeroQuery { NumeroConta = numeroConta };
        var conta = await _mediator.Send(query);

        if (conta != null && conta.Ativo)
        {
            return Ok(new { id = conta.Id });
        }

        return NotFound();
    }

    /// <summary>
    /// Cria uma nova conta corrente
    /// </summary>
    /// <param name="request">Dados para cadastro da conta (CPF, nome, senha)</param>
    /// <returns>Dados da conta criada com links HATEOAS</returns>
    /// <response code="201">Conta criada com sucesso. Retorna Location header com URI do recurso</response>
    /// <response code="400">CPF inválido ou dados inconsistentes (Problem Details)</response>
    /// <response code="422">Regra de negócio violada (Problem Details)</response>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ContaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ContaDto>> CriarConta([FromBody] CadastrarContaCommand request)
    {
        try
        {
            var conta = await _mediator.Send(request);

            // Adicionar links HATEOAS
            conta.Links = GerarLinksHateoas(conta.Id, incluirInativar: true, incluirMovimentos: true);

            _logger.LogInformation("Conta criada com sucesso: {IdConta}, Número: {Numero}", 
                conta.Id, conta.NumeroContaCorrente);

            // 201 Created com header Location
            return CreatedAtAction(
                nameof(ObterConta),
                new { id = conta.Id, version = "1.0" },
                conta
            );
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("INVALID_DOCUMENT"))
        {
            return BadRequest(CriarProblemDetails(
                "Documento Inválido",
                "O CPF fornecido não é válido",
                StatusCodes.Status400BadRequest,
                "INVALID_DOCUMENT"
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar conta");
            return UnprocessableEntity(CriarProblemDetails(
                "Erro ao Processar",
                ex.Message,
                StatusCodes.Status422UnprocessableEntity,
                "PROCESSING_ERROR"
            ));
        }
    }

    /// <summary>
    /// Obtém detalhes de uma conta específica
    /// </summary>
    /// <param name="id">ID único da conta corrente</param>
    /// <returns>Dados completos da conta com saldo e links HATEOAS</returns>
    /// <response code="200">Conta encontrada com sucesso</response>
    /// <response code="404">Conta não encontrada (Problem Details)</response>
    /// <response code="403">Token JWT inválido ou expirado</response>
    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(ContaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ContaDto>> ObterConta(string id)
    {
        try
        {
            var idContaCorrente = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

            // Verificar se o usuário está tentando acessar sua própria conta
            if (string.IsNullOrEmpty(idContaCorrente) || idContaCorrente != id)
            {
                return Forbid();
            }

            var query = new ObterContaQuery { IdContaCorrente = id };
            var conta = await _mediator.Send(query);

            if (conta == null)
            {
                return NotFound(CriarProblemDetails(
                    "Conta Não Encontrada",
                    $"A conta com ID '{id}' não foi encontrada",
                    StatusCodes.Status404NotFound,
                    "ACCOUNT_NOT_FOUND"
                ));
            }

            // Adicionar links HATEOAS
            conta.Links = GerarLinksHateoas(conta.Id, 
                incluirInativar: conta.Ativo, 
                incluirMovimentos: true);

            return Ok(conta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter conta {IdConta}", id);
            return StatusCode(500, CriarProblemDetails(
                "Erro Interno",
                "Ocorreu um erro ao processar sua solicitação",
                StatusCodes.Status500InternalServerError,
                "INTERNAL_ERROR"
            ));
        }
    }

    /// <summary>
    /// Consulta o saldo atual da conta
    /// </summary>
    /// <param name="id">ID único da conta corrente</param>
    /// <returns>Saldo atual com data/hora da consulta</returns>
    /// <response code="200">Saldo consultado com sucesso</response>
    /// <response code="400">Conta inválida ou inativa (Problem Details)</response>
    /// <response code="404">Conta não encontrada (Problem Details)</response>
    /// <response code="403">Token JWT inválido ou expirado</response>
    [HttpGet("{id}/saldo")]
    [Authorize]
    [ProducesResponseType(typeof(SaldoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SaldoDto>> ConsultarSaldo(string id)
    {
        try
        {
            var idContaCorrente = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

            if (string.IsNullOrEmpty(idContaCorrente) || idContaCorrente != id)
            {
                return Forbid();
            }

            var query = new ConsultarSaldoQuery { IdContaCorrente = id };
            var resultado = await _mediator.Send(query);

            return Ok(resultado);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("INVALID_ACCOUNT"))
        {
            return NotFound(CriarProblemDetails(
                "Conta Não Encontrada",
                "A conta não está cadastrada no sistema",
                StatusCodes.Status404NotFound,
                "INVALID_ACCOUNT"
            ));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("INACTIVE_ACCOUNT"))
        {
            return BadRequest(CriarProblemDetails(
                "Conta Inativa",
                "Não é possível consultar saldo de conta inativa",
                StatusCodes.Status400BadRequest,
                "INACTIVE_ACCOUNT"
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar saldo da conta {IdConta}", id);
            return StatusCode(500, CriarProblemDetails(
                "Erro Interno",
                "Ocorreu um erro ao consultar o saldo",
                StatusCodes.Status500InternalServerError,
                "INTERNAL_ERROR"
            ));
        }
    }

    /// <summary>
    /// Inativa uma conta corrente (requer confirmação de senha)
    /// </summary>
    /// <param name="id">ID único da conta corrente</param>
    /// <param name="request">Senha da conta para confirmação</param>
    /// <returns>Status 204 No Content em caso de sucesso</returns>
    /// <response code="204">Conta inativada com sucesso (sem corpo de resposta)</response>
    /// <response code="400">Dados inconsistentes (Problem Details)</response>
    /// <response code="401">Senha incorreta (Problem Details)</response>
    /// <response code="404">Conta não encontrada (Problem Details)</response>
    /// <response code="403">Token JWT inválido ou expirado</response>
    [HttpPut("{id}/inativar")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> InativarConta(string id, [FromBody] InativarContaRequest request)
    {
        try
        {
            var idContaCorrente = User.FindFirstValue("idcontacorrente");

            if (string.IsNullOrEmpty(idContaCorrente) || idContaCorrente != id)
            {
                return Forbid();
            }

            var command = new InativarContaCommand
            {
                IdContaCorrente = id,
                Senha = request.Senha
            };

            await _mediator.Send(command);

            _logger.LogInformation("Conta inativada com sucesso: {IdConta}", id);

            return NoContent();
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("INVALID_ACCOUNT"))
        {
            return NotFound(CriarProblemDetails(
                "Conta Não Encontrada",
                "A conta não está cadastrada no sistema",
                StatusCodes.Status404NotFound,
                "INVALID_ACCOUNT"
            ));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(CriarProblemDetails(
                "Senha Incorreta",
                "A senha fornecida não corresponde à senha da conta",
                StatusCodes.Status401Unauthorized,
                "USER_UNAUTHORIZED"
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inativar conta {IdConta}", id);
            return StatusCode(500, CriarProblemDetails(
                "Erro Interno",
                "Ocorreu um erro ao inativar a conta",
                StatusCodes.Status500InternalServerError,
                "INTERNAL_ERROR"
            ));
        }
    }

    /// <summary>
    /// Cria uma nova movimentação na conta (crédito ou débito)
    /// </summary>
    /// <param name="id">ID único da conta corrente</param>
    /// <param name="request">Dados da movimentação (tipo, valor, chave idempotência)</param>
    /// <returns>Status 204 No Content em caso de sucesso</returns>
    /// <response code="204">Movimentação realizada com sucesso</response>
    /// <response code="400">Dados inválidos (Problem Details)</response>
    /// <response code="404">Conta não encontrada (Problem Details)</response>
    /// <response code="422">Regra de negócio violada (Problem Details)</response>
    /// <response code="403">Token JWT inválido ou expirado</response>
    [HttpPost("{id}/movimentos")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CriarMovimento(string id, [FromBody] MovimentacaoRequest request)
    {
        try
        {
            // Verificar se é uma chamada de serviço interno (ex: API Transferência)
            var serviceKey = Request.Headers["X-Service-Key"].FirstOrDefault();
            var isInternalService = !string.IsNullOrEmpty(serviceKey) && 
                                   serviceKey == "BankMore-Internal-Service-Key-2024";

            if (!isInternalService)
            {
                // Chamada de usuário normal - validar token
                var idContaCorrente = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

                if (string.IsNullOrEmpty(idContaCorrente) || idContaCorrente != id)
                {
                    return Forbid();
                }
            }

            var command = new MovimentacaoCommand
            {
                ChaveIdempotencia = request.ChaveIdempotencia,
                IdContaCorrente = id,
                NumeroConta = request.NumeroConta,
                TipoMovimento = request.TipoMovimento,
                Valor = request.Valor
            };

            await _mediator.Send(command);

            _logger.LogInformation("Movimentação criada: Conta {IdConta}, Tipo {Tipo}, Valor {Valor}", 
                id, request.TipoMovimento, request.Valor);

            return NoContent();
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("INVALID_ACCOUNT"))
        {
            return NotFound(CriarProblemDetails(
                "Conta Não Encontrada",
                "A conta não está cadastrada no sistema",
                StatusCodes.Status404NotFound,
                "INVALID_ACCOUNT"
            ));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("INACTIVE_ACCOUNT"))
        {
            return UnprocessableEntity(CriarProblemDetails(
                "Conta Inativa",
                "Não é possível movimentar conta inativa",
                StatusCodes.Status422UnprocessableEntity,
                "INACTIVE_ACCOUNT"
            ));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("INVALID_VALUE"))
        {
            return BadRequest(CriarProblemDetails(
                "Valor Inválido",
                "O valor da movimentação deve ser maior que zero",
                StatusCodes.Status400BadRequest,
                "INVALID_VALUE"
            ));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("INVALID_TYPE"))
        {
            return BadRequest(CriarProblemDetails(
                "Tipo Inválido",
                ex.Message.Replace("INVALID_TYPE: ", ""),
                StatusCodes.Status400BadRequest,
                "INVALID_TYPE"
            ));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("INSUFFICIENT_BALANCE"))
        {
            var mensagem = ex.Message.Replace("INSUFFICIENT_BALANCE: ", "");
            return UnprocessableEntity(CriarProblemDetails(
                "Saldo Insuficiente",
                mensagem,
                StatusCodes.Status422UnprocessableEntity,
                "INSUFFICIENT_BALANCE"
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar movimentação na conta {IdConta}", id);
            return StatusCode(500, CriarProblemDetails(
                "Erro Interno",
                "Ocorreu um erro ao processar a movimentação",
                StatusCodes.Status500InternalServerError,
                "INTERNAL_ERROR"
            ));
        }
    }

    /// <summary>
    /// Lista movimentos da conta com paginação e filtros
    /// </summary>
    /// <param name="id">ID único da conta corrente</param>
    /// <param name="tipo">Filtro por tipo (C = Crédito, D = Débito)</param>
    /// <param name="dataInicio">Data inicial do período (formato: yyyy-MM-dd)</param>
    /// <param name="dataFim">Data final do período (formato: yyyy-MM-dd)</param>
    /// <param name="page">Número da página (base 1, padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 20, máximo: 100)</param>
    /// <returns>Lista paginada de movimentos com links HATEOAS</returns>
    /// <response code="200">Movimentos retornados com sucesso</response>
    /// <response code="400">Parâmetros inválidos (Problem Details)</response>
    /// <response code="404">Conta não encontrada (Problem Details)</response>
    /// <response code="403">Token JWT inválido ou expirado</response>
    [HttpGet("{id}/movimentos")]
    [Authorize]
    [ProducesResponseType(typeof(PaginatedList<MovimentoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginatedList<MovimentoDto>>> ListarMovimentos(
        string id,
        [FromQuery] char? tipo,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var idContaCorrente = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

            if (string.IsNullOrEmpty(idContaCorrente) || idContaCorrente != id)
            {
                return Forbid();
            }

            // Validações
            if (pageSize > 100) pageSize = 100;
            if (page < 1) page = 1;

            var query = new ListarMovimentosQuery
            {
                IdContaCorrente = id,
                TipoMovimento = tipo,
                DataInicio = dataInicio,
                DataFim = dataFim,
                Page = page,
                PageSize = pageSize
            };

            var resultado = await _mediator.Send(query);

            // Adicionar links HATEOAS para cada movimento
            foreach (var movimento in resultado.Items)
            {
                movimento.Links = new List<Link>
                {
                    new Link
                    {
                        Rel = "self",
                        Href = $"/api/v1/contas/{id}/movimentos/{movimento.Id}",
                        Method = "GET",
                        Description = "Detalhes do movimento"
                    },
                    new Link
                    {
                        Rel = "conta",
                        Href = $"/api/v1/contas/{id}",
                        Method = "GET",
                        Description = "Conta associada"
                    }
                };
            }

            // Adicionar links de paginação
            resultado.Links = GerarLinksPaginacao(id, page, pageSize, resultado.TotalPages, tipo, dataInicio, dataFim);

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar movimentos da conta {IdConta}", id);
            return StatusCode(500, CriarProblemDetails(
                "Erro Interno",
                "Ocorreu um erro ao listar os movimentos",
                StatusCodes.Status500InternalServerError,
                "INTERNAL_ERROR"
            ));
        }
    }

    #region Métodos Auxiliares

    private List<Link> GerarLinksHateoas(string idConta, bool incluirInativar, bool incluirMovimentos)
    {
        var links = new List<Link>
        {
            new Link
            {
                Rel = "self",
                Href = $"/api/v1/contas/{idConta}",
                Method = "GET",
                Description = "Detalhes da conta"
            },
            new Link
            {
                Rel = "saldo",
                Href = $"/api/v1/contas/{idConta}/saldo",
                Method = "GET",
                Description = "Consultar saldo"
            }
        };

        if (incluirMovimentos)
        {
            links.Add(new Link
            {
                Rel = "movimentos",
                Href = $"/api/v1/contas/{idConta}/movimentos",
                Method = "GET",
                Description = "Listar movimentos"
            });

            links.Add(new Link
            {
                Rel = "criar-movimento",
                Href = $"/api/v1/contas/{idConta}/movimentos",
                Method = "POST",
                Description = "Criar novo movimento"
            });
        }

        if (incluirInativar)
        {
            links.Add(new Link
            {
                Rel = "inativar",
                Href = $"/api/v1/contas/{idConta}/inativar",
                Method = "PUT",
                Description = "Inativar conta"
            });
        }

        links.Add(new Link
        {
            Rel = "transferir",
            Href = "/api/v1/transferencias",
            Method = "POST",
            Description = "Realizar transferência"
        });

        return links;
    }

    private List<Link> GerarLinksPaginacao(string idConta, int page, int pageSize, int totalPages, 
        char? tipo, DateTime? dataInicio, DateTime? dataFim)
    {
        var links = new List<Link>();
        var baseUrl = $"/api/v1/contas/{idConta}/movimentos";
        var queryParams = $"pageSize={pageSize}";

        if (tipo.HasValue) queryParams += $"&tipo={tipo}";
        if (dataInicio.HasValue) queryParams += $"&dataInicio={dataInicio:yyyy-MM-dd}";
        if (dataFim.HasValue) queryParams += $"&dataFim={dataFim:yyyy-MM-dd}";

        // Self
        links.Add(new Link
        {
            Rel = "self",
            Href = $"{baseUrl}?page={page}&{queryParams}",
            Method = "GET"
        });

        // First
        links.Add(new Link
        {
            Rel = "first",
            Href = $"{baseUrl}?page=1&{queryParams}",
            Method = "GET"
        });

        // Previous
        if (page > 1)
        {
            links.Add(new Link
            {
                Rel = "previous",
                Href = $"{baseUrl}?page={page - 1}&{queryParams}",
                Method = "GET"
            });
        }

        // Next
        if (page < totalPages)
        {
            links.Add(new Link
            {
                Rel = "next",
                Href = $"{baseUrl}?page={page + 1}&{queryParams}",
                Method = "GET"
            });
        }

        // Last
        links.Add(new Link
        {
            Rel = "last",
            Href = $"{baseUrl}?page={totalPages}&{queryParams}",
            Method = "GET"
        });

        return links;
    }

    private ApiProblemDetails CriarProblemDetails(string title, string detail, int status, string errorCode)
    {
        return new ApiProblemDetails
        {
            Type = $"https://bankmore.com.br/errors/{errorCode.ToLowerInvariant()}",
            Title = title,
            Status = status,
            Detail = detail,
            Instance = HttpContext.Request.Path,
            ErrorCode = errorCode,
            Timestamp = DateTime.UtcNow
        };
    }

    #endregion
}

/// <summary>
/// Request para inativar conta
/// </summary>
public class InativarContaRequest
{
    /// <summary>
    /// Senha da conta para confirmação
    /// </summary>
    public string Senha { get; set; } = string.Empty;
}

/// <summary>
/// Request para movimentação
/// </summary>
public class MovimentacaoRequest
{
    /// <summary>
    /// Chave de idempotência para evitar duplicação
    /// </summary>
    public string ChaveIdempotencia { get; set; } = string.Empty;

    /// <summary>
    /// Número da conta (opcional, se não informado usa a conta do token)
    /// </summary>
    public int? NumeroConta { get; set; }

    /// <summary>
    /// Tipo de movimento: C = Crédito, D = Débito
    /// </summary>
    public char TipoMovimento { get; set; }

    /// <summary>
    /// Valor da movimentação (sempre positivo)
    /// </summary>
    public decimal Valor { get; set; }
}
