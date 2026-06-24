using FluentAssertions;
using TechChallenge.Application.Features.Veiculos.CriarVeiculo;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Tests.Features.Veiculos.CriarVeiculo;

public class CriarVeiculoCommandValidatorTests
{
    private readonly CriarVeiculoCommandValidator _validator = new();

    [Fact]
    public void DeveValidarCommandQuandoDadosDoVeiculoEstaoCorretos()
    {
        var command = CriarCommandValido();

        var resultado = _validator.Validate(command);

        resultado.IsValid.Should().BeTrue();
    }

    [Fact]
    public void DeveRetornarErroQuandoClienteIdEstiverVazio()
    {
        var command = CriarCommandValido();
        command.ClienteId = Guid.Empty;

        var resultado = _validator.Validate(command);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(error => error.PropertyName == nameof(CriarVeiculoCommand.ClienteId));
    }

    [Fact]
    public void DeveRetornarErroQuandoAnoOuValoresNumericosForemInvalidos()
    {
        var command = CriarCommandValido();
        command.Ano = 0;
        command.Quilometragem = -1;
        command.Valor = -1;

        var resultado = _validator.Validate(command);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Select(error => error.PropertyName).Should().Contain([
            nameof(CriarVeiculoCommand.Ano),
            nameof(CriarVeiculoCommand.Quilometragem),
            nameof(CriarVeiculoCommand.Valor)
        ]);
    }

    private static CriarVeiculoCommand CriarCommandValido()
    {
        return new CriarVeiculoCommand
        {
            Tipo = TipoVeiculo.Carro,
            Placa = "ABC1234",
            Modelo = "Civic",
            Marca = "Honda",
            Cor = "Prata",
            Ano = 2022,
            Quilometragem = 10000,
            Valor = 90000,
            ClienteId = Guid.NewGuid()
        };
    }
}
