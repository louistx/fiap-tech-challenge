namespace TechChallenge.Application.Features.Inventario;

public class CriarItemInventarioCommand
{
    public string Nome { get; set; }
    public int Quantidade { get; set; }
    public decimal Preco { get; set; }
}