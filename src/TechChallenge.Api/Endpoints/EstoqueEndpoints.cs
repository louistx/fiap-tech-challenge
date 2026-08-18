using TechChallenge.Api.Models.Request;
using TechChallenge.Api.Models.Response;
using TechChallenge.Application.Features.CategoriaProdutos.ObterCategoriaProduto;
using TechChallenge.Application.Features.Estoque.AdicionarEstoque;
using TechChallenge.Application.Features.Estoque.BaixarEstoque;
using TechChallenge.Application.Features.Estoque.ListarEstoques;
using TechChallenge.Application.Features.Estoque.ObterEstoque;
using TechChallenge.Application.Features.Veiculos.ObterVeiculo;
using TechChallenge.Domain.Entities;

namespace TechChallenge.Api.Endpoints;

public static class EstoqueEndpoints
{
    public static IEndpointRouteBuilder MapEstoqueEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/estoque")
            .WithName("Estoque");

        group.MapPost("/", AdicionarEstoqueAsync)
            .WithName("AdicionarEstoque")
            .RequireAuthorization("AdminOuVendedor")
            .WithSummary("Adiciona produto ao estoque")
            .WithDescription("Adiciona uma quantidade específica de um produto ao estoque")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapGet("/", ListarEstoquesAsync)
            .WithName("ListarEstoques")
            .WithSummary("Lista todos os produtos em estoque")
            .WithDescription("Lista todos os produtos e suas quantidades em estoque")
            .Produces<List<EstoqueResponse>>();

        group.MapDelete("/{produtoId}", ObterEstoqueAsync)
            .WithName("ObterEstoque")
            .RequireAuthorization("AdminOuVendedor")
            .WithSummary("Obtém informações de um produto em estoque de acordo com o Id do produto informado")
            .WithDescription("Obtém informações sobre a quantidade disponível de um produto em estoque")
            .ProducesValidationProblem();

        group.MapPut("/", BaixarEstoqueAsync)
            .WithName("BaixarEstoque")
            .RequireAuthorization("AdminOuVendedor")
            .WithSummary("Baixa produto do estoque")
            .WithDescription("Baixa uma quantidade específica de um produto do estoque")
            .ProducesValidationProblem();

        return app;
    }

    private static IResult ListarEstoquesAsync(ListarEstoquesService service)
    {
        var categorias = service.ListarEstoques(new ListarEstoquesQuery());
        return Results.Ok(categorias.Select(MapearEstoqueResponse).ToList());
    }

    private static async Task<IResult> ObterEstoqueAsync(Guid id, ObterEstoqueService service)
    {
        Domain.Entities.Estoque estoque = await service.ObterEstoque(new ObterEstoqueQuery { Id = id });

        var response = new EstoqueResponse
        {
            ProdutoId = estoque.ProdutoId,
            Quantidade = estoque.Quantidade
        };

        return Results.Ok(response);
    }

    private static IResult AdicionarEstoqueAsync(AdicionarEstoqueRequest request, AdicionarEstoqueService service)
    {
        var command = new AdicionarEstoqueCommand
        {
            ProdutoId = request.ProdutoId
        };

        service.AdicionarEstoque(command);

        return Results.Created($"/api/v1/estoque/adicionar", $"Produto: {request.ProdutoId} adicionado ao estoque.");
    }

    private static IResult BaixarEstoqueAsync(BaixarEstoqueRequest request, BaixarEstoqueService service)
    {
        var command = new BaixarEstoqueCommand
        {
            ProdutoId = request.ProdutoId,
            Quantidade = request.Quantidade
        };

        service.BaixarEstoque(command);

        return Results.Created($"/api/v1/estoque/baixar", $"Produto: {request.ProdutoId} baixado do estoque.");  
    }

    private static EstoqueResponse MapearEstoqueResponse(Estoque estoque)
    {
        return new EstoqueResponse
        {
            ProdutoId = estoque.ProdutoId,
            Quantidade = estoque.Quantidade
        };
    }

}