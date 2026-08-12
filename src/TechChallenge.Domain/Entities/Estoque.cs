namespace TechChallenge.Domain.Entities
{
    public class Estoque
    {
        public Guid Id { get; private set; }
        public Guid IdProduto { get; private set; }
        public Produto Produto { get; private set; } = null!;
        public int Quantidade { get; private set; }

        public Estoque(Guid id, Guid idProduto, int quantidade)
        {
            Id = id;
            IdProduto = idProduto;
            Quantidade = quantidade;
        }

        public void AtualizarQuantidade(int quantidade)
        {
            Quantidade = quantidade;
        }
    }
}