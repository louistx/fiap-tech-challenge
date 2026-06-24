using System;
using FluentValidation;
using TechChallenge.Domain.Entities;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Servicos.CriarServico;

public class CriarServicoService
{
    private readonly IServicoRepository _servicoRepository;
    private readonly IValidator<CriarServicoCommand> _validator;

    public CriarServicoService(IServicoRepository servicoRepository, IValidator<CriarServicoCommand> validator)
    {
        _servicoRepository = servicoRepository;
        _validator = validator;
    }

    public Guid CriarServico(CriarServicoCommand command)
    {
        _validator.ValidateAndThrow(command);

        var servico = new Servico
        {
            Id = Guid.NewGuid(),
            Descricao = command.Descricao,
            Valor = command.Valor
        };

        _servicoRepository.AddAsync(servico).GetAwaiter().GetResult();
        return servico.Id;
    }
}
