using FluentValidation;
using TechChallenge.Application.Abstractions.Notifications;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Notifications;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Features.OS.FinalizarOS;

public class FinalizarOSService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IEstoqueRepository _estoqueRepository;
    private readonly IValidator<FinalizarOSCommand> _validator;
    private readonly INotificationService _notificationService;

    public FinalizarOSService(
        IOrdemServicoRepository ordemServicoRepository,
        IEstoqueRepository estoqueRepository,
        IValidator<FinalizarOSCommand> validator,
        INotificationService? notificationService = null)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _estoqueRepository = estoqueRepository;
        _validator = validator;
        _notificationService = notificationService ?? NullNotificationService.Instance;
    }

    public bool FinalizarOS(FinalizarOSCommand command)
    {
        List<Estoque> estoqueEntity = new List<Estoque>();

        _validator.ValidateAndThrow(command);

        var os = _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId).GetAwaiter().GetResult();
        if (os is null)
            throw new KeyNotFoundException($"OS com Id {command.OrdemServicoId} não encontrada.");

        foreach (var item in os.Produtos)
        {
            var estoque = _estoqueRepository.GetByIdProdutoAsync(item.ProdutoId).GetAwaiter().GetResult();

            if (estoque is null)
            {
                throw new InvalidOperationException($"Estoque não encontrado para o produto {item.Produto.Descricao}.");
            }
            else if (estoque.Quantidade < item.Quantidade)
            {
                NotificarEstoqueInsuficiente(
                    os.Id,
                    os.FuncionarioResponsavelId,
                    item.Produto.Descricao,
                    estoque.Quantidade,
                    item.Quantidade);

                throw new InvalidOperationException($"Estoque insuficiente para o produto {item.Produto.Descricao}.");
            }

            estoqueEntity.Add(estoque);
        }            

        foreach (var item in os.Produtos)
        {
            var estoque = estoqueEntity.FirstOrDefault(e => e.IdProduto == item.ProdutoId);
            
            estoque.AtualizarQuantidade(estoque.Quantidade - item.Quantidade);
            _estoqueRepository.UpdateAsync(estoque).GetAwaiter().GetResult();
        }

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
