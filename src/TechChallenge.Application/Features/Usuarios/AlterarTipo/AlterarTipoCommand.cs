using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Features.Usuarios.AlterarTipo;

public class AlterarTipoCommand
{
    public Guid UsuarioId { get; set; }
    public eTipoUsuario TipoUsuario { get; set; }
}
