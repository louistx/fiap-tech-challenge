using TechChallenge.Domain.Entities;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.CategoriaServicos.ListarCategoriasServicos;

public class ListarCategoriasServicosService
{
    private readonly ICategoriaServicoRepository _categoriaServicoRepository;

    public ListarCategoriasServicosService(ICategoriaServicoRepository categoriaServicoRepository)
    {
        _categoriaServicoRepository = categoriaServicoRepository;
    }

    public List<CategoriaServico> ListarCategoriasServicos(ListarCategoriasServicosQuery query)
    {
        return _categoriaServicoRepository.GetAllAsync().GetAwaiter().GetResult();
    }
}