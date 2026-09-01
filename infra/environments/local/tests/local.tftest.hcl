mock_provider "kubernetes" {}
mock_provider "helm" {}
mock_provider "random" {}

run "local_resources" {
  command = plan

  assert {
    condition     = kubernetes_namespace_v1.techchallenge.metadata[0].name == "techchallenge"
    error_message = "O namespace deve coincidir com o dos overlays locais."
  }
  assert {
    condition     = kubernetes_stateful_set_v1.postgres.spec[0].replicas == "1"
    error_message = "O PostgreSQL local deve ter uma única réplica."
  }
  assert {
    condition     = kubernetes_persistent_volume_claim_v1.postgres.wait_until_bound == false
    error_message = "Não aguardar bind antes de criar o pod em StorageClasses WaitForFirstConsumer."
  }
  assert {
    condition     = kubernetes_service_v1.postgres.spec[0].selector.app == kubernetes_stateful_set_v1.postgres.spec[0].template[0].metadata[0].labels.app
    error_message = "O Service deve selecionar o pod do PostgreSQL."
  }
  assert {
    condition     = kubernetes_secret_v1.api.metadata[0].name == "techchallenge-api-secrets" && random_password.jwt.length >= 32
    error_message = "O Secret da API deve ter o nome esperado e uma chave JWT com tamanho válido."
  }
  assert {
    condition     = length(helm_release.metrics_server) == 1
    error_message = "O ambiente deve fornecer métricas ao HPA por padrão."
  }
}

run "reuse_existing_metrics_server" {
  command = plan
  variables {
    install_metrics_server = false
  }
  assert {
    condition     = length(helm_release.metrics_server) == 0
    error_message = "Não instalar outro Metrics Server quando o cluster já possui um."
  }
}

run "reject_unpinned_database" {
  command = plan
  variables {
    postgres_image = "postgres:latest"
  }
  expect_failures = [var.postgres_image]
}

run "reject_empty_storage" {
  command = plan
  variables {
    postgres_storage_size = "0Gi"
  }
  expect_failures = [var.postgres_storage_size]
}
