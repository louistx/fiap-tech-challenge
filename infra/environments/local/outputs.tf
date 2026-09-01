output "namespace" {
  description = "Namespace preparado para os manifests em k8s/overlays/docker-local."
  value       = kubernetes_namespace_v1.techchallenge.metadata[0].name
}
