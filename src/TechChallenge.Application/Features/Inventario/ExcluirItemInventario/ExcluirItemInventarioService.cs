using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Inventario.ExcluirItemInventario;

public class ExcluirItemInventarioService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IOrdemServicoProdutosRepository _ordemServicoProdutosRepository;
    private readonly IValidator<ExcluirItemInventarioCommand> _validator;

    public ExcluirItemInventarioService(
        IProdutoRepository produtoRepository,
        IOrdemServicoProdutosRepository ordemServicoProdutosRepository,
        IValidator<ExcluirItemInventarioCommand> validator)
    {
        _produtoRepository = produtoRepository;
        _ordemServicoProdutosRepository = ordemServicoProdutosRepository;
        _validator = validator;
    }

    public bool ExcluirItemInventario(ExcluirItemInventarioCommand command)
    {
        _validator.ValidateAndThrow(command);

        var produto = _produtoRepository.GetByIdAsync(command.Id).GetAwaiter().GetResult();
        if (produto is null)
            throw new KeyNotFoundException($"Item de inventário com Id {command.Id} não encontrado.");

        var produtoEmUso = _ordemServicoProdutosRepository.ExisteProdutoEmOrdemServicoAsync(command.Id).GetAwaiter().GetResult();
        if (produtoEmUso)
            throw new InvalidOperationException("Não é possível excluir um produto associado a uma ordem de serviço.");

        _produtoRepository.DeleteAsync(produto).GetAwaiter().GetResult();
        return true;
    }
}
