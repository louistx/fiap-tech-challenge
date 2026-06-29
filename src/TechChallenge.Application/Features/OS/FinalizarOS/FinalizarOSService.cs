using FluentValidation;
using TechChallenge.Application.Abstractions.Notifications;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Notifications;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Features.OS.FinalizarOS;

public class FinalizarOSService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IValidator<FinalizarOSCommand> _validator;
    private readonly INotificationService _notificationService;

    public FinalizarOSService(
        IOrdemServicoRepository ordemServicoRepository,
        IValidator<FinalizarOSCommand> validator,
        INotificationService? notificationService = null)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _validator = validator;
        _notificationService = notificationService ?? NullNotificationService.Instance;
    }

    public bool FinalizarOS(FinalizarOSCommand command)
    {
        _validator.ValidateAndThrow(command);

        var os = _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId).GetAwaiter().GetResult();
        if (os is null)
            throw new KeyNotFoundException($"OS com Id {command.OrdemServicoId} não encontrada.");

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

        foreach (var item in os.Produtos)
            item.Produto.Quantidade -= item.Quantidade;

        var statusAnterior = os.Status;

        os.TransicionarPara(StatusOS.Finalizada);
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
