FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
RUN addgroup -S appgroup && adduser -S -u 10001 appuser -G appgroup

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY NuGet.config ./
COPY src/EmployeeCrud.Application/EmployeeCrud.Application.csproj src/EmployeeCrud.Application/
COPY src/EmployeeCrud.Web/EmployeeCrud.Web.csproj src/EmployeeCrud.Web/
RUN dotnet restore src/EmployeeCrud.Web/EmployeeCrud.Web.csproj --configfile NuGet.config
COPY . .
RUN dotnet publish src/EmployeeCrud.Web/EmployeeCrud.Web.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
RUN mkdir -p /app/App_Data/DataProtectionKeys /app/logs \
    && chown -R appuser:appgroup /app
USER appuser
ENTRYPOINT ["dotnet", "EmployeeCrud.Web.dll"]
