using TechChallenge.Api.Models.Request;
using TechChallenge.Api.Models.Response;

namespace TechChallenge.Api.Endpoints;

public static class InventarioEndpoints
{
    public static IEndpointRouteBuilder MapInventarioEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/produtos")
            .WithName("Inventário");

        group.MapPost("/", CriarItemInventarioAsync)
            .WithName("CriarItemInventario")
            .WithSummary("Cria um novo item de inventário")
            .WithDescription("Adiciona um novo item de inventário ao banco de dados")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapGet("/{id}", ObterItemInventarioAsync)
            .WithName("ObterItemInventario")
            .WithSummary("Obtém um item de inventário existente")
            .WithDescription("Obtém as informações de um item de inventário existente do banco de dados")
            .Produces<ProdutoResponse>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", ListarInventarioAsync)
            .WithName("ListarInventario")
            .WithSummary("Lista todos os itens de inventário")
            .WithDescription("Lista todos os itens de inventário do banco de dados")
            .Produces<List<ProdutoResponse>>();

        group.MapPut("/{id}", AtualizarItemInventarioAsync)
            .WithName("AtualizarItemInventario")
            .WithSummary("Atualiza um item de inventário existente")
            .WithDescription("Atualiza as informações de um item de inventário existente no banco de dados")
            .ProducesValidationProblem();

        group.MapDelete("/{id}", ExcluirItemInventarioAsync)
            .WithName("ExcluirItemInventario")
            .WithSummary("Exclui um item de inventário existente")
            .WithDescription("Exclui um item de inventário existente do banco de dados")
            .ProducesValidationProblem();

        return app;
    }

    private static IResult CriarItemInventarioAsync(CriarProdutoRequest request)
    {
        return Results.Created($"/api/v1/inventario/{Guid.NewGuid()}", Guid.NewGuid());
    }

    private static IResult ObterItemInventarioAsync(Guid id)
    {
        var produto = new ProdutoResponse
        {
            Id = id,
            Descricao = "Descrição do Item",
            Valor = 0
        };

        return Results.Ok(produto);
    }

    private static IResult ListarInventarioAsync()
    {
        return Results.Ok(new List<ProdutoResponse>());
    }

    private static IResult AtualizarItemInventarioAsync(Guid id, AtualizarProdutoRequest request)
    {
        return Results.Ok();
    }

    private static IResult ExcluirItemInventarioAsync(Guid id)
    {
        return Results.Ok();
    }
}
