using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Veiculos.ExcluirVeiculo;

public class ExcluirVeiculoService
{
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IValidator<ExcluirVeiculoCommand> _validator;

    public ExcluirVeiculoService(IVeiculoRepository veiculoRepository, IValidator<ExcluirVeiculoCommand> validator)
    {
        _veiculoRepository = veiculoRepository;
        _validator = validator;
    }

    public bool ExcluirVeiculo(ExcluirVeiculoCommand command)
    {
        _validator.ValidateAndThrow(command);

        var veiculo = _veiculoRepository.GetByIdAsync(command.Id).GetAwaiter().GetResult();
        if (veiculo is null)
            throw new KeyNotFoundException($"Veículo com Id {command.Id} não encontrado.");

        _veiculoRepository.DeleteAsync(veiculo).GetAwaiter().GetResult();
        return true;
    }
}
