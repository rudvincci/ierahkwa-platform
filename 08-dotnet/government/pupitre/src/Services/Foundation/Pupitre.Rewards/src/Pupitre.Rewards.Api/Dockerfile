FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 60007

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/Services/Foundation/Pupitre.Rewards/src/Pupitre.Rewards.Api/Pupitre.Rewards.Api.csproj", "src/Services/Foundation/Pupitre.Rewards/src/Pupitre.Rewards.Api/"]
COPY ["src/Services/Foundation/Pupitre.Rewards/src/Pupitre.Rewards.Application/Pupitre.Rewards.Application.csproj", "src/Services/Foundation/Pupitre.Rewards/src/Pupitre.Rewards.Application/"]
COPY ["src/Services/Foundation/Pupitre.Rewards/src/Pupitre.Rewards.Domain/Pupitre.Rewards.Domain.csproj", "src/Services/Foundation/Pupitre.Rewards/src/Pupitre.Rewards.Domain/"]
COPY ["src/Services/Foundation/Pupitre.Rewards/src/Pupitre.Rewards.Infrastructure/Pupitre.Rewards.Infrastructure.csproj", "src/Services/Foundation/Pupitre.Rewards/src/Pupitre.Rewards.Infrastructure/"]
COPY ["src/Services/Foundation/Pupitre.Rewards/src/Pupitre.Rewards.Contracts/Pupitre.Rewards.Contracts.csproj", "src/Services/Foundation/Pupitre.Rewards/src/Pupitre.Rewards.Contracts/"]
COPY ["src/Shared/Pupitre.Types/Pupitre.Types.csproj", "src/Shared/Pupitre.Types/"]
COPY ["src/Shared/Pupitre.Contracts/Pupitre.Contracts.csproj", "src/Shared/Pupitre.Contracts/"]
COPY ["src/Shared/Pupitre.Exceptions/Pupitre.Exceptions.csproj", "src/Shared/Pupitre.Exceptions/"]

RUN dotnet restore "src/Services/Foundation/Pupitre.Rewards/src/Pupitre.Rewards.Api/Pupitre.Rewards.Api.csproj"

COPY . .
WORKDIR "/src/src/Services/Foundation/Pupitre.Rewards/src/Pupitre.Rewards.Api"
RUN dotnet build "Pupitre.Rewards.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Pupitre.Rewards.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:60007
ENTRYPOINT ["dotnet", "Pupitre.Rewards.Api.dll"]
