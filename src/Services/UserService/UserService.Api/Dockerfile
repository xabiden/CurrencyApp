FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY Directory.Build.props .
COPY Directory.Packages.props .

COPY src/BuildingBlocks/Shared/Shared.csproj src/BuildingBlocks/Shared/
COPY src/BuildingBlocks/Shared.Api/Shared.Api.csproj src/BuildingBlocks/Shared.Api/
COPY src/Services/UserService/UserService.Domain/UserService.Domain.csproj src/Services/UserService/UserService.Domain/
COPY src/Services/UserService/UserService.Application/UserService.Application.csproj src/Services/UserService/UserService.Application/
COPY src/Services/UserService/UserService.Infrastructure/UserService.Infrastructure.csproj src/Services/UserService/UserService.Infrastructure/
COPY src/Services/UserService/UserService.Api/UserService.Api.csproj src/Services/UserService/UserService.Api/

RUN dotnet restore src/Services/UserService/UserService.Api/UserService.Api.csproj

COPY src/BuildingBlocks/ src/BuildingBlocks/
COPY src/Services/UserService/ src/Services/UserService/

RUN dotnet publish src/Services/UserService/UserService.Api/UserService.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "UserService.Api.dll"]