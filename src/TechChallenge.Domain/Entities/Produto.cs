namespace TechChallenge.Domain.Entities;

public class Produto
{
    public Guid Id { get; private set; }
    public string Descricao { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public Guid CategoriaId { get; private set; }
    public CategoriaProduto Categoria { get; private set; } = null!;

    public Produto(Guid id, string descricao, decimal valor, Guid categoria)
    {
        Id = id;
        Descricao = descricao;
        Valor = valor;
        CategoriaId = categoria;
    }
}