using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechChallenge.Application.Abstractions.Auth;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;
using TechChallenge.Infrastructure.Database.Context;

namespace TechChallenge.Infrastructure.Database.Seeding
{
    public static class DataSeeder
    {
        // Cria o usuário administrador inicial se não houver nenhum usuário.
        // Login/senha vêm de configuração (Seed:AdminLogin / Seed:AdminPassword), nunca hardcoded.
        public static async Task SeedAdminAsync(IServiceProvider provider)
        {
            using var scope = provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            if (await context.Usuario.AnyAsync())
                return;

            var login = configuration["Seed:AdminLogin"] ?? "admin";
            var senha = configuration["Seed:AdminPassword"];

            if (string.IsNullOrWhiteSpace(senha))
                return; // sem senha de bootstrap configurada, não cria admin

            var admin = new Usuario
            {
                Id = Guid.NewGuid(),
                Login = login,
                PasswordHash = hasher.Hash(senha),
                TipoUsuario = eTipoUsuario.Administrador,
                Ativo = true
            };

            context.Usuario.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
