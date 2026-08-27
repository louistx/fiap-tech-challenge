variable "namespace" {
  description = "Namespace onde o Postgres (e a aplicação) rodam. Deve bater com k8s/overlays/docker-local (campo `namespace:` da kustomization)."
  type        = string
  default     = "techchallenge"
}

variable "postgres_image" {
  description = "Imagem do PostgreSQL."
  type        = string
  default     = "postgres:16"
}

variable "postgres_user" {
  description = "Usuário do PostgreSQL."
  type        = string
  default     = "postgres"
}

variable "postgres_password" {
  description = "Senha do PostgreSQL. Valor padrão é apenas para uso local/dev (mesmo do docker-compose) — sobrescreva via tfvars para qualquer uso além de demonstração local."
  type        = string
  default     = "Dev@123456"
  sensitive   = true
}

variable "postgres_db" {
  description = "Nome do banco de dados."
  type        = string
  default     = "TechChallenge"
}

variable "storage_size" {
  description = "Tamanho do volume persistente do Postgres."
  type        = string
  default     = "1Gi"
}
