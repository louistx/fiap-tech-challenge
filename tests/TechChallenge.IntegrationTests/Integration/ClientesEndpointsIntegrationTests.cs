using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TechChallenge.Api.Models.Request;
using TechChallenge.Api.Models.Response;
using TechChallenge.Api.Tests.Integration.Factories;

namespace TechChallenge.Api.Tests.Integration;

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
        cliente!.Nome.Should().Be(criarRequest.Nome);
        cliente.Cpf.Should().Be(criarRequest.Cpf);
        cliente.Endereco.Should().NotBeNull();
        cliente.Endereco!.Cidade.Should().Be(criarRequest.Endereco.Cidade);

        var listarResponse = await _client.GetAsync("/api/v1/clientes");
        listarResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var clientes = await listarResponse.Content.ReadFromJsonAsync<List<ClienteResponse>>();
        clientes.Should().Contain(c => c.Id == clienteId);

        var atualizarRequest = AtualizarClienteRequest("39053344705");
        var atualizarResponse = await _client.PutAsJsonAsync($"/api/v1/clientes/{clienteId}", atualizarRequest);
        atualizarResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var clienteAtualizado = await _client.GetFromJsonAsync<ClienteResponse>($"/api/v1/clientes/{clienteId}");
        clienteAtualizado.Should().NotBeNull();
        clienteAtualizado!.Nome.Should().Be(atualizarRequest.Nome);
        clienteAtualizado.Cpf.Should().Be(atualizarRequest.Cpf);
        clienteAtualizado.Endereco!.Cidade.Should().Be(atualizarRequest.Endereco.Cidade);

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
