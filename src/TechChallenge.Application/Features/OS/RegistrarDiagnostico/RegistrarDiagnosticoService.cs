using FluentValidation;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.OS.RegistrarDiagnostico;

public class RegistrarDiagnosticoService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IServicoRepository _servicoRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IValidator<RegistrarDiagnosticoCommand> _validator;

    public RegistrarDiagnosticoService(
        IOrdemServicoRepository ordemServicoRepository,
        IServicoRepository servicoRepository,
        IProdutoRepository produtoRepository,
        IValidator<RegistrarDiagnosticoCommand> validator)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _servicoRepository = servicoRepository;
        _produtoRepository = produtoRepository;
        _validator = validator;
    }

    // RF11: registra serviços e produtos na OS
    public bool RegistrarDiagnostico(RegistrarDiagnosticoCommand command)
    {
        _validator.ValidateAndThrow(command);

        var os = _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId).GetAwaiter().GetResult();
        if (os is null)
            throw new KeyNotFoundException($"OS com Id {command.OrdemServicoId} não encontrada.");

        if (os.Status != eStatusOS.EmDiagnostico)
            throw new InvalidOperationException($"Apenas OS com status Em Diagnóstico aceitam registro. Status atual: {os.Status}.");

        foreach (var servicoId in command.ServicosIds)
        {
            var servico = _servicoRepository.GetByIdAsync(servicoId).GetAwaiter().GetResult();
            if (servico is null)
                throw new KeyNotFoundException($"Serviço com Id {servicoId} não encontrado.");

            os.Servicos.Add(servico);
        }

        foreach (var produtoId in command.ProdutosIds)
        {
            var produto = _produtoRepository.GetByIdAsync(produtoId).GetAwaiter().GetResult();
            if (produto is null)
                throw new KeyNotFoundException($"Produto com Id {produtoId} não encontrado.");

            os.Produtos.Add(produto);
        }

        os.DataAtualizacao = DateTime.UtcNow;

        _ordemServicoRepository.UpdateAsync(os).GetAwaiter().GetResult();
        return true;
    }
}
