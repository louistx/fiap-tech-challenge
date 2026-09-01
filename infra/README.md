# Infraestrutura local com Terraform

O cluster Kubernetes **já existe no Docker Desktop**. Este diretório não cria cluster,
VM, rede de nuvem ou banco gerenciado. Terraform e Kustomize ficam no mesmo
repositório, mas cada recurso tem um único responsável.

Resultados dos testes reais em 27/08/2026: [relatório de validação](../docs/validacao-infra-local.md).

| Responsável | Recursos |
| --- | --- |
| Docker Desktop | Cluster, nós, rede e StorageClass |
| Terraform | Namespace `techchallenge`, PostgreSQL, Service interno do banco, PVC, Secrets e Metrics Server |
| Kustomize | ConfigMap, Deployment, Service e HPA da API |
| GitHub Actions | Build e publicação da imagem no GHCR |
| Aplicação | Migrations e seed na inicialização, antes de servir HTTP |

Os arquivos `.tf` estão em `environments/local/`. O contexto `docker-desktop` é
explícito nos providers e comandos; não é necessário executar `kubectl config use-context`.
Não há `local-exec` ou `kubectl apply` escondido no Terraform.

Em `k8s/`, a base contém Deployment, Service e ConfigMap vazio.
O patch com os valores do ConfigMap e a política de HPA pertencem ao ambiente em
[`k8s/overlays/docker-local`](../k8s/overlays/docker-local). Um único overlay reúne
API e configuração; não há generator, Job de migrations ou scripts auxiliares.

## Pré-requisitos

- Docker Desktop com Kubernetes habilitado e nós `Ready`.
- Kubernetes **1.34 ou superior**, compatível com o Metrics Server 0.9.0 do chart fixado.
- Terraform >= 1.7 e < 2 e `kubectl` com Kustomize.
- StorageClass padrão com provisionamento dinâmico de volumes.
- Internet para baixar providers, chart, imagens e dependências do build.
- Permissão administrativa no cluster local para instalar os recursos globais do Metrics Server.

Não é necessário instalar o CLI Helm: o provider Helm faz a instalação.
Recomenda-se disponibilizar pelo menos 4 CPUs e 6 GiB ao Docker Desktop para a
demonstração; ajuste conforme os outros containers executados na máquina.

## 1. Verificar o ambiente

Execute os comandos a partir da raiz do repositório:

```bash
docker desktop start
docker --context=desktop-linux info
kubectl --context=docker-desktop get nodes
kubectl --context=docker-desktop get storageclass
kubectl --context=docker-desktop get apiservice v1beta1.metrics.k8s.io --ignore-not-found
```

Se o último comando encontrar um Metrics Server já mantido por outra ferramenta,
defina `install_metrics_server = false`. O Metrics Server serve o cluster inteiro,
embora o Deployment criado aqui fique no namespace do projeto. Não instalar dois.

## 2. Configurar e provisionar

```bash
cp infra/environments/local/terraform.tfvars.example infra/environments/local/terraform.tfvars
umask 077
terraform -chdir=infra/environments/local init
terraform -chdir=infra/environments/local fmt -check -recursive
terraform -chdir=infra/environments/local validate
terraform -chdir=infra/environments/local test
terraform -chdir=infra/environments/local plan -out=local.tfplan
terraform -chdir=infra/environments/local apply local.tfplan
```

Revise o plano antes de aplicar. Em um ambiente novo são **10 recursos Terraform**,
incluindo três geradores de senha e uma release Helm que cria vários objetos Kubernetes.

**TLS do Metrics Server:** o exemplo ativa `metrics_server_insecure_tls = true`
porque o kubelet do Docker Desktop testado usa certificado sem IP SAN válido.
Essa opção desativa apenas a validação do certificado entre Metrics Server e kubelet.
O padrão do módulo é `false`; mantenha `false` em clusters com certificados válidos.
Não reutilizar a exceção em produção. O TLS dos providers continua verificado.

O namespace é fixo (`techchallenge`) para coincidir com o overlay e os comandos.
As variáveis permitem selecionar a imagem PostgreSQL 17, o tamanho inicial do
volume e uma StorageClass existente. A imagem tem versão explícita; não usar `latest`.

## 3. Implantar a aplicação

O overlay usa `ghcr.io/louistx/fiap-tech-challenge:latest`. O workflow
`.github/workflows/docker-image.yml` publica `latest` e uma tag com os 12 primeiros
caracteres do SHA do commit. Para reproduzir uma versão, substitua
`images[].newTag` pela tag curta publicada
em `k8s/overlays/docker-local/kustomization.yaml` e aplique o overlay:

