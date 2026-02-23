FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 60109

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/Services/AI/Pupitre.AITranslation/", "src/Services/AI/Pupitre.AITranslation/"]
COPY ["src/Shared/", "src/Shared/"]
RUN dotnet restore "src/Services/AI/Pupitre.AITranslation/src/Pupitre.AITranslation.Api/Pupitre.AITranslation.Api.csproj"
RUN dotnet publish "src/Services/AI/Pupitre.AITranslation/src/Pupitre.AITranslation.Api/Pupitre.AITranslation.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:60109
ENTRYPOINT ["dotnet", "Pupitre.AITranslation.Api.dll"]
