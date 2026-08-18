using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Domain.Entities;

namespace TechChallenge.Application.Features.CategoriaVeiculos.ObterCategoriaVeiculo;

public class ObterCategoriaVeiculoService
{
    private readonly ICategoriaVeiculoRepository _categoriaVeiculoRepository;

    public ObterCategoriaVeiculoService(ICategoriaVeiculoRepository categoriaVeiculoRepository)
    {
        _categoriaVeiculoRepository = categoriaVeiculoRepository;
    }

    public CategoriaVeiculo ObterCategoriaVeiculo(ObterCategoriaVeiculoQuery query)
    {
        var categoriaVeiculo = _categoriaVeiculoRepository.GetByIdAsync(query.Id).GetAwaiter().GetResult();

        if (categoriaVeiculo is null)
            throw new KeyNotFoundException($"Categoria de veículo com Id {query.Id} não encontrada.");

        return categoriaVeiculo;
    }
}