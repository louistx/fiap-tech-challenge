using System;
using FluentValidation;
using TechChallenge.Domain.Entities;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Validation;

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
        var placa = PlacaValidator.Formatar(command.Placa);

        var placaExiste = _veiculoRepository.GetByPlacaAsync(placa).GetAwaiter().GetResult();
        if (placaExiste is not null)
            throw new InvalidOperationException($"Já existe um veículo cadastrado com a placa {placa}.");

        var cliente = _clienteRepository.GetByIdAsync(command.ClienteId).GetAwaiter().GetResult();
        if (cliente is null)
            throw new KeyNotFoundException($"Cliente com Id {command.ClienteId} não encontrado.");

        var veiculo = new Veiculo(Guid.NewGuid(), placa, command.Modelo, command.Marca, command.Cor, command.Ano, command.Quilometragem, command.Valor, command.ClienteId, command.CategoriaId);

        _veiculoRepository.AddAsync(veiculo).GetAwaiter().GetResult();
        return veiculo.Id;
    }
}
