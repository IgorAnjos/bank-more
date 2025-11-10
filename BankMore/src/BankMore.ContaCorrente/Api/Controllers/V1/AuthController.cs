using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Asp.Versioning;
using BankMore.ContaCorrente.Application.Commands;
using BankMore.ContaCorrente.Application.DTOs;

namespace BankMore.ContaCorrente.Api.Controllers.V1;

/// <summary>
/// Controller RESTful para autenticação e gerenciamento de tokens (v1)
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
[Produces("application/json", "application/problem+json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Autentica usuário e gera access token + refresh token JWT (login)
    /// </summary>
    /// <param name="request">Credenciais de login (CPF ou número da conta + senha)</param>
    /// <returns>Access token (10min) e refresh token (1 dia)</returns>
    /// <response code="200">Autenticação bem-sucedida. Retorna access token e refresh token</response>
    /// <response code="401">Credenciais inválidas (Problem Details)</response>
    /// <response code="400">Dados de entrada inválidos (Problem Details)</response>
    [HttpPost("tokens")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponseDto>> GerarToken([FromBody] LoginCommand request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.NumeroOuCpf) || 
                string.IsNullOrWhiteSpace(request.Senha))
            {
                return BadRequest(CriarProblemDetails(
                    "Dados Inválidos",
                    "Número da conta/CPF e senha são obrigatórios",
                    StatusCodes.Status400BadRequest,
                    "INVALID_INPUT"
                ));
            }

            var resultado = await _mediator.Send(request);

            _logger.LogInformation("Login bem-sucedido para conta: {NumeroConta}", 
                request.NumeroOuCpf);

            return Ok(resultado);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Tentativa de login falhou: {Mensagem}", ex.Message);

            return Unauthorized(CriarProblemDetails(
                "Credenciais Inválidas",
                "Número da conta/CPF ou senha incorretos",
                StatusCodes.Status401Unauthorized,
                "USER_UNAUTHORIZED"
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar login");

            return StatusCode(500, CriarProblemDetails(
                "Erro Interno",
                "Ocorreu um erro ao processar sua solicitação",
                StatusCodes.Status500InternalServerError,
                "INTERNAL_ERROR"
            ));
        }
    }

    /// <summary>
    /// Renova o access token usando o refresh token
    /// </summary>
    /// <param name="request">Refresh token válido</param>
    /// <returns>Novo access token e novo refresh token</returns>
    /// <response code="200">Token renovado com sucesso</response>
    /// <response code="401">Refresh token inválido, expirado ou revogado</response>
    /// <response code="400">Refresh token não fornecido</response>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponseDto>> RenovarToken([FromBody] RefreshTokenCommand request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return BadRequest(CriarProblemDetails(
                    "Dados Inválidos",
                    "Refresh token é obrigatório",
                    StatusCodes.Status400BadRequest,
                    "INVALID_INPUT"
                ));
            }

            var resultado = await _mediator.Send(request);

            _logger.LogInformation("Token renovado com sucesso para conta: {IdConta}", 
                resultado.IdContaCorrente);

            return Ok(resultado);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Tentativa de renovação falhou: {Mensagem}", ex.Message);

            return Unauthorized(CriarProblemDetails(
                "Refresh Token Inválido",
                ex.Message,
                StatusCodes.Status401Unauthorized,
                "INVALID_REFRESH_TOKEN"
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao renovar token");

            return StatusCode(500, CriarProblemDetails(
                "Erro Interno",
                "Ocorreu um erro ao processar sua solicitação",
                StatusCodes.Status500InternalServerError,
                "INTERNAL_ERROR"
            ));
        }
    }

    /// <summary>
    /// Revoga token JWT (logout) - Placeholder para implementação futura
    /// </summary>
    /// <returns>Status 204 No Content</returns>
    /// <response code="204">Token revogado com sucesso</response>
    /// <response code="401">Token inválido</response>
    [HttpDelete("tokens")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public IActionResult RevogarToken()
    {
        // TODO: Implementar blacklist de tokens ou usar refresh tokens
        // Por enquanto, apenas retorna sucesso (stateless JWT)
        _logger.LogInformation("Logout realizado");
        return NoContent();
    }

    #region Métodos Auxiliares

    private ApiProblemDetails CriarProblemDetails(string title, string detail, int status, string errorCode)
    {
        return new ApiProblemDetails
        {
            Type = $"https://www.httpstatus.com.br/{errorCode.ToLowerInvariant()}",
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
