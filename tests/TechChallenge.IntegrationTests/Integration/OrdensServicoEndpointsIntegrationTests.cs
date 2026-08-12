using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TechChallenge.Api.Models.Request;
using TechChallenge.Api.Models.Response;
using TechChallenge.Domain.Enums;
using TechChallenge.IntegrationTests.Integration.Factories;

namespace TechChallenge.IntegrationTests.Integration;

public class OrdensServicoEndpointsIntegrationTests : IClassFixture<WebAplicationFactory<Program>>
{
    private static int _sequencia;
    private readonly HttpClient _client;

    public OrdensServicoEndpointsIntegrationTests(WebAplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task DeveExecutarFluxoBasicoDaMaquinaDeEstados()
    {
        var dados = await CriarDadosBaseAsync();
        var osId = await CriarOrdemServicoAsync(dados);

        await PatchAsync($"/api/v1/ordens-servico/{osId}/atribuir", "Mecanico",
            new AtribuirOrdemServicoRequest { MecanicoId = dados.FuncionarioId });

        await PatchAsync($"/api/v1/ordens-servico/{osId}/diagnostico", "Mecanico",
            new RegistrarDiagnosticoRequest
            {
                Servicos =
                [
                    new ItemDiagnosticoRequest { Id = dados.ServicoId, Quantidade = 1 }
                ]
            });

        await PatchAsync($"/api/v1/ordens-servico/{osId}/orcamento/enviar", "Mecanico");
        await PatchAsync($"/api/v1/ordens-servico/{osId}/aprovar", "Administrador");
        await PatchAsync($"/api/v1/ordens-servico/{osId}/finalizar", "Mecanico");
        await PatchAsync($"/api/v1/ordens-servico/{osId}/entregar", "Administrador");

        var response = await _client.GetAsync($"/api/v1/ordens-servico/{osId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var os = await response.Content.ReadFromJsonAsync<OrdemServicoResponse>(JsonTestOptions.Web);
        os.Should().NotBeNull();
        os.Status.Should().Be(StatusOS.Entregue);
        os.Valor.Should().Be(120);
        os.DataFinalizacao.Should().NotBeNull();
        os.CodigoAcompanhamento.Should().NotBeNullOrWhiteSpace();

        var acompanhamentoResponse = await _client.GetAsync($"/api/v1/ordens-servico/acompanhamento/{os.CodigoAcompanhamento}");
        acompanhamentoResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var osAcompanhamento =
            await acompanhamentoResponse.Content.ReadFromJsonAsync<OrdemServicoResponse>(JsonTestOptions.Web);
        osAcompanhamento.Should().NotBeNull();
        osAcompanhamento.Id.Should().Be(osId);

        var metricaResponse = await _client.GetAsync("/api/v1/ordens-servico/tempo-medio-execucao");
        metricaResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var metrica = await metricaResponse.Content.ReadFromJsonAsync<TempoMedioExecucaoResponse>();
        metrica.Should().NotBeNull();
        metrica.QuantidadeOrdensFinalizadas.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task DeveBloquearFinalizarOSQuandoRoleNaoForMecanico()
    {
        var dados = await CriarDadosBaseAsync();
        var osId = await CriarOrdemServicoAsync(dados);

        var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/v1/ordens-servico/{osId}/finalizar");
        request.Headers.Add(TestAuthHandler.RoleHeader, "Vendedor");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<DadosBase> CriarDadosBaseAsync()
    {
        var sequencia = Interlocked.Increment(ref _sequencia);

        var clienteId = await CriarAsync("/api/v1/clientes", new CriarClienteRequest
        {
            Nome = "Maria Cliente OS",
            TipoDocumento = TipoDocumento.Cpf,
            Documento = GerarCpf(sequencia * 2),
            Endereco = CriarEnderecoRequest()
        });

        var funcionarioId = await CriarAsync("/api/v1/funcionarios", new CriarFuncionarioRequest
        {
            Nome = "Joao Mecanico OS",
            Cpf = GerarCpf(sequencia * 2 + 1),
            Rg = "987654321",
            Cargo = TipoFuncionario.Mecanico,
            Endereco = CriarEnderecoRequest()
        });

        var veiculoId = await CriarAsync("/api/v1/veiculos", new CriarVeiculoRequest
        {
            Placa = $"ABC{sequencia % 10}D{sequencia % 10}{(sequencia + 1) % 10}",
            Modelo = "Onix",
            Marca = "Chevrolet",
            Cor = "Prata",
            Ano = 2020,
            Quilometragem = 50000,
            Valor = 50000,
            ClienteId = clienteId,
            CategoriaId = categoriaId
        });

        var servicoId = await CriarAsync("/api/v1/servicos", new CriarServicoRequest
        {
            Descricao = "Diagnostico eletronico",
            Valor = 120
        });

        return new DadosBase(clienteId, funcionarioId, veiculoId, servicoId);
    }

    private async Task<Guid> CriarOrdemServicoAsync(DadosBase dados)
    {
        return await CriarAsync("/api/v1/ordens-servico", new CriarOrdemServicoRequest
        {
            Descricao = "Motor falhando",
            ClienteResponsavelId = dados.ClienteId,
            FuncionarioResponsavelId = dados.FuncionarioId,
            VeiculoId = dados.VeiculoId
        });
    }

    private async Task<Guid> CriarAsync<TRequest>(string rota, TRequest request)
    {
        var response = await _client.PostAsJsonAsync(rota, request);
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Created, body);
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    private async Task PatchAsync(string rota, string role, object? body = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, rota);
        request.Headers.Add(TestAuthHandler.RoleHeader, role);

        if (body is not null)
            request.Content = JsonContent.Create(body);

        var response = await _client.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, responseBody);
    }

    private static EnderecoRequest CriarEnderecoRequest()
    {
        return new EnderecoRequest
        {
            Logradouro = "Rua Teste",
            Complemento = "Casa",
            Numero = "100",
            Bairro = "Centro",
            Cidade = "Sao Paulo",
            Estado = "SP",
            Cep = "01001000"
        };
    }

    private static string GerarCpf(int seed)
    {
        var digitos = new int[11];
        var valor = Math.Abs(seed) + 12345678;

        for (var i = 7; i >= 0; i--)
        {
            digitos[i] = valor % 10;
            valor /= 10;
        }

        digitos[8] = seed % 10;
        digitos[9] = CalcularDigito(digitos, 9);
        digitos[10] = CalcularDigito(digitos, 10);

        return string.Concat(digitos);
    }

    private static int CalcularDigito(int[] digitos, int tamanho)
    {
        var soma = 0;
        for (var i = 0; i < tamanho; i++)
            soma += digitos[i] * (tamanho + 1 - i);

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    private sealed record DadosBase(Guid ClienteId, Guid FuncionarioId, Guid VeiculoId, Guid ServicoId);

    private sealed class TempoMedioExecucaoResponse
    {
        public int QuantidadeOrdensFinalizadas { get; set; }
        public double TempoMedioMinutos { get; set; }
        public double TempoMedioHoras { get; set; }
    }
}
