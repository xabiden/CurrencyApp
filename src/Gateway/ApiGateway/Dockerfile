FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY Directory.Build.props .
COPY Directory.Packages.props .

COPY src/Gateway/ApiGateway/ApiGateway.csproj src/Gateway/ApiGateway/

RUN dotnet restore src/Gateway/ApiGateway/ApiGateway.csproj

COPY src/Gateway/ApiGateway/ src/Gateway/ApiGateway/

RUN dotnet publish src/Gateway/ApiGateway/ApiGateway.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ApiGateway.dll"]