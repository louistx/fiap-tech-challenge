using FluentValidation;
using TechChallenge.Application.Abstractions.Notifications;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Notifications;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Features.OS.EnviarOrcamento;

public class EnviarOrcamentoService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IValidator<EnviarOrcamentoCommand> _validator;
    private readonly INotificationService _notificationService;

    public EnviarOrcamentoService(
        IOrdemServicoRepository ordemServicoRepository,
        IValidator<EnviarOrcamentoCommand> validator,
        INotificationService? notificationService = null)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _validator = validator;
        _notificationService = notificationService ?? NullNotificationService.Instance;
    }

    public bool EnviarOrcamento(EnviarOrcamentoCommand command)
    {
        _validator.ValidateAndThrow(command);

        var os = _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId).GetAwaiter().GetResult();
        if (os is null)
            throw new KeyNotFoundException($"OS com Id {command.OrdemServicoId} não encontrada.");

        if (os.Servicos.Count == 0 && os.Produtos.Count == 0)
            throw new InvalidOperationException("Informe ao menos um serviço ou produto antes de enviar o orçamento.");

        foreach (var item in os.Produtos)
        {
            if (item.Produto.Quantidade < item.Quantidade)
            {
                NotificarEstoqueInsuficiente(
                    os.Id,
                    os.FuncionarioResponsavelId,
                    item.Produto.Descricao,
                    item.Produto.Quantidade,
                    item.Quantidade);

                throw new InvalidOperationException($"Estoque insuficiente para o produto {item.Produto.Descricao}.");
            }
        }

        os.Valor =
            os.Servicos.Sum(item => (item.Valor * item.Quantidade) + item.Acrescimo - item.Desconto) +
            os.Produtos.Sum(item => (item.Valor * item.Quantidade) + item.Acrescimo - item.Desconto) +
            os.Acrescimo -
            os.Desconto;

        var statusAnterior = os.Status;

        os.TransicionarPara(StatusOS.AguardandoAprovacao);
        _notificationService.NotificarTransicaoOS(os, statusAnterior);
        _ordemServicoRepository.UpdateAsync(os).GetAwaiter().GetResult();
        return true;
    }

    private void NotificarEstoqueInsuficiente(Guid ordemServicoId, Guid funcionarioResponsavelId, string produtoDescricao, int quantidadeDisponivel, int quantidadeNecessaria)
    {
        var mensagem = $"OS {ordemServicoId} precisa de {quantidadeNecessaria} unidade(s) de {produtoDescricao}, mas há {quantidadeDisponivel} em estoque.";

        _notificationService.NotificarFuncionariosPorFuncao(
            TipoFuncionario.Administrador,
            "Estoque insuficiente",
            mensagem);

        _notificationService.NotificarFuncionario(
            funcionarioResponsavelId,
            "Estoque insuficiente",
            mensagem);
    }
}
