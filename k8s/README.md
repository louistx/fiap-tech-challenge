# Kubernetes

Manifests em `base/` (recursos comuns) e `overlays/docker-local/` (ambiente local).

## Pré-requisitos

- Cluster e banco provisionados: `infra/` (`terraform -chdir=infra/environments/dev apply`).
- `kubectl` apontando para o cluster.

## Configurar segredos

```bash
cp k8s/overlays/docker-local/secrets.env.example k8s/overlays/docker-local/secrets.env
# editar k8s/overlays/docker-local/secrets.env com os valores reais
```

## Aplicar

```bash
kubectl apply -k k8s/overlays/docker-local
kubectl rollout status deployment/fiap-tech-challenge-api -n techchallenge
kubectl get pods,svc,hpa -n techchallenge
```

API em `http://localhost:8080`.

## Testar o HPA

```bash
kubectl get hpa -n techchallenge -w
kubectl run carga --rm -it --image=williamyeh/hey --restart=Never -- \
  -z 60s -c 50 http://fiap-tech-challenge-api-service.techchallenge.svc.cluster.local:8080/health
```
