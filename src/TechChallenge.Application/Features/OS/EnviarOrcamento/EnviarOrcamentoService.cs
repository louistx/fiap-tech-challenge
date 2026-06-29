using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Features.OS.EnviarOrcamento;

public class EnviarOrcamentoService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IValidator<EnviarOrcamentoCommand> _validator;

    public EnviarOrcamentoService(
        IOrdemServicoRepository ordemServicoRepository,
        IValidator<EnviarOrcamentoCommand> validator)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _validator = validator;
    }

    public bool EnviarOrcamento(EnviarOrcamentoCommand command)
    {
        _validator.ValidateAndThrow(command);

        var os = _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId).GetAwaiter().GetResult();
        if (os is null)
            throw new KeyNotFoundException($"OS com Id {command.OrdemServicoId} não encontrada.");

        if (os.Servicos.Count == 0 && os.Produtos.Count == 0)
            throw new InvalidOperationException("Informe ao menos um serviço ou produto antes de enviar o orçamento.");

        os.Valor =
            os.Servicos.Sum(item => item.Valor + item.Acrescimo - item.Desconto) +
            os.Produtos.Sum(item => item.Valor + item.Acrescimo - item.Desconto) +
            os.Acrescimo -
            os.Desconto;

        os.TransicionarPara(StatusOS.AguardandoAprovacao);
        _ordemServicoRepository.UpdateAsync(os).GetAwaiter().GetResult();
        return true;
    }
}
