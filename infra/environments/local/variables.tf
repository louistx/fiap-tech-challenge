variable "kubeconfig_path" {
  description = "Kubeconfig que contém o contexto docker-desktop. O contexto global não é alterado."
  type        = string
  default     = "~/.kube/config"
}

variable "postgres_image" {
  description = "Imagem PostgreSQL 17 com versão explícita. Upgrade de major exige migração dos dados."
  type        = string
  default     = "postgres:17.11-bookworm"

  validation {
    condition     = can(regex("^postgres:17\\.[0-9]+(-[a-z0-9.]+)?$", var.postgres_image))
    error_message = "Use uma versão explícita do PostgreSQL 17, por exemplo postgres:17.11-bookworm."
  }
}

variable "postgres_storage_size" {
  description = "Tamanho inicial do PVC. O storage local do Docker Desktop pode não permitir expansão."
  type        = string
  default     = "2Gi"

  validation {
    condition     = can(regex("^[1-9][0-9]*(Mi|Gi)$", var.postgres_storage_size))
    error_message = "Informe um tamanho positivo em Mi ou Gi, por exemplo 2Gi."
  }
}

variable "storage_class_name" {
  description = "StorageClass existente; null utiliza a classe padrão do cluster."
  type        = string
  default     = null
}

variable "install_metrics_server" {
  description = "Instala o Metrics Server para o HPA. Desative se o cluster já tiver um gerenciado externamente."
  type        = bool
  default     = true
}

variable "metrics_server_insecure_tls" {
  description = "Exceção apenas para kubelets com certificado local do Docker Desktop; não usar em produção. Não altera TLS do provider."
  type        = bool
  default     = false
}
