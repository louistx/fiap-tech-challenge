using FluentValidation;

namespace TechChallenge.Application.Features.OS.CriarOS;

public class CriarOSCommandValidator : AbstractValidator<CriarOSCommand>
{
    public CriarOSCommandValidator()
    {
        RuleFor(command => command.Descricao)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(command => command.ClienteResponsavelId)
            .NotEmpty();

        RuleFor(command => command.FuncionarioResponsavelId)
            .NotEmpty();

        RuleFor(command => command.VeiculoId)
            .NotEmpty();
    }
}
