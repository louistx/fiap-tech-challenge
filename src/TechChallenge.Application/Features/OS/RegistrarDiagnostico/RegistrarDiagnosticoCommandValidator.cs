using FluentValidation;

namespace TechChallenge.Application.Features.OS.RegistrarDiagnostico;

public class RegistrarDiagnosticoCommandValidator : AbstractValidator<RegistrarDiagnosticoCommand>
{
    public RegistrarDiagnosticoCommandValidator()
    {
        RuleFor(command => command.OrdemServicoId)
            .NotEmpty();

        RuleFor(command => command)
            .Must(command => command.ServicosIds.Count != 0 || command.ProdutosIds.Count != 0)
            .WithMessage("Informe ao menos um serviço ou produto para registrar o diagnóstico.");

        RuleForEach(command => command.ServicosIds)
            .NotEmpty();

        RuleForEach(command => command.ProdutosIds)
            .NotEmpty();
    }
}
