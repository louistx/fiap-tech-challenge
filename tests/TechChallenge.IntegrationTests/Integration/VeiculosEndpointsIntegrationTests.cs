using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TechChallenge.Api.Models.Response;

namespace TechChallenge.Api.Tests.Integration;

public class VeiculosEndpointsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public VeiculosEndpointsIntegrationTests(
            WebApplicationFactory<Program> factory
        )
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    
    [Fact]
    public void Test()
    {
        // arrange
        // act
        var response = _client.PostAsync("/veiculos", null);
        // assert
        
    }
}