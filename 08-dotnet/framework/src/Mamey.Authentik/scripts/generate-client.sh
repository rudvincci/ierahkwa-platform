#!/bin/bash
# Bash script to generate Authentik API client from OpenAPI schema

set -e

AUTHENTIK_URL="${1:-}"
OUTPUT_PATH="${2:-src/Mamey.Authentik.Generated}"
SCHEMA_PATH="${3:-schema.json}"

if [ -z "$AUTHENTIK_URL" ]; then
    echo "Usage: $0 <authentik-url> [output-path] [schema-path]"
    echo "Example: $0 https://authentik.company.com"
    exit 1
fi

echo "Fetching OpenAPI schema from $AUTHENTIK_URL/api/v3/schema/"

# Fetch schema
SCHEMA_URL="$AUTHENTIK_URL/api/v3/schema/?format=json"
curl -s -o "$SCHEMA_PATH" "$SCHEMA_URL" || {
    echo "Error: Failed to fetch schema from $SCHEMA_URL"
    exit 1
}

echo "Schema saved to $SCHEMA_PATH"

# Check if NSwag is installed
if ! command -v nswag &> /dev/null; then
    echo "NSwag CLI not found. Installing..."
    dotnet tool install -g NSwag.ConsoleCore
fi

echo "Generating client code using NSwag..."

# Generate client
nswag openapi2csclient \
    /input:"$SCHEMA_PATH" \
    /output:"$OUTPUT_PATH" \
    /namespace:Mamey.Authentik.Generated \
    /className:AuthentikClient \
    /generateClientClasses:true \
    /generateClientInterfaces:true \
    /useHttpClientCreationMethod:true \
    /useHttpRequestMessageCreationMethod:true \
    /useBaseUrl:true \
    /generateExceptionClasses:true \
    /exceptionClass:AuthentikApiException \
    /jsonLibrary:SystemTextJson \
    /dateType:System.DateTimeOffset \
    /generateOptionalParameters:true \
    /generateJsonMethods:false \
    /useTransformOptionsMethod:true \
    /useTransformResultMethod:true \
    /targetFramework:net9.0

echo "Client generation complete!"
