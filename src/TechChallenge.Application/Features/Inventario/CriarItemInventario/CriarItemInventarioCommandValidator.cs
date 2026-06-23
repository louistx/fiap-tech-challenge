using FluentValidation;

namespace TechChallenge.Application.Features.Inventario.CriarItemInventario;

public class CriarItemInventarioCommandValidator : AbstractValidator<CriarItemInventarioCommand>
{
    public CriarItemInventarioCommandValidator()
    {
        RuleFor(command => command.Descricao)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(command => command.Valor)
            .GreaterThanOrEqualTo(0);
    }
}
