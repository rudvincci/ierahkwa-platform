# Mamey Government Portal - Dockerfile
# See https://aka.ms/containerfastmode for VS optimization

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the Mamey framework first (shared libraries)
COPY Mamey/ ./Mamey/

# Copy project files for restore
COPY Mamey.Government/Monolith/src/Bootstrapper/Mamey.Government.BlazorServer/Mamey.Government.BlazorServer.csproj ./Mamey.Government/Monolith/src/Bootstrapper/Mamey.Government.BlazorServer/
COPY Mamey.Government/Monolith/src/UI/Mamey.Government.UI/Mamey.Government.UI.csproj ./Mamey.Government/Monolith/src/UI/Mamey.Government.UI/

# Copy all module project files
COPY Mamey.Government/Monolith/src/Modules/ ./Mamey.Government/Monolith/src/Modules/

# Restore dependencies
RUN dotnet restore "Mamey.Government/Monolith/src/Bootstrapper/Mamey.Government.BlazorServer/Mamey.Government.BlazorServer.csproj"

# Copy everything else
COPY Mamey.Government/Monolith/ ./Mamey.Government/Monolith/

WORKDIR "/src/Mamey.Government/Monolith/src/Bootstrapper/Mamey.Government.BlazorServer"
RUN dotnet build "Mamey.Government.BlazorServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Mamey.Government.BlazorServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Default environment
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=https://+:443;http://+:80

HEALTHCHECK --interval=30s --timeout=10s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:80/health || exit 1

ENTRYPOINT ["dotnet", "Mamey.Government.BlazorServer.dll"]
