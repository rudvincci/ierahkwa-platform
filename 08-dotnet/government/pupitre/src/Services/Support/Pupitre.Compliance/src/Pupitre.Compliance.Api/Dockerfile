FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 60027

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/Services/Support/Pupitre.Compliance/", "src/Services/Support/Pupitre.Compliance/"]
COPY ["src/Shared/", "src/Shared/"]
RUN dotnet restore "src/Services/Support/Pupitre.Compliance/src/Pupitre.Compliance.Api/Pupitre.Compliance.Api.csproj"
RUN dotnet publish "src/Services/Support/Pupitre.Compliance/src/Pupitre.Compliance.Api/Pupitre.Compliance.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:60027
ENTRYPOINT ["dotnet", "Pupitre.Compliance.Api.dll"]
