# Infraestrutura local da demonstração

O cluster Kubernetes é fornecido pelo Docker Desktop. O Terraform prepara somente
a fundação necessária para a apresentação; a aplicação continua separada nos
manifests Kustomize.

| Responsável | Recursos |
| --- | --- |
| Docker Desktop | Cluster, nó, rede e StorageClass padrão |
| Terraform | Namespace, Secrets, PostgreSQL persistente e Metrics Server |
| Kustomize | ConfigMap, Deployment, Service e HPA da API |
| GitHub Actions | Build, testes, publicação no GHCR e promoção da tag no overlay |
| Aplicação | Migrations e seed durante a inicialização |

Não há cluster de nuvem, banco gerenciado, Ingress, backend remoto ou deploy
escondido no Terraform. Todo o ambiente é fixo para o contexto `docker-desktop` e
não exige `terraform.tfvars`.

## Pré-requisitos

- Docker Desktop com Kubernetes habilitado;
- Terraform entre 1.7 e 2.0;
- `kubectl`;
- internet para baixar providers, chart e imagens.

Antes da gravação, o `terraform init` pode ser executado sem criar recursos:

```bash
terraform -chdir=infra/environments/local init
terraform -chdir=infra/environments/local fmt -check
terraform -chdir=infra/environments/local validate
terraform -chdir=infra/environments/local test
```

Os testes usam providers mockados e possuem quatro cenários: namespace/Secrets,
PostgreSQL, persistência e métricas do HPA.

Para preparar uma gravação realmente do zero, remova primeiro a aplicação e,
depois, a fundação. Esse procedimento apaga o banco e deve ser usado somente
quando os dados locais não precisarem ser preservados:

```bash
kubectl --context=docker-desktop delete -k k8s/overlays/docker-local \
  --ignore-not-found
terraform -chdir=infra/environments/local destroy
```

Esses comandos não são parte do vídeo; servem apenas para preparar o estado
inicial. Nenhum recurso fora do namespace e da release gerenciados pelo projeto
é removido.

## 1. Criar a fundação

Com o cluster Docker Desktop vazio, confirme primeiro que o overlay depende da
fundação. Este comando deve falhar com `namespaces "techchallenge" not found`:

```bash
kubectl --context=docker-desktop apply -k k8s/overlays/docker-local
```

O erro é esperado: Kustomize não é responsável por criar o namespace. Em
seguida, gere e aplique um plano revisável:

```bash
terraform -chdir=infra/environments/local plan -out=local.tfplan
terraform -chdir=infra/environments/local apply local.tfplan
```

Revise o plano exibido e confirme o `apply`. O Terraform cria:

- namespace `techchallenge`;
- Secrets do PostgreSQL e da API, com senhas geradas;
- PostgreSQL 17.11 em StatefulSet;
- Service interno `postgres`;
- PVC de 2 GiB;
- Metrics Server usado pelo HPA.

O Metrics Server é obrigatório porque o HPA utiliza CPU. Sem ele,
`kubectl top pods` não retorna dados e o alvo do HPA fica como `<unknown>`. A
opção `--kubelet-insecure-tls` está fixa somente para o certificado local do
kubelet do Docker Desktop; a comunicação dos providers com o cluster continua
validando TLS.

Verificação rápida:

```bash
kubectl --context=docker-desktop -n techchallenge get pods,svc,pvc
kubectl --context=docker-desktop get apiservice v1beta1.metrics.k8s.io
```

## 2. Implantar a aplicação

Depois que a tag imutável da imagem estiver definida no overlay:

```bash
kubectl --context=docker-desktop apply -k k8s/overlays/docker-local
kubectl --context=docker-desktop -n techchallenge rollout status \
  deployment/fiap-tech-challenge-api --timeout=300s
kubectl --context=docker-desktop -n techchallenge get pods,svc,pvc,hpa
```

Esse único `apply -k` cria ConfigMap, Deployment, Service e HPA. A API executa
migrations e seed antes de servir HTTP; não existe Job de migrations.

Para acessar a API:

```bash
kubectl --context=docker-desktop -n techchallenge port-forward \
  svc/fiap-tech-challenge-api-service 18080:8080
```

- Swagger: `http://localhost:18080/swagger`
- Liveness: `http://localhost:18080/health/live`
- Readiness: `http://localhost:18080/health/ready`

## 3. Demonstrar o HPA

Observe o HPA e os pods:

```bash
kubectl --context=docker-desktop -n techchallenge get hpa,pods -w
```

Em outro terminal, suba o Job de carga separadamente:

```bash
kubectl --context=docker-desktop apply -f k8s/tests/hpa-load.yaml
```

O Job faz requisições por 120 segundos. O HPA usa como alvo 70% do request de
CPU de 100m e pode aumentar a API de uma para três réplicas. O PostgreSQL
permanece com uma réplica.

Para repetir a demonstração, remova somente o Job concluído:

```bash
kubectl --context=docker-desktop -n techchallenge delete job \
  techchallenge-hpa-load --ignore-not-found
```

## Estado e credenciais

O state é local e sensível porque contém os valores gerados. State, planos e
`terraform.tfvars` estão fora do Git e não devem aparecer no vídeo.

O login inicial é `admin`. Se for necessário consultar a senha fora da gravação:

```bash
kubectl --context=docker-desktop -n techchallenge get secret \
  techchallenge-api-secrets \
  -o 'jsonpath={.data.Seed__AdminPassword}' | base64 --decode
```

O PostgreSQL usa um volume local persistente, suficiente para a demonstração,
mas não representa alta disponibilidade ou uma estratégia de backup de produção.
