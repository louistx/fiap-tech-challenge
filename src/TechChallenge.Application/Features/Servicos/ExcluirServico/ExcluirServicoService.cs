using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Servicos.ExcluirServico;

public class ExcluirServicoService
{
    private readonly IServicoRepository _servicoRepository;
    private readonly IValidator<ExcluirServicoCommand> _validator;

    public ExcluirServicoService(IServicoRepository servicoRepository, IValidator<ExcluirServicoCommand> validator)
    {
        _servicoRepository = servicoRepository;
        _validator = validator;
    }

    public bool ExcluirServico(ExcluirServicoCommand command)
    {
        _validator.ValidateAndThrow(command);

        var servico = _servicoRepository.GetByIdAsync(command.Id).GetAwaiter().GetResult();
        if (servico is null)
            throw new KeyNotFoundException($"Serviço com Id {command.Id} não encontrado.");

        _servicoRepository.DeleteAsync(servico).GetAwaiter().GetResult();
        return true;
    }
}
