using FluentValidation;
using TechChallenge.Application.Validation;

namespace TechChallenge.Application.Features.Clientes.CriarCliente;

public class CriarClienteCommandValidator : AbstractValidator<CriarClienteCommand>
{
    public CriarClienteCommandValidator()
    {
        RuleFor(command => command.Nome)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(command => command.Cpf)
            .NotEmpty()
            .MaximumLength(14)
            .Must(CpfValidator.CpfValido)
            .WithMessage("CPF inválido.");

        RuleFor(command => command.Rg)
            .MaximumLength(9);

        RuleFor(command => command.Logradouro)
            .MaximumLength(100);

        RuleFor(command => command.Complemento)
            .MaximumLength(80);

        RuleFor(command => command.Numero)
            .MaximumLength(10);

        RuleFor(command => command.Bairro)
            .MaximumLength(50);

        RuleFor(command => command.Cidade)
            .MaximumLength(50);

        RuleFor(command => command.Estado)
            .MaximumLength(30);

        RuleFor(command => command.Cep)
            .MaximumLength(30);
    }
}
