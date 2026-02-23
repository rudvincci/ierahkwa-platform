FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 60108

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/Services/AI/Pupitre.AIRecommendations/", "src/Services/AI/Pupitre.AIRecommendations/"]
COPY ["src/Shared/", "src/Shared/"]
RUN dotnet restore "src/Services/AI/Pupitre.AIRecommendations/src/Pupitre.AIRecommendations.Api/Pupitre.AIRecommendations.Api.csproj"
RUN dotnet publish "src/Services/AI/Pupitre.AIRecommendations/src/Pupitre.AIRecommendations.Api/Pupitre.AIRecommendations.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:60108
ENTRYPOINT ["dotnet", "Pupitre.AIRecommendations.Api.dll"]
