locals {
  namespace     = "techchallenge"
  database_name = "TechChallenge"
  database_user = "techchallenge"
  labels = {
    "app.kubernetes.io/part-of"    = "techchallenge"
    "app.kubernetes.io/managed-by" = "terraform"
  }
}

resource "kubernetes_namespace_v1" "techchallenge" {
  metadata {
    name   = local.namespace
    labels = local.labels
  }

  # A remoção de um namespace também remove recursos criados pelo Kustomize.
  lifecycle {
    prevent_destroy = true
  }
}
