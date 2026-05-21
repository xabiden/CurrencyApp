FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY Directory.Build.props .
COPY Directory.Packages.props .

COPY src/BuildingBlocks/Shared/Shared.csproj src/BuildingBlocks/Shared/
COPY src/Services/FinanceService/FinanceService.Domain/FinanceService.Domain.csproj src/Services/FinanceService/FinanceService.Domain/
COPY src/Services/FinanceService/FinanceService.Application/FinanceService.Application.csproj src/Services/FinanceService/FinanceService.Application/
COPY src/Services/FinanceService/FinanceService.Infrastructure/FinanceService.Infrastructure.csproj src/Services/FinanceService/FinanceService.Infrastructure/
COPY src/Workers/BackgroundWorker/BackgroundWorker.csproj src/Workers/BackgroundWorker/

RUN dotnet restore src/Workers/BackgroundWorker/BackgroundWorker.csproj

COPY src/BuildingBlocks/ src/BuildingBlocks/
COPY src/Services/FinanceService/ src/Services/FinanceService/
COPY src/Workers/BackgroundWorker/ src/Workers/BackgroundWorker/

RUN dotnet publish src/Workers/BackgroundWorker/BackgroundWorker.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "BackgroundWorker.dll"]