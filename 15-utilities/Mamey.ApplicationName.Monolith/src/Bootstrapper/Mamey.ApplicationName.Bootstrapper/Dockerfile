#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /
COPY Mamey/ ./Mamey/
COPY Mamey.Mamey.ApplicationName.Monolith/src/Bootstrapper/Mamey.ApplicationName.Bootstrapper/Mamey.ApplicationName.Bootstrapper.csproj ./Mamey.Mamey.ApplicationName.Monolith/src/Bootstrapper/Mamey.ApplicationName.Bootstrapper/Mamey.ApplicationName.Bootstrapper.csproj
COPY Mamey.Mamey.ApplicationName.Monolith/src/Modules/**/*.csproj ./Mamey.Mamey.ApplicationName.Monolith/src/Modules/**/*.csproj
# COPY ["src/Bootstrapper/Mamey.Bootstrapper/Mamey.Bootstrapper.csproj", "src/Bootstrapper/Mamey.Bootstrapper/"]
# COPY ["src/Shared/Mamey.MicroMonolith.Infrastructure/Mamey.MicroMonolith.Infrastructure.csproj", "src/Shared/Mamey.MicroMonolith.Infrastructure/"]

# COPY ["src/Shared/Mamey.ISO/Mamey.ISO.csproj", "src/Shared/Mamey.ISO/"]
# COPY ["src/Shared/Mamey.Shared.Abstractions.Modules/Mamey.Shared.Abstractions.Modules.csproj", "src/Shared/Mamey.Shared.Abstractions.Modules/"]
# COPY ["src/Modules/Saga/Mamey.ApplicationName.Modules.Saga.Api/Mamey.ApplicationName.Modules.Saga.Api.csproj", "src/Modules/Saga/Mamey.ApplicationName.Modules.Saga.Api/"]
# COPY ["src/Modules/Leads/Mamey.Modules.Leads.Api/Mamey.Modules.Leads.Api.csproj", "src/Modules/Leads/Mamey.Modules.Leads.Api/"]
# COPY ["src/Modules/Leads/Mamey.Modules.Leads.Core/Mamey.Modules.Leads.Core.csproj", "src/Modules/Leads/Mamey.Modules.Leads.Core/"]
# COPY ["src/Shared/Mamey.Azure.Graph/Mamey.Azure.Graph.csproj", "src/Shared/Mamey.Azure.Graph/"]

RUN dotnet restore "Mamey.Mamey.ApplicationName.Monolith/src/Bootstrapper/Mamey.ApplicationName.Bootstrapper/Mamey.ApplicationName.Bootstrapper.csproj" 

COPY Mamey.Mamey.ApplicationName.Monolith ./Mamey.Mamey.ApplicationName.Monolith/

WORKDIR "/Mamey.Mamey.ApplicationName.Monolith/src/Bootstrapper/Mamey.ApplicationName.Bootstrapper"



RUN dotnet build "Mamey.ApplicationName.Bootstrapper.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mamey.ApplicationName.Bootstrapper.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mamey.ApplicationName.Bootstrapper.dll"]
