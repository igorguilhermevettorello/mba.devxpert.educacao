# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/services/conteudo/PlataformaEducacional.Conteudo.Api/PlataformaEducacional.Conteudo.Api.csproj", "src/services/conteudo/PlataformaEducacional.Conteudo.Api/"]
COPY ["src/buildingBlocks/PlataformaEducacional.Core/PlataformaEducacional.Core.csproj", "src/buildingBlocks/PlataformaEducacional.Core/"]
COPY ["src/buildingBlocks/PlataformaEducacional.WebApi.Core/PlataformaEducacional.WebApi.Core.csproj", "src/buildingBlocks/PlataformaEducacional.WebApi.Core/"]
COPY ["src/SeedDados/PlataformaEducacional.SeedDados/PlataformaEducacional.SeedDados.csproj", "src/SeedDados/PlataformaEducacional.SeedDados/"]
COPY ["src/services/conteudo/PlataformaEducacional.Conteudo.Application/PlataformaEducacional.Conteudo.Application.csproj", "src/services/conteudo/PlataformaEducacional.Conteudo.Application/"]
COPY ["src/services/conteudo/PlataformaEducacional.Conteudo.Domain/PlataformaEducacional.Conteudo.Domain.csproj", "src/services/conteudo/PlataformaEducacional.Conteudo.Domain/"]
COPY ["src/services/conteudo/PlataformaEducacional.Conteudo.Data/PlataformaEducacional.Conteudo.Data.csproj", "src/services/conteudo/PlataformaEducacional.Conteudo.Data/"]
RUN dotnet restore "./src/services/conteudo/PlataformaEducacional.Conteudo.Api/PlataformaEducacional.Conteudo.Api.csproj"
COPY . .
WORKDIR "/src/src/services/conteudo/PlataformaEducacional.Conteudo.Api"
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