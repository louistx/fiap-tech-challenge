using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.CategoriaProdutos.ExcluirCategoriaProduto;

public class ExcluirCategoriaProdutoService
{
    private readonly ICategoriaProdutoRepository _categoriaProdutoRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IValidator<ExcluirCategoriaProdutoCommand> _validator;

    public ExcluirCategoriaProdutoService(
        ICategoriaProdutoRepository categoriaProdutoRepository,
        IProdutoRepository produtoRepository,
        IValidator<ExcluirCategoriaProdutoCommand> validator)
    {
        _categoriaProdutoRepository = categoriaProdutoRepository;
        _produtoRepository = produtoRepository;
        _validator = validator;
    }

    public bool ExcluirCategoriaProduto(ExcluirCategoriaProdutoCommand command)
    {
        _validator.ValidateAndThrow(command);

        var categoriaProduto = _categoriaProdutoRepository.GetByIdAsync(command.Id).GetAwaiter().GetResult();

        if (categoriaProduto is null)
            throw new KeyNotFoundException($"Categoria de produto com Id {command.Id} não encontrada.");

        var categoriaPossuiProduto = _produtoRepository.ExisteProdutoComCategoria(command.Id).GetAwaiter().GetResult();
        
        if (categoriaPossuiProduto)
            throw new InvalidOperationException("Não é possível excluir uma categoria de produto associada a um produto.");

        _categoriaProdutoRepository.DeleteAsync(categoriaProduto).GetAwaiter().GetResult();

        return true;
    }
}