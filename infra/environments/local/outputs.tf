output "namespace" {
  description = "Namespace preparado para os manifests em k8s/overlays/docker-local."
  value       = kubernetes_namespace_v1.techchallenge.metadata[0].name
}

output "database_host" {
  description = "DNS do PostgreSQL acessível apenas dentro do cluster."
  value       = "${kubernetes_service_v1.postgres.metadata[0].name}.${local.namespace}.svc.cluster.local"
}

output "api_secret_name" {
  description = "Nome do Secret consumido pela API e pelo Job de migrations; não expõe seus valores."
  value       = kubernetes_secret_v1.api.metadata[0].name
}
