resource "helm_release" "metrics_server" {
  name       = "techchallenge-metrics-server"
  namespace  = kubernetes_namespace_v1.techchallenge.metadata[0].name
  repository = "https://kubernetes-sigs.github.io/metrics-server/"
  chart      = "metrics-server"
  version    = "3.14.0"
  atomic     = true
  timeout    = 300

  values = [yamlencode({
    fullnameOverride = "techchallenge-metrics-server"
    # Necessário no Docker Desktop, cujo certificado local do kubelet não possui IP SAN.
    args = ["--kubelet-insecure-tls"]
    resources = {
      requests = { cpu = "100m", memory = "128Mi" }
      limits   = { cpu = "250m", memory = "256Mi" }
    }
  })]
}
