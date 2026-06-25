using System;
using FluentValidation;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.OS.CriarOS;

public class CriarOSService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IFuncionarioRepository _funcionarioRepository;
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IValidator<CriarOSCommand> _validator;

    public CriarOSService(
        IOrdemServicoRepository ordemServicoRepository,
        IClienteRepository clienteRepository,
        IFuncionarioRepository funcionarioRepository,
        IVeiculoRepository veiculoRepository,
        IValidator<CriarOSCommand> validator)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _clienteRepository = clienteRepository;
        _funcionarioRepository = funcionarioRepository;
        _veiculoRepository = veiculoRepository;
        _validator = validator;
    }

    public Guid CriarOS(CriarOSCommand command)
    {
        _validator.ValidateAndThrow(command);

        var cliente = _clienteRepository.GetByIdAsync(command.ClienteResponsavelId).GetAwaiter().GetResult();
        if (cliente is null)
            throw new KeyNotFoundException($"Cliente com Id {command.ClienteResponsavelId} não encontrado.");

        var funcionario = _funcionarioRepository.GetByIdAsync(command.FuncionarioResponsavelId).GetAwaiter().GetResult();
        if (funcionario is null)
            throw new KeyNotFoundException($"Funcionário com Id {command.FuncionarioResponsavelId} não encontrado.");

        var veiculo = _veiculoRepository.GetByIdAsync(command.VeiculoId).GetAwaiter().GetResult();
        if (veiculo is null)
            throw new KeyNotFoundException($"Veículo com Id {command.VeiculoId} não encontrado.");

        var os = new OrdemServico
        {
            Id = Guid.NewGuid(),
            Descricao = command.Descricao,
            Status = eStatusOS.Recebida,
            ClienteResponsavelId = command.ClienteResponsavelId,
            FuncionarioResponsavelId = command.FuncionarioResponsavelId,
            VeiculoId = command.VeiculoId,
            DataCriacao = DateTime.UtcNow
        };

        _ordemServicoRepository.AddAsync(os).GetAwaiter().GetResult();
        return os.Id;
    }
}
