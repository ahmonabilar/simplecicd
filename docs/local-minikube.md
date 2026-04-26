# Local Minikube Deployment

Use the local pipeline script to build the Docker image inside Minikube and deploy the Helm chart.

## Prerequisites

- Docker
- Minikube
- kubectl
- Helm

## Deploy

From the repository root:

```powershell
.\scripts\local-minikube-deploy.ps1
```

The script:

1. Starts Minikube if needed.
2. Points Docker at Minikube's Docker daemon.
3. Builds `employeecrud:local`.
4. Installs or upgrades the Helm release in namespace `employeecrud-local`.
5. Prints the Minikube service URL.

## Delete

```powershell
.\scripts\local-minikube-delete.ps1
```

The local deployment uses SQLite and seeds 10 employee records on first startup.
