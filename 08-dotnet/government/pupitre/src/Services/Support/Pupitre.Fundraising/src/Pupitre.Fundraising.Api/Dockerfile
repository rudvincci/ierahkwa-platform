FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 60023

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/Services/Support/Pupitre.Fundraising/", "src/Services/Support/Pupitre.Fundraising/"]
COPY ["src/Shared/", "src/Shared/"]
RUN dotnet restore "src/Services/Support/Pupitre.Fundraising/src/Pupitre.Fundraising.Api/Pupitre.Fundraising.Api.csproj"
RUN dotnet publish "src/Services/Support/Pupitre.Fundraising/src/Pupitre.Fundraising.Api/Pupitre.Fundraising.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:60023
ENTRYPOINT ["dotnet", "Pupitre.Fundraising.Api.dll"]
