using TechChallenge.Infrastructure.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Veiculos;

public class AtualizarVeiculoService
{
    private readonly IVeiculoRepository _veiculoRepository;

    public AtualizarVeiculoService(IVeiculoRepository veiculoRepository)
    {
        _veiculoRepository = veiculoRepository;
    }

    public bool AtualizarVeiculo(AtualizarVeiculoCommand command)
    {
        var veiculo = _veiculoRepository.GetByIdAsync(command.Id).GetAwaiter().GetResult();
        if (veiculo is null)
            throw new KeyNotFoundException($"Veículo com Id {command.Id} não encontrado.");

        var placaExiste = _veiculoRepository.GetByPlacaAsync(command.Placa).GetAwaiter().GetResult();
        if (placaExiste is not null && placaExiste.Id != command.Id)
            throw new InvalidOperationException($"Já existe outro veículo cadastrado com a placa {command.Placa}.");

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
