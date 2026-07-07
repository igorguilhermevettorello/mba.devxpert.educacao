# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./services/auth/PlataformaEducacional.Auth.Api/PlataformaEducacional.Auth.Api.csproj", "services/auth/PlataformaEducacional.Auth.Api//"]
COPY ["./buildingBlocks/PlataformaEducacional.Core/PlataformaEducacional.Core.csproj", "buildingBlocks/PlataformaEducacional.Core//"]
COPY ["./buildingBlocks/PlataformaEducacional.MessageBus/PlataformaEducacional.MessageBus.csproj", "buildingBlocks/PlataformaEducacional.MessageBus//"]
COPY ["./buildingBlocks/PlataformaEducacional.WebApi.Core/PlataformaEducacional.WebApi.Core.csproj", "buildingBlocks/PlataformaEducacional.WebApi.Core//"]

COPY ["./SeedDados/PlataformaEducacional.SeedDados/PlataformaEducacional.SeedDados.csproj", "SeedDados/PlataformaEducacional.SeedDados//"]
RUN dotnet restore "services/auth/PlataformaEducacional.Auth.Api/PlataformaEducacional.Auth.Api.csproj"
COPY . .
WORKDIR "services/auth/PlataformaEducacional.Auth.Api"
RUN dotnet build "PlataformaEducacional.Auth.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PlataformaEducacional.Auth.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PlataformaEducacional.Auth.Api.dll"]