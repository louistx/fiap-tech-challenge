# Manifests da aplicação

O Terraform cria a fundação do ambiente. O Kustomize cuida somente da API e o
Job de carga permanece separado para ser iniciado no momento da demonstração.

```text
k8s/
├── base/api/                    # ConfigMap, Deployment e Service
├── overlays/docker-local/       # configuração, imagem e HPA
└── tests/hpa-load.yaml          # Job de carga de 120 segundos
```

## Aplicar a API

```bash
kubectl --context=docker-desktop apply -k k8s/overlays/docker-local
kubectl --context=docker-desktop -n techchallenge rollout status \
  deployment/fiap-tech-challenge-api --timeout=300s
kubectl --context=docker-desktop -n techchallenge get pods,svc,pvc,hpa
```

O `apply -k` cria quatro recursos:

- ConfigMap `techchallenge-api-config`;
- Deployment da API;
- Service ClusterIP;
- HPA de uma a três réplicas.

O Secret consumido pelo Deployment e o PostgreSQL referenciado pela connection
string já foram criados pelo Terraform. A API executa migrations e seed durante
a inicialização; não existe Job de migrations.

O overlay aponta para uma tag imutável publicada pelo GitHub Actions. Depois de
publicar a imagem, o workflow atualiza `images[].newTag` automaticamente; para
usar a nova versão no cluster local, basta repetir o mesmo `apply -k`.

## Acessar a API

```bash
kubectl --context=docker-desktop -n techchallenge port-forward \
  svc/fiap-tech-challenge-api-service 18080:8080
```

- Swagger: `http://localhost:18080/swagger`
- Liveness: `http://localhost:18080/health/live`
- Readiness: `http://localhost:18080/health/ready`

## Executar a carga do HPA

Em um terminal:

```bash
kubectl --context=docker-desktop -n techchallenge get hpa,pods -w
```

Em outro:

```bash
kubectl --context=docker-desktop apply -f k8s/tests/hpa-load.yaml
```

O Job não faz parte do overlay da aplicação. Ele existe somente para elevar o
uso de CPU durante 120 segundos e demonstrar o HPA. Para executá-lo novamente,
exclua o Job concluído antes do novo `apply`.

## Validar sem aplicar

```bash
kubectl kustomize k8s/base
kubectl kustomize k8s/overlays/docker-local
kubectl kustomize k8s/tests
```

O ambiente é um laboratório local: usa port-forward, uma réplica de PostgreSQL
e migrations na inicialização da API. Não representa uma topologia de produção.
