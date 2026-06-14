using System;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Domain.Entities;

public class Funcionario
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public string Rg { get; set; }
    public eTipoFuncionario TipoFuncionario { get; set; }
    public Guid? EnderecoId { get; set; }
    public Endereco Endereco { get; set; }
}