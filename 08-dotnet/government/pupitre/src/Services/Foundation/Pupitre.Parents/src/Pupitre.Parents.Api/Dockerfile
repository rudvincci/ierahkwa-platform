FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 60009

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/Services/Foundation/Pupitre.Parents/src/Pupitre.Parents.Api/Pupitre.Parents.Api.csproj", "src/Services/Foundation/Pupitre.Parents/src/Pupitre.Parents.Api/"]
COPY ["src/Services/Foundation/Pupitre.Parents/src/Pupitre.Parents.Application/Pupitre.Parents.Application.csproj", "src/Services/Foundation/Pupitre.Parents/src/Pupitre.Parents.Application/"]
COPY ["src/Services/Foundation/Pupitre.Parents/src/Pupitre.Parents.Domain/Pupitre.Parents.Domain.csproj", "src/Services/Foundation/Pupitre.Parents/src/Pupitre.Parents.Domain/"]
COPY ["src/Services/Foundation/Pupitre.Parents/src/Pupitre.Parents.Infrastructure/Pupitre.Parents.Infrastructure.csproj", "src/Services/Foundation/Pupitre.Parents/src/Pupitre.Parents.Infrastructure/"]
COPY ["src/Services/Foundation/Pupitre.Parents/src/Pupitre.Parents.Contracts/Pupitre.Parents.Contracts.csproj", "src/Services/Foundation/Pupitre.Parents/src/Pupitre.Parents.Contracts/"]
COPY ["src/Shared/Pupitre.Types/Pupitre.Types.csproj", "src/Shared/Pupitre.Types/"]
COPY ["src/Shared/Pupitre.Contracts/Pupitre.Contracts.csproj", "src/Shared/Pupitre.Contracts/"]
COPY ["src/Shared/Pupitre.Exceptions/Pupitre.Exceptions.csproj", "src/Shared/Pupitre.Exceptions/"]

RUN dotnet restore "src/Services/Foundation/Pupitre.Parents/src/Pupitre.Parents.Api/Pupitre.Parents.Api.csproj"

COPY . .
WORKDIR "/src/src/Services/Foundation/Pupitre.Parents/src/Pupitre.Parents.Api"
RUN dotnet build "Pupitre.Parents.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Pupitre.Parents.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:60009
ENTRYPOINT ["dotnet", "Pupitre.Parents.Api.dll"]
