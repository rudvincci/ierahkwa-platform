FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 60006

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/Services/Foundation/Pupitre.Progress/src/Pupitre.Progress.Api/Pupitre.Progress.Api.csproj", "src/Services/Foundation/Pupitre.Progress/src/Pupitre.Progress.Api/"]
COPY ["src/Services/Foundation/Pupitre.Progress/src/Pupitre.Progress.Application/Pupitre.Progress.Application.csproj", "src/Services/Foundation/Pupitre.Progress/src/Pupitre.Progress.Application/"]
COPY ["src/Services/Foundation/Pupitre.Progress/src/Pupitre.Progress.Domain/Pupitre.Progress.Domain.csproj", "src/Services/Foundation/Pupitre.Progress/src/Pupitre.Progress.Domain/"]
COPY ["src/Services/Foundation/Pupitre.Progress/src/Pupitre.Progress.Infrastructure/Pupitre.Progress.Infrastructure.csproj", "src/Services/Foundation/Pupitre.Progress/src/Pupitre.Progress.Infrastructure/"]
COPY ["src/Services/Foundation/Pupitre.Progress/src/Pupitre.Progress.Contracts/Pupitre.Progress.Contracts.csproj", "src/Services/Foundation/Pupitre.Progress/src/Pupitre.Progress.Contracts/"]
COPY ["src/Shared/Pupitre.Types/Pupitre.Types.csproj", "src/Shared/Pupitre.Types/"]
COPY ["src/Shared/Pupitre.Contracts/Pupitre.Contracts.csproj", "src/Shared/Pupitre.Contracts/"]
COPY ["src/Shared/Pupitre.Exceptions/Pupitre.Exceptions.csproj", "src/Shared/Pupitre.Exceptions/"]

RUN dotnet restore "src/Services/Foundation/Pupitre.Progress/src/Pupitre.Progress.Api/Pupitre.Progress.Api.csproj"

COPY . .
WORKDIR "/src/src/Services/Foundation/Pupitre.Progress/src/Pupitre.Progress.Api"
RUN dotnet build "Pupitre.Progress.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Pupitre.Progress.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:60006
ENTRYPOINT ["dotnet", "Pupitre.Progress.Api.dll"]
