using TechChallenge.Api.Models.Request;
using TechChallenge.Api.Models.Response;
using TechChallenge.Application.Features.OS.AtribuirOS;
using TechChallenge.Application.Features.OS.CriarOS;
using TechChallenge.Application.Features.OS.ListarOS;
using TechChallenge.Application.Features.OS.ListarOSOficina;
using TechChallenge.Application.Features.OS.RegistrarDiagnostico;
using TechChallenge.Domain.Enums;

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
            .WithDescription("Filtra opcionalmente por status")
            .Produces<List<OrdemServicoResponse>>();

        group.MapGet("/oficina", ListarOrdensServicoOficinaAsync)
            .WithName("ListarOrdensServicoOficina")
            .WithSummary("Lista ordens de serviço da oficina")
            .WithDescription("Retorna OS Em Diagnóstico com informações básicas para exibição na oficina")
            .Produces<List<ListarOSOficinaResponseDto>>();

        group.MapGet("/{id}", ObterOrdemServicoAsync)
            .WithName("ObterOrdemServico")
            .WithSummary("Obtém uma ordem de serviço existente")
            .WithDescription("Obtém as informações de uma ordem de serviço existente do banco de dados")
            .Produces<OrdemServicoResponse>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPatch("/{id}/atribuir", AtribuirOrdemServicoAsync)
            .WithName("AtribuirOrdemServico")
            .WithSummary("Atribui uma ordem de serviço a um mecânico")
            .WithDescription("RF10: mecânico assume a OS, status muda para Em Diagnóstico")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapPatch("/{id}/diagnostico", RegistrarDiagnosticoAsync)
            .WithName("RegistrarDiagnosticoOrdemServico")
            .WithSummary("Registra diagnóstico de uma ordem de serviço")
            .WithDescription("RF11: associa serviços e produtos a uma OS Em Diagnóstico")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapDelete("/{id}", ExcluirOrdemServicoAsync)
            .WithName("ExcluirOrdemServico")
            .WithSummary("Exclui uma ordem de serviço existente")
            .WithDescription("Exclui uma ordem de serviço existente do banco de dados")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static IResult CriarOrdemServicoAsync(CriarOrdemServicoRequest request, CriarOSService service)
    {
        var command = new CriarOSCommand
        {
            Descricao = request.Descricao,
            ClienteResponsavelId = request.ClienteResponsavelId,
            FuncionarioResponsavelId = request.FuncionarioResponsavelId,
            VeiculoId = request.VeiculoId
        };

        var id = service.CriarOS(command);
        return Results.Created($"/api/v1/ordens-servico/{id}", id);
    }

    private static IResult ObterOrdemServicoAsync(Guid id, ListarOSService service)
    {
        var query = new ListarOSQuery();
        var lista = service.ListarOS(query);
        var os = lista.FirstOrDefault(o => o.Id == id);

        if (os is null)
            return Results.NotFound();

        return Results.Ok(MapToResponse(os));
    }

    private static IResult ListarOrdensServicoAsync(ListarOSService service, eStatusOS? status = null)
    {
        var query = new ListarOSQuery { Status = status };
        var lista = service.ListarOS(query);
        return Results.Ok(lista.Select(MapToResponse).ToList());
    }

    private static IResult ListarOrdensServicoOficinaAsync(ListarOSOficinaService service)
    {
        return Results.Ok(service.ListarOSOficina());
    }

    private static IResult AtribuirOrdemServicoAsync(Guid id, AtribuirOrdemServicoRequest request, AtribuirOSService service)
    {
        var command = new AtribuirOSCommand
        {
            OrdemServicoId = id,
            MecanicoId = request.MecanicoId
        };

        service.AtribuirOS(command);
        return Results.Ok();
    }

    private static IResult RegistrarDiagnosticoAsync(Guid id, RegistrarDiagnosticoRequest request, RegistrarDiagnosticoService service)
    {
        var command = new RegistrarDiagnosticoCommand
        {
            OrdemServicoId = id,
            ServicosIds = request.ServicosIds,
            ProdutosIds = request.ProdutosIds
        };

        service.RegistrarDiagnostico(command);
        return Results.Ok();
    }

    private static IResult ExcluirOrdemServicoAsync(Guid id, ListarOSService service)
    {
        var os = service.ListarOS(new ListarOSQuery()).FirstOrDefault(o => o.Id == id);

        if (os is null)
            return Results.NotFound();

        // TODO: ExcluirOSService será implementado em feature futura
        return Results.NoContent();
    }

    private static OrdemServicoResponse MapToResponse(Domain.Entities.OrdemServico os) => new()
    {
        Id = os.Id,
        Descricao = os.Descricao,
        Status = os.Status,
        ClienteResponsavelId = os.ClienteResponsavelId,
        FuncionarioResponsavelId = os.FuncionarioResponsavelId,
        VeiculoId = os.VeiculoId,
        DataCriacao = os.DataCriacao,
        DataAtualizacao = os.DataAtualizacao,
        DataFinalizacao = os.DataFinalizacao,
        Servicos = os.Servicos.Select(s => new ServicoResponse { Id = s.Id, Descricao = s.Descricao, Valor = s.Valor }).ToList(),
        Produtos = os.Produtos.Select(p => new ProdutoResponse { Id = p.Id, Descricao = p.Descricao, Valor = p.Valor }).ToList()
    };
}
