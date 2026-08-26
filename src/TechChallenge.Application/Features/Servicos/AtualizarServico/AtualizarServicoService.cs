using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Servicos.AtualizarServico;

public class AtualizarServicoService
{
    private readonly IServicoRepository _servicoRepository;
    private readonly IValidator<AtualizarServicoCommand> _validator;

    public AtualizarServicoService(IServicoRepository servicoRepository, IValidator<AtualizarServicoCommand> validator)
    {
        _servicoRepository = servicoRepository;
        _validator = validator;
    }

    public bool AtualizarServico(AtualizarServicoCommand command)
    {
        _validator.ValidateAndThrow(command);

        var servico = _servicoRepository.GetByIdAsync(command.Id).GetAwaiter().GetResult();
        if (servico is null)
            throw new KeyNotFoundException($"Serviço com Id {command.Id} não encontrado.");

        servico.Atualizar(command.Descricao, command.Valor);

        _servicoRepository.UpdateAsync(servico).GetAwaiter().GetResult();
        return true;
    }
}
