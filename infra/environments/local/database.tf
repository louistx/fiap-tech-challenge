resource "kubernetes_persistent_volume_claim_v1" "postgres" {
  metadata {
    name      = "postgres-data"
    namespace = kubernetes_namespace_v1.techchallenge.metadata[0].name
    labels    = local.labels
  }

  # StorageClasses com WaitForFirstConsumer só fazem bind depois que o pod existe.
  wait_until_bound = false

  spec {
    access_modes       = ["ReadWriteOnce"]
    storage_class_name = var.storage_class_name
    resources {
      requests = { storage = var.postgres_storage_size }
    }
  }

  lifecycle {
    prevent_destroy = true
  }
}

resource "kubernetes_service_v1" "postgres" {
  metadata {
    name      = "postgres"
    namespace = kubernetes_namespace_v1.techchallenge.metadata[0].name
    labels    = local.labels
  }

  spec {
    cluster_ip = "None"
    selector   = { app = "techchallenge-postgres" }
    port {
      name        = "postgres"
      port        = 5432
      target_port = 5432
    }
  }
}

resource "kubernetes_stateful_set_v1" "postgres" {
  metadata {
    name      = "postgres"
    namespace = kubernetes_namespace_v1.techchallenge.metadata[0].name
    labels    = local.labels
  }

  spec {
    service_name = kubernetes_service_v1.postgres.metadata[0].name
    replicas     = 1
    selector {
      match_labels = { app = "techchallenge-postgres" }
    }
    template {
      metadata {
        labels = merge(local.labels, { app = "techchallenge-postgres" })
      }
      spec {
        automount_service_account_token = false
        security_context {
          run_as_non_root = true
          run_as_user     = 999
          run_as_group    = 999
          fs_group        = 999
        }
        container {
          name  = "postgres"
          image = var.postgres_image
          port {
            name           = "postgres"
            container_port = 5432
          }
          env_from {
            secret_ref {
              name = kubernetes_secret_v1.postgres.metadata[0].name
            }
          }
          env {
            name  = "PGDATA"
            value = "/var/lib/postgresql/data/pgdata"
          }
          volume_mount {
            name       = "data"
            mount_path = "/var/lib/postgresql/data"
          }
          resources {
            requests = { cpu = "100m", memory = "128Mi" }
            limits   = { cpu = "500m", memory = "512Mi" }
          }
          security_context {
            allow_privilege_escalation = false
            capabilities {
              drop = ["ALL"]
            }
          }
          startup_probe {
            exec {
              command = ["sh", "-c", "pg_isready -U \"$POSTGRES_USER\" -d \"$POSTGRES_DB\""]
            }
            period_seconds    = 5
            failure_threshold = 30
          }
          readiness_probe {
            exec {
              command = ["sh", "-c", "pg_isready -U \"$POSTGRES_USER\" -d \"$POSTGRES_DB\""]
            }
            period_seconds = 5
          }
          liveness_probe {
            exec {
              command = ["sh", "-c", "pg_isready -U \"$POSTGRES_USER\" -d \"$POSTGRES_DB\""]
            }
            period_seconds    = 10
            failure_threshold = 6
          }
        }
        volume {
          name = "data"
          persistent_volume_claim {
            claim_name = kubernetes_persistent_volume_claim_v1.postgres.metadata[0].name
          }
        }
      }
    }
  }
}
