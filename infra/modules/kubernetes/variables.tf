variable "cluster_name" {
  description = "Nome do cluster kind."
  type        = string
  default     = "fiap-tech-challenge"
}

variable "worker_count" {
  description = "Quantidade de nós worker além do control-plane."
  type        = number
  default     = 2
}

variable "kubeconfig_path" {
  description = "Caminho local onde o kubeconfig do cluster kind será escrito."
  type        = string
  default     = "kind-kubeconfig.yaml"
}

variable "host_http_port" {
  description = "Porta no host (sua máquina) mapeada para o NodePort da API."
  type        = number
  default     = 8080
}

variable "node_port" {
  description = "NodePort exposto pelo Service da API dentro do cluster (k8s/base/TechChallengeApi/service.yaml)."
  type        = number
  default     = 30080
}

variable "install_metrics_server" {
  description = "Instala o metrics-server no cluster (necessário para o HPA funcionar)."
  type        = bool
  default     = true
}
