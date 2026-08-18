using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Domain.Entities;

namespace TechChallenge.Application.Features.CategoriaProdutos.CriarCategoriaProduto;

public class CriarCategoriaProdutoService
{
    private readonly ICategoriaProdutoRepository _categoriaProdutoRepository;
    private readonly IValidator<CriarCategoriaProdutoCommand> _validator;

    public CriarCategoriaProdutoService(ICategoriaProdutoRepository categoriaProdutoRepository, IValidator<CriarCategoriaProdutoCommand> validator)
    {
        _categoriaProdutoRepository = categoriaProdutoRepository;
        _validator = validator;
    }

    public Guid CriarCategoriaProduto(CriarCategoriaProdutoCommand command)
    {
        _validator.ValidateAndThrow(command);
        var descricao = command.Descricao.Trim();

        var categoriaProdutoExiste = _categoriaProdutoRepository.GetByDescricaoAsync(descricao).GetAwaiter().GetResult();
        
        if (categoriaProdutoExiste is not null)
            throw new InvalidOperationException($"Já existe uma categoria de produto cadastrada com a descrição {descricao}.");

        var categoriaProduto = new CategoriaProduto(Guid.NewGuid(), command.Descricao);

        _categoriaProdutoRepository.AddAsync(categoriaProduto).GetAwaiter().GetResult();

        return categoriaProduto.Id;
    }
}