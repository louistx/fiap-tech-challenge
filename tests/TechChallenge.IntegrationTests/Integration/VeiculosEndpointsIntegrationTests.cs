using FluentAssertions;
using System.Net;
using System.Text;
using TechChallenge.IntegrationTests.Integration.Factories;

namespace TechChallenge.IntegrationTests.Integration;

public class VeiculosEndpointsIntegrationTests : IClassFixture<WebAplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public VeiculosEndpointsIntegrationTests(WebAplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task DeveRetornarBadRequestQuandoCriarVeiculoSemBody()
    {
        var response = await _client.PostAsync("/api/v1/veiculos", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeveRetornarBadRequestQuandoCriarVeiculoComBodyVazio()
    {
        using var content = new StringContent("{}", Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/v1/veiculos", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
