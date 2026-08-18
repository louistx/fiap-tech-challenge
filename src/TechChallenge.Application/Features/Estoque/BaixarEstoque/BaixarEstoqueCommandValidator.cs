using FluentValidation;

namespace TechChallenge.Application.Features.Estoque.BaixarEstoque;

public class BaixarEstoqueCommandValidator : AbstractValidator<BaixarEstoqueCommand>
{
    public BaixarEstoqueCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();

        RuleFor(command => command.Quantidade)
            .GreaterThan(0);
    }
}