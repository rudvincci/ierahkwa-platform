FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 60025

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/Services/Support/Pupitre.Aftercare/", "src/Services/Support/Pupitre.Aftercare/"]
COPY ["src/Shared/", "src/Shared/"]
RUN dotnet restore "src/Services/Support/Pupitre.Aftercare/src/Pupitre.Aftercare.Api/Pupitre.Aftercare.Api.csproj"
RUN dotnet publish "src/Services/Support/Pupitre.Aftercare/src/Pupitre.Aftercare.Api/Pupitre.Aftercare.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:60025
ENTRYPOINT ["dotnet", "Pupitre.Aftercare.Api.dll"]
