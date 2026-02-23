FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 60003

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/Services/Foundation/Pupitre.Curricula/src/Pupitre.Curricula.Api/Pupitre.Curricula.Api.csproj", "src/Services/Foundation/Pupitre.Curricula/src/Pupitre.Curricula.Api/"]
COPY ["src/Services/Foundation/Pupitre.Curricula/src/Pupitre.Curricula.Application/Pupitre.Curricula.Application.csproj", "src/Services/Foundation/Pupitre.Curricula/src/Pupitre.Curricula.Application/"]
COPY ["src/Services/Foundation/Pupitre.Curricula/src/Pupitre.Curricula.Domain/Pupitre.Curricula.Domain.csproj", "src/Services/Foundation/Pupitre.Curricula/src/Pupitre.Curricula.Domain/"]
COPY ["src/Services/Foundation/Pupitre.Curricula/src/Pupitre.Curricula.Infrastructure/Pupitre.Curricula.Infrastructure.csproj", "src/Services/Foundation/Pupitre.Curricula/src/Pupitre.Curricula.Infrastructure/"]
COPY ["src/Services/Foundation/Pupitre.Curricula/src/Pupitre.Curricula.Contracts/Pupitre.Curricula.Contracts.csproj", "src/Services/Foundation/Pupitre.Curricula/src/Pupitre.Curricula.Contracts/"]
COPY ["src/Shared/Pupitre.Types/Pupitre.Types.csproj", "src/Shared/Pupitre.Types/"]
COPY ["src/Shared/Pupitre.Contracts/Pupitre.Contracts.csproj", "src/Shared/Pupitre.Contracts/"]
COPY ["src/Shared/Pupitre.Exceptions/Pupitre.Exceptions.csproj", "src/Shared/Pupitre.Exceptions/"]

RUN dotnet restore "src/Services/Foundation/Pupitre.Curricula/src/Pupitre.Curricula.Api/Pupitre.Curricula.Api.csproj"

COPY . .
WORKDIR "/src/src/Services/Foundation/Pupitre.Curricula/src/Pupitre.Curricula.Api"
RUN dotnet build "Pupitre.Curricula.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Pupitre.Curricula.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:60003
ENTRYPOINT ["dotnet", "Pupitre.Curricula.Api.dll"]
