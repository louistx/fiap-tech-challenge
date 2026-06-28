using FluentValidation;
using TechChallenge.Application.Validation;

namespace TechChallenge.Application.Features.Clientes.AtualizarCliente;

public class AtualizarClienteCommandValidator : AbstractValidator<AtualizarClienteCommand>
{
    public AtualizarClienteCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();

        RuleFor(command => command.Nome)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(command => command.Cpf)
            .NotEmpty()
            .MaximumLength(14)
            .Must(CpfValidator.CpfValido)
            .WithMessage("CPF inválido.");

        RuleFor(command => command.Rg)
            .NotEmpty()
            .MaximumLength(9);

        RuleFor(command => command.Logradouro)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(command => command.Complemento)
            .MaximumLength(80);

        RuleFor(command => command.Numero)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(command => command.Bairro)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(command => command.Cidade)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(command => command.Estado)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(command => command.Cep)
            .NotEmpty()
            .MaximumLength(30);
    }
}
