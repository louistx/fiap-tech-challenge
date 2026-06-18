using TechChallenge.Api.Models.Request;
using TechChallenge.Api.Models.Response;

namespace TechChallenge.Api.Endpoints;

public static class ClientesEndpoints
{
    public static IEndpointRouteBuilder MapClientesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/clientes")
            .WithName("Clientes");

        group.MapPost("/", CriarClienteAsync)
            .WithName("CriarCliente")
            .WithSummary("Cria um novo cliente")
            .WithDescription("Adiciona um novo cliente ao banco de dados")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapGet("/{id}", ObterClienteAsync)
            .WithName("ObterCliente")
            .WithSummary("Obtém um cliente existente")
            .WithDescription("Obtém as informações de um cliente existente do banco de dados")
            .Produces<ClienteResponse>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", ListarClientesAsync)
            .WithName("ListarClientes")
            .WithSummary("Lista todos os clientes")
            .WithDescription("Lista todos os clientes do banco de dados")
            .Produces<List<ClienteResponse>>();

        group.MapPut("/{id}", AtualizarClienteAsync)
            .WithName("AtualizarCliente")
            .WithSummary("Atualiza um cliente existente")
            .WithDescription("Atualiza as informações de um cliente existente no banco de dados")
            .ProducesValidationProblem();

        group.MapDelete("/{id}", ExcluirClienteAsync)
            .WithName("ExcluirCliente")
            .WithSummary("Exclui um cliente existente")
            .WithDescription("Exclui um cliente existente do banco de dados")
            .ProducesValidationProblem();

        return app;
    }

    private static IResult CriarClienteAsync(CriarClienteRequest request)
    {
        var guid = Guid.NewGuid();
        return Results.Created($"/api/v1/clientes/{guid}", guid);
    }

    private static IResult AtualizarClienteAsync(Guid id, AtualizarClienteRequest request)
    {
        return Results.Ok();
    }

    private static IResult ExcluirClienteAsync(Guid id)
    {
        return Results.Ok();
    }

    private static IResult ObterClienteAsync(Guid id)
    {
        var cliente = new ClienteResponse
        {
            Id = id,
            Nome = "Nome do Cliente",
            Cpf = "00000000000",
            Rg = "000000000",
            Endereco = new EnderecoResponse
            {
                Id = Guid.NewGuid(),
                Logradouro = "Logradouro",
                Complemento = "Complemento",
                Numero = "0",
                Bairro = "Bairro",
                Cidade = "Cidade",
                Estado = "Estado",
                Cep = "00000000"
            }
        };

        return Results.Ok(cliente);
    }

    private static IResult ListarClientesAsync()
    {
        return Results.Ok(new List<ClienteResponse>());
    }
}
