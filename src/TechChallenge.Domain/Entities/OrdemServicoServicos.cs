namespace TechChallenge.Domain.Entities
{
    public class OrdemServicoServicos
    {
        public Guid Id { get; set; }
        public Guid OrdemServicoId { get; set; }
        public Servico Servico { get; set; }
        public double Valor { get; set; }
        public double Desconto { get; set; }
        public double Acrescimo { get; set; }
    }
}