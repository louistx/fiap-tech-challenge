using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Estoque.ObterEstoque;

public class ObterEstoqueService
{
    private readonly IEstoqueRepository _estoqueRepository;

    public ObterEstoqueService(IEstoqueRepository estoqueRepository)
    {
        _estoqueRepository = estoqueRepository;
    }

    public async Task<Domain.Entities.Estoque> ObterEstoque(ObterEstoqueQuery query)
    {
        var estoque = await _estoqueRepository.GetByIdAsync(query.Id);

        if (estoque is null)
            throw new KeyNotFoundException($"Estoque do produto Id {query.Id} não encontrado.");

        return estoque;
    }
}