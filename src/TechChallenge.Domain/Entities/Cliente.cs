using System;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Domain.Entities;

public class Cliente
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public TipoDocumento TipoDocumento { get; private set; }
    public string Documento { get; private set; } = string.Empty;
    public Guid EnderecoId { get; private set; }
    public Endereco Endereco { get; private set; } = null!;

    public Cliente(Guid id, string nome, TipoDocumento tipoDocumento, string documento, Guid enderecoId)
    {
        Id = id;
        Nome = nome;
        TipoDocumento = tipoDocumento;
        Documento = documento;
        EnderecoId = enderecoId;
    }

    public void AtribuirEndereco(Endereco endereco)
    {
        Endereco = endereco;
        EnderecoId = endereco.Id;
    }

    public void Atualizar(string nome, TipoDocumento tipoDocumento, string documento)
    {
        Nome = nome;
        TipoDocumento = tipoDocumento;
        Documento = documento;
    }
}
