FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY Directory.Build.props .
COPY Directory.Packages.props .

COPY src/BuildingBlocks/Shared/Shared.csproj src/BuildingBlocks/Shared/
COPY src/BuildingBlocks/Shared.Api/Shared.Api.csproj src/BuildingBlocks/Shared.Api/
COPY src/Services/FinanceService/FinanceService.Domain/FinanceService.Domain.csproj src/Services/FinanceService/FinanceService.Domain/
COPY src/Services/FinanceService/FinanceService.Application/FinanceService.Application.csproj src/Services/FinanceService/FinanceService.Application/
COPY src/Services/FinanceService/FinanceService.Infrastructure/FinanceService.Infrastructure.csproj src/Services/FinanceService/FinanceService.Infrastructure/
COPY src/Services/FinanceService/FinanceService.Api/FinanceService.Api.csproj src/Services/FinanceService/FinanceService.Api/

RUN dotnet restore src/Services/FinanceService/FinanceService.Api/FinanceService.Api.csproj

COPY src/BuildingBlocks/ src/BuildingBlocks/
COPY src/Services/FinanceService/ src/Services/FinanceService/

RUN dotnet publish src/Services/FinanceService/FinanceService.Api/FinanceService.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FinanceService.Api.dll"]