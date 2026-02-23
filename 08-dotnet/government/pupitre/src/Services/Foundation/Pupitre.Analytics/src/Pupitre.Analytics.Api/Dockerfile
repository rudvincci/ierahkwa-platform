FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 60010

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/Services/Foundation/Pupitre.Analytics/src/Pupitre.Analytics.Api/Pupitre.Analytics.Api.csproj", "src/Services/Foundation/Pupitre.Analytics/src/Pupitre.Analytics.Api/"]
COPY ["src/Services/Foundation/Pupitre.Analytics/src/Pupitre.Analytics.Application/Pupitre.Analytics.Application.csproj", "src/Services/Foundation/Pupitre.Analytics/src/Pupitre.Analytics.Application/"]
COPY ["src/Services/Foundation/Pupitre.Analytics/src/Pupitre.Analytics.Domain/Pupitre.Analytics.Domain.csproj", "src/Services/Foundation/Pupitre.Analytics/src/Pupitre.Analytics.Domain/"]
COPY ["src/Services/Foundation/Pupitre.Analytics/src/Pupitre.Analytics.Infrastructure/Pupitre.Analytics.Infrastructure.csproj", "src/Services/Foundation/Pupitre.Analytics/src/Pupitre.Analytics.Infrastructure/"]
COPY ["src/Services/Foundation/Pupitre.Analytics/src/Pupitre.Analytics.Contracts/Pupitre.Analytics.Contracts.csproj", "src/Services/Foundation/Pupitre.Analytics/src/Pupitre.Analytics.Contracts/"]
COPY ["src/Shared/Pupitre.Types/Pupitre.Types.csproj", "src/Shared/Pupitre.Types/"]
COPY ["src/Shared/Pupitre.Contracts/Pupitre.Contracts.csproj", "src/Shared/Pupitre.Contracts/"]
COPY ["src/Shared/Pupitre.Exceptions/Pupitre.Exceptions.csproj", "src/Shared/Pupitre.Exceptions/"]

RUN dotnet restore "src/Services/Foundation/Pupitre.Analytics/src/Pupitre.Analytics.Api/Pupitre.Analytics.Api.csproj"

COPY . .
WORKDIR "/src/src/Services/Foundation/Pupitre.Analytics/src/Pupitre.Analytics.Api"
RUN dotnet build "Pupitre.Analytics.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Pupitre.Analytics.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:60010
ENTRYPOINT ["dotnet", "Pupitre.Analytics.Api.dll"]
