using Microsoft.EntityFrameworkCore;
using TechChallenge.Domain.Entities;

namespace TechChallenge.Infrastructure.Database.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Endereco> Endereco { get; set; }
        public DbSet<Funcionario> Funcionario { get; set; }
        public DbSet<OrdemServico> OrdemServico { get; set; }
        public DbSet<Produto> Produto { get; set; }
        public DbSet<Servico> Servico { get; set; }
        public DbSet<Veiculo> Veiculo { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<OrdemServicoProdutos> OrdemServicoProdutos { get; set; }
        public DbSet<OrdemServicoServicos> OrdemServicoServicos { get; set; }
        public DbSet<CategoriaProduto> CategoriaProduto { get; set; }
        public DbSet<CategoriaServico> CategoriaServico { get; set; }
        public DbSet<CategoriaVeiculo> CategoriaVeiculo { get; set; }
        public DbSet<Estoque> Estoque { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}