FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["MeetingHub.API/MeetingHub.API.csproj", "MeetingHub.API/"]
COPY ["MeetingHub.Infrastructure/MeetingHub.Infrastructure.csproj", "MeetingHub.Infrastructure/"]
COPY ["MeetingHub.Core/MeetingHub.Core.csproj", "MeetingHub.Core/"]
RUN dotnet restore "MeetingHub.API/MeetingHub.API.csproj"
COPY . .
WORKDIR "/src/MeetingHub.API"
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM runtime AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "MeetingHub.API.dll"]
