FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["ReportEngine.API/ReportEngine.API.csproj", "ReportEngine.API/"]
COPY ["ReportEngine.Core/ReportEngine.Core.csproj", "ReportEngine.Core/"]
COPY ["ReportEngine.Infrastructure/ReportEngine.Infrastructure.csproj", "ReportEngine.Infrastructure/"]
RUN dotnet restore "ReportEngine.API/ReportEngine.API.csproj"
COPY . .
WORKDIR "/src/ReportEngine.API"
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM runtime AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "ReportEngine.API.dll"]