```bash
kubectl --context=docker-desktop apply -k k8s/overlays/docker-local
kubectl --context=docker-desktop -n techchallenge rollout status deployment/fiap-tech-challenge-api --timeout=300s
```

Um único apply configura a aplicação. A API executa migrations e seed antes de
abrir a porta HTTP. Se a inicialização falhar, o processo termina e o Kubernetes
tenta novamente. Não há Job, script de deploy ou build local obrigatório.
Terraform não aplica os manifests da aplicação.

A imagem publicada precisa incluir os endpoints de saúde usados nas probes;
alterar o YAML não atualiza o código da imagem. O workflow publica a imagem para
`linux/amd64`; no Docker Desktop em Macs com Apple Silicon, ela é executada por
emulação.

Se o pacote no GHCR for privado, configure `imagePullSecrets` com um Secret
gerenciado pelo Terraform. As credenciais do Docker no host não configuram a
autenticação dos pods. Não versionar tokens do registry.

## 4. Acessar e verificar

Em um terminal:

```bash
kubectl --context=docker-desktop -n techchallenge port-forward svc/fiap-tech-challenge-api-service 18080:8080
```

Em outro terminal:

```bash
curl --fail http://localhost:18080/health/live
curl --fail http://localhost:18080/health/ready
curl --fail http://localhost:18080/openapi/v1.json -o /dev/null
curl --fail http://localhost:18080/swagger/index.html -o /dev/null
kubectl --context=docker-desktop -n techchallenge get pods,svc,pvc,hpa
kubectl --context=docker-desktop -n techchallenge top pods
terraform -chdir=infra/environments/local plan
```

