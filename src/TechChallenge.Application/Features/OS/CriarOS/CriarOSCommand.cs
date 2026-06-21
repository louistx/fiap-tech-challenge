namespace TechChallenge.Application.Features.OS.CriarOS;

public class CriarOSCommand
{
    public string Descricao { get; set; }
    public Guid ClienteResponsavelId { get; set; }
    public Guid FuncionarioResponsavelId { get; set; }
    public Guid VeiculoId { get; set; }
}
