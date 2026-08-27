using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using TechChallenge.Api.Models.Request;
using TechChallenge.IntegrationTests.Integration.Factories;

namespace TechChallenge.IntegrationTests.Integration;

public class CategoriasEndpointsIntegrationTests : IClassFixture<WebAplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CategoriasEndpointsIntegrationTests(WebAplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    public static IEnumerable<object[]> Categorias()
    {
        yield return ["/api/v1/categoriaproduto"];
        yield return ["/api/v1/categoriaservico"];
        yield return ["/api/v1/categoriaveiculo"];
    }

    [Theory]
    [MemberData(nameof(Categorias))]
    public async Task DeveExecutarCrudDaCategoria(string rota)
    {
        var descricao = $"Categoria {Guid.NewGuid():N}";
        var criarResponse = await _client.PostAsJsonAsync(rota, new CriarCategoriaProdutoRequest
        {
            Descricao = descricao
        });

        var criarBody = await criarResponse.Content.ReadAsStringAsync();
        criarResponse.StatusCode.Should().Be(HttpStatusCode.Created, criarBody);
        var categoriaId = await criarResponse.Content.ReadFromJsonAsync<Guid>();
        categoriaId.Should().NotBeEmpty();
        criarResponse.Headers.Location?.ToString().Should().Be($"{rota}/{categoriaId}");

        var obterResponse = await _client.GetAsync($"{rota}/{categoriaId}");
        obterResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        (await ObterDescricaoAsync(obterResponse)).Should().Be(descricao);

        var listarResponse = await _client.GetAsync(rota);
        listarResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var categorias = await listarResponse.Content.ReadFromJsonAsync<JsonDocument>();
        categorias.Should().NotBeNull();
        categorias!.RootElement.EnumerateArray()
            .Should().Contain(categoria => categoria.GetProperty("id").GetGuid() == categoriaId);

        var descricaoAtualizada = $"Categoria atualizada {Guid.NewGuid():N}";
        var atualizarResponse = await _client.PutAsJsonAsync($"{rota}/{categoriaId}", new AtualizarCategoriaProdutoRequest
        {
            Descricao = descricaoAtualizada
        });
        atualizarResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var obterAtualizadaResponse = await _client.GetAsync($"{rota}/{categoriaId}");
        obterAtualizadaResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        (await ObterDescricaoAsync(obterAtualizadaResponse)).Should().Be(descricaoAtualizada);

        var excluirResponse = await _client.DeleteAsync($"{rota}/{categoriaId}");
        excluirResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var obterExcluidaResponse = await _client.GetAsync($"{rota}/{categoriaId}");
        obterExcluidaResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [MemberData(nameof(Categorias))]
    public async Task DeveRetornarBadRequestQuandoCategoriaForInvalidaOuDuplicada(string rota)
    {
        var invalidaResponse = await _client.PostAsJsonAsync(rota, new CriarCategoriaProdutoRequest
        {
            Descricao = string.Empty
        });
        invalidaResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var descricaoNulaResponse = await _client.PostAsJsonAsync(rota, new { Descricao = (string?)null });
        descricaoNulaResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var atualizacaoComDescricaoNulaResponse = await _client.PutAsJsonAsync($"{rota}/{Guid.NewGuid()}", new
        {
            Descricao = (string?)null
        });
        atualizacaoComDescricaoNulaResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var descricao = $"Categoria unica {Guid.NewGuid():N}";
        var primeiraResponse = await _client.PostAsJsonAsync(rota, new CriarCategoriaProdutoRequest
        {
            Descricao = descricao
        });
        primeiraResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var duplicadaResponse = await _client.PostAsJsonAsync(rota, new CriarCategoriaProdutoRequest
        {
            Descricao = descricao
        });
        duplicadaResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [MemberData(nameof(Categorias))]
    public async Task DeveBloquearAlteracaoDeCategoriaParaPerfilSemPermissao(string rota)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, $"{rota}/{Guid.NewGuid()}")
        {
            Content = JsonContent.Create(new AtualizarCategoriaProdutoRequest { Descricao = "Categoria" })
        };
        request.Headers.Add(TestAuthHandler.RoleHeader, "Mecanico");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [MemberData(nameof(Categorias))]
    public async Task DeveRetornarNotFoundAoAtualizarOuExcluirCategoriaInexistente(string rota)
    {
        var categoriaId = Guid.NewGuid();

        var atualizarResponse = await _client.PutAsJsonAsync($"{rota}/{categoriaId}", new AtualizarCategoriaProdutoRequest
        {
            Descricao = "Categoria inexistente"
        });
        atualizarResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var excluirResponse = await _client.DeleteAsync($"{rota}/{categoriaId}");
        excluirResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static async Task<string> ObterDescricaoAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadFromJsonAsync<JsonDocument>();
        body.Should().NotBeNull();
        return body!.RootElement.GetProperty("descricao").GetString()!;
    }
}
