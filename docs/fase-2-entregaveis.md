# Fase 2 - Checklist de Entregáveis

Este checklist compara os requisitos do PDF **SOAT - Fase 2 - Tech challenge** com o estado verificado no commit `3a41407`, em 25/08/2026.

## Legenda

- [x] Concluído e localizado ou validado no repositório.
- [ ] Pendente, parcial ou bloqueado.

Quando um item estiver parcialmente desenvolvido, ele permanece desmarcado e recebe a indicação **Parcial**. Assim, somente itens realmente prontos aparecem com check.

## 1. Evolução da aplicação

- [ ] Aplicar Clean Code em toda a solução. **Parcial.**
  - [x] Organização dos casos de uso por feature.
  - [x] Nomes de comandos, validadores e serviços relacionados ao domínio.
  - [ ] Substituir `GetAwaiter().GetResult()` por `async/await`.
  - [ ] Remover helpers duplicados.
  - [ ] Resolver avisos de nulabilidade e dependências não inicializadas.

- [ ] Consolidar Clean Architecture ou Arquitetura Hexagonal. **Parcial.**
  - [x] Projetos separados para API, aplicação, abstrações, domínio e infraestrutura.
  - [x] Repositórios definidos por interfaces na camada de abstrações.
  - [ ] Remover acoplamentos indevidos entre autenticação, banco e aplicação.
  - [ ] Garantir que regras de negócio não dependam de detalhes de persistência.

- [ ] Concluir a refatoração das entidades e acessores. **Bloqueado.**
  - [x] Entidades refatoradas com `private set`.
  - [x] Métodos de alteração adicionados às entidades.
  - [ ] Corrigir o construtor de `Estoque`, incompatível com o EF Core.
  - [ ] Proteger invariantes como saldo não negativo.
  - [ ] Padronizar os tipos usados para quantidade.

- [ ] Manter testes automatizados verdes para os fluxos críticos. **Bloqueado.**
  - [x] Projeto de testes unitários existente.
  - [x] Projeto de testes de integração existente.
  - [x] 94 testes unitários aprovados na última verificação.
  - [ ] Corrigir os 8 testes unitários falhos.
  - [ ] Corrigir os 20 testes de integração falhos.

## 2. APIs obrigatórias

### Abertura da Ordem de Serviço

- [ ] Receber cliente, veículo, serviços e peças na abertura da OS. **Parcial.**
  - [x] `POST /api/v1/ordens-servico` existe.
  - [x] A API retorna o identificador único da OS.
  - [x] Cliente e veículo são recebidos por identificador.
  - [ ] Incluir serviços no contrato de abertura.
  - [ ] Incluir peças/produtos no contrato de abertura.
  - [ ] Definir se cliente e veículo serão criados ou apenas referenciados.

### Consulta do status

- [ ] Disponibilizar uma rota exclusiva para consultar o status da OS. **Bloqueado.**
  - [x] Handler de consulta de status foi criado.
  - [ ] Remover o conflito entre os dois handlers `GET /api/v1/ordens-servico/{id}`.
  - [ ] Adotar uma rota não ambígua, como `GET /api/v1/ordens-servico/{id}/status`.
  - [ ] Adicionar testes HTTP para todos os estados obrigatórios.

### Aprovação ou recusa externa

- [ ] Criar endpoint para receber aprovação ou recusa de um sistema externo.
  - [x] Aprovação administrativa interna existe.
  - [x] Reprovação administrativa interna existe.
  - [ ] Criar callback/webhook externo.
  - [ ] Definir autenticação do integrador.
  - [ ] Implementar idempotência e correlação.
  - [ ] Testar notificações repetidas, inválidas e fora de ordem.

### Listagem priorizada

- [ ] Ordenar a listagem conforme a prioridade exigida. **Parcial.**
  - [x] Finalizadas e entregues são removidas da listagem operacional.
  - [x] `DataCriacao` é usada como critério de desempate.
  - [ ] Corrigir a ordenação booleana atualmente invertida.
  - [ ] Garantir a sequência `EmExecucao > AguardandoAprovacao > EmDiagnostico > Recebida`.
  - [ ] Exibir as mais antigas primeiro dentro de cada prioridade.
  - [ ] Definir o tratamento de OS reprovadas e canceladas.
  - [ ] Adicionar testes unitários e de integração para a ordem final.

### Notificação de mudança de status

- [ ] Atualizar o cliente por e-mail ou ferramenta equivalente.
  - [x] Notificações internas por log existem.
  - [ ] Implementar integração externa para mudanças de status.
  - [ ] Implementar retentativa e tratamento de falha.
  - [ ] Evitar perda de notificação com fila ou padrão outbox.

