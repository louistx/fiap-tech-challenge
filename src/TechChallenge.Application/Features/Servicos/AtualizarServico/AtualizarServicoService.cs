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

        servico = new Domain.Entities.Servico(servico.Id, command.Descricao, command.Valor, command.CategoriaId);

        _servicoRepository.UpdateAsync(servico).GetAwaiter().GetResult();
        return true;
    }
}
