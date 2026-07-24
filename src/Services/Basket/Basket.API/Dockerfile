# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

WORKDIR /app
EXPOSE 8080



# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Basket/Basket.API/Basket.API.csproj", "src/Services/Basket/Basket.API/"]
COPY ["src/BuildingBlocks/Common/AtlasCommerce.BuildingBlocks.Common/AtlasCommerce.BuildingBlocks.Common.csproj", "src/BuildingBlocks/Common/AtlasCommerce.BuildingBlocks.Common/"]
COPY ["src/BuildingBlocks/HealthChecks/AtlasCommerce.BuildingBlocks.HealthChecks/AtlasCommerce.BuildingBlocks.HealthChecks.csproj", "src/BuildingBlocks/HealthChecks/AtlasCommerce.BuildingBlocks.HealthChecks/"]
COPY ["src/BuildingBlocks/Logging/AtlasCommerce.BuildingBlocks.Logging/AtlasCommerce.BuildingBlocks.Logging.csproj", "src/BuildingBlocks/Logging/AtlasCommerce.BuildingBlocks.Logging/"]
COPY ["src/BuildingBlocks/Observability/AtlasCommerce.BuildingBlocks.Observability/AtlasCommerce.BuildingBlocks.Observability.csproj", "src/BuildingBlocks/Observability/AtlasCommerce.BuildingBlocks.Observability/"]
RUN dotnet restore "./src/Services/Basket/Basket.API/Basket.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Basket/Basket.API"
RUN dotnet build "./Basket.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Basket.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Basket.API.dll"]