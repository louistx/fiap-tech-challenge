using System;
namespace TechChallenge.Application.Features.Auth.Sessoes;

public record SessaoDto(
    Guid SessaoId,
    DateTime CriadoEm,
    DateTime ExpiraEm,
    string? UserAgent,
    string? IpCriacao);
