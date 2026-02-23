FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 60002

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/Services/Foundation/Pupitre.GLEs/src/Pupitre.GLEs.Api/Pupitre.GLEs.Api.csproj", "src/Services/Foundation/Pupitre.GLEs/src/Pupitre.GLEs.Api/"]
COPY ["src/Services/Foundation/Pupitre.GLEs/src/Pupitre.GLEs.Application/Pupitre.GLEs.Application.csproj", "src/Services/Foundation/Pupitre.GLEs/src/Pupitre.GLEs.Application/"]
COPY ["src/Services/Foundation/Pupitre.GLEs/src/Pupitre.GLEs.Domain/Pupitre.GLEs.Domain.csproj", "src/Services/Foundation/Pupitre.GLEs/src/Pupitre.GLEs.Domain/"]
COPY ["src/Services/Foundation/Pupitre.GLEs/src/Pupitre.GLEs.Infrastructure/Pupitre.GLEs.Infrastructure.csproj", "src/Services/Foundation/Pupitre.GLEs/src/Pupitre.GLEs.Infrastructure/"]
COPY ["src/Services/Foundation/Pupitre.GLEs/src/Pupitre.GLEs.Contracts/Pupitre.GLEs.Contracts.csproj", "src/Services/Foundation/Pupitre.GLEs/src/Pupitre.GLEs.Contracts/"]
COPY ["src/Shared/Pupitre.Types/Pupitre.Types.csproj", "src/Shared/Pupitre.Types/"]
COPY ["src/Shared/Pupitre.Contracts/Pupitre.Contracts.csproj", "src/Shared/Pupitre.Contracts/"]
COPY ["src/Shared/Pupitre.Exceptions/Pupitre.Exceptions.csproj", "src/Shared/Pupitre.Exceptions/"]

RUN dotnet restore "src/Services/Foundation/Pupitre.GLEs/src/Pupitre.GLEs.Api/Pupitre.GLEs.Api.csproj"

COPY . .
WORKDIR "/src/src/Services/Foundation/Pupitre.GLEs/src/Pupitre.GLEs.Api"
RUN dotnet build "Pupitre.GLEs.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Pupitre.GLEs.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:60002
ENTRYPOINT ["dotnet", "Pupitre.GLEs.Api.dll"]
