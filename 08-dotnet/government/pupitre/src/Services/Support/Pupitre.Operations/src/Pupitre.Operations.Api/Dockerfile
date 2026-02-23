FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 60029

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/Services/Support/Pupitre.Operations/", "src/Services/Support/Pupitre.Operations/"]
COPY ["src/Shared/", "src/Shared/"]
RUN dotnet restore "src/Services/Support/Pupitre.Operations/src/Pupitre.Operations.Api/Pupitre.Operations.Api.csproj"
RUN dotnet publish "src/Services/Support/Pupitre.Operations/src/Pupitre.Operations.Api/Pupitre.Operations.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:60029
ENTRYPOINT ["dotnet", "Pupitre.Operations.Api.dll"]
