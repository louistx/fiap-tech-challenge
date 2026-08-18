using TechChallenge.Domain.Entities;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.CategoriaVeiculos.ListarCategoriasVeiculos;

public class ListarCategoriasVeiculosService
{
    private readonly ICategoriaVeiculoRepository _categoriaVeiculoRepository;

    public ListarCategoriasVeiculosService(ICategoriaVeiculoRepository categoriaVeiculoRepository)
    {
        _categoriaVeiculoRepository = categoriaVeiculoRepository;
    }

    public List<CategoriaVeiculo> ListarCategoriasVeiculos(ListarCategoriasVeiculosQuery query)
    {
        return _categoriaVeiculoRepository.GetAllAsync().GetAwaiter().GetResult();
    }
}