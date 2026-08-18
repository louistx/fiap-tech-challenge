namespace TechChallenge.Domain.Entities
{
    public class Estoque
    {
        public Guid Id { get; private set; }
        public Guid ProdutoId { get; private set; }
        public Produto Produto { get; private set; } = null!;
        public double Quantidade { get; private set; }

        public Estoque(Guid id, Guid idProduto, double quantidade)
        {
            Id = id;
            ProdutoId = idProduto;
            Quantidade = quantidade;
        }

        public void AtualizarQuantidade(double quantidade)
        {
            Quantidade = quantidade;
        }
    }
}