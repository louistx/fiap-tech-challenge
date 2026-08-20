using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Validation;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Features.Estoque.BaixarEstoque;

public class BaixarEstoqueService
{
    private readonly IEstoqueRepository _estoqueRepository;
    private readonly IValidator<BaixarEstoqueCommand> _validator;

    public BaixarEstoqueService(IEstoqueRepository estoqueRepository, IValidator<BaixarEstoqueCommand> validator)
    {
        _estoqueRepository = estoqueRepository;
        _validator = validator;
    }

    public bool BaixarEstoque(BaixarEstoqueCommand command)
    {
        _validator.ValidateAndThrow(command);

        var estoque = _estoqueRepository.GetByIdAsync(command.ProdutoId).GetAwaiter().GetResult();

        if (estoque is null)
            throw new KeyNotFoundException($"Estoque com Id {command.ProdutoId} não encontrado.");

        estoque.AtualizarQuantidade(estoque.Quantidade - command.Quantidade);

        _estoqueRepository.UpdateAsync(estoque).GetAwaiter().GetResult();

        return true;
    }
}