#!/bin/bash
# Cleanup script to remove Authentik resources created by setup-authentik.sh
# This removes providers, applications, flows, groups, and users created for specified government tenants

set +e  # Don't exit on error - continue cleanup

AUTHENTIK_URL="${AUTHENTIK_URL:-http://localhost:9100}"
GOV_ID="${GOV_ID:-ink}"  # Default to INK, can be overridden

echo "=== Authentik Cleanup Script ==="
echo ""
echo "This script will remove resources created for government tenant: $GOV_ID"
echo "To clean a different tenant, set GOV_ID environment variable:"
echo "  export GOV_ID=springfield"
echo "  ./scripts/cleanup-authentik.sh"
echo ""

if [ -z "$AUTHENTIK_TOKEN" ]; then
    read -p "Enter your Authentik API token: " AUTHENTIK_TOKEN
fi

if [ -z "$AUTHENTIK_TOKEN" ]; then
    echo "Error: No token provided"
    exit 1
fi

AUTH_HEADER="Authorization: Bearer $AUTHENTIK_TOKEN"
CONTENT_TYPE="Content-Type: application/json"

echo ""
echo "Testing API connection..."
response=$(curl -s -w "\n%{http_code}" -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/core/users/")
http_code=$(echo "$response" | tail -n1)

if [ "$http_code" != "200" ]; then
    echo "Error: Failed to authenticate with Authentik API (HTTP $http_code)"
    exit 1
fi
echo "✅ API connection successful"
echo ""

# Function to delete resource by name pattern
delete_resources() {
    local resource_type=$1
    local api_endpoint=$2
    local name_pattern=$3
    local description=$4
    
    echo "Deleting $description..."
    
    # Get all resources
    resources=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL$api_endpoint" | jq -r ".results[] | select(.name | test(\"$name_pattern\")) | .pk" 2>/dev/null)
    
    if [ -z "$resources" ]; then
        echo "  ⏭️  No $description found"
        return
    fi
    
    count=0
    echo "$resources" | while read -r pk; do
        if [ -n "$pk" ] && [ "$pk" != "null" ]; then
            result=$(curl -s -w "\n%{http_code}" -X DELETE -H "$AUTH_HEADER" "$AUTHENTIK_URL$api_endpoint$pk/")
            http_code=$(echo "$result" | tail -n1)
            if [ "$http_code" = "204" ] || [ "$http_code" = "200" ]; then
                echo "  ✅ Deleted $description (PK: $pk)"
                count=$((count + 1))
            else
                echo "  ⚠️  Failed to delete $description (PK: $pk, HTTP: $http_code)"
            fi
        fi
    done
}

# Delete OAuth2 Providers
delete_resources "providers" "/api/v3/providers/oauth2/" "springfield|california|ink|INK|mamey-government-springfield|mamey-government-california|mamey-government-ink" "OAuth2 Providers"

# Delete Applications
delete_resources "applications" "/api/v3/core/applications/" "Springfield|California|INK|Ierahkwa|springfield|california|ink" "Applications"

# Delete Flows
delete_resources "flows" "/api/v3/flows/instances/" "Springfield|California|INK|Ierahkwa|springfield|california|ink" "Flows"

# Delete Prompt Stages (using correct endpoint)
delete_resources "prompts" "/api/v3/stages/prompt/stages/" "Springfield|California|springfield|california|INK|ink|Terms Acceptance" "Prompt Stages"

# Delete Groups
delete_resources "groups" "/api/v3/core/groups/" "springfield|california|ink-" "Groups"

# Delete Users
echo "Deleting test users..."
users=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/core/users/" | jq -r ".results[] | select(.username | test(\"springfield|california|ink-\")) | .pk" 2>/dev/null)

if [ -n "$users" ]; then
    echo "$users" | while read -r pk; do
        if [ -n "$pk" ] && [ "$pk" != "null" ]; then
            result=$(curl -s -w "\n%{http_code}" -X DELETE -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/core/users/$pk/")
            http_code=$(echo "$result" | tail -n1)
            if [ "$http_code" = "204" ] || [ "$http_code" = "200" ]; then
                echo "  ✅ Deleted user (PK: $pk)"
            else
                echo "  ⚠️  Failed to delete user (PK: $pk, HTTP: $http_code)"
            fi
        fi
    done
else
    echo "  ⏭️  No test users found"
fi

# Delete Property Mappings
delete_resources "mappings" "/api/v3/propertymappings/scope/" "Springfield|California|INK|Ierahkwa|springfield|california|ink|Tenant Mapping|Roles Mapping" "Property Mappings"

# Delete Brands (if API available)
echo "Deleting brands..."
brands=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/brands/" 2>/dev/null | jq -r ".results[]? | select(.domain | test(\"springfield|california|ink\")) | .pk" 2>/dev/null)
if [ -n "$brands" ]; then
    echo "$brands" | while read -r pk; do
        if [ -n "$pk" ] && [ "$pk" != "null" ]; then
            result=$(curl -s -w "\n%{http_code}" -X DELETE -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/brands/$pk/" 2>/dev/null)
            http_code=$(echo "$result" | tail -n1)
            if [ "$http_code" = "204" ] || [ "$http_code" = "200" ]; then
                echo "  ✅ Deleted brand (PK: $pk)"
            else
                echo "  ⚠️  Failed to delete brand (PK: $pk, HTTP: $http_code)"
            fi
        fi
    done
else
    echo "  ⏭️  No brands found or brands API not available"
fi

# Delete Tenants (if using native tenancy)
echo ""
echo "Deleting Authentik tenants..."
tenants=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/tenants/tenants/" 2>/dev/null | jq -r ".results[]? | select(.schema_name | test(\"springfield|california|ink\")) | .pk" 2>/dev/null)

if [ -n "$tenants" ]; then
    echo "$tenants" | while read -r pk; do
        if [ -n "$pk" ] && [ "$pk" != "null" ]; then
            result=$(curl -s -w "\n%{http_code}" -X DELETE -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/tenants/tenants/$pk/" 2>/dev/null)
            http_code=$(echo "$result" | tail -n1)
            if [ "$http_code" = "204" ] || [ "$http_code" = "200" ]; then
                echo "  ✅ Deleted tenant (PK: $pk)"
            else
                echo "  ⚠️  Failed to delete tenant (PK: $pk, HTTP: $http_code)"
            fi
        fi
    done
else
    echo "  ⏭️  No tenants found or tenancy API not available"
fi

echo ""
echo "========================================"
echo "✅ Cleanup Complete!"
echo "========================================"
echo ""
echo "You can now run setup-authentik.sh again with INK configuration."
echo ""
