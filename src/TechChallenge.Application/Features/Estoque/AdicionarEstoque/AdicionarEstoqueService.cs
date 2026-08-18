using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Estoque.AdicionarEstoque;

public class AdicionarEstoqueService
{
    private readonly IEstoqueRepository _estoqueRepository;
    private readonly IValidator<AdicionarEstoqueCommand> _validator;

    public AdicionarEstoqueService(IEstoqueRepository estoqueRepository, IValidator<AdicionarEstoqueCommand> validator)
    {
        _estoqueRepository = estoqueRepository;
        _validator = validator;
    }

    public bool AdicionarEstoque(AdicionarEstoqueCommand command)
    {
        _validator.ValidateAndThrow(command);

        var estoque = _estoqueRepository.GetByIdAsync(command.ProdutoId).GetAwaiter().GetResult();
        if (estoque is null)
            throw new KeyNotFoundException($"Estoque com Id {command.ProdutoId} não encontrado.");

        estoque.AtualizarQuantidade(estoque.Quantidade + command.Quantidade);

        _estoqueRepository.UpdateAsync(estoque).GetAwaiter().GetResult();
        return true;
    }
}