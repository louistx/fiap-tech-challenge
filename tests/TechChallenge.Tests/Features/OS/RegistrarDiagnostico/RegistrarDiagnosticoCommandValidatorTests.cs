using FluentAssertions;
using TechChallenge.Application.Features.OS.RegistrarDiagnostico;

namespace TechChallenge.Tests.Features.OS.RegistrarDiagnostico;

public class RegistrarDiagnosticoCommandValidatorTests
{
    private readonly RegistrarDiagnosticoCommandValidator _validator = new();

    [Fact]
    public void DeveValidarCommandQuandoServicoOuProdutoForInformado()
    {
        var command = new RegistrarDiagnosticoCommand
        {
            OrdemServicoId = Guid.NewGuid(),
            ServicosIds = [Guid.NewGuid()]
        };

        var resultado = _validator.Validate(command);

        resultado.IsValid.Should().BeTrue();
    }

    [Fact]
    public void DeveRetornarErroQuandoNaoHouverServicoNemProduto()
    {
        var command = new RegistrarDiagnosticoCommand
        {
            OrdemServicoId = Guid.NewGuid()
        };

        var resultado = _validator.Validate(command);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(error =>
            error.ErrorMessage == "Informe ao menos um serviço ou produto para registrar o diagnóstico.");
    }

    [Fact]
    public void DeveRetornarErroQuandoOrdemServicoIdOuItensEstiveremVazios()
    {
        var command = new RegistrarDiagnosticoCommand
        {
            OrdemServicoId = Guid.Empty,
            ServicosIds = [Guid.Empty],
            ProdutosIds = [Guid.Empty]
        };

        var resultado = _validator.Validate(command);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Select(error => error.PropertyName).Should().Contain([
            nameof(RegistrarDiagnosticoCommand.OrdemServicoId),
            $"{nameof(RegistrarDiagnosticoCommand.ServicosIds)}[0]",
            $"{nameof(RegistrarDiagnosticoCommand.ProdutosIds)}[0]"
        ]);
    }
}
