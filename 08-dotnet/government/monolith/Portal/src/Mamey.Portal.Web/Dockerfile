FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

#
# Release builds restore Mamey.* packages from the host-local feed.
# For macOS/Windows Docker Desktop that is typically: host.docker.internal:4000
#
ARG NUGET_FEED=http://host.docker.internal:4000/v3/index.json

# Cache-friendly restore:
# Copy only project files first, restore, then copy the rest.
COPY src/Mamey.Portal.Web/Mamey.Portal.Web.csproj src/Mamey.Portal.Web/
COPY src/Mamey.Portal.Shared/Mamey.Portal.Shared.csproj src/Mamey.Portal.Shared/
COPY src/Mamey.Portal.Citizenship.Application/Mamey.Portal.Citizenship.Application.csproj src/Mamey.Portal.Citizenship.Application/
COPY src/Mamey.Portal.Citizenship.Infrastructure/Mamey.Portal.Citizenship.Infrastructure.csproj src/Mamey.Portal.Citizenship.Infrastructure/
COPY src/Mamey.Portal.Cms.Application/Mamey.Portal.Cms.Application.csproj src/Mamey.Portal.Cms.Application/
COPY src/Mamey.Portal.Tenant.Application/Mamey.Portal.Tenant.Application.csproj src/Mamey.Portal.Tenant.Application/
COPY src/Mamey.Portal.Library.Application/Mamey.Portal.Library.Application.csproj src/Mamey.Portal.Library.Application/
COPY src/Mamey.Portal.Auth.Application/Mamey.Portal.Auth.Application.csproj src/Mamey.Portal.Auth.Application/

COPY Directory.Build.props ./
COPY nuget.portal.config /tmp/nuget.portal.config

RUN dotnet restore src/Mamey.Portal.Web/Mamey.Portal.Web.csproj \
  -p:Configuration=Release \
  --configfile /tmp/nuget.portal.config \
  --source https://api.nuget.org/v3/index.json \
  --source $NUGET_FEED

# Now copy the rest of the source (build context is kept small via .dockerignore)
COPY . .

RUN dotnet publish src/Mamey.Portal.Web/Mamey.Portal.Web.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Mamey.Portal.Web.dll"]