## 3. Estoque e categorias

- [ ] Concluir a entidade e persistência de Estoque. **Bloqueado.**
  - [x] Entidade `Estoque` separada de `Produto`.
  - [x] Interface `IEstoqueRepository` criada.
  - [x] Implementação `EstoqueRepository` criada.
  - [x] Configuração inicial do EF Core criada.
  - [ ] Corrigir o binding entre o parâmetro `idProduto` e a propriedade `ProdutoId`.
  - [ ] Criar constraint única para `ProdutoId`.
  - [ ] Criar migration e atualizar o model snapshot.

- [ ] Corrigir a entrada de estoque. **Bloqueado.**
  - [x] Caso de uso e validador existem.
  - [x] Rota `POST /api/v1/estoque` existe.
  - [ ] Copiar `request.Quantidade` para o comando.
  - [ ] Aguardar a persistência assíncrona.
  - [ ] Proteger a atualização contra concorrência.

- [ ] Corrigir a consulta de estoque. **Bloqueado.**
  - [x] Caso de uso de consulta existe.
  - [ ] Alterar o verbo de `DELETE` para `GET`.
  - [ ] Alinhar `produtoId` entre rota, handler e repositório.
  - [ ] Retornar 404 quando o produto não possuir estoque.

- [ ] Corrigir a baixa manual de estoque. **Bloqueado.**
  - [x] Caso de uso e validador existem.
  - [ ] Consultar por `ProdutoId`, não pelo ID do registro de estoque.
  - [ ] Impedir saldo negativo na entidade.
  - [ ] Retornar `200 OK` ou `204 No Content`, em vez de `201 Created`.
  - [ ] Tornar a operação atômica e concorrente-segura.

- [ ] Corrigir a integração entre Produto, Categoria e Estoque. **Bloqueado.**
  - [x] Categorias de produto, serviço e veículo foram modeladas.
  - [x] Endpoints e casos de uso de categorias foram criados.
  - [ ] Incluir `IdCategoria` no request e no mapeamento de criação de produto.
  - [ ] Inicializar corretamente `_estoqueRepository` em `AtualizarItemInventarioService`.
  - [ ] Criar migrations para todas as categorias.
  - [ ] Adicionar testes para Estoque e Categorias.

Consulte a análise detalhada em [Auditoria do Endpoint de Estoque](auditoria-estoque.md).

## 4. Docker

- [ ] Entregar Dockerfile revisado e validado. **Parcial.**
  - [x] Dockerfile multi-stage existente.
  - [x] Imagem final configurada com usuário não-root.
  - [ ] Copiar `TechChallenge.Infrastructure.Auth.csproj` antes do `dotnet restore` da imagem.
  - [ ] Executar e registrar um build completo da imagem.
  - [ ] Executar smoke test do container produzido.

- [x] Manter Docker Compose para desenvolvimento local.
  - [x] API configurada.
  - [x] PostgreSQL configurado.
  - [x] SonarQube configurado.
  - [x] `docker compose config --quiet` validado em 25/08/2026.

## 5. Kubernetes

Os arquivos desta seção já existiam na `main`. Nenhum manifesto novo foi implementado durante a revisão documental.

- [ ] Entregar Deployment funcional. **Parcial.**
  - [x] Deployment presente em `/k8s`.
  - [x] Base renderiza com `kubectl kustomize k8s/base`.
  - [ ] Adicionar readiness probe.
  - [ ] Adicionar liveness probe.
  - [ ] Definir requests e limits de CPU/memória.

- [ ] Entregar Service funcional. **Bloqueado.**
  - [x] Service presente.
  - [ ] Corrigir o selector `fiap-tech-challenge-api-webapi`, que não corresponde aos labels dos pods.
  - [ ] Validar endpoints e acesso à API dentro do cluster.

- [ ] Criar ConfigMap.
  - [ ] Separar configurações não sensíveis da imagem.
  - [ ] Referenciar o ConfigMap no Deployment.

- [ ] Criar Secret.
  - [ ] Fornecer conexão do banco e segredo JWT sem versionar valores reais.
  - [ ] Referenciar o Secret no Deployment.

- [ ] Criar HPA por CPU e/ou memória.
  - [ ] Definir métricas e limites de escala.
  - [ ] Instalar/validar metrics-server.
  - [ ] Demonstrar aumento e redução de réplicas.

- [ ] Disponibilizar banco para o ambiente Kubernetes.
  - [ ] Escolher banco gerenciado ou execução no cluster.
  - [ ] Configurar persistência, backup e conexão.
  - [ ] Definir estratégia segura para migrations.

