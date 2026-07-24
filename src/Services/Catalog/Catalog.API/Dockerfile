# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

WORKDIR /app
EXPOSE 8080



# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Catalog/Catalog.API/Catalog.API.csproj", "src/Services/Catalog/Catalog.API/"]
COPY ["src/BuildingBlocks/Common/AtlasCommerce.BuildingBlocks.Common/AtlasCommerce.BuildingBlocks.Common.csproj", "src/BuildingBlocks/Common/AtlasCommerce.BuildingBlocks.Common/"]
COPY ["src/BuildingBlocks/HealthChecks/AtlasCommerce.BuildingBlocks.HealthChecks/AtlasCommerce.BuildingBlocks.HealthChecks.csproj", "src/BuildingBlocks/HealthChecks/AtlasCommerce.BuildingBlocks.HealthChecks/"]
COPY ["src/BuildingBlocks/Logging/AtlasCommerce.BuildingBlocks.Logging/AtlasCommerce.BuildingBlocks.Logging.csproj", "src/BuildingBlocks/Logging/AtlasCommerce.BuildingBlocks.Logging/"]
COPY ["src/BuildingBlocks/Observability/AtlasCommerce.BuildingBlocks.Observability/AtlasCommerce.BuildingBlocks.Observability.csproj", "src/BuildingBlocks/Observability/AtlasCommerce.BuildingBlocks.Observability/"]
COPY ["src/Services/Catalog/Catalog.Infrastructure/Catalog.Infrastructure.csproj", "src/Services/Catalog/Catalog.Infrastructure/"]
COPY ["src/BuildingBlocks/EventBus/AtlasCommerce.BuildingBlocks.EventBus/AtlasCommerce.BuildingBlocks.EventBus.csproj", "src/BuildingBlocks/EventBus/AtlasCommerce.BuildingBlocks.EventBus/"]
COPY ["src/Services/Catalog/Catalog.Application/Catalog.Application.csproj", "src/Services/Catalog/Catalog.Application/"]
COPY ["src/Services/Catalog/Catalog.Domain/Catalog.Domain.csproj", "src/Services/Catalog/Catalog.Domain/"]
RUN dotnet restore "./src/Services/Catalog/Catalog.API/Catalog.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Catalog/Catalog.API"
RUN dotnet build "./Catalog.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Catalog.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catalog.API.dll"]