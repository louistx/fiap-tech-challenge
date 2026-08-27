variable "cluster_name" {
  description = "Nome do cluster kind."
  type        = string
  default     = "fiap-tech-challenge"
}

variable "worker_count" {
  description = "Quantidade de nós worker do cluster."
  type        = number
  default     = 2
}

variable "kubeconfig_path" {
  description = "Caminho local onde o kubeconfig do cluster kind será escrito."
  type        = string
  default     = "kind-kubeconfig.yaml"
}

variable "host_http_port" {
  description = "Porta no host mapeada para a API (kind extraPortMappings)."
  type        = number
  default     = 8080
}

variable "node_port" {
  description = "NodePort do Service da API (deve bater com k8s/base/TechChallengeApi/service.yaml)."
  type        = number
  default     = 30080
}

variable "install_metrics_server" {
  description = "Instala o metrics-server automaticamente (necessário para o HPA)."
  type        = bool
  default     = true
}

variable "namespace" {
  description = "Namespace do Postgres e da aplicação (deve bater com k8s/overlays/docker-local)."
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
  description = "Senha do PostgreSQL. Default é apenas para uso local/dev."
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
