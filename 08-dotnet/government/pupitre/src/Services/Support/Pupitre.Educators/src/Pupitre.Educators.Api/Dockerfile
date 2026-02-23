FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 60022

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/Services/Support/Pupitre.Educators/", "src/Services/Support/Pupitre.Educators/"]
COPY ["src/Shared/", "src/Shared/"]
RUN dotnet restore "src/Services/Support/Pupitre.Educators/src/Pupitre.Educators.Api/Pupitre.Educators.Api.csproj"
RUN dotnet publish "src/Services/Support/Pupitre.Educators/src/Pupitre.Educators.Api/Pupitre.Educators.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:60022
ENTRYPOINT ["dotnet", "Pupitre.Educators.Api.dll"]
