using Xunit;
using FluentAssertions;
using BankMore.ContaCorrente.Application.Services;

namespace BankMore.ContaCorrente.Tests.Services;

public class CpfValidatorTests
{
    // CpfValidator é static, não precisa de instância

    [Theory]
    [InlineData("52998224725")]  // CPF válido 1
    [InlineData("11144477735")]  // CPF válido 2
    [InlineData("12345678909")]  // CPF válido 3
    [InlineData("00000000191")]  // CPF válido 4
    [InlineData("123.456.789-09")]  // CPF válido formatado
    public void Validar_ComCpfValido_DeveRetornarTrue(string cpf)
    {
        // Act
        var resultado = CpfValidator.Validar(cpf);

        // Assert
        resultado.Should().BeTrue();
    }

    [Theory]
    [InlineData("12345678901")]  // Dígitos verificadores inválidos
    [InlineData("11111111111")]  // CPF com todos dígitos iguais
    [InlineData("00000000000")]  // CPF com todos dígitos iguais
    [InlineData("99999999999")]  // CPF com todos dígitos iguais
    [InlineData("12345678900")]  // Dígito verificador errado
    public void Validar_ComCpfInvalido_DeveRetornarFalse(string cpf)
    {
        // Act
        var resultado = CpfValidator.Validar(cpf);

        // Assert
        resultado.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Validar_ComCpfVazioOuNulo_DeveRetornarFalse(string cpf)
    {
        // Act
        var resultado = CpfValidator.Validar(cpf);

        // Assert
        resultado.Should().BeFalse();
    }

    [Theory]
    [InlineData("123456789")]     // Menos de 11 dígitos
    [InlineData("123456789012")]  // Mais de 11 dígitos
    [InlineData("1234567890A")]   // Contém letra
    public void Validar_ComTamanhoOuFormatoInvalido_DeveRetornarFalse(string cpf)
    {
        // Act
        var resultado = CpfValidator.Validar(cpf);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void Validar_ComSequenciaDeZeros_DeveRetornarFalse()
    {
        // Arrange
        var cpf = "00000000000";

        // Act
        var resultado = CpfValidator.Validar(cpf);

        // Assert
        resultado.Should().BeFalse();
    }

    [Theory]
    [InlineData("11111111111")]
    [InlineData("22222222222")]
    [InlineData("33333333333")]
    [InlineData("44444444444")]
    [InlineData("55555555555")]
    [InlineData("66666666666")]
    [InlineData("77777777777")]
    [InlineData("88888888888")]
    [InlineData("99999999999")]
    public void Validar_ComDigitosRepetidos_DeveRetornarFalse(string cpf)
    {
        // Act
        var resultado = CpfValidator.Validar(cpf);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void Validar_DeveFuncionarRapido()
    {
        // Arrange
        var cpf = "52998224725";
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        for (int i = 0; i < 1000; i++)
        {
            CpfValidator.Validar(cpf);
        }
        stopwatch.Stop();

        // Assert - 1000 validações em menos de 100ms
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100);
    }
}
