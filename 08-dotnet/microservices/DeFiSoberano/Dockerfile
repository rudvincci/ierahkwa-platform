FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["DeFi.API/DeFi.API.csproj", "DeFi.API/"]
COPY ["DeFi.Core/DeFi.Core.csproj", "DeFi.Core/"]
COPY ["DeFi.Infrastructure/DeFi.Infrastructure.csproj", "DeFi.Infrastructure/"]
RUN dotnet restore "DeFi.API/DeFi.API.csproj"
COPY . .
WORKDIR "/src/DeFi.API"
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM runtime AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "DeFi.Core.dll"]
