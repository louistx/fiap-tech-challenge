# Testes Automatizados

## Visão geral

O projeto utiliza testes automatizados para validar as regras de domínio, os serviços de aplicação, a segurança e o comportamento HTTP da API. A suíte é dividida entre testes unitários e testes de integração.

No levantamento atual, existem 61 declarações de testes unitários e 16 declarações de testes de integração.

## Organização

Os testes estão distribuídos em dois projetos:

| Projeto | Responsabilidade |
| --- | --- |
| `TechChallenge.Tests` | Testes unitários do domínio, validadores, serviços de aplicação e componentes de segurança |
| `TechChallenge.IntegrationTests` | Testes da API, autenticação, autorização, persistência e fluxos completos entre endpoints |

As principais ferramentas utilizadas são:

- **xUnit:** estrutura e execução dos testes;
- **Moq:** criação de dependências simuladas nos testes unitários;
- **FluentAssertions:** escrita das verificações de resultado;
- **WebApplicationFactory:** inicialização da API em memória nos testes de integração;
- **SQLite em memória:** banco isolado utilizado durante os testes de integração;
- **Coverlet:** coleta de cobertura de código.

## Testes unitários

Os testes unitários executam regras isoladas, substituindo repositórios e outras dependências por objetos simulados quando necessário.

### Domínio e Ordem de Serviço

Os principais cenários cobertos são:

- transições válidas e inválidas da máquina de estados;
- registro da data de finalização;
- atribuição da OS e limite de uma OS ativa por mecânico;
- registro e validação do diagnóstico;
- cálculo e envio do orçamento;
- aprovação e reprovação do orçamento;
- retorno de uma OS reprovada para diagnóstico;
- finalização, entrega, cancelamento e exclusão;
- listagem geral, filtro por estado e listagem para a oficina.

### Cadastros e validações

Os testes verificam:

- criação de cliente e rejeição de CPF duplicado ou inválido;
- criação de veículo e vínculo com cliente existente;
- validação e normalização de placas antigas e Mercosul;
- rejeição de placa duplicada, ano e valores inválidos;
- criação de usuário, login duplicado e vínculo com funcionário.

### Autenticação e segurança

São validados:

- login com credenciais válidas e inválidas;
- bloqueio de usuário inativo;
- criação e rotação de refresh tokens;
- rejeição de refresh token inexistente ou revogado;
- geração e verificação de hash PBKDF2;
- geração das claims do access token;
- geração e hash de refresh tokens.

## Testes de integração

Os testes de integração inicializam a aplicação por meio de `WebApplicationFactory<Program>` e exercitam os endpoints com requisições HTTP reais dentro do processo de teste.

Durante a execução:

- o ambiente da aplicação é alterado para `Testing`;
- PostgreSQL é substituído por SQLite em memória;
- o esquema é recriado para manter os testes isolados;
- a autenticação JWT é substituída por um esquema de teste;
- o perfil padrão é `Administrador`, podendo ser alterado por cabeçalho nos testes de autorização.

Os fluxos atualmente cobertos incluem:

- CRUD e validação de clientes;
- CRUD e validação de funcionários;
- CRUD e validação de serviços;
- validação das requisições de veículos;
- criação e listagem de usuários;
- login e rotação de refresh token;
- retorno `Unauthorized` para senha incorreta;
- execução do ciclo principal da OS até a entrega;
- bloqueio de operações quando o perfil não possui autorização;
- respostas padronizadas com `ProblemDetails`.

> Os testes de integração validam a aplicação com Entity Framework e SQLite em memória. Eles não substituem testes específicos de compatibilidade e operação com uma instância real do PostgreSQL.

## Como executar

Para executar toda a suíte a partir da raiz do repositório:

```bash
dotnet test TechChallenge.slnx
```

Para executar somente os testes unitários:

```bash
dotnet test tests/TechChallenge.Tests/TechChallenge.Tests.csproj
```

Para executar somente os testes de integração:

```bash
dotnet test tests/TechChallenge.IntegrationTests/TechChallenge.IntegrationTests.csproj
```

Para executar sem restaurar novamente as dependências:

```bash
dotnet test TechChallenge.slnx --no-restore
```

## Cobertura de código

O Coverlet está configurado nos dois projetos de teste. Para gerar a cobertura no formato padrão do coletor:

```bash
dotnet test TechChallenge.slnx \
  --collect:"XPlat Code Coverage"
```

Também é possível gerar relatórios no formato OpenCover para análise no SonarQube:

```bash
dotnet test tests/TechChallenge.Tests/TechChallenge.Tests.csproj \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=opencover \
  /p:CoverletOutput=../../.sonar/coverage/unit/

dotnet test tests/TechChallenge.IntegrationTests/TechChallenge.IntegrationTests.csproj \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=opencover \
  /p:CoverletOutput=../../.sonar/coverage/integration/
```

A meta do projeto é atingir pelo menos 80% de cobertura nos fluxos críticos. A existência de cobertura não substitui a validação da qualidade e da relevância dos cenários testados.

## Integração contínua

O repositório possui workflows separados para testes unitários e de integração:

- `.github/workflows/unit-tests.yml`;
- `.github/workflows/integration-tests.yml`.

Atualmente, ambos são iniciados manualmente por `workflow_dispatch`. O build da solução possui um workflow separado executado em atualizações da branch `main`.

## Situação atual

A estrutura de testes unitários e de integração está implementada e as duas suítes executam com sucesso. A cobertura ainda deve ser ampliada, principalmente para:

- CRUD completo de veículos nos testes de integração;
- endpoints de produtos e inventário;
- estados alternativos da OS pela API, incluindo reprovação, retorno para diagnóstico e cancelamento;
- autorização dos demais endpoints e perfis;
- execução com PostgreSQL real;
- fluxos futuros de reserva de estoque, notificações externas, pagamento e recibo.
