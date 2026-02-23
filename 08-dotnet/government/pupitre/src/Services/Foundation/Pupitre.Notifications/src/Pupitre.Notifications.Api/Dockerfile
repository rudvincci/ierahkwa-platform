FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 60008

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/Services/Foundation/Pupitre.Notifications/src/Pupitre.Notifications.Api/Pupitre.Notifications.Api.csproj", "src/Services/Foundation/Pupitre.Notifications/src/Pupitre.Notifications.Api/"]
COPY ["src/Services/Foundation/Pupitre.Notifications/src/Pupitre.Notifications.Application/Pupitre.Notifications.Application.csproj", "src/Services/Foundation/Pupitre.Notifications/src/Pupitre.Notifications.Application/"]
COPY ["src/Services/Foundation/Pupitre.Notifications/src/Pupitre.Notifications.Domain/Pupitre.Notifications.Domain.csproj", "src/Services/Foundation/Pupitre.Notifications/src/Pupitre.Notifications.Domain/"]
COPY ["src/Services/Foundation/Pupitre.Notifications/src/Pupitre.Notifications.Infrastructure/Pupitre.Notifications.Infrastructure.csproj", "src/Services/Foundation/Pupitre.Notifications/src/Pupitre.Notifications.Infrastructure/"]
COPY ["src/Services/Foundation/Pupitre.Notifications/src/Pupitre.Notifications.Contracts/Pupitre.Notifications.Contracts.csproj", "src/Services/Foundation/Pupitre.Notifications/src/Pupitre.Notifications.Contracts/"]
COPY ["src/Shared/Pupitre.Types/Pupitre.Types.csproj", "src/Shared/Pupitre.Types/"]
COPY ["src/Shared/Pupitre.Contracts/Pupitre.Contracts.csproj", "src/Shared/Pupitre.Contracts/"]
COPY ["src/Shared/Pupitre.Exceptions/Pupitre.Exceptions.csproj", "src/Shared/Pupitre.Exceptions/"]

RUN dotnet restore "src/Services/Foundation/Pupitre.Notifications/src/Pupitre.Notifications.Api/Pupitre.Notifications.Api.csproj"

COPY . .
WORKDIR "/src/src/Services/Foundation/Pupitre.Notifications/src/Pupitre.Notifications.Api"
RUN dotnet build "Pupitre.Notifications.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Pupitre.Notifications.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:60008
ENTRYPOINT ["dotnet", "Pupitre.Notifications.Api.dll"]
