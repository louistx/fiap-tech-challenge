using FluentAssertions;
using TechChallenge.Application.Features.Clientes.CriarCliente;

namespace TechChallenge.Tests.Features.Clientes.CriarCliente;

public class CriarClienteCommandValidatorTests
{
    private readonly CriarClienteCommandValidator _validator = new();

    [Fact]
    public void DeveValidarCommandQuandoDadosObrigatoriosEstaoCorretos()
    {
        var command = CriarCommandValido();

        var resultado = _validator.Validate(command);

        resultado.IsValid.Should().BeTrue();
    }

    [Fact]
    public void DeveRetornarErroQuandoCpfForInvalido()
    {
        var command = CriarCommandValido();
        command.Cpf = "12345678901";

        var resultado = _validator.Validate(command);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CriarClienteCommand.Cpf) &&
            error.ErrorMessage == "CPF inválido.");
    }

    [Fact]
    public void DeveRetornarErroQuandoCamposObrigatoriosEstiveremVazios()
    {
        var command = CriarCommandValido();
        command.Nome = string.Empty;
        command.Logradouro = string.Empty;
        command.Numero = string.Empty;

        var resultado = _validator.Validate(command);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Select(error => error.PropertyName).Should().Contain([
            nameof(CriarClienteCommand.Nome),
            nameof(CriarClienteCommand.Logradouro),
            nameof(CriarClienteCommand.Numero)
        ]);
    }

    private static CriarClienteCommand CriarCommandValido()
    {
        return new CriarClienteCommand
        {
            Nome = "Maria Cliente",
            Cpf = "52998224725",
            Rg = "123456789",
            Logradouro = "Rua Teste",
            Complemento = "Apto 10",
            Numero = "100",
            Bairro = "Centro",
            Cidade = "Sao Paulo",
            Estado = "SP",
            Cep = "01001000"
        };
    }
}
