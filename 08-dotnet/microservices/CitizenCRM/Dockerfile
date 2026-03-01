FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["CitizenCRM.Infrastructure/CitizenCRM.Infrastructure.csproj", "CitizenCRM.Infrastructure/"]
COPY ["CitizenCRM.API/CitizenCRM.API.csproj", "CitizenCRM.API/"]
COPY ["CitizenCRM.Core/CitizenCRM.Core.csproj", "CitizenCRM.Core/"]
RUN dotnet restore "CitizenCRM.Infrastructure/CitizenCRM.Infrastructure.csproj"
COPY . .
WORKDIR "/src/CitizenCRM.Infrastructure"
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM runtime AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "CitizenCRM.Infrastructure.dll"]
