using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Estoque.AdicionarEstoque;

public class AdicionarEstoqueService
{
    private readonly IEstoqueRepository _estoqueRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IValidator<AdicionarEstoqueCommand> _validator;

    public AdicionarEstoqueService(IEstoqueRepository estoqueRepository, IProdutoRepository produtoRepository, IValidator<AdicionarEstoqueCommand> validator)
    {
        _estoqueRepository = estoqueRepository;
        _produtoRepository = produtoRepository;
        _validator = validator;
    }

    public bool AdicionarEstoque(AdicionarEstoqueCommand command)
    {
        _validator.ValidateAndThrow(command);

        var produto = _produtoRepository.GetByIdAsync(command.ProdutoId).GetAwaiter().GetResult();

        if (produto is null)
            throw new KeyNotFoundException($"Produto {command.ProdutoId} não encontrado");

        var estoque = _estoqueRepository.GetByIdProdutoAsync(command.ProdutoId).GetAwaiter().GetResult();
        
        if (estoque is null)
            _estoqueRepository.AddAsync(new Domain.Entities.Estoque(Guid.NewGuid(), command.ProdutoId, command.Quantidade));
        else
        {
            estoque.AtualizarQuantidade(estoque.Quantidade + command.Quantidade);
            _estoqueRepository.UpdateAsync(estoque).GetAwaiter().GetResult();
        }

        return true;
    }
}