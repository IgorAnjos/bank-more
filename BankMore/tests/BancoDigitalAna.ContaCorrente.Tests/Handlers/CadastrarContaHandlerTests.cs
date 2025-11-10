using Xunit;
using Moq;
using FluentAssertions;
using BancoDigitalAna.ContaCorrente.Application.Commands;
using BancoDigitalAna.ContaCorrente.Application.Handlers;
using BancoDigitalAna.ContaCorrente.Domain.Interfaces;
using BancoDigitalAna.ContaCorrente.Domain.Entities;

namespace BancoDigitalAna.ContaCorrente.Tests.Handlers;

public class CadastrarContaHandlerTests
{
    private readonly Mock<IContaCorrenteRepository> _mockRepository;
    private readonly CadastrarContaHandler _handler;

    public CadastrarContaHandlerTests()
    {
        _mockRepository = new Mock<IContaCorrenteRepository>();
        _handler = new CadastrarContaHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ComCpfValido_DeveCriarContaComSucesso()
    {
        // Arrange
        var command = new CadastrarContaCommand
        {
            Cpf = "12345678909", // CPF válido
            Nome = "João Silva",
            Senha = "SenhaForte123!"
        };

        _mockRepository
            .Setup(r => r.ProximoNumeroConta())
            .ReturnsAsync(100);

        _mockRepository
            .Setup(r => r.AdicionarAsync(It.IsAny<Domain.Entities.ContaCorrente>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.NumeroContaCorrente.Should().Be(100);
        resultado.Nome.Should().Be("João Silva");
        resultado.Ativo.Should().BeTrue();
        resultado.Saldo.Should().Be(0);

        _mockRepository.Verify(r => r.AdicionarAsync(It.IsAny<Domain.Entities.ContaCorrente>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ComCpfInvalido_DeveLancarExcecao()
    {
        // Arrange
        var command = new CadastrarContaCommand
        {
            Cpf = "12345678901", // CPF inválido
            Nome = "João Silva",
            Senha = "SenhaForte123!"
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*INVALID_DOCUMENT*");
    }

    [Fact]
    public async Task Handle_ComCpfVazio_DeveLancarExcecao()
    {
        // Arrange
        var command = new CadastrarContaCommand
        {
            Cpf = "",
            Nome = "João Silva",
            Senha = "SenhaForte123!"
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_DeveCriptografarSenha()
    {
        // Arrange
        var command = new CadastrarContaCommand
        {
            Cpf = "12345678909",
            Nome = "Maria Santos",
            Senha = "SenhaSecreta456!"
        };

        Domain.Entities.ContaCorrente? contaSalva = null;

        _mockRepository
            .Setup(r => r.ProximoNumeroConta())
            .ReturnsAsync(200);

        _mockRepository
            .Setup(r => r.AdicionarAsync(It.IsAny<Domain.Entities.ContaCorrente>()))
            .Callback<Domain.Entities.ContaCorrente>(c => contaSalva = c)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        contaSalva.Should().NotBeNull();
        contaSalva!.Senha.Should().NotBe("SenhaSecreta456!"); // Senha deve estar hasheada
        contaSalva.Senha.Should().StartWith("$2"); // BCrypt hash
        contaSalva.Salt.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("52998224725")]  // CPF válido 1
    [InlineData("11144477735")]  // CPF válido 2
    [InlineData("12345678909")]  // CPF válido 3
    public async Task Handle_ComDiferentesCpfsValidos_DeveCriarConta(string cpfValido)
    {
        // Arrange
        var command = new CadastrarContaCommand
        {
            Cpf = cpfValido,
            Nome = "Teste Usuario",
            Senha = "Senha123!"
        };

        _mockRepository.Setup(r => r.ProximoNumeroConta()).ReturnsAsync(1);
        _mockRepository.Setup(r => r.AdicionarAsync(It.IsAny<Domain.Entities.ContaCorrente>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.NumeroContaCorrente.Should().BeGreaterThan(0);
    }
}
