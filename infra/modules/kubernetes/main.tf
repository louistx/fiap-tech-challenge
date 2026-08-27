terraform {
  required_providers {
    kind = {
      source = "tehcyx/kind"
    }
    null = {
      source = "hashicorp/null"
    }
  }
}

resource "kind_cluster" "this" {
  name            = var.cluster_name
  wait_for_ready  = true
  kubeconfig_path = pathexpand(var.kubeconfig_path)

  kind_config {
    kind        = "Cluster"
    api_version = "kind.x-k8s.io/v1alpha4"

    node {
      role = "control-plane"

      extra_port_mappings {
        container_port = var.node_port
        host_port      = var.host_http_port
        protocol       = "TCP"
      }
    }

    dynamic "node" {
      for_each = range(var.worker_count)
      content {
        role = "worker"
      }
    }
  }
}

resource "null_resource" "metrics_server" {
  count = var.install_metrics_server ? 1 : 0

  depends_on = [kind_cluster.this]

  triggers = {
    cluster_id = kind_cluster.this.id
  }

  provisioner "local-exec" {
    environment = {
      KUBECONFIG = kind_cluster.this.kubeconfig_path
    }
    command = <<-EOT
      set -euo pipefail
      kubectl apply -f https://github.com/kubernetes-sigs/metrics-server/releases/latest/download/components.yaml
      kubectl -n kube-system patch deployment metrics-server --type=json \
        -p='[{"op":"add","path":"/spec/template/spec/containers/0/args/-","value":"--kubelet-insecure-tls"}]'
      kubectl -n kube-system rollout status deployment/metrics-server --timeout=180s
    EOT
  }
}
