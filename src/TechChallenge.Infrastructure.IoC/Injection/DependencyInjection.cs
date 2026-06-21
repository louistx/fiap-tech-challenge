using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechChallenge.Application.Features.OS.AtribuirOS;
using TechChallenge.Application.Features.OS.CriarOS;
using TechChallenge.Application.Features.OS.ListarOS;
using TechChallenge.Application.Features.OS.ListarOSOficina;
using TechChallenge.Application.Features.OS.RegistrarDiagnostico;
using TechChallenge.Application.Features.Servicos;
using TechChallenge.Application.Features.Veiculos;
using TechChallenge.Infrastructure.Abstractions.Repositories;
using TechChallenge.Infrastructure.Database.Context;
using TechChallenge.Infrastructure.Database.Repositories;

namespace TechChallenge.Infrastructure.IoC.Injection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            });

            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IEnderecoRepository, EnderecoRepository>();
            services.AddScoped<IFuncionarioRepository, FuncionarioRepository>();
            services.AddScoped<IOrdemServicoRepository, OrdemServicoRepository>();
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IServicoRepository, ServicoRepository>();
            services.AddScoped<IVeiculoRepository, VeiculoRepository>();

            services.AddScoped<CriarOSService>();
            services.AddScoped<ListarOSService>();
            services.AddScoped<AtribuirOSService>();
            services.AddScoped<RegistrarDiagnosticoService>();
            services.AddScoped<ListarOSOficinaService>();

            services.AddScoped<CriarServicoService>();
            services.AddScoped<ObterServicoService>();
            services.AddScoped<ListarServicosService>();
            services.AddScoped<AtualizarServicoService>();
            services.AddScoped<ExcluirServicoService>();

            services.AddScoped<CriarVeiculoService>();
            services.AddScoped<ObterVeiculoService>();
            services.AddScoped<ListarVeiculosService>();
            services.AddScoped<AtualizarVeiculoService>();
            services.AddScoped<ExcluirVeiculoService>();

            return services;
        }
    }
}