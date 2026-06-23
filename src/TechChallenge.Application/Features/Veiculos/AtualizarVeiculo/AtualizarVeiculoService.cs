using FluentValidation;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Veiculos.AtualizarVeiculo;

public class AtualizarVeiculoService
{
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IValidator<AtualizarVeiculoCommand> _validator;

    public AtualizarVeiculoService(
        IVeiculoRepository veiculoRepository,
        IClienteRepository clienteRepository,
        IValidator<AtualizarVeiculoCommand> validator)
    {
        _veiculoRepository = veiculoRepository;
        _clienteRepository = clienteRepository;
        _validator = validator;
    }

    public bool AtualizarVeiculo(AtualizarVeiculoCommand command)
    {
        _validator.ValidateAndThrow(command);

        var veiculo = _veiculoRepository.GetByIdAsync(command.Id).GetAwaiter().GetResult();
        if (veiculo is null)
            throw new KeyNotFoundException($"Veículo com Id {command.Id} não encontrado.");

        var placaExiste = _veiculoRepository.GetByPlacaAsync(command.Placa).GetAwaiter().GetResult();
        if (placaExiste is not null && placaExiste.Id != command.Id)
            throw new InvalidOperationException($"Já existe outro veículo cadastrado com a placa {command.Placa}.");

        var cliente = _clienteRepository.GetByIdAsync(command.ClienteId).GetAwaiter().GetResult();
        if (cliente is null)
            throw new KeyNotFoundException($"Cliente com Id {command.ClienteId} não encontrado.");

        veiculo.Tipo = command.Tipo;
        veiculo.Placa = command.Placa;
        veiculo.Modelo = command.Modelo;
        veiculo.Marca = command.Marca;
        veiculo.Cor = command.Cor;
        veiculo.Ano = command.Ano;
        veiculo.Quilometragem = command.Quilometragem;
        veiculo.Valor = command.Valor;
        veiculo.ClienteId = command.ClienteId;

        _veiculoRepository.UpdateAsync(veiculo).GetAwaiter().GetResult();
        return true;
    }
}
