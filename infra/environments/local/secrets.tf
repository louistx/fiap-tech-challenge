resource "random_password" "database" {
  length  = 32
  special = false
}

resource "random_password" "jwt" {
  length  = 64
  special = false
}

resource "random_password" "admin" {
  length  = 32
  special = false
}

# sensitive oculta a saída, mas os valores continuam no state local. Não versionar o state.
resource "kubernetes_secret_v1" "postgres" {
  metadata {
    name      = "postgres-credentials"
    namespace = kubernetes_namespace_v1.techchallenge.metadata[0].name
    labels    = local.labels
  }

  data = {
    POSTGRES_DB       = local.database_name
    POSTGRES_USER     = local.database_user
    POSTGRES_PASSWORD = random_password.database.result
  }
}

resource "kubernetes_secret_v1" "api" {
  metadata {
    name      = "techchallenge-api-secrets"
    namespace = kubernetes_namespace_v1.techchallenge.metadata[0].name
    labels    = local.labels
  }

  data = {
    ConnectionStrings__DefaultConnection = "Host=postgres;Port=5432;Database=${local.database_name};Username=${local.database_user};Password=${random_password.database.result}"
    Jwt__SecretKey                       = random_password.jwt.result
    Seed__AdminPassword                  = random_password.admin.result
  }
}
