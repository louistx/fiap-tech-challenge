# Manifests da aplicação

Terraform cria namespace, PostgreSQL, volume, Secrets e Metrics Server em
[`infra/environments/local`](../infra/environments/local). Kustomize cuida apenas
da aplicação. Não há Job de migrations nem script de deploy.

```text
k8s/
├── base/
│   └── api/                 # Deployment, Service e ConfigMap vazio
├── overlays/
│   └── docker-local/
│       ├── configmap-patch.yaml
│       ├── hpa.yaml
│       └── kustomization.yaml
└── tests/
    └── hpa-load.yaml        # Carga limitada a 120 segundos
```

O ConfigMap comum se chama `techchallenge-api-config`; seus valores vêm do patch
local, sem generator. Senhas e connection string continuam no Secret fornecido
pelo Terraform. O HPA pertence ao ambiente e controla de 1 a 3 réplicas.

## Deploy local

Após provisionar as dependências com Terraform e publicar a imagem no GHCR:

```bash
kubectl --context=docker-desktop apply -k k8s/overlays/docker-local
```

O comando aplica ConfigMap, Deployment, Service e HPA. Em cada inicialização,
a API executa as migrations pendentes e o seed antes de iniciar o servidor HTTP.
Se isso falhar, o processo termina e o Kubernetes tenta novamente, com backoff.
A aplicação não precisa esperar um Job externo.

Para acompanhar o resultado e acessar a API:

```bash
kubectl --context=docker-desktop -n techchallenge rollout status deployment/fiap-tech-challenge-api --timeout=300s
kubectl --context=docker-desktop -n techchallenge port-forward svc/fiap-tech-challenge-api-service 18080:8080
```

A `startupProbe` permite até cinco minutos para inicialização. Depois disso,
`/health/live` verifica o processo e `/health/ready` consulta a tabela de usuários.
Uma falha posterior do banco retira o pod do Service sem provocar reinícios por
liveness. Uma migration inválida ou credencial errada exige correção: reiniciar
sozinho não resolve um erro permanente.

## Atualizar a imagem ou configuração

O padrão é `ghcr.io/louistx/fiap-tech-challenge:latest`. O workflow publica `latest`
e uma tag com os 12 primeiros caracteres do SHA do commit. Para cada versão,
altere `images[].newTag` em `overlays/docker-local/kustomization.yaml` para a tag
curta publicada e execute o mesmo `kubectl apply -k`. A mudança de tag faz o
Deployment substituir os pods;
os novos processos executam migrations antes de servir tráfego.

Publicar outra imagem sob a mesma tag `latest` não altera o manifesto e não
reinicia pods existentes. Alterar somente os valores do ConfigMap também não
recarrega variáveis de ambiente de processos já iniciados. Nesses casos:

```bash
kubectl --context=docker-desktop -n techchallenge rollout restart deployment/fiap-tech-challenge-api
```

O ConfigMap tem nome fixo: `rollout undo` não restaura seus valores antigos.
Restaure também a configuração correspondente. As referências ao comportamento
são [Deployments](https://kubernetes.io/docs/concepts/workloads/controllers/deployment/#updating-a-deployment)
e [ConfigMaps](https://kubernetes.io/docs/concepts/configuration/configmap/#mounted-configmaps-are-updated-automatically).

A imagem publicada deve conter os endpoints de saúde usados nas probes. O
workflow gera `linux/amd64` e `linux/arm64`. Se o pacote no GHCR for privado, configure
`imagePullSecrets` com um Secret gerenciado pelo Terraform; não versionar tokens.

## Validar sem aplicar

```bash
kubectl kustomize k8s/base
kubectl kustomize k8s/overlays/docker-local
kubectl --context=docker-desktop apply --dry-run=server -k k8s/overlays/docker-local
```

## Limites do laboratório

Migrations na inicialização simplificam este trabalho escolar. EF Core 10 usa
locking durante migrations, mas isso não torna mudanças destrutivas compatíveis
com as réplicas antigas ainda em execução, nem serializa o seed customizado.
O seed verifica os registros existentes; inicializações simultâneas em banco vazio
podem exigir nova tentativa se disputarem a criação do mesmo registro.
Em produção, revisar permissões de alteração do schema e a estratégia de migrations.
Referência: [migrations em runtime](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying#apply-migrations-at-runtime).

Veja o [guia de infraestrutura](../infra/README.md) para credenciais, persistência,
HPA, backup e configuração do Terraform.
