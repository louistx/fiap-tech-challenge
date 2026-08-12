using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Inventario.AtualizarItemInventario;

public class AtualizarItemInventarioService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IEstoqueRepository _estoqueRepository;
    private readonly IValidator<AtualizarItemInventarioCommand> _validator;

    public AtualizarItemInventarioService(
        IProdutoRepository produtoRepository,
        IEstoqueRepository estoqueRepository,
        IValidator<AtualizarItemInventarioCommand> validator)
    {
        _produtoRepository = produtoRepository;
        _validator = validator;
    }

    public bool AtualizarItemInventario(AtualizarItemInventarioCommand command)
    {
        _validator.ValidateAndThrow(command);

        var produto = _produtoRepository.GetByIdAsync(command.Id).GetAwaiter().GetResult();
        if (produto is null)
            throw new KeyNotFoundException($"Item de inventário com Id {command.Id} não encontrado.");

        produto = new Domain.Entities.Produto(produto.Id, command.Descricao, command.Valor, produto.CategoriaId);

        var estoque = _estoqueRepository.GetByIdProdutoAsync(command.Id).GetAwaiter().GetResult();

        if (estoque is null)
            throw new KeyNotFoundException($"Estoque para o item de inventário com Id {command.Id} não encontrado.");

        _produtoRepository.UpdateAsync(produto).GetAwaiter().GetResult();
        return true;
    }
}
