using FluentValidation;
using TechChallenge.Domain.Entities;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Veiculos.CriarVeiculo;

public class CriarVeiculoService
{
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IValidator<CriarVeiculoCommand> _validator;

    public CriarVeiculoService(
        IVeiculoRepository veiculoRepository,
        IClienteRepository clienteRepository,
        IValidator<CriarVeiculoCommand> validator)
    {
        _veiculoRepository = veiculoRepository;
        _clienteRepository = clienteRepository;
        _validator = validator;
    }

    public Guid CriarVeiculo(CriarVeiculoCommand command)
    {
        _validator.ValidateAndThrow(command);

        var placaExiste = _veiculoRepository.GetByPlacaAsync(command.Placa).GetAwaiter().GetResult();
        if (placaExiste is not null)
            throw new InvalidOperationException($"Já existe um veículo cadastrado com a placa {command.Placa}.");

        var cliente = _clienteRepository.GetByIdAsync(command.ClienteId).GetAwaiter().GetResult();
        if (cliente is null)
            throw new KeyNotFoundException($"Cliente com Id {command.ClienteId} não encontrado.");

        var veiculo = new Veiculo
        {
            Id = Guid.NewGuid(),
            Tipo = command.Tipo,
            Placa = command.Placa,
            Modelo = command.Modelo,
            Marca = command.Marca,
            Cor = command.Cor,
            Ano = command.Ano,
            Quilometragem = command.Quilometragem,
            Valor = command.Valor,
            ClienteId = command.ClienteId
        };

        _veiculoRepository.AddAsync(veiculo).GetAwaiter().GetResult();
        return veiculo.Id;
    }
}
