using TechChallenge.Api.Models.Request;
using TechChallenge.Api.Models.Response;
using TechChallenge.Application.Features.Veiculos.AtualizarVeiculo;
using TechChallenge.Application.Features.Veiculos.CriarVeiculo;
using TechChallenge.Application.Features.Veiculos.ExcluirVeiculo;
using TechChallenge.Application.Features.Veiculos.ListarVeiculos;
using TechChallenge.Application.Features.Veiculos.ObterVeiculo;

namespace TechChallenge.Api.Endpoints;

public static class VeiculosEndpoints
{
    public static IEndpointRouteBuilder MapVeiculosEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/veiculos");

        group.MapPost("/", CriarVeiculoAsync)
            .WithName("CriarVeiculo")
            .RequireAuthorization("AdminOuVendedor")
            .WithSummary("Cria um novo veículo")
            .WithDescription("Adiciona um novo veículo ao banco de dados")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapGet("/{id}", ObterVeiculoAsync)
            .WithName("ObterVeiculo")
            .WithSummary("Obtém um veículo existente")
            .WithDescription("Obtém as informações de um veículo existente do banco de dados")
            .Produces<VeiculoResponse>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", ListarVeiculosAsync)
            .WithName("ListarVeiculos")
            .WithSummary("Lista todos os veículos")
            .WithDescription("Lista todos os veículos do banco de dados")
            .Produces<List<VeiculoResponse>>();

        group.MapPut("/{id}", AtualizarVeiculoAsync)
            .WithName("AtualizarVeiculo")
            .RequireAuthorization("AdminOuVendedor")
            .WithSummary("Atualiza um veículo existente")
            .WithDescription("Atualiza as informações de um veículo existente no banco de dados")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapDelete("/{id}", ExcluirVeiculoAsync)
            .WithName("ExcluirVeiculo")
            .RequireAuthorization("AdminOuVendedor")
            .WithSummary("Exclui um veículo existente")
            .WithDescription("Exclui um veículo existente do banco de dados")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static IResult CriarVeiculoAsync(CriarVeiculoRequest request, CriarVeiculoService service)
    {
        var command = new CriarVeiculoCommand
        {
            Tipo = request.Tipo,
            Placa = request.Placa,
            Modelo = request.Modelo,
            Marca = request.Marca,
            Cor = request.Cor,
            Ano = request.Ano,
            Quilometragem = request.Quilometragem,
            Valor = request.Valor,
            ClienteId = request.ClienteId
        };

        var id = service.CriarVeiculo(command);
        return Results.Created($"/api/v1/veiculos/{id}", id);
    }

    private static IResult ObterVeiculoAsync(Guid id, ObterVeiculoService service)
    {
        var veiculo = service.ObterVeiculo(new ObterVeiculoQuery { Id = id });

        var response = new VeiculoResponse
        {
            Id = veiculo.Id,
            Tipo = veiculo.Tipo,
            Placa = veiculo.Placa,
            Modelo = veiculo.Modelo,
            Marca = veiculo.Marca,
            Cor = veiculo.Cor,
            Ano = veiculo.Ano,
            Quilometragem = veiculo.Quilometragem,
            Valor = veiculo.Valor,
            ClienteId = veiculo.ClienteId
        };

        return Results.Ok(response);
    }

    private static IResult ListarVeiculosAsync(ListarVeiculosService service)
    {
        var veiculos = service.ListarVeiculos(new ListarVeiculosQuery());

        var response = veiculos.Select(v => new VeiculoResponse
        {
            Id = v.Id,
            Tipo = v.Tipo,
            Placa = v.Placa,
            Modelo = v.Modelo,
            Marca = v.Marca,
            Cor = v.Cor,
            Ano = v.Ano,
            Quilometragem = v.Quilometragem,
            Valor = v.Valor,
            ClienteId = v.ClienteId
        }).ToList();

        return Results.Ok(response);
    }

    private static IResult AtualizarVeiculoAsync(Guid id, AtualizarVeiculoRequest request, AtualizarVeiculoService service)
    {
        var command = new AtualizarVeiculoCommand
        {
            Id = id,
            Tipo = request.Tipo,
            Placa = request.Placa,
            Modelo = request.Modelo,
            Marca = request.Marca,
            Cor = request.Cor,
            Ano = request.Ano,
            Quilometragem = request.Quilometragem,
            Valor = request.Valor,
            ClienteId = request.ClienteId
        };

        service.AtualizarVeiculo(command);
        return Results.Ok();
    }

    private static IResult ExcluirVeiculoAsync(Guid id, ExcluirVeiculoService service)
    {
        service.ExcluirVeiculo(new ExcluirVeiculoCommand { Id = id });
        return Results.NoContent();
    }
}
