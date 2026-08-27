output "namespace" {
  description = "Namespace onde o Postgres foi criado."
  value       = kubernetes_namespace.this.metadata[0].name
}

output "service_name" {
  description = "Nome do Service ClusterIP do Postgres."
  value       = kubernetes_service.postgres.metadata[0].name
}

output "service_fqdn" {
  description = "Nome DNS interno do cluster para o Postgres."
  value       = "${kubernetes_service.postgres.metadata[0].name}.${kubernetes_namespace.this.metadata[0].name}.svc.cluster.local"
}

output "port" {
  description = "Porta do Postgres."
  value       = 5432
}

output "database_name" {
  description = "Nome do banco de dados."
  value       = var.postgres_db
}

output "connection_string" {
  description = "Connection string pronta para ConnectionStrings__DefaultConnection (contém a senha — sensível)."
  value       = "Host=${kubernetes_service.postgres.metadata[0].name}.${kubernetes_namespace.this.metadata[0].name}.svc.cluster.local;Port=5432;Database=${var.postgres_db};Username=${var.postgres_user};Password=${var.postgres_password}"
  sensitive   = true
}
