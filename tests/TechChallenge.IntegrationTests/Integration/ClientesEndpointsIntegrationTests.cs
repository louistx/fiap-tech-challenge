using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using TechChallenge.Api.Models.Request;
using TechChallenge.Api.Models.Response;
using TechChallenge.IntegrationTests.Integration.Factories;

namespace TechChallenge.IntegrationTests.Integration;

public class ClientesEndpointsIntegrationTests : IClassFixture<WebAplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ClientesEndpointsIntegrationTests(WebAplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task DeveExecutarCrudDeCliente()
    {
        var criarRequest = CriarClienteRequest("52998224725");

        var criarResponse = await _client.PostAsJsonAsync("/api/v1/clientes", criarRequest);

        var criarBody = await criarResponse.Content.ReadAsStringAsync();
        criarResponse.StatusCode.Should().Be(HttpStatusCode.Created, criarBody);
        var clienteId = await criarResponse.Content.ReadFromJsonAsync<Guid>();
        clienteId.Should().NotBeEmpty();

        var obterResponse = await _client.GetAsync($"/api/v1/clientes/{clienteId}");
        obterResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var cliente = await obterResponse.Content.ReadFromJsonAsync<ClienteResponse>();
        cliente.Should().NotBeNull();
        cliente.Nome.Should().Be(criarRequest.Nome);
        cliente.Cpf.Should().Be("529.982.247-25");
        cliente.Endereco.Should().NotBeNull();
        cliente.Endereco.Cidade.Should().Be(criarRequest.Endereco!.Cidade);

        var listarResponse = await _client.GetAsync("/api/v1/clientes");
        listarResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var clientes = await listarResponse.Content.ReadFromJsonAsync<List<ClienteResponse>>();
        clientes.Should().Contain(c => c.Id == clienteId);

        var atualizarRequest = AtualizarClienteRequest("39053344705");
        var atualizarResponse = await _client.PutAsJsonAsync($"/api/v1/clientes/{clienteId}", atualizarRequest);
        atualizarResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var clienteAtualizado = await _client.GetFromJsonAsync<ClienteResponse>($"/api/v1/clientes/{clienteId}");
        clienteAtualizado.Should().NotBeNull();
        clienteAtualizado.Nome.Should().Be(atualizarRequest.Nome);
        clienteAtualizado.Cpf.Should().Be("390.533.447-05");
        clienteAtualizado.Endereco!.Cidade.Should().Be(atualizarRequest.Endereco!.Cidade);

        var excluirResponse = await _client.DeleteAsync($"/api/v1/clientes/{clienteId}");
        excluirResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var obterExcluidoResponse = await _client.GetAsync($"/api/v1/clientes/{clienteId}");
        obterExcluidoResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeveRetornarBadRequestQuandoClienteInvalido()
    {
        var request = CriarClienteRequest("12345678901");
        request.Nome = string.Empty;

        var response = await _client.PostAsJsonAsync("/api/v1/clientes", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeveCriarClienteQuandoRgEEnderecoNaoForemInformados()
    {
        var request = new CriarClienteRequest
        {
            Nome = "Maria Cliente",
            Cpf = "52998224725"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/clientes", request);
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Created, body);
    }

    [Fact]
    public async Task DeveRetornarProblemDetailsSemDetalhesInternosQuandoClienteJaExiste()
    {
        var request = CriarClienteRequest("121.344.187-02");

        var criarResponse = await _client.PostAsJsonAsync("/api/v1/clientes", request);
        criarResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        request.Cpf = "121344187-02";
        var response = await _client.PostAsJsonAsync("/api/v1/clientes", request);
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest, body);

        using var json = JsonDocument.Parse(body);
        var root = json.RootElement;

        root.GetProperty("status").GetInt32().Should().Be((int)HttpStatusCode.BadRequest);
        root.GetProperty("title").GetString().Should().Be("Requisição inválida.");
        root.GetProperty("detail").GetString().Should().Be("Já existe um cliente cadastrado com o CPF 121.344.187-02.");
        root.TryGetProperty("traceId", out _).Should().BeTrue();
        root.TryGetProperty("details", out _).Should().BeFalse();
        root.TryGetProperty("stackTrace", out _).Should().BeFalse();
    }

    private static CriarClienteRequest CriarClienteRequest(string cpf)
    {
        return new CriarClienteRequest
        {
            Nome = "Maria Cliente",
            Cpf = cpf,
            Rg = "123456789",
            Endereco = CriarEnderecoRequest("Sao Paulo")
        };
    }

    private static AtualizarClienteRequest AtualizarClienteRequest(string cpf)
    {
        return new AtualizarClienteRequest
        {
            Nome = "Maria Cliente Atualizada",
            Cpf = cpf,
            Rg = "987654321",
            Endereco = CriarEnderecoRequest("Curitiba")
        };
    }

    private static EnderecoRequest CriarEnderecoRequest(string cidade)
    {
        return new EnderecoRequest
        {
            Logradouro = "Rua Teste",
            Complemento = "Apto 10",
            Numero = "100",
            Bairro = "Centro",
            Cidade = cidade,
            Estado = "SP",
            Cep = "01001000"
        };
    }
}
