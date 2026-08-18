using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.CategoriaVeiculos.CriarCategoriaVeiculo;

public class CriarCategoriaVeiculoService
{
    private readonly ICategoriaVeiculoRepository _categoriaVeiculoRepository;
    private readonly IValidator<CriarCategoriaVeiculoCommand> _validator;

    public CriarCategoriaVeiculoService(ICategoriaVeiculoRepository categoriaVeiculoRepository, IValidator<CriarCategoriaVeiculoCommand> validator)
    {
        _categoriaVeiculoRepository = categoriaVeiculoRepository;
        _validator = validator;
    }

    public Guid CriarCategoriaVeiculo(CriarCategoriaVeiculoCommand command)
    {
        _validator.ValidateAndThrow(command);
        var descricao = command.Descricao.Trim();

        var categoriaVeiculoExiste = _categoriaVeiculoRepository.GetByDescricaoAsync(descricao).GetAwaiter().GetResult();
        if (categoriaVeiculoExiste is not null)
            throw new InvalidOperationException($"Já existe uma categoria de veículo cadastrada com a descrição {descricao}.");

        var categoriaVeiculo = new Domain.Entities.CategoriaVeiculo(Guid.NewGuid(), command.Descricao);

        _categoriaVeiculoRepository.AddAsync(categoriaVeiculo).GetAwaiter().GetResult();

        return categoriaVeiculo.Id;
    }
}