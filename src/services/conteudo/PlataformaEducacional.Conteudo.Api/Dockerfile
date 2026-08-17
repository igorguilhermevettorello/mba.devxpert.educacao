# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV DOCKER_SECRETS_PATH=/run/secrets/conteudo-api-segredo	

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./services/conteudo/PlataformaEducacional.Conteudo.Api/PlataformaEducacional.Conteudo.Api.csproj", "services/conteudo/PlataformaEducacional.Conteudo.Api//"]
COPY ["./buildingBlocks/PlataformaEducacional.Core/PlataformaEducacional.Core.csproj", "buildingBlocks/PlataformaEducacional.Core//"]
COPY ["./buildingBlocks/PlataformaEducacional.WebApi.Core/PlataformaEducacional.WebApi.Core.csproj", "buildingBlocks/PlataformaEducacional.WebApi.Core//"]
COPY ["./SeedDados/PlataformaEducacional.SeedDados/PlataformaEducacional.SeedDados.csproj", "SeedDados/PlataformaEducacional.SeedDados//"]
COPY ["./services/conteudo/PlataformaEducacional.Conteudo.Application/PlataformaEducacional.Conteudo.Application.csproj", "services/conteudo/PlataformaEducacional.Conteudo.Application//"]
COPY ["./services/conteudo/PlataformaEducacional.Conteudo.Domain/PlataformaEducacional.Conteudo.Domain.csproj", "services/conteudo/PlataformaEducacional.Conteudo.Domain//"]
COPY ["./services/conteudo/PlataformaEducacional.Conteudo.Data/PlataformaEducacional.Conteudo.Data.csproj", "services/conteudo/PlataformaEducacional.Conteudo.Data//"]
RUN dotnet restore "./services/conteudo/PlataformaEducacional.Conteudo.Api/PlataformaEducacional.Conteudo.Api.csproj"
COPY . .
WORKDIR "services/conteudo/PlataformaEducacional.Conteudo.Api"
RUN dotnet build "./PlataformaEducacional.Conteudo.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PlataformaEducacional.Conteudo.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PlataformaEducacional.Conteudo.Api.dll"]