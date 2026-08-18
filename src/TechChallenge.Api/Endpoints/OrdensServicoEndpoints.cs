using System;
using TechChallenge.Api.Models.Request;
using TechChallenge.Api.Models.Response;
using TechChallenge.Application.Features.OS.AtribuirOS;
using TechChallenge.Application.Features.OS.AprovarOrcamento;
using TechChallenge.Application.Features.OS.CancelarOS;
using TechChallenge.Application.Features.OS.CriarOS;
using TechChallenge.Application.Features.OS.EntregarOS;
using TechChallenge.Application.Features.OS.ExcluirOS;
using TechChallenge.Application.Features.OS.EnviarOrcamento;
using TechChallenge.Application.Features.OS.FinalizarOS;
using TechChallenge.Application.Features.OS.ListarOS;
using TechChallenge.Application.Features.OS.ListarOSOficina;
using TechChallenge.Application.Features.OS.ObterOSAcompanhamento;
using TechChallenge.Application.Features.OS.ObterOS;
using TechChallenge.Application.Features.OS.ObterTempoMedioExecucao;
using TechChallenge.Application.Features.OS.RegistrarDiagnostico;
using TechChallenge.Application.Features.OS.RetornarParaDiagnostico;
using TechChallenge.Application.Features.OS.ReprovarOrcamento;
using TechChallenge.Domain.Enums;
using TechChallenge.Infrastructure.IoC.Helpers;

namespace TechChallenge.Api.Endpoints;

public static class OrdensServicoEndpoints
{
    private const string AdminOnlyPolicy = "AdminOnly";
    private const string AdminOuVendedorPolicy = "AdminOuVendedor";
    private const string MecanicoPolicy = "Mecanico";
    private const string MecanicoOuVendedorPolicy = "MecanicoOuVendedor";

    public static IEndpointRouteBuilder MapOrdensServicoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/ordens-servico");

        group.MapPost("/", CriarOrdemServicoAsync)
            .WithName("CriarOrdemServico")
            .WithSummary("Cria uma nova ordem de serviço")
            .WithDescription("Adiciona uma nova ordem de serviço ao banco de dados")
            .RequireAuthorization(AdminOuVendedorPolicy)
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapGet("/", ListarOrdensServicoAsync)
            .WithName("ListarOrdensServico")
            .WithSummary("Lista todas as ordens de serviço")
            .WithDescription("Filtra opcionalmente por status")
            .RequireAuthorization(AdminOuVendedorPolicy)
            .Produces<List<OrdemServicoResponse>>();

        group.MapGet("/oficina", ListarOrdensServicoOficinaAsync)
            .WithName("ListarOrdensServicoOficina")
            .WithSummary("Lista ordens de serviço da oficina")
            .WithDescription("Retorna OS Em Diagnóstico com informações básicas para exibição na oficina")
            .RequireAuthorization(MecanicoOuVendedorPolicy)
            .Produces<List<ListarOSOficinaResponseDto>>();

        group.MapGet("/tempo-medio-execucao", ObterTempoMedioExecucaoAsync)
            .WithName("ObterTempoMedioExecucaoOrdensServico")
            .WithSummary("Obtém a métrica de tempo médio de execução das ordens de serviço")
            .RequireAuthorization(AdminOuVendedorPolicy)
            .Produces<TempoMedioExecucaoResponseDto>();

        group.MapGet("/acompanhamento/{codigo}", ObterOrdemServicoPorCodigoAcompanhamentoAsync)
            .WithName("ObterOrdemServicoPorCodigoAcompanhamento")
            .WithSummary("Obtém o andamento da OS pelo código de acompanhamento")
            .AllowAnonymous()
            .Produces<OrdemServicoResponse>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{id}", ObterOrdemServicoAsync)
            .WithName("ObterOrdemServico")
            .WithSummary("Obtém uma ordem de serviço existente")
            .WithDescription("Obtém as informações de uma ordem de serviço existente do banco de dados")
            .RequireAuthorization(MecanicoOuVendedorPolicy)
            .Produces<OrdemServicoResponse>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{id}", ObterStatusOrdemServicoAsync)
            .WithName("ObterStatusOrdemServico")
            .WithSummary("Obtém o status de uma ordem de serviço")
            .WithDescription("Obtém o status de uma ordem de serviço existente do banco de dados")
            .RequireAuthorization(MecanicoOuVendedorPolicy)
            .Produces<StatusOS>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPatch("/{id}/atribuir", AtribuirOrdemServicoAsync)
            .WithName("AtribuirOrdemServico")
            .WithSummary("Atribui uma ordem de serviço a um mecânico")
            .WithDescription("RF10: mecânico assume a OS, status muda para Em Diagnóstico")
            .RequireAuthorization(MecanicoPolicy)
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapPatch("/{id}/diagnostico", RegistrarDiagnosticoAsync)
            .WithName("RegistrarDiagnosticoOrdemServico")
            .WithSummary("Registra diagnóstico de uma ordem de serviço")
            .WithDescription("RF11: associa serviços e produtos a uma OS Em Diagnóstico")
            .RequireAuthorization(MecanicoPolicy)
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapPatch("/{id}/orcamento/enviar", EnviarOrcamentoAsync)
            .WithName("EnviarOrcamentoOrdemServico")
            .WithSummary("Envia orçamento de uma ordem de serviço")
            .WithDescription("Calcula o orçamento e move a OS para Aguardando Aprovação")
            .RequireAuthorization(MecanicoOuVendedorPolicy)
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapPatch("/{id}/aprovar", AprovarOrcamentoAsync)
            .WithName("AprovarOrcamentoOrdemServico")
            .WithSummary("Aprova o orçamento de uma ordem de serviço")
            .WithDescription("Move a OS de Aguardando Aprovação para Em Execução")
            .RequireAuthorization(AdminOuVendedorPolicy)
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapPatch("/{id}/reprovar", ReprovarOrcamentoAsync)
            .WithName("ReprovarOrcamentoOrdemServico")
            .WithSummary("Reprova o orçamento de uma ordem de serviço")
            .WithDescription("Move a OS de Aguardando Aprovação para Reprovada")
            .RequireAuthorization(AdminOuVendedorPolicy)
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapPatch("/{id}/retornar-para-diagnostico", RetornarParaDiagnosticoAsync)
            .WithName("RetornarParaDiagnosticoOrdemServico")
            .WithSummary("Retorna uma OS reprovada para diagnóstico")
            .RequireAuthorization(MecanicoOuVendedorPolicy)
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapPatch("/{id}/finalizar", FinalizarOrdemServicoAsync)
            .WithName("FinalizarOrdemServico")
            .WithSummary("Finaliza a execução de uma ordem de serviço")
            .RequireAuthorization(MecanicoPolicy)
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapPatch("/{id}/entregar", EntregarOrdemServicoAsync)
            .WithName("EntregarOrdemServico")
            .WithSummary("Registra a entrega do veículo")
            .RequireAuthorization(AdminOuVendedorPolicy)
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapPatch("/{id}/cancelar", CancelarOrdemServicoAsync)
            .WithName("CancelarOrdemServico")
            .WithSummary("Cancela uma ordem de serviço")
            .RequireAuthorization(AdminOnlyPolicy)
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapDelete("/{id}", ExcluirOrdemServicoAsync)
            .WithName("ExcluirOrdemServico")
            .WithSummary("Exclui uma ordem de serviço existente")
            .WithDescription("Exclui uma ordem de serviço existente do banco de dados")
            .RequireAuthorization(AdminOnlyPolicy)
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

