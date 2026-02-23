#!/bin/bash
# Quick script to test Authentik API token

AUTHENTIK_URL="${AUTHENTIK_URL:-http://localhost:9100}"

if [ -z "$1" ]; then
    echo "Usage: $0 <AUTHENTIK_TOKEN>"
    echo ""
    echo "Example:"
    echo "  export AUTHENTIK_TOKEN='your-token-here'"
    echo "  $0 \$AUTHENTIK_TOKEN"
    exit 1
fi

TOKEN="$1"

echo "Testing Authentik API connection..."
echo "URL: $AUTHENTIK_URL"
echo "Token length: ${#TOKEN} characters"
echo ""

# Test endpoint
ENDPOINT="/api/v3/core/users/me/"
echo "Testing endpoint: $ENDPOINT"

response=$(curl -s -w "\n%{http_code}" \
    -H "Authorization: Bearer $TOKEN" \
    -H "Content-Type: application/json" \
    "$AUTHENTIK_URL$ENDPOINT")

http_code=$(echo "$response" | tail -n1)
body=$(echo "$response" | sed '$d')

echo "HTTP Status: $http_code"
echo ""

if [ "$http_code" = "200" ]; then
    echo "✅ SUCCESS! Token is valid"
    echo ""
    echo "Response:"
    echo "$body" | jq '.' 2>/dev/null || echo "$body"
elif [ "$http_code" = "401" ]; then
    echo "❌ Authentication failed (401 Unauthorized)"
    echo ""
    echo "Possible issues:"
    echo "  - Token is incorrect or expired"
    echo "  - Token format is wrong"
    echo "  - Token doesn't have required permissions"
    echo ""
    echo "Response:"
    echo "$body"
elif [ "$http_code" = "403" ]; then
    echo "❌ Forbidden (403)"
    echo "  Token is valid but lacks permissions"
    echo ""
    echo "Response:"
    echo "$body"
elif [ "$http_code" = "404" ]; then
    echo "⚠️  Endpoint not found (404)"
    echo "  Trying alternative endpoint..."
    
    # Try alternative
    response2=$(curl -s -w "\n%{http_code}" \
        -H "Authorization: Bearer $TOKEN" \
        -H "Content-Type: application/json" \
        "$AUTHENTIK_URL/api/v3/core/users/")
    
    http_code2=$(echo "$response2" | tail -n1)
    if [ "$http_code2" = "200" ]; then
        echo "✅ SUCCESS with alternative endpoint!"
    else
        echo "❌ Alternative endpoint also failed (HTTP $http_code2)"
    fi
else
    echo "❌ Unexpected response (HTTP $http_code)"
    echo ""
    echo "Response:"
    echo "$body"
fi

echo ""
echo "Testing basic connectivity..."
if curl -s -o /dev/null -w "%{http_code}" "$AUTHENTIK_URL/if/admin/" | grep -q "200\|302"; then
    echo "✅ Authentik is accessible"
else
    echo "❌ Cannot reach Authentik"
    echo "   Check: docker compose -f docker-compose.authentik.console.yml ps"
fi
