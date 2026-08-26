# Roteiro do Vídeo Demonstrativo - Fase 2

## Objetivo

O vídeo deve ter no máximo 15 minutos e demonstrar deploy da aplicação, CI/CD, consumo das APIs e escalabilidade automática. Grave somente depois que o checklist de entregáveis estiver sem bloqueadores.

## Pré-requisitos da gravação

- [ ] commit/tag final identificado;
- [ ] workflows verdes;
- [ ] imagem publicada no GHCR com tag imutável;
- [ ] cluster e banco provisionados por Terraform;
- [ ] Deployment, Service, ConfigMap, Secret e HPA aplicados;
- [ ] Swagger ou collection acessível;
- [ ] dados de demonstração sem segredos reais;
- [ ] ferramenta de carga e painéis/terminais preparados.

## Sequência sugerida

| Tempo | Demonstração |
| --- | --- |
| 0:00-1:00 | Problema, objetivos da Fase 2 e arquitetura |
| 1:00-3:00 | Estrutura Clean Architecture, Dockerfile, K8s e Terraform |
| 3:00-5:00 | Execução da pipeline: build, testes e imagem |
| 5:00-7:00 | Terraform e aplicação dos manifests; rollout saudável |
| 7:00-11:00 | APIs: abertura completa da OS, status, decisão externa e listagem priorizada |
| 11:00-13:00 | Estoque: entrada, diagnóstico/orçamento e baixa sem saldo negativo |
| 13:00-14:30 | Geração de carga e aumento/redução de réplicas pelo HPA |
| 14:30-15:00 | Resumo, repositório, documentação e conclusão |

## Evidências que devem aparecer

- SHA/tag do commit e imagem usada;
- resultado dos testes;
- `terraform plan/apply` ou evidência do ambiente já aplicado;
- pods prontos, Service com endpoints e HPA com métricas;
- requisições e respostas das APIs obrigatórias;
- ordenação correta das OS e exclusão lógica de finalizadas/entregues;
- notificação externa de mudança de status;
- escala automática durante carga e retorno posterior.

Nunca mostre tokens, secrets, senhas, connection strings ou dados pessoais reais durante a gravação.

## Situação atual

O roteiro não é executável integralmente no commit `3a41407`: a suíte está vermelha, Estoque bloqueia o EF Core, K8s está incompleto, Terraform está ausente e não há deploy na pipeline. Esses pontos estão detalhados no [Checklist de Entregáveis](fase-2-entregaveis.md).
