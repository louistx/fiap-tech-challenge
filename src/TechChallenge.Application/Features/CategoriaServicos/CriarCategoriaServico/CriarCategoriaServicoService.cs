using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.CategoriaServicos.CriarCategoriaServico;

public class CriarCategoriaServicoService
{
    private readonly ICategoriaServicoRepository _categoriaServicoRepository;
    private readonly IValidator<CriarCategoriaServicoCommand> _validator;

    public CriarCategoriaServicoService(ICategoriaServicoRepository categoriaServicoRepository, IValidator<CriarCategoriaServicoCommand> validator)
    {
        _categoriaServicoRepository = categoriaServicoRepository;
        _validator = validator;
    }

    public Guid CriarCategoriaServico(CriarCategoriaServicoCommand command)
    {
        _validator.ValidateAndThrow(command);
        var descricao = command.Descricao.Trim();

        var categoriaServicoExiste = _categoriaServicoRepository.GetByDescricaoAsync(descricao).GetAwaiter().GetResult();
        if (categoriaServicoExiste is not null)
            throw new InvalidOperationException($"Já existe uma categoria de serviço cadastrada com a descrição {descricao}.");

        var categoriaServico = new Domain.Entities.CategoriaServico(Guid.NewGuid(), descricao);

        _categoriaServicoRepository.AddAsync(categoriaServico).GetAwaiter().GetResult();
        
        return categoriaServico.Id;
    }
}