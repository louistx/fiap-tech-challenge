mock_provider "kubernetes" {}
mock_provider "helm" {}
mock_provider "random" {}

run "namespace_and_secrets" {
  command = plan

  assert {
    condition     = kubernetes_namespace_v1.techchallenge.metadata[0].name == "techchallenge"
    error_message = "O namespace deve coincidir com o overlay local."
  }
  assert {
    condition     = kubernetes_secret_v1.postgres.metadata[0].name == "postgres-credentials"
    error_message = "O PostgreSQL deve receber suas credenciais por Secret."
  }
  assert {
    condition     = kubernetes_secret_v1.api.metadata[0].name == "techchallenge-api-secrets"
    error_message = "A API deve receber connection string e credenciais por Secret."
  }
}

run "postgres" {
  command = plan

  assert {
    condition     = kubernetes_stateful_set_v1.postgres.spec[0].replicas == "1"
    error_message = "O PostgreSQL local deve ter uma única réplica."
  }
  assert {
    condition     = kubernetes_service_v1.postgres.spec[0].selector.app == kubernetes_stateful_set_v1.postgres.spec[0].template[0].metadata[0].labels.app
    error_message = "O Service deve selecionar o pod do PostgreSQL."
  }
}

run "persistent_database" {
  command = plan

  assert {
    condition     = kubernetes_persistent_volume_claim_v1.postgres.spec[0].resources[0].requests.storage == "2Gi"
    error_message = "O PostgreSQL deve usar o PVC local de 2 GiB."
  }
  assert {
    condition     = kubernetes_persistent_volume_claim_v1.postgres.wait_until_bound == false
    error_message = "O plano não deve bloquear antes da criação do pod consumidor."
  }
}

run "metrics_for_hpa" {
  command = plan

  assert {
    condition     = helm_release.metrics_server.name == "techchallenge-metrics-server"
    error_message = "O ambiente local deve instalar o Metrics Server usado pelo HPA."
  }
  assert {
    condition     = helm_release.metrics_server.namespace == "techchallenge"
    error_message = "O Metrics Server da demonstração deve ficar no namespace do projeto."
  }
}
