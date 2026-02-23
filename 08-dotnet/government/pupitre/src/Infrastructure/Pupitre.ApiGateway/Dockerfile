FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 60000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Infrastructure/Pupitre.ApiGateway/Pupitre.ApiGateway.csproj", "src/Infrastructure/Pupitre.ApiGateway/"]
RUN dotnet restore "src/Infrastructure/Pupitre.ApiGateway/Pupitre.ApiGateway.csproj"
COPY . .
WORKDIR "/src/src/Infrastructure/Pupitre.ApiGateway"
RUN dotnet build "Pupitre.ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Pupitre.ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pupitre.ApiGateway.dll"]
