using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TechChallenge.Api.Models.Request;
using TechChallenge.Api.Models.Response;
using TechChallenge.IntegrationTests.Integration.Factories;

namespace TechChallenge.IntegrationTests.Integration;

public class AuthEndpointsIntegrationTests : IClassFixture<WebAplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthEndpointsIntegrationTests(WebAplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task DeveGerenciarDadosESessoesDoUsuarioAutenticado()
    {
        var login = $"usuario.auth.{Guid.NewGuid():N}";
        var senhaInicial = GerarSenha();
        var senhaNova = GerarSenha();
        var usuarioId = await CriarUsuarioAsync(login, senhaInicial);
        var sessao = await AutenticarAsync(login, senhaInicial);

        using var meRequest = CriarRequestAutenticado(HttpMethod.Get, "/api/v1/auth/me", usuarioId);
        var meResponse = await _client.SendAsync(meRequest);
        meResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var me = await meResponse.Content.ReadFromJsonAsync<UsuarioLogadoResponse>();
        me!.UsuarioId.Should().Be(usuarioId);
        me.Login.Should().Be(login);

        using var tokensRequest = CriarRequestAutenticado(HttpMethod.Get, "/api/v1/auth/refresh-tokens", usuarioId);
        var tokensResponse = await _client.SendAsync(tokensRequest);
        tokensResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var tokens = await tokensResponse.Content.ReadFromJsonAsync<List<RefreshTokenResponse>>();
        var token = tokens.Should().ContainSingle().Subject;

        using var revogarRequest = CriarRequestAutenticado(HttpMethod.Delete,
            $"/api/v1/auth/refresh-tokens/{token.Id}", usuarioId);
        var revogarResponse = await _client.SendAsync(revogarRequest);
        revogarResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var logoutRequest = CriarRequestAutenticado(HttpMethod.Post, "/api/v1/auth/logout", usuarioId);
        (await _client.SendAsync(logoutRequest)).StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var logoutAllRequest = CriarRequestAutenticado(HttpMethod.Post, "/api/v1/auth/logout-all", usuarioId);
        (await _client.SendAsync(logoutAllRequest)).StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var senhaRequest = CriarRequestAutenticado(HttpMethod.Patch, "/api/v1/auth/senha", usuarioId,
            new TrocarSenhaRequest { SenhaAtual = senhaInicial, NovaSenha = senhaNova });
        (await _client.SendAsync(senhaRequest)).StatusCode.Should().Be(HttpStatusCode.NoContent);

        var acessoAntigo = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequest { Login = login, Senha = senhaInicial });
        acessoAntigo.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var novoAcesso = await AutenticarAsync(login, senhaNova);
        novoAcesso.AccessToken.Should().NotBeNullOrWhiteSpace();
        sessao.RefreshToken.Should().NotBe(novoAcesso.RefreshToken);
    }

    [Fact]
    public async Task DeveImpedirRevogacaoDeRefreshTokenDeOutroUsuario()
    {
        var primeiroLogin = $"usuario.auth.primeiro.{Guid.NewGuid():N}";
        var senhaPrimeiroUsuario = GerarSenha();
        var primeiroUsuarioId = await CriarUsuarioAsync(primeiroLogin, senhaPrimeiroUsuario);
        await AutenticarAsync(primeiroLogin, senhaPrimeiroUsuario);

        using var tokensRequest = CriarRequestAutenticado(HttpMethod.Get, "/api/v1/auth/refresh-tokens", primeiroUsuarioId);
        var tokens = await (await _client.SendAsync(tokensRequest)).Content.ReadFromJsonAsync<List<RefreshTokenResponse>>();
        var tokenId = tokens.Should().ContainSingle().Subject.Id;

        var segundoUsuarioId = await CriarUsuarioAsync($"usuario.auth.segundo.{Guid.NewGuid():N}", GerarSenha());
        using var revogarRequest = CriarRequestAutenticado(HttpMethod.Delete,
            $"/api/v1/auth/refresh-tokens/{tokenId}", segundoUsuarioId);

        var response = await _client.SendAsync(revogarRequest);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeveRejeitarClaimDeUsuarioInvalidaNoEndpointMe()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/auth/me");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "identificador-invalido");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<Guid> CriarUsuarioAsync(string login, string senha)
    {
        var response = await _client.PostAsJsonAsync("/api/v1/usuarios", new CriarUsuarioRequest
        {
            Login = login,
            Senha = senha,
            TipoUsuario = "Administrador"
        });
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Created, body);
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    private async Task<LoginResponse> AutenticarAsync(string login, string senha)
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest { Login = login, Senha = senha });
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, body);
        return (await response.Content.ReadFromJsonAsync<LoginResponse>())!;
    }

    private static HttpRequestMessage CriarRequestAutenticado(
        HttpMethod method,
        string url,
        Guid usuarioId,
        object? content = null)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", usuarioId.ToString());
        if (content is not null)
            request.Content = JsonContent.Create(content);
        return request;
    }

    private static string GerarSenha() => $"Aa1!{Guid.NewGuid():N}";
}
