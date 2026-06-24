using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechChallenge.Domain.Entities;

namespace TechChallenge.Infrastructure.Database.Configuration
{
    public class OrdemServicoConfiguration : IEntityTypeConfiguration<OrdemServico>
    {
        public void Configure(EntityTypeBuilder<OrdemServico> builder)
        {
            builder.HasKey(os => os.Id);

            builder.Property(o => o.Descricao)
                .HasMaxLength(500);

            builder.HasOne(os => os.ClienteResponsavel)
                .WithMany()
                .HasForeignKey(os => os.ClienteResponsavelId)
                .IsRequired();

            builder.HasOne(os => os.FuncionarioResponsavel)
                .WithMany()
                .HasForeignKey(os => os.FuncionarioResponsavelId)
                .IsRequired();

            builder.HasOne(os => os.Veiculo)
                .WithMany()
                .HasForeignKey(os => os.VeiculoId)
                .IsRequired();

            builder.Property(os => os.Status)
                .IsRequired();

            builder.Property(os => os.DataCriacao)
                .IsRequired();

            builder.Property(os => os.DataAtualizacao)
                .IsRequired(false);

            builder.Property(os => os.DataFinalizacao)
                .IsRequired(false);

            builder.HasMany(os => os.Servicos);
            builder.HasMany(os => os.Produtos);

            builder.Property(os => os.Valor);
            builder.Property(os => os.Desconto);
            builder.Property(os => os.Acrescimo);
        }
    }
}