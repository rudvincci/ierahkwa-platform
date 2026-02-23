# =========================
#   BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /

# Copy csproj and restore
COPY Mamey/ ./Mamey/
COPY Mamey.Bank.Shared/ ./Mamey.Bank.Shared/
COPY Mamey.Mamey.ApplicationName.Monolith/src/UI/Mamey.ApplicationName.BlazorWasm/Mamey.ApplicationName.BlazorWasm.csproj ./Mamey.Mamey.ApplicationName.Monolith/src/UI/Mamey.ApplicationName.BlazorWasm/Mamey.ApplicationName.BlazorWasm.csproj
COPY Mamey.Mamey.ApplicationName.Monolith/src/UI/Mamey.ApplicationName.BlazorWasm/*.csproj ./Mamey.Mamey.ApplicationName.Monolith/src/UI/Mamey.ApplicationName.BlazorWasm/

RUN dotnet restore "Mamey.Mamey.ApplicationName.Monolith/src/UI/Mamey.ApplicationName.BlazorWasm/Mamey.ApplicationName.BlazorWasm.csproj"

# Copy the rest of the source
COPY Mamey.Mamey.ApplicationName.Monolith/src/UI/Mamey.ApplicationName.BlazorWasm/. ./Mamey.ApplicationName.BlazorWasm/src/UI/Mamey.ApplicationName.BlazorWasm/

WORKDIR "/Mamey.Mamey.ApplicationName.Monolith/src/UI/Mamey.ApplicationName.BlazorWasm/"

RUN dotnet build "Mamey.ApplicationName.BlazorWasm.csproj" -c Release -o /app/build

# Publish the Blazor WASM project
FROM build AS publish
RUN dotnet publish "Mamey.ApplicationName.BlazorWasm.csproj" -c Release -o /out


# =========================
#   FINAL STAGE
# =========================
# Use a small Linux image such as Nginx or Caddy to serve static content
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Copy the published wwwroot from the build stage
COPY --from=publish /out/wwwroot ./

# Expose port 80 (used by nginx)
EXPOSE 80

# Start nginx in foreground
ENTRYPOINT ["nginx", "-g", "daemon off;"]
