using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Inventario.AtualizarItemInventario;

public class AtualizarItemInventarioService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IValidator<AtualizarItemInventarioCommand> _validator;

    public AtualizarItemInventarioService(
        IProdutoRepository produtoRepository,
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

        produto.Descricao = command.Descricao;
        produto.Valor = command.Valor;

        _produtoRepository.UpdateAsync(produto).GetAwaiter().GetResult();
        return true;
    }
}
