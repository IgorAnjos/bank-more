using Xunit;
using Moq;
using FluentAssertions;
using BancoDigitalAna.ContaCorrente.Application.Commands;
using BancoDigitalAna.ContaCorrente.Application.Handlers;
using BancoDigitalAna.ContaCorrente.Domain.Interfaces;
using BancoDigitalAna.ContaCorrente.Domain.Entities;

namespace BancoDigitalAna.ContaCorrente.Tests.Handlers;

public class MovimentacaoHandlerTests
{
    private readonly Mock<IContaCorrenteRepository> _mockContaRepository;
    private readonly Mock<IMovimentoRepository> _mockMovimentoRepository;
    private readonly Mock<IIdempotenciaRepository> _mockIdempotenciaRepository;
    private readonly MovimentacaoHandler _handler;

    public MovimentacaoHandlerTests()
    {
        _mockContaRepository = new Mock<IContaCorrenteRepository>();
        _mockMovimentoRepository = new Mock<IMovimentoRepository>();
        _mockIdempotenciaRepository = new Mock<IIdempotenciaRepository>();
        
        _handler = new MovimentacaoHandler(
            _mockContaRepository.Object,
            _mockMovimentoRepository.Object,
            _mockIdempotenciaRepository.Object
        );
    }

    [Fact]
    public async Task Handle_ComCreditoValido_DeveCriarMovimento()
    {
        // Arrange
        var contaId = Guid.NewGuid().ToString();
        var command = new MovimentacaoCommand
        {
            ChaveIdempotencia = "chave-test-001",
            IdContaCorrente = contaId,
            TipoMovimento = 'C',
            Valor = 100.50m
        };

        var conta = new Domain.Entities.ContaCorrente("12345678909", "Teste", "senha", "salt")
        {
            Ativo = true
        };

        _mockIdempotenciaRepository
            .Setup(r => r.ObterPorChaveAsync(command.ChaveIdempotencia))
            .ReturnsAsync((Idempotencia?)null);

        _mockContaRepository
            .Setup(r => r.ObterPorIdAsync(contaId))
            .ReturnsAsync(conta);

        _mockMovimentoRepository
            .Setup(r => r.AdicionarAsync(It.IsAny<Movimento>()))
            .Returns(Task.CompletedTask);

        _mockIdempotenciaRepository
            .Setup(r => r.AdicionarAsync(It.IsAny<Idempotencia>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().BeTrue();
        _mockMovimentoRepository.Verify(r => r.AdicionarAsync(It.IsAny<Movimento>()), Times.Once);
        _mockIdempotenciaRepository.Verify(r => r.AdicionarAsync(It.IsAny<Idempotencia>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ComContaInativa_DeveLancarExcecao()
    {
        // Arrange
        var contaId = Guid.NewGuid().ToString();
        var command = new MovimentacaoCommand
        {
            ChaveIdempotencia = "chave-test-002",
            IdContaCorrente = contaId,
            TipoMovimento = 'D',
            Valor = 50m
        };

        var conta = new Domain.Entities.ContaCorrente("12345678909", "Teste", "senha", "salt")
        {
            Ativo = false // Conta inativa
        };

        _mockIdempotenciaRepository
            .Setup(r => r.ObterPorChaveAsync(command.ChaveIdempotencia))
            .ReturnsAsync((Idempotencia?)null);

        _mockContaRepository
            .Setup(r => r.ObterPorIdAsync(contaId))
            .ReturnsAsync(conta);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*INACTIVE_ACCOUNT*");
    }

    [Fact]
    public async Task Handle_ComValorNegativo_DeveLancarExcecao()
    {
        // Arrange
        var command = new MovimentacaoCommand
        {
            ChaveIdempotencia = "chave-test-003",
            IdContaCorrente = Guid.NewGuid().ToString(),
            TipoMovimento = 'C',
            Valor = -50m // Valor negativo
        };

        _mockIdempotenciaRepository
            .Setup(r => r.ObterPorChaveAsync(command.ChaveIdempotencia))
            .ReturnsAsync((Idempotencia?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*INVALID_VALUE*");
    }

    [Fact]
    public async Task Handle_ComTipoInvalido_DeveLancarExcecao()
    {
        // Arrange
        var command = new MovimentacaoCommand
        {
            ChaveIdempotencia = "chave-test-004",
            IdContaCorrente = Guid.NewGuid().ToString(),
            TipoMovimento = 'X', // Tipo inválido
            Valor = 100m
        };

        _mockIdempotenciaRepository
            .Setup(r => r.ObterPorChaveAsync(command.ChaveIdempotencia))
            .ReturnsAsync((Idempotencia?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*INVALID_TYPE*");
    }

    [Fact]
    public async Task Handle_ComIdempotenciaExistente_DeveRetornarSemCriarNovo()
    {
        // Arrange
        var command = new MovimentacaoCommand
        {
            ChaveIdempotencia = "chave-duplicada",
            IdContaCorrente = Guid.NewGuid().ToString(),
            TipoMovimento = 'C',
            Valor = 100m
        };

        var idempotenciaExistente = new Idempotencia("chave-duplicada", "{}", "SUCESSO");

        _mockIdempotenciaRepository
            .Setup(r => r.ObterPorChaveAsync(command.ChaveIdempotencia))
            .ReturnsAsync(idempotenciaExistente);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().BeTrue();
        _mockMovimentoRepository.Verify(r => r.AdicionarAsync(It.IsAny<Movimento>()), Times.Never);
    }

    [Theory]
    [InlineData('C', 100.00)]
    [InlineData('C', 0.01)]
    [InlineData('C', 9999.99)]
    [InlineData('D', 50.00)]
    [InlineData('D', 1.50)]
    public async Task Handle_ComDiferentesValoresETipos_DeveFuncionar(char tipo, decimal valor)
    {
        // Arrange
        var contaId = Guid.NewGuid().ToString();
        var command = new MovimentacaoCommand
        {
            ChaveIdempotencia = $"chave-{Guid.NewGuid()}",
            IdContaCorrente = contaId,
            TipoMovimento = tipo,
            Valor = valor
        };

        var conta = new Domain.Entities.ContaCorrente("12345678909", "Teste", "senha", "salt")
        {
            Ativo = true
        };

        _mockIdempotenciaRepository
            .Setup(r => r.ObterPorChaveAsync(It.IsAny<string>()))
            .ReturnsAsync((Idempotencia?)null);

        _mockContaRepository
            .Setup(r => r.ObterPorIdAsync(contaId))
            .ReturnsAsync(conta);

        _mockMovimentoRepository
            .Setup(r => r.AdicionarAsync(It.IsAny<Movimento>()))
            .Returns(Task.CompletedTask);

        _mockIdempotenciaRepository
            .Setup(r => r.AdicionarAsync(It.IsAny<Idempotencia>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().BeTrue();
    }
}
