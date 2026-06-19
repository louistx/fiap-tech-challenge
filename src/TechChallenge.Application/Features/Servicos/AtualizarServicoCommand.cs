namespace TechChallenge.Application.Features.Servicos;

public class AtualizarServicoCommand
{
    public Guid Id { get; set; }
    public string Descricao { get; set; }
    public decimal Valor { get; set; }
}
