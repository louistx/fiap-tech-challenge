using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Domain.Entities;

namespace TechChallenge.Application.Features.Inventario.ListarInventario;

public class ListarInventarioService
{
    private readonly IProdutoRepository _produtoRepository;

    public ListarInventarioService(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public List<Produto> ListarInventario(ListarInventarioQuery query)
    {
        return _produtoRepository.GetAllAsync().GetAwaiter().GetResult();
    }
}
