using System;
using FluentValidation;
using TechChallenge.Application.Abstractions.Notifications;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Notifications;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Features.OS.RegistrarDiagnostico;

public class RegistrarDiagnosticoService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IServicoRepository _servicoRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IValidator<RegistrarDiagnosticoCommand> _validator;
    private readonly INotificationService _notificationService;

    public RegistrarDiagnosticoService(
        IOrdemServicoRepository ordemServicoRepository,
        IServicoRepository servicoRepository,
        IProdutoRepository produtoRepository,
        IValidator<RegistrarDiagnosticoCommand> validator,
        INotificationService? notificationService = null)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _servicoRepository = servicoRepository;
        _produtoRepository = produtoRepository;
        _validator = validator;
        _notificationService = notificationService ?? NullNotificationService.Instance;
    }

    // RF11: registra serviços e produtos na OS
    public bool RegistrarDiagnostico(RegistrarDiagnosticoCommand command)
    {
        _validator.ValidateAndThrow(command);

        var os = _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId).GetAwaiter().GetResult();
        if (os is null)
            throw new KeyNotFoundException($"OS com Id {command.OrdemServicoId} não encontrada.");

        if (os.Status != StatusOS.EmDiagnostico)
            throw new InvalidOperationException($"Apenas OS com status Em Diagnóstico aceitam registro. Status atual: {os.Status}.");

        foreach (var item in command.Servicos)
        {
            var servico = _servicoRepository.GetByIdAsync(item.Id).GetAwaiter().GetResult();
            if (servico is null)
                throw new KeyNotFoundException($"Serviço com Id {item.Id} não encontrado.");

            os.Servicos.Add(new OrdemServicoServicos
            {
                ServicoId = servico.Id,
                Servico = servico,
                Valor = (double)servico.Valor,
                Quantidade = item.Quantidade
            });
        }

        foreach (var item in command.Produtos)
        {
            var produto = _produtoRepository.GetByIdAsync(item.Id).GetAwaiter().GetResult();
            if (produto is null)
                throw new KeyNotFoundException($"Produto com Id {item.Id} não encontrado.");

            if (produto.Quantidade < item.Quantidade)
            {
                NotificarEstoqueInsuficiente(os, produto.Descricao, produto.Quantidade, item.Quantidade);
                throw new InvalidOperationException($"Estoque insuficiente para o produto {produto.Descricao}.");
            }

            os.Produtos.Add(new OrdemServicoProdutos
            {
                ProdutoId = produto.Id,
                Produto = produto,
                Valor = (double)produto.Valor,
                Quantidade = item.Quantidade
            });
        }

        os.DataAtualizacao = DateTime.UtcNow;

        _ordemServicoRepository.UpdateAsync(os).GetAwaiter().GetResult();
        return true;
    }

    private void NotificarEstoqueInsuficiente(OrdemServico os, string produtoDescricao, int quantidadeDisponivel, int quantidadeNecessaria)
    {
        var mensagem = $"OS {os.Id} precisa de {quantidadeNecessaria} unidade(s) de {produtoDescricao}, mas há {quantidadeDisponivel} em estoque.";

        _notificationService.NotificarFuncionariosPorFuncao(
            TipoFuncionario.Administrador,
            "Estoque insuficiente",
            mensagem);

        _notificationService.NotificarFuncionario(
            os.FuncionarioResponsavelId,
            "Estoque insuficiente",
            mensagem);
    }
}
