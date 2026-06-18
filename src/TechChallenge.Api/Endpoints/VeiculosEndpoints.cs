using TechChallenge.Api.Models.Request;
using TechChallenge.Api.Models.Response;

namespace TechChallenge.Api.Endpoints;

public static class VeiculosEndpoints
{
    public static IEndpointRouteBuilder MapVeiculosEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/veiculos");

        group.MapPost("/", CriarVeiculoAsync)
            .WithName("CriarVeiculo")
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
            .WithSummary("Atualiza um veículo existente")
            .WithDescription("Atualiza as informações de um veículo existente no banco de dados")
            .ProducesValidationProblem();

        group.MapDelete("/{id}", ExcluirVeiculoAsync)
            .WithName("ExcluirVeiculo")
            .WithSummary("Exclui um veículo existente")
            .WithDescription("Exclui um veículo existente do banco de dados")
            .ProducesValidationProblem();

        return app;
    }

    private static IResult CriarVeiculoAsync(CriarVeiculoRequest request)
    {
        return Results.Created($"/api/v1/veiculos/{Guid.NewGuid()}", Guid.NewGuid());
    }

    private static IResult ObterVeiculoAsync(Guid id)
    {
        var veiculo = new VeiculoResponse
        {
            Id = id,
            Placa = "ABC1234",
            Modelo = "Modelo",
            Cor = "Cor",
            Marca = "Marca",
            Valor = 0
        };

        return Results.Ok(veiculo);
    }

    private static IResult ListarVeiculosAsync()
    {
        return Results.Ok(new List<VeiculoResponse>());
    }

    private static IResult AtualizarVeiculoAsync(Guid id, AtualizarVeiculoRequest request)
    {
        return Results.Ok();
    }

    private static IResult ExcluirVeiculoAsync(Guid id)
    {
        return Results.Ok();
    }
}
