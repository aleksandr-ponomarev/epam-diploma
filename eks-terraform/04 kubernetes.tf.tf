provider "kubernetes" {
  host                   = data.aws_eks_cluster.cluster.endpoint
  token                  = data.aws_eks_cluster_auth.cluster.token
  cluster_ca_certificate = base64decode(data.aws_eks_cluster.cluster.certificate_authority.0.data)
}

provider "helm" {
  kubernetes {
    host                   = data.aws_eks_cluster.cluster.endpoint
    cluster_ca_certificate = base64decode(data.aws_eks_cluster.cluster.certificate_authority.0.data)
    token                  = data.aws_eks_cluster_auth.cluster.token
  }
}

resource "helm_release" "ingress" {
  name       = "ingress"
  chart      = "aws-load-balancer-controller"
  repository = "https://aws.github.io/eks-charts"
  namespace  = "kube-system"
  version    = "1.3.3"

  set {
    name  = "clusterName"
    value = local.cluster_name
  }
}

resource "kubernetes_namespace" "ns-dev" {
  metadata {
    name = "dev"
  }
  lifecycle {
    prevent_destroy = true
  }
}

resource "kubernetes_namespace" "ns-prod" {
  metadata {
    name = "prod"
  }
  lifecycle {
    prevent_destroy = true
  }
}

resource "kubernetes_secret" "psql_conn_string_dev" {
  metadata {
      name      = "psql-conn-string"
      namespace = "dev"
  }
  data = {
      psqlConnString = "Host=${aws_db_instance.rds-dev.address};Database=${aws_db_instance.rds-dev.name};Username=${aws_db_instance.rds-dev.username};Password=${aws_db_instance.rds-dev.password};CommandTimeout=3600"
  }
  type = "Opaque"
  lifecycle {
    prevent_destroy = true
  }
}

resource "kubernetes_secret" "psql_conn_string_prod" {
  metadata {
      name      = "psql-conn-string"
      namespace = "prod"
  }
  data = {
      psqlConnString = "Host=${aws_db_instance.rds-prod.address};Database=${aws_db_instance.rds-prod.name};Username=${aws_db_instance.rds-prod.username};Password=${aws_db_instance.rds-prod.password};CommandTimeout=3600"
  }
  type = "Opaque"
  lifecycle {
    prevent_destroy = true
  }
}