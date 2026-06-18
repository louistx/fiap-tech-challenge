using TechChallenge.Api.Models.Request;
using TechChallenge.Api.Models.Response;

namespace TechChallenge.Api.Endpoints;

public static class ServicosEndpoints
{
    public static IEndpointRouteBuilder MapServicosEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/servicos");

        group.MapPost("/", CriarServicoAsync)
            .WithName("CriarServico")
            .WithSummary("Cria um novo serviço")
            .WithDescription("Adiciona um novo serviço ao banco de dados")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapGet("/{id}", ObterServicoAsync)
            .WithName("ObterServico")
            .WithSummary("Obtém um serviço existente")
            .WithDescription("Obtém as informações de um serviço existente do banco de dados")
            .Produces<ServicoResponse>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", ListarServicosAsync)
            .WithName("ListarServicos")
            .WithSummary("Lista todos os serviços")
            .WithDescription("Lista todos os serviços do banco de dados")
            .Produces<List<ServicoResponse>>();

        group.MapPut("/{id}", AtualizarServicoAsync)
            .WithName("AtualizarServico")
            .WithSummary("Atualiza um serviço existente")
            .WithDescription("Atualiza as informações de um serviço existente no banco de dados")
            .ProducesValidationProblem();

        group.MapDelete("/{id}", ExcluirServicoAsync)
            .WithName("ExcluirServico")
            .WithSummary("Exclui um serviço existente")
            .WithDescription("Exclui um serviço existente do banco de dados")
            .ProducesValidationProblem();

        return app;
    }

    private static IResult CriarServicoAsync(CriarServicoRequest request)
    {
        return Results.Created($"/api/v1/servicos/{Guid.NewGuid()}", Guid.NewGuid());
    }

    private static IResult ObterServicoAsync(Guid id)
    {
        var servico = new ServicoResponse
        {
            Id = id,
            Descricao = "Descrição do Serviço",
            Valor = 0
        };

        return Results.Ok(servico);
    }

    private static IResult ListarServicosAsync()
    {
        return Results.Ok(new List<ServicoResponse>());
    }

    private static IResult AtualizarServicoAsync(Guid id, AtualizarServicoRequest request)
    {
        return Results.Ok();
    }

    private static IResult ExcluirServicoAsync(Guid id)
    {
        return Results.Ok();
    }
}
