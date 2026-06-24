namespace TechChallenge.Domain.Entities
{
    public class OrdemServicoProdutos
    {
        public Guid Id { get; set; }
        public Guid OrdemServicoId { get; set; }
        public Produto Produto { get; set; }
        public double Valor { get; set; }
        public double Desconto { get; set; }
        public double Acrescimo { get; set; }
    }
}