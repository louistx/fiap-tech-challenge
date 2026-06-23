using FluentValidation;
using TechChallenge.Domain.Entities;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Inventario.CriarItemInventario;

public class CriarItemInventarioService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IValidator<CriarItemInventarioCommand> _validator;

    public CriarItemInventarioService(IProdutoRepository produtoRepository, IValidator<CriarItemInventarioCommand> validator)
    {
        _produtoRepository = produtoRepository;
        _validator = validator;
    }

    public Guid CriarItemInventario(CriarItemInventarioCommand command)
    {
        _validator.ValidateAndThrow(command);

        var produto = new Produto
        {
            Id = Guid.NewGuid(),
            Descricao = command.Descricao,
            Valor = command.Valor
        };

        _produtoRepository.AddAsync(produto).GetAwaiter().GetResult();
        return produto.Id;
    }
}
