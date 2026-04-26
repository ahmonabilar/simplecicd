param(
    [string]$Namespace = "employeecrud-local",
    [string]$ReleaseName = "employeecrud"
)

$ErrorActionPreference = "Stop"

helm uninstall $ReleaseName --namespace $Namespace --ignore-not-found
kubectl delete namespace $Namespace --ignore-not-found