    private static IResult ObterOrdemServicoAsync(Guid id, ObterOSService service)
    {
        var os = service.ObterOS(new ObterOSQuery { Id = id });
        return Results.Ok(MapToResponse(os));
    }

    private static IResult ObterStatusOrdemServicoAsync(Guid id, ObterOSService service)
    {
        var os = service.ObterOS(new ObterOSQuery { Id = id });

        return Results.Ok(SystemHelper.GetStatusDescription(os.Status));
    }

    private static IResult ObterOrdemServicoPorCodigoAcompanhamentoAsync(string codigo, ObterOSAcompanhamentoService service)
    {
        var os = service.ObterOSAcompanhamento(new ObterOSAcompanhamentoQuery { CodigoAcompanhamento = codigo });
        return Results.Ok(MapToResponse(os));
    }

    private static IResult ObterTempoMedioExecucaoAsync(ObterTempoMedioExecucaoService service)
    {
        return Results.Ok(service.ObterTempoMedioExecucao());
    }

    private static IResult ListarOrdensServicoAsync(ListarOSService service, StatusOS? status = null)
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
            Servicos = request.Servicos.Select(item => new ItemDiagnosticoCommand
            {
                Id = item.Id,
                Quantidade = item.Quantidade
            }).ToList(),
            Produtos = request.Produtos.Select(item => new ItemDiagnosticoCommand
            {
                Id = item.Id,
                Quantidade = item.Quantidade
            }).ToList()
        };

        service.RegistrarDiagnostico(command);
        return Results.Ok();
    }

    private static IResult EnviarOrcamentoAsync(Guid id, EnviarOrcamentoService service)
    {
        service.EnviarOrcamento(new EnviarOrcamentoCommand { OrdemServicoId = id });
        return Results.Ok();
    }

    private static IResult AprovarOrcamentoAsync(Guid id, AprovarOrcamentoService service)
    {
        service.AprovarOrcamento(new AprovarOrcamentoCommand { OrdemServicoId = id });
        return Results.Ok();
    }

    private static IResult ReprovarOrcamentoAsync(Guid id, ReprovarOrcamentoService service)
    {
        service.ReprovarOrcamento(new ReprovarOrcamentoCommand { OrdemServicoId = id });
        return Results.Ok();
    }

    private static IResult RetornarParaDiagnosticoAsync(Guid id, RetornarParaDiagnosticoService service)
    {
        service.RetornarParaDiagnostico(new RetornarParaDiagnosticoCommand { OrdemServicoId = id });
        return Results.Ok();
    }

    private static IResult FinalizarOrdemServicoAsync(Guid id, FinalizarOSService service)
    {
        service.FinalizarOS(new FinalizarOSCommand { OrdemServicoId = id });
        return Results.Ok();
    }

    private static IResult EntregarOrdemServicoAsync(Guid id, EntregarOSService service)
    {
        service.EntregarOS(new EntregarOSCommand { OrdemServicoId = id });
        return Results.Ok();
    }

    private static IResult CancelarOrdemServicoAsync(Guid id, CancelarOSService service)
    {
        service.CancelarOS(new CancelarOSCommand { OrdemServicoId = id });
        return Results.Ok();
    }

    private static IResult ExcluirOrdemServicoAsync(Guid id, ExcluirOSService service)
    {
        service.ExcluirOS(new ExcluirOSCommand { Id = id });
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
        Valor = os.Valor,
        CodigoAcompanhamento = os.CodigoAcompanhamento,
        Servicos = os.Servicos.Select(s => new ServicoResponse { Id = s.Servico.Id, Descricao = s.Servico.Descricao, Valor = (decimal)s.Valor }).ToList(),
        Produtos = os.Produtos.Select(p => new ProdutoResponse
        {
            Id = p.Produto.Id,
            Descricao = p.Produto.Descricao,
            Valor = (decimal)p.Valor,
            Quantidade = p.Quantidade
        }).ToList()
    };
}
