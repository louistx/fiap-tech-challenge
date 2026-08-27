output "cluster_name" {
  description = "Nome do cluster kind criado."
  value       = kind_cluster.this.name
}

output "kubeconfig_path" {
  description = "Caminho local do kubeconfig gerado para o cluster."
  value       = kind_cluster.this.kubeconfig_path
}

output "host" {
  description = "Endpoint da API do Kubernetes (kube-apiserver)."
  value       = kind_cluster.this.endpoint
}

output "cluster_ca_certificate" {
  description = "CA certificate do cluster, em base64."
  value       = kind_cluster.this.cluster_ca_certificate
  sensitive   = true
}

output "client_certificate" {
  description = "Certificado de cliente para autenticação no cluster."
  value       = kind_cluster.this.client_certificate
  sensitive   = true
}

output "client_key" {
  description = "Chave privada do cliente para autenticação no cluster."
  value       = kind_cluster.this.client_key
  sensitive   = true
}
