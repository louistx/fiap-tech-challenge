using TechChallenge.Domain.Entities;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.CategoriaProdutos.ListarCategoriasProdutos;

public class ListarEstoquesService
{
    private readonly ICategoriaProdutoRepository _categoriaProdutoRepository;

    public ListarEstoquesService(ICategoriaProdutoRepository categoriaProdutoRepository)
    {
        _categoriaProdutoRepository = categoriaProdutoRepository;
    }

    public List<CategoriaProduto> ListarCategoriasProdutos(ListarCategoriasProdutosQuery query)
    {
        return _categoriaProdutoRepository.GetAllAsync().GetAwaiter().GetResult();
    }
}