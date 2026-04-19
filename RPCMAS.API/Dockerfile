# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RPCMAS.API/RPCMAS.API.csproj", "RPCMAS.API/"]
COPY ["RPCMAS.Core/RPCMAS.Core.csproj", "RPCMAS.Core/"]
COPY ["RPCMAS.Infrastructure/RPCMAS.Infrastructure.csproj", "RPCMAS.Infrastructure/"]
COPY ["RPCMAS.ServiceDefaults/RPCMAS.ServiceDefaults.csproj", "RPCMAS.ServiceDefaults/"]
RUN dotnet restore "./RPCMAS.API/RPCMAS.API.csproj"
COPY . .
WORKDIR "/src/RPCMAS.API"
RUN dotnet build "./RPCMAS.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RPCMAS.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RPCMAS.API.dll"]