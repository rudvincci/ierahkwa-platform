FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 60103

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/Services/AI/Pupitre.AIContent/", "src/Services/AI/Pupitre.AIContent/"]
COPY ["src/Shared/", "src/Shared/"]
RUN dotnet restore "src/Services/AI/Pupitre.AIContent/src/Pupitre.AIContent.Api/Pupitre.AIContent.Api.csproj"
RUN dotnet publish "src/Services/AI/Pupitre.AIContent/src/Pupitre.AIContent.Api/Pupitre.AIContent.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:60103
ENTRYPOINT ["dotnet", "Pupitre.AIContent.Api.dll"]
