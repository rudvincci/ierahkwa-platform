FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 60105

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/Services/AI/Pupitre.AIAdaptive/", "src/Services/AI/Pupitre.AIAdaptive/"]
COPY ["src/Shared/", "src/Shared/"]
RUN dotnet restore "src/Services/AI/Pupitre.AIAdaptive/src/Pupitre.AIAdaptive.Api/Pupitre.AIAdaptive.Api.csproj"
RUN dotnet publish "src/Services/AI/Pupitre.AIAdaptive/src/Pupitre.AIAdaptive.Api/Pupitre.AIAdaptive.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:60105
ENTRYPOINT ["dotnet", "Pupitre.AIAdaptive.Api.dll"]
