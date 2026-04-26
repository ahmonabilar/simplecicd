# Azure DevOps And Azure Setup

This repository includes `azure-pipelines.yml` for CI/CD to Azure Kubernetes Service using Docker, ACR, Helm, MegaLinter, .NET analyzers, NuGet vulnerability checks, Trivy scans, and NUnit coverage gates.

## Azure Resources

Create or confirm these Azure resources:

```powershell
az group create --name rg-employeecrud-test --location uksouth
az group create --name rg-employeecrud-prod --location uksouth
az acr create --resource-group rg-employeecrud-test --name <acrName> --sku Standard
az aks create --resource-group rg-employeecrud-test --name aks-employeecrud-test --attach-acr <acrName>
az aks create --resource-group rg-employeecrud-prod --name aks-employeecrud-prod --attach-acr <acrName>
```

For SQL Server deployments, create Azure SQL databases for test and production and keep their connection strings in Azure DevOps secret variables.

## Azure DevOps Service Connections

Create these service connections in Project Settings:

- `azureServiceConnection`: Azure Resource Manager connection with access to AKS, Azure SQL, and resource groups.
- `acrServiceConnection`: Docker Registry connection to Azure Container Registry.

The names can be different, but they must match the variable values in the variable group below.

## Variable Group

Create a variable group named `employeecrud-cicd` and authorize the pipeline to use it.

Required variables:

| Name | Secret | Example |
| --- | --- | --- |
| `azureServiceConnection` | No | `sc-employeecrud-azure` |
| `acrServiceConnection` | No | `sc-employeecrud-acr` |
| `acrLoginServer` | No | `myregistry.azurecr.io` |
| `aksTestResourceGroup` | No | `rg-employeecrud-test` |
| `aksTestClusterName` | No | `aks-employeecrud-test` |
| `aksProdResourceGroup` | No | `rg-employeecrud-prod` |
| `aksProdClusterName` | No | `aks-employeecrud-prod` |
| `testSqlConnectionString` | Yes | Azure SQL test connection string |
| `prodSqlConnectionString` | Yes | Azure SQL production connection string |

Optional variable:

| Name | Secret | Example |
| --- | --- | --- |
| `deployDatabase` | No | `true` to publish the DACPAC during deployments |

If `deployDatabase` is missing or set to any value other than `true`, the pipeline still builds and publishes the DACPAC artifact, but it does not publish schema changes.

## Environments And Approvals

Create Azure DevOps environments:

- `employeecrud-test`
- `employeecrud-production`

Add approval checks to `employeecrud-production` so production releases from `release` or `release/*` branches require human approval before Helm deploys.

## Branch Flow

- Pull requests to `main`, `release`, or `release/*` run lint, build, scans, tests, coverage, database build, and image scan.
- Any branch push triggers the build quality gates.
- `main` deploys to the test AKS namespace `employeecrud-test`.
- `release` or `release/*` deploys to the production AKS namespace `employeecrud-prod`.

## Kubernetes Secrets

The Helm chart expects a secret named `employeecrud-db` with key `connectionString` for test and production. The pipeline creates or updates that secret from the secret variable when the variable is available.

Manual creation:

```powershell
az aks get-credentials --resource-group rg-employeecrud-test --name aks-employeecrud-test
kubectl create namespace employeecrud-test --dry-run=client -o yaml | kubectl apply -f -
kubectl create secret generic employeecrud-db --namespace employeecrud-test --from-literal=connectionString="<connection-string>"
```

Repeat for production using the production cluster and namespace.

## Database Project Build Requirement

The database project uses the Visual Studio SSDT project format so it loads in Visual Studio. The pipeline database job runs on `windows-latest` with `VSBuild@1`. If your hosted agent image does not include SSDT, use a self-hosted Windows agent with Visual Studio and SQL Server Data Tools installed.
