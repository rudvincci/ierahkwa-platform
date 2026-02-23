#!/bin/bash
# Verification script to check Authentik setup for INK government

set -e

AUTHENTIK_URL="${AUTHENTIK_URL:-http://localhost:9100}"

if [ -z "$AUTHENTIK_TOKEN" ]; then
    read -p "Enter your Authentik API token: " AUTHENTIK_TOKEN
fi

if [ -z "$AUTHENTIK_TOKEN" ]; then
    echo "Error: No token provided"
    exit 1
fi

AUTH_HEADER="Authorization: Bearer $AUTHENTIK_TOKEN"

echo "=== Authentik Setup Verification for INK ==="
echo ""

# Function to check resource
check_resource() {
    local resource_type=$1
    local api_endpoint=$2
    local filter=$3
    local description=$4
    
    echo "Checking $description..."
    result=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL$api_endpoint" | jq -r "$filter" 2>/dev/null)
    
    if [ -n "$result" ] && [ "$result" != "null" ] && [ "$result" != "[]" ]; then
        count=$(echo "$result" | jq 'length' 2>/dev/null || echo "1")
        echo "  ✅ Found $count $description"
        if [ "$DEBUG" = "true" ]; then
            echo "$result" | jq '.' 2>/dev/null || echo "$result"
        fi
        return 0
    else
        echo "  ❌ No $description found"
        return 1
    fi
}

# Check groups
echo "1. Groups"
check_resource "groups" "/api/v3/core/groups/" '.results[] | select(.name | test("ink-|portal-|citizen")) | {name: .name, pk: .pk}' "INK groups" || true
echo ""

# Check flows
echo "2. Authentication Flows"
check_resource "flows" "/api/v3/flows/instances/" '.results[] | select(.name | test("INK|ink")) | {name: .name, slug: .slug, designation: .designation}' "INK flows" || true
echo ""

# Check prompt stages
echo "3. Prompt Stages"
check_resource "prompts" "/api/v3/stages/prompt/" '.results[] | select(.name | test("Terms|INK|ink")) | {name: .name, pk: .pk}' "prompt stages" || true
echo ""

# Check OAuth2 providers
echo "4. OAuth2/OIDC Providers"
check_resource "providers" "/api/v3/providers/oauth2/" '.results[] | select(.name | test("ink|INK")) | {name: .name, client_id: .client_id, pk: .pk}' "INK providers" || true
echo ""

# Check applications
echo "5. Applications"
check_resource "applications" "/api/v3/core/applications/" '.results[] | select(.name | test("INK|ink")) | {name: .name, slug: .slug, pk: .pk}' "INK applications" || true
echo ""

# Check brands
echo "6. Brands"
check_resource "brands" "/api/v3/brands/" '.results[] | select(.domain | test("ink|INK")) | {domain: .domain, pk: .pk}' "INK brands" || true
echo ""

# Check property mappings
echo "7. Property Mappings"
check_resource "mappings" "/api/v3/propertymappings/scope/" '.results[] | select(.name | test("INK|ink|Tenant|Roles")) | {name: .name, scope_name: .scope_name, pk: .pk}' "property mappings" || true
echo ""

# Check test users
echo "8. Test Users"
check_resource "users" "/api/v3/core/users/" '.results[] | select(.username | test("ink-")) | {username: .username, email: .email, is_active: .is_active}' "INK test users" || true
echo ""

# Get provider details for configuration
echo "9. OIDC Configuration"
providers=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/providers/oauth2/" | jq -r '.results[] | select(.name | test("ink|INK"))' 2>/dev/null)

if [ -n "$providers" ] && [ "$providers" != "null" ]; then
    provider_count=$(echo "$providers" | jq -s 'length')
    echo "  ✅ Found $provider_count provider(s)"
    echo ""
    echo "$providers" | jq -r '{
        name: .name,
        client_id: .client_id,
        authority: "'$AUTHENTIK_URL'/application/o/" + .client_id + "/"
    }' | jq -s '.'
    echo ""
    echo "To get Client Secret, check Authentik Admin UI or use:"
    echo "  curl -H \"$AUTH_HEADER\" \"$AUTHENTIK_URL/api/v3/providers/oauth2/{PROVIDER_PK}/\" | jq -r '.client_secret'"
else
    echo "  ❌ No providers found"
fi

echo ""
echo "========================================"
echo "Verification Complete"
echo "========================================"
echo ""
echo "To view in Authentik Admin UI:"
echo "  $AUTHENTIK_URL/if/admin/"
echo ""
