using System;
using TechChallenge.Api.Models.Request;
using TechChallenge.Api.Models.Response;
using TechChallenge.Application.Features.Funcionarios.AtualizarFuncionario;
using TechChallenge.Application.Features.Funcionarios.CriarFuncionario;
using TechChallenge.Application.Features.Funcionarios.ExcluirFuncionario;
using TechChallenge.Application.Features.Funcionarios.ListarFuncionarios;
using TechChallenge.Application.Features.Funcionarios.ObterFuncionario;
using TechChallenge.Domain.Entities;

namespace TechChallenge.Api.Endpoints;

public static class FuncionariosEndpoints
{
    public static IEndpointRouteBuilder MapFuncionariosEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/funcionarios")
            .WithName("Funcionarios")
            .RequireAuthorization("AdminOnly");

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

    private static IResult CriarFuncionarioAsync(CriarFuncionarioRequest request, CriarFuncionarioService service)
    {
        var command = new CriarFuncionarioCommand
        {
            Nome = request.Nome,
            Cpf = request.Cpf,
            Rg = request.Rg,
            Cargo = request.Cargo.ToString(),
            Logradouro = request.Endereco.Logradouro,
            Complemento = request.Endereco.Complemento,
            Numero = request.Endereco.Numero,
            Bairro = request.Endereco.Bairro,
            Cidade = request.Endereco.Cidade,
            Estado = request.Endereco.Estado,
            Cep = request.Endereco.Cep
        };

        var id = service.CriarFuncionario(command);
        return Results.Created($"/api/v1/funcionarios/{id}", id);
    }

    private static IResult ObterFuncionarioAsync(Guid id, ObterFuncionarioService service)
    {
        var funcionario = service.ObterFuncionario(new ObterFuncionarioQuery { Id = id });
        return Results.Ok(MapearFuncionarioResponse(funcionario));
    }

    private static IResult ListarFuncionariosAsync(ListarFuncionariosService service)
    {
        var funcionarios = service.ListarFuncionarios(new ListarFuncionariosQuery());
        return Results.Ok(funcionarios.Select(MapearFuncionarioResponse).ToList());
    }

    private static IResult AtualizarFuncionarioAsync(Guid id, AtualizarFuncionarioRequest request, AtualizarFuncionarioService service)
    {
        var command = new AtualizarFuncionarioCommand
        {
            Id = id,
            Nome = request.Nome,
            Cpf = request.Cpf,
            Rg = request.Rg,
            Cargo = request.Cargo,
            Logradouro = request.Endereco.Logradouro,
            Complemento = request.Endereco.Complemento,
            Numero = request.Endereco.Numero,
            Bairro = request.Endereco.Bairro,
            Cidade = request.Endereco.Cidade,
            Estado = request.Endereco.Estado,
            Cep = request.Endereco.Cep
        };

        service.AtualizarFuncionario(command);
        return Results.Ok();
    }

    private static IResult ExcluirFuncionarioAsync(Guid id, ExcluirFuncionarioService service)
    {
        service.ExcluirFuncionario(new ExcluirFuncionarioCommand { Id = id });
        return Results.NoContent();
    }

    private static FuncionarioResponse MapearFuncionarioResponse(Funcionario funcionario)
    {
        return new FuncionarioResponse
        {
            Id = funcionario.Id,
            Nome = funcionario.Nome,
            Cpf = funcionario.Cpf,
            Rg = funcionario.Rg,
            Cargo = funcionario.TipoFuncionario.ToString(),
            Endereco = new EnderecoResponse
            {
                Id = funcionario.Endereco.Id,
                Logradouro = funcionario.Endereco.Logradouro,
                Complemento = funcionario.Endereco.Complemento,
                Numero = funcionario.Endereco.Numero,
                Bairro = funcionario.Endereco.Bairro,
                Cidade = funcionario.Endereco.Cidade,
                Estado = funcionario.Endereco.Estado,
                Cep = funcionario.Endereco.Cep
            }
        };
    }
}
