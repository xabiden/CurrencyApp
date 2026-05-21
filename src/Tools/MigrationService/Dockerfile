FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY Directory.Build.props .
COPY Directory.Packages.props .

COPY src/BuildingBlocks/Shared/Shared.csproj src/BuildingBlocks/Shared/
COPY src/Services/UserService/UserService.Domain/UserService.Domain.csproj src/Services/UserService/UserService.Domain/
COPY src/Services/UserService/UserService.Application/UserService.Application.csproj src/Services/UserService/UserService.Application/
COPY src/Services/UserService/UserService.Infrastructure/UserService.Infrastructure.csproj src/Services/UserService/UserService.Infrastructure/
COPY src/Services/FinanceService/FinanceService.Domain/FinanceService.Domain.csproj src/Services/FinanceService/FinanceService.Domain/
COPY src/Services/FinanceService/FinanceService.Application/FinanceService.Application.csproj src/Services/FinanceService/FinanceService.Application/
COPY src/Services/FinanceService/FinanceService.Infrastructure/FinanceService.Infrastructure.csproj src/Services/FinanceService/FinanceService.Infrastructure/
COPY src/Tools/MigrationService/MigrationService.csproj src/Tools/MigrationService/

RUN dotnet restore src/Tools/MigrationService/MigrationService.csproj

COPY src/BuildingBlocks/ src/BuildingBlocks/
COPY src/Services/UserService/ src/Services/UserService/
COPY src/Services/FinanceService/ src/Services/FinanceService/
COPY src/Tools/MigrationService/ src/Tools/MigrationService/

RUN dotnet publish src/Tools/MigrationService/MigrationService.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MigrationService.dll"]