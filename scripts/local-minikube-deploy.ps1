param(
    [string]$Profile = "minikube",
    [string]$Namespace = "employeecrud-local",
    [string]$ReleaseName = "employeecrud"
)

$ErrorActionPreference = "Stop"

function Invoke-NativeCommand {
    param(
        [string]$FilePath,
        [string[]]$Arguments
    )

    & $FilePath @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "$FilePath failed with exit code $LASTEXITCODE."
    }
}

function Assert-Command {
    param([string]$Name)

    if (-not (Get-Command $Name -ErrorAction SilentlyContinue)) {
        throw "$Name is required but was not found on PATH."
    }
}

Assert-Command "docker"
Assert-Command "helm"
Assert-Command "kubectl"
Assert-Command "minikube"

$minikubeIsRunning = $false
try {
    minikube -p $Profile status *> $null
    $minikubeIsRunning = $LASTEXITCODE -eq 0
}
catch {
    $minikubeIsRunning = $false
}

if (-not $minikubeIsRunning) {
    Invoke-NativeCommand "minikube" @("start", "-p", $Profile)
}

$dockerEnvironment = minikube -p $Profile docker-env --shell powershell
if ($LASTEXITCODE -ne 0) {
    throw "minikube docker-env failed with exit code $LASTEXITCODE."
}

Invoke-Expression ($dockerEnvironment -join [Environment]::NewLine)

Invoke-NativeCommand "docker" @("build", "--file", "Dockerfile", "--tag", "employeecrud:local", ".")

Invoke-NativeCommand "helm" @(
    "upgrade",
    "--install",
    $ReleaseName,
    "deploy/helm/employeecrud",
    "--namespace",
    $Namespace,
    "--create-namespace",
    "--values",
    "deploy/helm/employeecrud/values-local.yaml",
    "--set",
    "image.repository=employeecrud",
    "--set",
    "image.tag=local",
    "--atomic",
    "--timeout",
    "5m"
)

Invoke-NativeCommand "kubectl" @("rollout", "status", "deployment/$ReleaseName", "--namespace", $Namespace, "--timeout=180s")

$clusterIp = minikube -p $Profile ip
if ($LASTEXITCODE -ne 0) {
    throw "minikube ip failed with exit code $LASTEXITCODE."
}

$nodePort = kubectl get service $ReleaseName --namespace $Namespace --output jsonpath="{.spec.ports[0].nodePort}"
if ($LASTEXITCODE -ne 0) {
    throw "kubectl service lookup failed with exit code $LASTEXITCODE."
}

Write-Host "Employee CRUD is available at: http://$($clusterIp):$nodePort"