- [ ] Corrigir o overlay local. **Bloqueado.**
  - [ ] Criar o `namespace.yaml` referenciado.
  - [ ] Informar o nome da imagem no bloco `images`.
  - [ ] Validar `kubectl kustomize k8s/overlays/docker-local`.

## 6. Terraform

- [ ] Criar o diretório `/infra`.
- [ ] Definir o provedor local ou cloud.
- [ ] Provisionar o cluster Kubernetes.
- [ ] Provisionar o PostgreSQL.
- [ ] Configurar rede, variáveis e outputs.
- [ ] Configurar estado remoto e locking.
- [ ] Documentar `init`, `validate`, `plan` e `apply` com comandos executáveis.
- [ ] Validar o provisionamento em um ambiente de demonstração.

A arquitetura e a estrutura planejada estão em [Arquitetura Proposta - Fase 2](arquitetura-fase-2.md). Os scripts ainda não existem.

## 7. CI/CD

- [x] Executar build da aplicação em GitHub Actions.
  - [x] Workflow `build.yml` presente.
  - [x] Execução configurada para push na `main` e acionamento manual.

- [ ] Executar testes automaticamente como gate. **Parcial.**
  - [x] Workflow de testes unitários presente.
  - [x] Workflow de testes de integração presente.
  - [ ] Executar os testes automaticamente em pull request/push.
  - [ ] Impedir deploy quando qualquer teste falhar.
  - [ ] Recuperar a suíte atualmente vermelha.

- [ ] Automatizar build e publicação da imagem. **Parcial.**
  - [x] Workflow `docker-image.yml` presente.
  - [x] Publicação no GHCR configurada.
  - [ ] Integrar a publicação ao pipeline principal.
  - [ ] Publicar tag imutável e promover somente imagem validada.

- [ ] Automatizar o deploy no cluster Kubernetes.
- [ ] Automatizar o provisionamento/deploy do banco.
- [ ] Executar Terraform com revisão do plano e aprovação do ambiente.
- [ ] Aplicar manifests/Kustomize pela pipeline.
- [ ] Validar rollout e smoke test após o deploy.

## 8. README e documentação

- [x] Atualizar a descrição da solução e os objetivos da Fase 2.
- [x] Documentar os componentes da aplicação.
- [x] Documentar a infraestrutura proposta.
- [x] Documentar o fluxo de deploy proposto.
- [x] Manter instruções de execução local.
- [ ] Disponibilizar instruções executáveis de deploy Kubernetes. **Parcial:** procedimento-alvo documentado, manifests ainda incompletos.
- [ ] Disponibilizar instruções executáveis de Terraform. **Parcial:** estrutura planejada documentada, scripts ausentes.
- [ ] Publicar uma collection completa das APIs.
  - [x] Swagger UI gerado em runtime.
  - [x] OpenAPI JSON gerado em runtime.
  - [ ] Publicar URL estável ou versionar collection Postman.

## 9. Vídeo e entrega no portal

- [x] Preparar o [Roteiro do Vídeo da Fase 2](roteiro-video-fase-2.md).
- [ ] Gravar vídeo de até 15 minutos.
  - [ ] Demonstrar deploy da aplicação.
  - [ ] Demonstrar execução do CI/CD.
  - [ ] Demonstrar consumo das APIs.
  - [ ] Demonstrar escalabilidade automática com HPA.
  - [ ] Publicar no YouTube ou Vimeo como público ou não listado.
  - [ ] Inserir o link no README e no documento final.

- [x] Preparar a fonte do [Documento de Entrega da Fase 2](entrega-fase-2.md).
- [ ] Gerar e revisar o PDF final do portal.
  - [ ] Inserir link do repositório.
  - [ ] Inserir o desenho da arquitetura realmente implantada.
  - [ ] Inserir o link do vídeo.
  - [ ] Validar visualmente todas as páginas e links.

- [ ] Confirmar que o repositório privado foi compartilhado com `soat-architecture`.

## 10. Bloqueadores prioritários

- [ ] Corrigir o modelo EF Core e criar as migrations de Estoque/Categorias.
- [ ] Corrigir contratos, rotas, invariantes e testes do Estoque.
- [ ] Recuperar os 8 testes unitários e os 20 testes de integração.
- [ ] Corrigir a rota de status e a ordenação das OS.
- [ ] Completar a abertura da OS e a integração externa de decisão/notificação.
- [ ] Completar e validar Kubernetes.
- [ ] Implementar Terraform.
- [ ] Integrar a pipeline de entrega.
- [ ] Publicar a collection, gravar o vídeo e gerar o PDF final.
