using TechChallenge.Api.Models.Request;
using TechChallenge.Api.Models.Response;
using TechChallenge.Application.Features.Clientes.AtualizarCliente;
using TechChallenge.Application.Features.Clientes.CriarCliente;
using TechChallenge.Application.Features.Clientes.ExcluirCliente;
using TechChallenge.Application.Features.Clientes.ListarClientes;
using TechChallenge.Application.Features.Clientes.ObterCliente;
using TechChallenge.Domain.Entities;

namespace TechChallenge.Api.Endpoints;

public static class ClientesEndpoints
{
    public static IEndpointRouteBuilder MapClientesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/clientes")
            .WithName("Clientes");

        group.MapPost("/", CriarClienteAsync)
            .WithName("CriarCliente")
            .RequireAuthorization("AdminOuVendedor")
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
            .RequireAuthorization("AdminOuVendedor")
            .WithSummary("Atualiza um cliente existente")
            .WithDescription("Atualiza as informações de um cliente existente no banco de dados")
            .ProducesValidationProblem();

        group.MapDelete("/{id}", ExcluirClienteAsync)
            .WithName("ExcluirCliente")
            .RequireAuthorization("AdminOuVendedor")
            .WithSummary("Exclui um cliente existente")
            .WithDescription("Exclui um cliente existente do banco de dados")
            .ProducesValidationProblem();

        return app;
    }

    private static IResult CriarClienteAsync(CriarClienteRequest request, CriarClienteService service)
    {
        var endereco = request.Endereco ?? new EnderecoRequest();

        var command = new CriarClienteCommand
        {
            Nome = request.Nome ?? string.Empty,
            Cpf = request.Cpf ?? string.Empty,
            Rg = request.Rg ?? string.Empty,
            Logradouro = endereco.Logradouro ?? string.Empty,
            Complemento = endereco.Complemento ?? string.Empty,
            Numero = endereco.Numero ?? string.Empty,
            Bairro = endereco.Bairro ?? string.Empty,
            Cidade = endereco.Cidade ?? string.Empty,
            Estado = endereco.Estado ?? string.Empty,
            Cep = endereco.Cep ?? string.Empty
        };

        var id = service.CriarCliente(command);
        return Results.Created($"/api/v1/clientes/{id}", id);
    }

    private static IResult AtualizarClienteAsync(Guid id, AtualizarClienteRequest request, AtualizarClienteService service)
    {
        var endereco = request.Endereco ?? new EnderecoRequest();

        var command = new AtualizarClienteCommand
        {
            Id = id,
            Nome = request.Nome ?? string.Empty,
            Cpf = request.Cpf ?? string.Empty,
            Rg = request.Rg ?? string.Empty,
            Logradouro = endereco.Logradouro ?? string.Empty,
            Complemento = endereco.Complemento ?? string.Empty,
            Numero = endereco.Numero ?? string.Empty,
            Bairro = endereco.Bairro ?? string.Empty,
            Cidade = endereco.Cidade ?? string.Empty,
            Estado = endereco.Estado ?? string.Empty,
            Cep = endereco.Cep ?? string.Empty
        };

        service.AtualizarCliente(command);
        return Results.Ok();
    }

    private static IResult ExcluirClienteAsync(Guid id, ExcluirClienteService service)
    {
        service.ExcluirCliente(new ExcluirClienteCommand { Id = id });
        return Results.NoContent();
    }

    private static IResult ObterClienteAsync(Guid id, ObterClienteService service)
    {
        var cliente = service.ObterCliente(new ObterClienteQuery { Id = id });
        return Results.Ok(MapearClienteResponse(cliente));
    }

    private static IResult ListarClientesAsync(ListarClientesService service)
    {
        var clientes = service.ListarClientes(new ListarClientesQuery());
        return Results.Ok(clientes.Select(MapearClienteResponse).ToList());
    }

    private static ClienteResponse MapearClienteResponse(Cliente cliente)
    {
        return new ClienteResponse
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            Cpf = cliente.Cpf,
            Rg = cliente.Rg,
            Endereco = new EnderecoResponse
            {
                Id = cliente.Endereco.Id,
                Logradouro = cliente.Endereco.Logradouro,
                Complemento = cliente.Endereco.Complemento,
                Numero = cliente.Endereco.Numero,
                Bairro = cliente.Endereco.Bairro,
                Cidade = cliente.Endereco.Cidade,
                Estado = cliente.Endereco.Estado,
                Cep = cliente.Endereco.Cep
            }
        };
    }
}
