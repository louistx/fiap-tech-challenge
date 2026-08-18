namespace TechChallenge.Application.Features.Estoque.BaixarEstoque;

public class BaixarEstoqueCommand
{
    public Guid ProdutoId { get; set; }
    public double Quantidade { get; set; }
}