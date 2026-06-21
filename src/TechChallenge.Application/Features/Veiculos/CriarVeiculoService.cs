using TechChallenge.Domain.Entities;
using TechChallenge.Infrastructure.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Veiculos;

public class CriarVeiculoService
{
    private readonly IVeiculoRepository _veiculoRepository;

    public CriarVeiculoService(IVeiculoRepository veiculoRepository)
    {
        _veiculoRepository = veiculoRepository;
    }

    public Guid CriarVeiculo(CriarVeiculoCommand command)
    {
        var placaExiste = _veiculoRepository.GetByPlacaAsync(command.Placa).GetAwaiter().GetResult();
        if (placaExiste is not null)
            throw new InvalidOperationException($"Já existe um veículo cadastrado com a placa {command.Placa}.");

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