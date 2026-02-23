#!/bin/bash
# Fetch Authentik API schema to verify endpoints

AUTHENTIK_URL="${AUTHENTIK_URL:-http://localhost:9100}"

if [ -z "$AUTHENTIK_TOKEN" ]; then
    echo "Usage: AUTHENTIK_TOKEN=your-token $0"
    exit 1
fi

echo "Fetching Authentik API schema..."
echo "URL: $AUTHENTIK_URL/api/v3/schema/"

curl -s -H "Authorization: Bearer $AUTHENTIK_TOKEN" \
     "$AUTHENTIK_URL/api/v3/schema/" | jq '.' > /tmp/authentik-schema.json 2>&1

if [ $? -eq 0 ]; then
    echo "✅ Schema saved to /tmp/authentik-schema.json"
    echo ""
    echo "Key endpoints to check:"
    echo "  - Prompt stages: /api/v3/stages/prompt/"
    echo "  - Flows: /api/v3/flows/instances/"
    echo "  - Flow bindings: /api/v3/flows/bindings/"
    echo ""
    echo "To inspect: jq '.paths | keys[]' /tmp/authentik-schema.json | grep -E '(prompt|flow|stage)'"
else
    echo "❌ Failed to fetch schema"
    cat /tmp/authentik-schema.json
fi
