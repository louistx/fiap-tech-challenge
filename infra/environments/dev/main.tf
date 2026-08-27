provider "kind" {}

module "kubernetes" {
  source = "../../modules/kubernetes"

  cluster_name           = var.cluster_name
  worker_count           = var.worker_count
  kubeconfig_path        = var.kubeconfig_path
  host_http_port         = var.host_http_port
  node_port              = var.node_port
  install_metrics_server = var.install_metrics_server
}

provider "kubernetes" {
  host                   = module.kubernetes.host
  cluster_ca_certificate = module.kubernetes.cluster_ca_certificate
  client_certificate     = module.kubernetes.client_certificate
  client_key             = module.kubernetes.client_key
}

module "database" {
  source = "../../modules/database"

  namespace         = var.namespace
  postgres_image    = var.postgres_image
  postgres_user     = var.postgres_user
  postgres_password = var.postgres_password
  postgres_db       = var.postgres_db
  storage_size      = var.storage_size

  depends_on = [module.kubernetes]
}
