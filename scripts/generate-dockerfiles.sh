#!/bin/bash
# Generate Dockerfiles for all microservices
# v3.3.0 — Ierahkwa Platform

MICRO_DIR="/Users/ruddie/Desktop/files/Soberano-Organizado/08-dotnet/microservices"
COUNT=0

for dir in "$MICRO_DIR"/*/; do
  SERVICE=$(basename "$dir")
  CSPROJ="$SERVICE.csproj"
  DOCKERFILE="$dir/Dockerfile"

  # Skip if no .csproj at root level (multi-project services handled separately)
  if [ ! -f "$dir/$CSPROJ" ]; then
    # Try to find a .csproj in API subfolder pattern
    API_CSPROJ=$(find "$dir" -maxdepth 2 -name "*.API.csproj" -o -name "*.csproj" | head -1)
    if [ -z "$API_CSPROJ" ]; then
      echo "SKIP: $SERVICE (no .csproj found)"
      continue
    fi
    REL_CSPROJ=$(realpath --relative-to="$dir" "$API_CSPROJ")
    CSPROJ_DIR=$(dirname "$REL_CSPROJ")
    DLL_NAME=$(basename "$API_CSPROJ" .csproj)
  else
    REL_CSPROJ="$CSPROJ"
    CSPROJ_DIR="."
    DLL_NAME="$SERVICE"
  fi

  cat > "$DOCKERFILE" << DOCKERFILE_EOF
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["$REL_CSPROJ", "$CSPROJ_DIR/"]
RUN dotnet restore "$REL_CSPROJ"
COPY . .
WORKDIR "/src/$CSPROJ_DIR"
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM runtime AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \\
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "${DLL_NAME}.dll"]
DOCKERFILE_EOF

  COUNT=$((COUNT + 1))
  echo "OK: $SERVICE ($COUNT)"
done

echo ""
echo "=== Generated $COUNT Dockerfiles ==="
