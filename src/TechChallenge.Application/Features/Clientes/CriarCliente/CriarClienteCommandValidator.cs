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
            .Must(DocumentoValidator.CpfValido)
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
