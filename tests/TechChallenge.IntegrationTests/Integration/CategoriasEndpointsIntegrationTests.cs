using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TechChallenge.Api.Models.Request;
using TechChallenge.Api.Models.Response;
using TechChallenge.IntegrationTests.Integration.Factories;

namespace TechChallenge.IntegrationTests.Integration;

public class CategoriasEndpointsIntegrationTests : IClassFixture<WebAplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CategoriasEndpointsIntegrationTests(WebAplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task DeveExecutarCrudDeCategoriaDeProduto()
    {
        var descricao = $"Filtros {Guid.NewGuid():N}";
        var id = await CriarCategoriaProdutoAsync(descricao);

        var obtida = await _client.GetFromJsonAsync<CategoriaProdutoResponse>($"/api/v1/categoriaproduto/{id}");
        obtida.Should().NotBeNull();
        obtida.Descricao.Should().Be(descricao);

        var listagem = await _client.GetFromJsonAsync<List<CategoriaProdutoResponse>>("/api/v1/categoriaproduto");
        listagem.Should().Contain(categoria => categoria.Id == id);

        var atualizarResponse = await _client.PutAsJsonAsync($"/api/v1/categoriaproduto/{id}",
            new AtualizarCategoriaProdutoRequest { Descricao = "Filtros premium" });
        var atualizarBody = await atualizarResponse.Content.ReadAsStringAsync();
        atualizarResponse.StatusCode.Should().Be(HttpStatusCode.OK, atualizarBody);

        var atualizada = await _client.GetFromJsonAsync<CategoriaProdutoResponse>($"/api/v1/categoriaproduto/{id}");
        atualizada!.Descricao.Should().Be("Filtros premium");

        var excluirResponse = await _client.DeleteAsync($"/api/v1/categoriaproduto/{id}");
        excluirResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var naoEncontrada = await _client.GetAsync($"/api/v1/categoriaproduto/{id}");
        naoEncontrada.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeveValidarEAutorizarAlteracoesDeCategoriaDeProduto()
    {
        var invalida = await _client.PostAsJsonAsync("/api/v1/categoriaproduto", new CriarCategoriaProdutoRequest());
        invalida.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var descricao = $"Lubrificantes {Guid.NewGuid():N}";
        var id = await CriarCategoriaProdutoAsync(descricao);
        var duplicada = await _client.PostAsJsonAsync("/api/v1/categoriaproduto",
            new CriarCategoriaProdutoRequest { Descricao = descricao });
        duplicada.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var atualizarInvalida = await _client.PutAsJsonAsync($"/api/v1/categoriaproduto/{id}",
            new AtualizarCategoriaProdutoRequest());
        atualizarInvalida.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        using var atualizarSemPermissao = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/categoriaproduto/{id}")
        {
            Content = JsonContent.Create(new AtualizarCategoriaProdutoRequest { Descricao = "Lubrificantes sintéticos" })
        };
        atualizarSemPermissao.Headers.Add(TestAuthHandler.RoleHeader, "Mecanico");

        var response = await _client.SendAsync(atualizarSemPermissao);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeveExecutarCrudDeCategoriaDeServico()
    {
        var descricao = $"Motor {Guid.NewGuid():N}";
        var id = await CriarCategoriaServicoAsync(descricao);

        var obtida = await _client.GetFromJsonAsync<CategoriaServicoResponse>($"/api/v1/categoriaservico/{id}");
        obtida.Should().NotBeNull();
        obtida.Descricao.Should().Be(descricao);

        var listagem = await _client.GetFromJsonAsync<List<CategoriaServicoResponse>>("/api/v1/categoriaservico");
        listagem.Should().Contain(categoria => categoria.Id == id);

        var atualizarResponse = await _client.PutAsJsonAsync($"/api/v1/categoriaservico/{id}",
            new AtualizarCategoriaServicoRequest { Descricao = "Motor e transmissão" });
        var atualizarBody = await atualizarResponse.Content.ReadAsStringAsync();
        atualizarResponse.StatusCode.Should().Be(HttpStatusCode.OK, atualizarBody);

        var atualizada = await _client.GetFromJsonAsync<CategoriaServicoResponse>($"/api/v1/categoriaservico/{id}");
        atualizada!.Descricao.Should().Be("Motor e transmissão");

        var excluirResponse = await _client.DeleteAsync($"/api/v1/categoriaservico/{id}");
        excluirResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var naoEncontrada = await _client.GetAsync($"/api/v1/categoriaservico/{id}");
        naoEncontrada.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeveValidarEAutorizarAlteracoesDeCategoriaDeServico()
    {
        var invalida = await _client.PostAsJsonAsync("/api/v1/categoriaservico", new CriarCategoriaServicoRequest());
        invalida.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var id = await CriarCategoriaServicoAsync($"Elétrica {Guid.NewGuid():N}");
        var naoEncontrada = await _client.PutAsJsonAsync($"/api/v1/categoriaservico/{Guid.NewGuid()}",
            new AtualizarCategoriaServicoRequest { Descricao = "Inexistente" });
        naoEncontrada.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var atualizarInvalida = await _client.PutAsJsonAsync($"/api/v1/categoriaservico/{id}",
            new AtualizarCategoriaServicoRequest());
        atualizarInvalida.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        using var excluirSemPermissao = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/categoriaservico/{id}");
        excluirSemPermissao.Headers.Add(TestAuthHandler.RoleHeader, "Mecanico");

        var response = await _client.SendAsync(excluirSemPermissao);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeveExecutarCrudDeCategoriaDeVeiculo()
    {
        var descricao = $"SUV {Guid.NewGuid():N}";
        var id = await CriarCategoriaVeiculoAsync(descricao);

        var obtida = await _client.GetFromJsonAsync<CategoriaVeiculoResponse>($"/api/v1/categoriaveiculo/{id}");
        obtida.Should().NotBeNull();
        obtida.Descricao.Should().Be(descricao);

        var listagem = await _client.GetFromJsonAsync<List<CategoriaVeiculoResponse>>("/api/v1/categoriaveiculo");
        listagem.Should().Contain(categoria => categoria.Id == id);

        var atualizarResponse = await _client.PutAsJsonAsync($"/api/v1/categoriaveiculo/{id}",
            new AtualizarCategoriaVeiculoRequest { Descricao = "SUV compacto" });
        var atualizarBody = await atualizarResponse.Content.ReadAsStringAsync();
        atualizarResponse.StatusCode.Should().Be(HttpStatusCode.OK, atualizarBody);

        var atualizada = await _client.GetFromJsonAsync<CategoriaVeiculoResponse>($"/api/v1/categoriaveiculo/{id}");
        atualizada!.Descricao.Should().Be("SUV compacto");

        var excluirResponse = await _client.DeleteAsync($"/api/v1/categoriaveiculo/{id}");
        excluirResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var naoEncontrada = await _client.GetAsync($"/api/v1/categoriaveiculo/{id}");
        naoEncontrada.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeveValidarEAutorizarAlteracoesDeCategoriaDeVeiculo()
    {
        var invalida = await _client.PostAsJsonAsync("/api/v1/categoriaveiculo", new CriarCategoriaVeiculoRequest());
        invalida.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var id = await CriarCategoriaVeiculoAsync($"Sedan {Guid.NewGuid():N}");
        var naoEncontrada = await _client.DeleteAsync($"/api/v1/categoriaveiculo/{Guid.NewGuid()}");
        naoEncontrada.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var atualizarInvalida = await _client.PutAsJsonAsync($"/api/v1/categoriaveiculo/{id}",
            new AtualizarCategoriaVeiculoRequest());
        atualizarInvalida.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        using var criarSemPermissao = new HttpRequestMessage(HttpMethod.Post, "/api/v1/categoriaveiculo")
        {
            Content = JsonContent.Create(new CriarCategoriaVeiculoRequest { Descricao = "Coupé" })
        };
        criarSemPermissao.Headers.Add(TestAuthHandler.RoleHeader, "Mecanico");

        var response = await _client.SendAsync(criarSemPermissao);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        id.Should().NotBeEmpty();
    }

    private async Task<Guid> CriarCategoriaProdutoAsync(string descricao)
    {
        var response = await _client.PostAsJsonAsync("/api/v1/categoriaproduto",
            new CriarCategoriaProdutoRequest { Descricao = descricao });
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Created, body);
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    private async Task<Guid> CriarCategoriaServicoAsync(string descricao)
    {
        var response = await _client.PostAsJsonAsync("/api/v1/categoriaservico",
            new CriarCategoriaServicoRequest { Descricao = descricao });
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Created, body);
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    private async Task<Guid> CriarCategoriaVeiculoAsync(string descricao)
    {
        var response = await _client.PostAsJsonAsync("/api/v1/categoriaveiculo",
            new CriarCategoriaVeiculoRequest { Descricao = descricao });
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Created, body);
        return await response.Content.ReadFromJsonAsync<Guid>();
    }
}
