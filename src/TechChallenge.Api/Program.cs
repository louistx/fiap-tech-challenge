using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using TechChallenge.Api.Endpoints;
using TechChallenge.Api.Middleware;
using TechChallenge.Infrastructure.Auth;
using TechChallenge.Infrastructure.Database.Context;
using TechChallenge.Infrastructure.Database.Seeding;
using TechChallenge.Infrastructure.IoC.Injection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Informe o token JWT."
        };

        document.Security =
        [
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
            }
        ];

        return Task.CompletedTask;
    });
});
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuth(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }
    await DataSeeder.SeedAdminAsync(app.Services);
}

app.UseMiddleware<ExceptionMiddleware>();

app.MapOpenApi().AllowAnonymous();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "TechChallenge API");
});
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapUsuariosEndpoints();
app.MapClientesEndpoints();
app.MapFuncionariosEndpoints();
app.MapVeiculosEndpoints();
app.MapServicosEndpoints();
app.MapInventarioEndpoints();
app.MapOrdensServicoEndpoints();

app.Run();

public partial class Program;
