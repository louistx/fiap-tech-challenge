using TechChallenge.Api.Models.Request;
using TechChallenge.Api.Models.Response;

namespace TechChallenge.Api.Endpoints;

public static class FuncionariosEndpoints
{
    public static IEndpointRouteBuilder MapFuncionariosEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/funcionarios")
            .WithName("Funcionarios");

        group.MapPost("/", CriarFuncionarioAsync)
            .WithName("CriarFuncionario")
            .WithSummary("Cria um novo funcionário")
            .WithDescription("Adiciona um novo funcionário ao banco de dados")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapGet("/{id}", ObterFuncionarioAsync)
            .WithName("ObterFuncionario")
            .WithSummary("Obtém um funcionário existente")
            .WithDescription("Obtém as informações de um funcionário existente do banco de dados")
            .Produces<FuncionarioResponse>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", ListarFuncionariosAsync)
            .WithName("ListarFuncionarios")
            .WithSummary("Lista todos os funcionários")
            .WithDescription("Lista todos os funcionários do banco de dados")
            .Produces<List<FuncionarioResponse>>();

        group.MapPut("/{id}", AtualizarFuncionarioAsync)
            .WithName("AtualizarFuncionario")
            .WithSummary("Atualiza um funcionário existente")
            .WithDescription("Atualiza as informações de um funcionário existente no banco de dados")
            .ProducesValidationProblem();

        group.MapDelete("/{id}", ExcluirFuncionarioAsync)
            .WithName("ExcluirFuncionario")
            .WithSummary("Exclui um funcionário existente")
            .WithDescription("Exclui um funcionário existente do banco de dados")
            .ProducesValidationProblem();

        return app;
    }

    private static IResult CriarFuncionarioAsync(CriarFuncionarioRequest request)
    {
        return Results.Created($"/api/v1/funcionarios/{Guid.NewGuid()}", Guid.NewGuid());
    }

    private static IResult ObterFuncionarioAsync(Guid id)
    {
        var funcionario = new FuncionarioResponse
        {
            Id = id,
            Nome = "Nome do Funcionário",
            Cpf = "00000000000",
            Rg = "000000000",
            Cargo = "Mecanico"
        };

        return Results.Ok(funcionario);
    }

    private static IResult ListarFuncionariosAsync()
    {
        return Results.Ok(new List<FuncionarioResponse>());
    }

    private static IResult AtualizarFuncionarioAsync(Guid id, AtualizarFuncionarioRequest request)
    {
        return Results.Ok();
    }

    private static IResult ExcluirFuncionarioAsync(Guid id)
    {
        return Results.Ok();
    }
}
