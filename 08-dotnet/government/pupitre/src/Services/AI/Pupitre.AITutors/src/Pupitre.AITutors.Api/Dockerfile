FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 60101

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/Services/AI/Pupitre.AITutors/", "src/Services/AI/Pupitre.AITutors/"]
COPY ["src/Shared/", "src/Shared/"]
RUN dotnet restore "src/Services/AI/Pupitre.AITutors/src/Pupitre.AITutors.Api/Pupitre.AITutors.Api.csproj"
RUN dotnet publish "src/Services/AI/Pupitre.AITutors/src/Pupitre.AITutors.Api/Pupitre.AITutors.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:60101
ENTRYPOINT ["dotnet", "Pupitre.AITutors.Api.dll"]
