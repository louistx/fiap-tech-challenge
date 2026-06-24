using System;
namespace TechChallenge.Domain.Entities
{
    public class OrdemServicoProdutos
    {
        public Guid Id { get; set; }
        public Guid OrdemServicoId { get; set; }
        public OrdemServico OrdemServico { get; set; } = null!;
        public Guid ProdutoId { get; set; }
        public Produto Produto { get; set; } = null!;
        public double Valor { get; set; }
        public double Desconto { get; set; }
        public double Acrescimo { get; set; }
    }
}
