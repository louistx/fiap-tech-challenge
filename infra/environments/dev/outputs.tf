output "cluster_name" {
  description = "Nome do cluster kind."
  value       = module.kubernetes.cluster_name
}

output "kubeconfig_path" {
  description = "Caminho local do kubeconfig gerado. Use com `export KUBECONFIG=<valor>` antes dos comandos kubectl."
  value       = module.kubernetes.kubeconfig_path
}

output "api_url" {
  description = "URL local da API depois do deploy dos manifests (k8s/overlays/docker-local)."
  value       = "http://localhost:${var.host_http_port}"
}

output "postgres_namespace" {
  description = "Namespace onde o Postgres (e a aplicação) foram provisionados."
  value       = module.database.namespace
}

output "postgres_service_fqdn" {
  description = "Host do Postgres para usar em ConnectionStrings__DefaultConnection."
  value       = module.database.service_fqdn
}

output "postgres_port" {
  value = module.database.port
}

output "postgres_database_name" {
  value = module.database.database_name
}

output "postgres_connection_string" {
  description = "Connection string completa (com senha) para copiar em k8s/overlays/docker-local/secrets.env."
  value       = module.database.connection_string
  sensitive   = true
}
