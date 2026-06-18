using TechChallenge.Api.Models.Request;
using TechChallenge.Api.Models.Response;

namespace TechChallenge.Api.Endpoints;

public static class OrdensServicoEndpoints
{
    public static IEndpointRouteBuilder MapOrdensServicoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/ordens-servico");

        group.MapPost("/", CriarOrdemServicoAsync)
            .WithName("CriarOrdemServico")
            .WithSummary("Cria uma nova ordem de serviço")
            .WithDescription("Adiciona uma nova ordem de serviço ao banco de dados")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapGet("/", ListarOrdensServicoAsync)
            .WithName("ListarOrdensServico")
            .WithSummary("Lista todas as ordens de serviço")
            .WithDescription("Lista todas as ordens de serviço do banco de dados")
            .Produces<List<OrdemServicoResponse>>();

        group.MapGet("/oficina", ListarOrdensServicoOficinaAsync)
            .WithName("ListarOrdensServicoOficina")
            .WithSummary("Lista ordens de serviço da oficina")
            .WithDescription("Lista ordens de serviço com informações necessárias para a oficina")
            .Produces<List<OrdemServicoResponse>>();

        group.MapGet("/{id}", ObterOrdemServicoAsync)
            .WithName("ObterOrdemServico")
            .WithSummary("Obtém uma ordem de serviço existente")
            .WithDescription("Obtém as informações de uma ordem de serviço existente do banco de dados")
            .Produces<OrdemServicoResponse>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id}", AtualizarOrdemServicoAsync)
            .WithName("AtualizarOrdemServico")
            .WithSummary("Atualiza uma ordem de serviço existente")
            .WithDescription("Atualiza as informações de uma ordem de serviço existente no banco de dados")
            .ProducesValidationProblem();

        group.MapPatch("/{id}/atribuir", AtribuirOrdemServicoAsync)
            .WithName("AtribuirOrdemServico")
            .WithSummary("Atribui uma ordem de serviço a um mecânico")
            .WithDescription("Atualiza o funcionário responsável por uma ordem de serviço existente")
            .ProducesValidationProblem();

        group.MapPatch("/{id}/diagnostico", RegistrarDiagnosticoAsync)
            .WithName("RegistrarDiagnosticoOrdemServico")
            .WithSummary("Registra diagnóstico de uma ordem de serviço")
            .WithDescription("Associa serviços e itens de inventário a uma ordem de serviço existente")
            .ProducesValidationProblem();

        group.MapDelete("/{id}", ExcluirOrdemServicoAsync)
            .WithName("ExcluirOrdemServico")
            .WithSummary("Exclui uma ordem de serviço existente")
            .WithDescription("Exclui uma ordem de serviço existente do banco de dados")
            .ProducesValidationProblem();

        return app;
    }

    private static IResult CriarOrdemServicoAsync(CriarOrdemServicoRequest request)
    {
        return Results.Created($"/api/v1/ordens-servico/{Guid.NewGuid()}", Guid.NewGuid());
    }

    private static IResult ObterOrdemServicoAsync(Guid id)
    {
        var ordemServico = new OrdemServicoResponse
        {
            Id = id,
            Descricao = "Descrição da Ordem de Serviço",
            ClienteResponsavelId = Guid.NewGuid(),
            FuncionarioResponsavelId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            DataCriacao = DateTime.UtcNow
        };

        return Results.Ok(ordemServico);
    }

    private static IResult ListarOrdensServicoAsync()
    {
        return Results.Ok(new List<OrdemServicoResponse>());
    }

    private static IResult ListarOrdensServicoOficinaAsync()
    {
        return Results.Ok(new List<OrdemServicoResponse>());
    }

    private static IResult AtualizarOrdemServicoAsync(Guid id, AtualizarOrdemServicoRequest request)
    {
        return Results.Ok();
    }

    private static IResult AtribuirOrdemServicoAsync(Guid id, AtribuirOrdemServicoRequest request)
    {
        return Results.Ok();
    }

    private static IResult RegistrarDiagnosticoAsync(Guid id, RegistrarDiagnosticoRequest request)
    {
        return Results.Ok();
    }

    private static IResult ExcluirOrdemServicoAsync(Guid id)
    {
        return Results.Ok();
    }
}
