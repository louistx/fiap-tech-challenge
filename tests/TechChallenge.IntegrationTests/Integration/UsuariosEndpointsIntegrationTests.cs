using System;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TechChallenge.Api.Models.Request;
using TechChallenge.Api.Models.Response;
using TechChallenge.Domain.Enums;
using TechChallenge.IntegrationTests.Integration.Factories;

namespace TechChallenge.IntegrationTests.Integration;

public class UsuariosEndpointsIntegrationTests : IClassFixture<WebAplicationFactory<Program>>
{
    private static int _sequencia;
    private readonly HttpClient _client;

    public UsuariosEndpointsIntegrationTests(WebAplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task DeveCriarEListarUsuario()
    {
        var request = new CriarUsuarioRequest
        {
            Login = "vendedor.teste",
            Senha = "Senha@123",
            TipoUsuario = "Vendedor"
        };

        var criarResponse = await _client.PostAsJsonAsync("/api/v1/usuarios", request);
        var body = await criarResponse.Content.ReadAsStringAsync();
        criarResponse.StatusCode.Should().Be(HttpStatusCode.Created, body);

        var id = await criarResponse.Content.ReadFromJsonAsync<Guid>();
        id.Should().NotBeEmpty();

        var obterResponse = await _client.GetAsync($"/api/v1/usuarios/{id}");
        obterResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var usuario = await obterResponse.Content.ReadFromJsonAsync<UsuarioResponse>();
        usuario.Should().NotBeNull();
        usuario.Login.Should().Be("vendedor.teste");
        usuario.TipoUsuario.Should().Be("Vendedor");
        usuario.Ativo.Should().BeTrue();
    }

    [Fact]
    public async Task NaoMecanicoNaoPodeCriarUsuario()
    {
        var request = new CriarUsuarioRequest
        {
            Login = "bloqueado",
            Senha = "Senha@123",
            TipoUsuario = "Vendedor"
        };

        using var mensagem = new HttpRequestMessage(HttpMethod.Post, "/api/v1/usuarios")
        {
            Content = JsonContent.Create(request)
        };
        mensagem.Headers.Add(TestAuthHandler.RoleHeader, "Mecanico");

        var response = await _client.SendAsync(mensagem);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeveAutenticarERotacionarRefreshToken()
    {
        var criar = new CriarUsuarioRequest
        {
            Login = "login.refresh",
            Senha = "Senha@123",
            TipoUsuario = "Administrador"
        };
        var criarResponse = await _client.PostAsJsonAsync("/api/v1/usuarios", criar);
        criarResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Login real (endpoint anônimo) exercita LoginService + hasher + token JWT.
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequest { Login = "login.refresh", Senha = "Senha@123" });
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var login = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        login.Should().NotBeNull();
        login.AccessToken.Should().NotBeNullOrWhiteSpace();
        login.RefreshToken.Should().NotBeNullOrWhiteSpace();

        // Rotação: novo par, refresh diferente.
        var refreshResponse = await _client.PostAsJsonAsync("/api/v1/auth/refresh",
            new RefreshRequest { RefreshToken = login.RefreshToken });
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var refreshed = await refreshResponse.Content.ReadFromJsonAsync<LoginResponse>();
        refreshed.Should().NotBeNull();
        refreshed.RefreshToken.Should().NotBe(login.RefreshToken);

        // Reuso do refresh antigo revogado -> 401.
        var reuseResponse = await _client.PostAsJsonAsync("/api/v1/auth/refresh",
            new RefreshRequest { RefreshToken = login.RefreshToken });
        reuseResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // Refresh novo continua válido, pois não há cadeia de sessão.
        var aposReuso = await _client.PostAsJsonAsync("/api/v1/auth/refresh",
            new RefreshRequest { RefreshToken = refreshed.RefreshToken });
        aposReuso.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task LoginComSenhaErradaRetornaUnauthorized()
    {
        var criar = new CriarUsuarioRequest
        {
            Login = "login.errado",
            Senha = "Senha@123",
            TipoUsuario = "Vendedor"
        };
        await _client.PostAsJsonAsync("/api/v1/usuarios", criar);

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequest { Login = "login.errado", Senha = "SenhaIncorreta1" });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeveAdministrarTipoVinculoStatusESenhaDoUsuario()
    {
        var login = $"usuario.admin.{Guid.NewGuid():N}";
        var senhaInicial = GerarSenha();
        var senhaNova = GerarSenha();
        var criarResponse = await _client.PostAsJsonAsync("/api/v1/usuarios", new CriarUsuarioRequest
        {
            Login = login,
            Senha = senhaInicial,
            TipoUsuario = "Vendedor"
        });
        criarResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var usuarioId = await criarResponse.Content.ReadFromJsonAsync<Guid>();

        var listagem = await _client.GetFromJsonAsync<List<UsuarioResponse>>("/api/v1/usuarios");
        listagem.Should().Contain(usuario => usuario.Id == usuarioId);

        var tipoResponse = await _client.PatchAsJsonAsync($"/api/v1/usuarios/{usuarioId}/tipo",
            new AlterarTipoRequest("Mecanico"));
        tipoResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var funcionarioId = await CriarFuncionarioAsync();
        var vincularResponse = await _client.PatchAsJsonAsync($"/api/v1/usuarios/{usuarioId}/vincular-funcionario",
            new VincularFuncionarioRequest(funcionarioId));
        vincularResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var vinculado = await _client.GetFromJsonAsync<UsuarioResponse>($"/api/v1/usuarios/{usuarioId}");
        vinculado!.TipoUsuario.Should().Be("Mecanico");
        vinculado.FuncionarioId.Should().Be(funcionarioId);

        var desvincularResponse = await _client.PatchAsync($"/api/v1/usuarios/{usuarioId}/desvincular-funcionario", null);
        desvincularResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var desativarResponse = await _client.PatchAsync($"/api/v1/usuarios/{usuarioId}/desativar", null);
        desativarResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var desativado = await _client.GetFromJsonAsync<UsuarioResponse>($"/api/v1/usuarios/{usuarioId}");
        desativado!.Ativo.Should().BeFalse();

        var ativarResponse = await _client.PatchAsync($"/api/v1/usuarios/{usuarioId}/ativar", null);
        ativarResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var resetarSenhaResponse = await _client.PatchAsJsonAsync($"/api/v1/usuarios/{usuarioId}/resetar-senha",
            new ResetarSenhaRequest(senhaNova));
        resetarSenhaResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequest { Login = login, Senha = senhaNova });
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeveRejeitarTipoDeUsuarioInvalido()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/usuarios", new CriarUsuarioRequest
        {
            Login = $"usuario.invalido.{Guid.NewGuid():N}",
            Senha = GerarSenha(),
            TipoUsuario = "PerfilInexistente"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<Guid> CriarFuncionarioAsync()
    {
        var sequencia = Interlocked.Increment(ref _sequencia);
        var response = await _client.PostAsJsonAsync("/api/v1/funcionarios", new CriarFuncionarioRequest
        {
            Nome = "Funcionário vinculado",
            Cpf = GerarCpf(sequencia),
            Rg = $"RG{sequencia:D7}",
            Cargo = TipoFuncionario.Mecanico,
            Endereco = new EnderecoRequest
            {
                Logradouro = "Rua Teste",
                Complemento = "Casa",
                Numero = "1",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP",
                Cep = "01001000"
            }
        });
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Created, body);
        return await response.Content.ReadFromJsonAsync<Guid>();
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

    private static string GerarSenha() => $"Aa1!{Guid.NewGuid():N}";
}
