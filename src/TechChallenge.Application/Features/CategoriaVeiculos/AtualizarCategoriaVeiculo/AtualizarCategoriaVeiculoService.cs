using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Validation;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Features.CategoriaVeiculos.AtualizarCategoriaVeiculo;

public class AtualizarCategoriaVeiculoService
{
    private readonly ICategoriaVeiculoRepository _categoriaVeiculoRepository;
    private readonly IValidator<AtualizarCategoriaVeiculoCommand> _validator;

    public AtualizarCategoriaVeiculoService(ICategoriaVeiculoRepository categoriaVeiculoRepository, IValidator<AtualizarCategoriaVeiculoCommand> validator)
    {
        _categoriaVeiculoRepository = categoriaVeiculoRepository;
        _validator = validator;
    }

    public bool AtualizarCategoriaVeiculo(AtualizarCategoriaVeiculoCommand command)
    {
        _validator.ValidateAndThrow(command);

        var categoriaVeiculo = _categoriaVeiculoRepository.GetByIdAsync(command.Id).GetAwaiter().GetResult();

        if (categoriaVeiculo is null)
            throw new KeyNotFoundException($"Categoria de Veículo com Id {command.Id} não encontrada.");

        categoriaVeiculo.AtualizarDescricao(command.Descricao);

        _categoriaVeiculoRepository.UpdateAsync(categoriaVeiculo).GetAwaiter().GetResult();
        return true;
    }
}
