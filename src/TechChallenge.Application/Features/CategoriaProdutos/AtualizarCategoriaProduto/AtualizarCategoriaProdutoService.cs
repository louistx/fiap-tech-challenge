using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.CategoriaProdutos.AtualizarCategoriaProduto;

public class AtualizarCategoriaProdutoService
{
    private readonly ICategoriaProdutoRepository _categoriaProdutoRepository;
    private readonly IValidator<AtualizarCategoriaProdutoCommand> _validator;

    public AtualizarCategoriaProdutoService(ICategoriaProdutoRepository categoriaProdutoRepository, IValidator<AtualizarCategoriaProdutoCommand> validator)
    {
        _categoriaProdutoRepository = categoriaProdutoRepository;
        _validator = validator;
    }

    public bool AtualizarCategoriaProduto(AtualizarCategoriaProdutoCommand command)
    {
        _validator.ValidateAndThrow(command);

        var categoriaProduto = _categoriaProdutoRepository.GetByIdAsync(command.Id).GetAwaiter().GetResult();

        if (categoriaProduto is null)
            throw new KeyNotFoundException($"Categoria de Produto com Id {command.Id} não encontrada.");
        
        categoriaProduto = new Domain.Entities.CategoriaProduto(
            categoriaProduto.Id,
            command.Descricao
        );

        _categoriaProdutoRepository.UpdateAsync(categoriaProduto).GetAwaiter().GetResult();
        return true;
    }
}