Acesse [Swagger](http://localhost:18080/swagger). O acesso fica restrito à máquina
local pelo port-forward; não há Ingress, TLS público ou banco exposto no host.
O overlay local desabilita o redirecionamento HTTPS da API para permitir esse acesso HTTP.

O último `terraform plan` deve mostrar ausência de mudanças. Isso não verifica
os recursos da API, que pertencem ao Kustomize.

### Encontrar a aplicação no Lens

Selecione o cluster **docker-desktop** e o namespace **techchallenge**. Os pods da
API possuem prefixo `fiap-tech-challenge-api-`; seus logs mostram as migrations
antes do início do servidor HTTP. Não confundir com uma implantação antiga no namespace
`default`: os Secrets pertencem ao namespace novo e não são injetados em pods antigos.

Em 27/08/2026 foi encontrada uma implantação antiga em `default`, com imagem GHCR
`latest`, sem conexão configurada e 350 reinícios. Ela foi reduzida a zero réplicas,
sem apagar sua configuração. O Deployment e o Service antigos não pertencem ao
Terraform local. O serviço de acesso deste guia é o de `techchallenge` na porta 18080.

Executar a API pela IDE ou por `dotnet run` é outro modo de uso: nesse caso é preciso
configurar `ConnectionStrings:DefaultConnection` via User Secrets ou variável de
ambiente, conforme o README principal. Secrets do Kubernetes não são exportados para
processos executados no host.

### Credenciais

As senhas do banco e do administrador, além da chave JWT, são geradas pelo Terraform.
O login inicial é `admin`. Para consultar sua senha **somente no terminal local**:

```bash
kubectl --context=docker-desktop -n techchallenge get secret techchallenge-api-secrets \
  -o 'jsonpath={.data.Seed__AdminPassword}' | base64 --decode; echo
```

Não cole senhas, tokens, state ou planos em commits, logs de CI ou gravações.
Os dados de demonstração e fictícios ficam desabilitados; o seed cria apenas o administrador.

## Migrations, saúde e configuração

- A API executa `MigrateAsync` e o seed antes de servir HTTP, exceto no ambiente `Testing`.
- Uma falha de inicialização encerra o processo; Kubernetes reinicia com backoff. Erros permanentes precisam de correção.
- `/health/live`: verifica se o processo responde; não depende do banco.
- `/health/ready`: consulta a tabela de usuários, verificando conectividade e presença do schema.
- `startupProbe` permite até cinco minutos para inicialização; requests/limits definem o orçamento de CPU e memória.
- O ConfigMap tem nome fixo, `techchallenge-api-config`; os valores vêm de `k8s/overlays/docker-local/configmap-patch.yaml`.
- Alterar a tag da imagem no overlay e aplicar inicia o rollout. Se mudar só o ConfigMap ou republicar `latest`, execute `rollout restart` para substituir os pods existentes.
- Em rollback, restaure também os valores do ConfigMap; `rollout undo` não versiona a configuração.

Docker Compose, `dotnet run` e Kubernetes usam o mesmo fluxo de inicialização.
EF Core 10 protege a execução de migrations concorrentes com locking. O seed
customizado verifica os registros existentes, mas está fora desse lock e pode
exigir nova tentativa em inicializações simultâneas de um banco vazio.
Migrations devem continuar compatíveis com a versão anterior da API durante um
rolling update. Esta escolha simplifica o trabalho escolar; uma implantação de
produção requer revisão das permissões da aplicação e da estratégia de migrations.

## Demonstrar HPA

O HPA escala **a API**, de 1 a 3 réplicas, com alvo de 70% do request de CPU de 100m
(aproximadamente 70m por pod). O PostgreSQL permanece com uma réplica.
Esses valores são definidos em
[`k8s/overlays/docker-local/hpa.yaml`](../k8s/overlays/docker-local/hpa.yaml).

```bash
kubectl --context=docker-desktop -n techchallenge top pods
kubectl --context=docker-desktop apply -f k8s/tests/hpa-load.yaml
kubectl --context=docker-desktop -n techchallenge get hpa,pods -w
```

O Job gera carga durante 120 segundos. Aguarde também a estabilização de redução
(60 segundos configurados, mais os ciclos de coleta e avaliação). O resultado depende
dos recursos disponíveis; um YAML válido sozinho não comprova que houve escala.
Para repetir, exclua apenas o Job de carga já concluído e aplique-o novamente.

## Persistência, backup e destruição

O PostgreSQL usa um PVC de 2 GiB. Recriar o pod preserva os dados, mas o volume
local **não substitui backup** e não oferece alta disponibilidade. Resetar o cluster
ou apagar dados do Docker Desktop pode perder o banco mesmo com proteções no Terraform.

Backup local:

```bash
mkdir -p .local/backups
umask 077
kubectl --context=docker-desktop -n techchallenge exec postgres-0 -- \
  sh -c 'pg_dump -U "$POSTGRES_USER" -d "$POSTGRES_DB" -Fc' > .local/backups/techchallenge.dump
```

Para testar a restauração, use um banco vazio separado; não sobrescreva o banco da aplicação:

```bash
kubectl --context=docker-desktop -n techchallenge exec postgres-0 -- \
  sh -c 'createdb -U "$POSTGRES_USER" TechChallengeRestore'
kubectl --context=docker-desktop -n techchallenge exec -i postgres-0 -- \
  sh -c 'pg_restore -U "$POSTGRES_USER" -d TechChallengeRestore --exit-on-error' < .local/backups/techchallenge.dump
```

O usuário do banco é também o usuário de bootstrap/migration, uma simplificação do
laboratório. Em produção, separar usuário de aplicação e usuário de migration.

Namespace e PVC têm `prevent_destroy = true`. Um `terraform destroy` é bloqueado
enquanto essas proteções existirem. Removê-las é uma decisão explícita após backup;
apagar o namespace também elimina os recursos criados pelo Kustomize. Para parar
temporariamente o laboratório, pode-se encerrar o Docker Desktop, lembrando que isso
afeta **todos** os containers e clusters locais.

Não substituir o gerador da senha do banco sem planejar sua rotação: alterar a variável
`POSTGRES_PASSWORD` não troca a senha de um banco já inicializado. Alterações em Secrets
consumidos por variáveis de ambiente exigem reinício dos pods consumidores.

## Estado e testes

O backend é local porque cada desenvolvedor usa seu próprio cluster. O state é sensível
e fica fora do Git, assim como planos, tfvars e backups; `.dockerignore` também os exclui
do contexto de build. `.terraform.lock.hcl` é versionado. Guarde o state com segurança:
perdê-lo exige recuperação/importação, não simplesmente aplicar de novo sobre o banco existente.

Os comandos `terraform fmt -check`, `validate`, `test` e `kubectl kustomize` validam
os arquivos e quatro cenários com providers mockados sem acessar o cluster.
A validação real exige `plan/apply`, rollout e requisições HTTP à API.

Para um futuro ambiente compartilhado, definir backend remoto com locking e controle
de acesso. A automação de deploy no GitHub Actions ainda não faz parte deste ambiente;
um runner hospedado não alcança o Kubernetes do computador sem conectividade adicional.

## Referências

- [Provider Kubernetes](https://developer.hashicorp.com/terraform/tutorials/kubernetes/kubernetes-provider)
- [Metrics Server: requisitos e compatibilidade](https://github.com/kubernetes-sigs/metrics-server/tree/v0.9.0)
- [Estado e dados sensíveis](https://developer.hashicorp.com/terraform/language/manage-sensitive-data)
- [Health checks ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-10.0)
