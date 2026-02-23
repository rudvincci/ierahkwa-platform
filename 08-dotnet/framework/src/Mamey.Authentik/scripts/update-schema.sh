#!/bin/bash
# Script to fetch latest schema from Authentik instance

set -e

AUTHENTIK_URL="${1:-}"
SCHEMA_PATH="${2:-schema.json}"

if [ -z "$AUTHENTIK_URL" ]; then
    echo "Usage: $0 <authentik-url> [schema-path]"
    echo "Example: $0 https://authentik.company.com"
    exit 1
fi

echo "Fetching latest OpenAPI schema from $AUTHENTIK_URL/api/v3/schema/"

SCHEMA_URL="$AUTHENTIK_URL/api/v3/schema/?format=json"
curl -s -o "$SCHEMA_PATH" "$SCHEMA_URL" || {
    echo "Error: Failed to fetch schema from $SCHEMA_URL"
    exit 1
}

echo "Schema saved to $SCHEMA_PATH"
echo "Run generate-client.sh to regenerate the client code"
