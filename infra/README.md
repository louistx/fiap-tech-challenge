# Infraestrutura (Terraform)

Provisiona um cluster kind e um PostgreSQL dentro dele.

```
infra/
├── modules/
│   ├── kubernetes/   # cluster kind + metrics-server
│   └── database/     # namespace, Secret, PVC, Deployment e Service do Postgres
├── environments/
│   └── dev/
└── README.md
```

## Recursos criados

| Módulo | Recurso |
| --- | --- |
| `kubernetes` | `kind_cluster` (1 control-plane + 2 workers) |
| `kubernetes` | metrics-server |
| `database` | `kubernetes_namespace` (`techchallenge`) |
| `database` | `kubernetes_secret` (credenciais do Postgres) |
| `database` | `kubernetes_persistent_volume_claim` |
| `database` | `kubernetes_deployment` (Postgres 16) |
| `database` | `kubernetes_service` (ClusterIP 5432) |

## Pré-requisitos

- Docker
- Terraform >= 1.6
- kubectl

## Aplicar

```bash
cd infra/environments/dev

terraform init
terraform fmt -check
terraform validate

terraform apply -target=module.kubernetes -auto-approve
terraform apply -auto-approve
```

```bash
terraform output
terraform output -raw postgres_connection_string
```

Copie o valor de `postgres_connection_string` para
`k8s/overlays/docker-local/secrets.env` (`ConnectionStrings__DefaultConnection`).

## Destruir

```bash
terraform -chdir=infra/environments/dev destroy -auto-approve
```
