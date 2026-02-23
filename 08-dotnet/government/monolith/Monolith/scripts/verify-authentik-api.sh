#!/bin/bash
# Script to verify Authentik API endpoints and schema
# This helps identify correct endpoints, methods, and required fields

set +e

AUTHENTIK_URL="${AUTHENTIK_URL:-http://localhost:9100}"

if [ -z "$AUTHENTIK_TOKEN" ]; then
    read -p "Enter your Authentik API token: " AUTHENTIK_TOKEN
fi

if [ -z "$AUTHENTIK_TOKEN" ]; then
    echo "Error: No token provided"
    exit 1
fi

AUTH_HEADER="Authorization: Bearer $AUTHENTIK_TOKEN"
CONTENT_TYPE="Content-Type: application/json"

echo "=== Authentik API Schema Verification ==="
echo ""
echo "Authentik URL: $AUTHENTIK_URL"
echo ""

# Test API connection
echo "1. Testing API connection..."
response=$(curl -s -w "\n%{http_code}" -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/core/users/me/")
http_code=$(echo "$response" | tail -n1)
if [ "$http_code" = "200" ]; then
    echo "  ✅ API connection successful"
else
    echo "  ❌ API connection failed (HTTP $http_code)"
    exit 1
fi
echo ""

# Fetch OpenAPI schema
echo "2. Fetching OpenAPI schema..."
schema_file="/tmp/authentik-schema-$$.json"
curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/schema/" > "$schema_file" 2>/dev/null

if [ -s "$schema_file" ] && jq . "$schema_file" > /dev/null 2>&1; then
    echo "  ✅ Schema fetched successfully"
else
    echo "  ⚠️  Could not fetch schema, will test endpoints directly"
    rm -f "$schema_file"
fi
echo ""

# Check property mappings endpoints
echo "3. Checking Property Mappings endpoints..."
echo "  Testing GET /api/v3/propertymappings/scope/..."
response=$(curl -s -w "\n%{http_code}" -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/propertymappings/scope/")
http_code=$(echo "$response" | tail -n1)
if [ "$http_code" = "200" ]; then
    echo "    ✅ GET endpoint works"
    count=$(echo "$response" | sed '$d' | jq -r '.results | length' 2>/dev/null || echo "0")
    echo "    Found $count existing scope mappings"
    
    # Test POST
    echo "  Testing POST /api/v3/propertymappings/scope/..."
    test_data='{"name":"test-mapping-'$(date +%s)'","scope_name":"test","expression":"return \"test\""}'
    post_response=$(curl -s -w "\n%{http_code}" -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
        "$AUTHENTIK_URL/api/v3/propertymappings/scope/" \
        -d "$test_data")
    post_code=$(echo "$post_response" | tail -n1)
    post_body=$(echo "$post_response" | sed '$d')
    
    if [ "$post_code" = "201" ] || [ "$post_code" = "200" ]; then
        echo "    ✅ POST endpoint works"
        test_pk=$(echo "$post_body" | jq -r '.pk // empty' 2>/dev/null)
        if [ -n "$test_pk" ] && [ "$test_pk" != "null" ]; then
            echo "    Created test mapping (PK: $test_pk) - cleaning up..."
            curl -s -X DELETE -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/propertymappings/scope/$test_pk/" > /dev/null
        fi
    elif [ "$post_code" = "405" ]; then
        echo "    ❌ POST not allowed (HTTP 405)"
        echo "    Response: $post_body" | head -c 200
        echo ""
    elif [ "$post_code" = "400" ]; then
        echo "    ⚠️  POST endpoint exists but validation failed (HTTP 400)"
        echo "    Response: $post_body" | head -c 500
        echo ""
    else
        echo "    ⚠️  POST returned HTTP $post_code"
        echo "    Response: $post_body" | head -c 200
        echo ""
    fi
else
    echo "    ❌ GET endpoint failed (HTTP $http_code)"
fi
echo ""

# Check prompt endpoints
echo "4. Checking Prompt endpoints..."
echo "  Testing GET /api/v3/stages/prompt/prompts/..."
response=$(curl -s -w "\n%{http_code}" -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/stages/prompt/prompts/")
http_code=$(echo "$response" | tail -n1)
if [ "$http_code" = "200" ]; then
    echo "    ✅ GET endpoint works"
    
    # Check what types are available from existing prompts
    types=$(echo "$response" | sed '$d' | jq -r '.results[].type' 2>/dev/null | sort -u)
    if [ -n "$types" ]; then
        echo "    Valid prompt types found: $(echo $types | tr '\n' ' ')"
    fi
    
    # Test POST with minimal data
    echo "  Testing POST /api/v3/stages/prompt/prompts/..."
    test_data='{"name":"test-prompt-'$(date +%s)'","field_key":"test_field","label":"Test Field","type":"text","required":false,"order":0}'
    post_response=$(curl -s -w "\n%{http_code}" -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
        "$AUTHENTIK_URL/api/v3/stages/prompt/prompts/" \
        -d "$test_data")
    post_code=$(echo "$post_response" | tail -n1)
    post_body=$(echo "$post_response" | sed '$d')
    
    if [ "$post_code" = "201" ] || [ "$post_code" = "200" ]; then
        echo "    ✅ POST endpoint works"
        test_pk=$(echo "$post_body" | jq -r '.pk // empty' 2>/dev/null)
        if [ -n "$test_pk" ] && [ "$test_pk" != "null" ]; then
            echo "    Created test prompt (PK: $test_pk) - cleaning up..."
            curl -s -X DELETE -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/stages/prompt/prompts/$test_pk/" > /dev/null
        fi
    elif [ "$post_code" = "400" ]; then
        echo "    ⚠️  POST validation failed (HTTP 400)"
        echo "    Response: $post_body" | head -c 500
        echo ""
        echo "    This shows what fields are required/invalid"
    else
        echo "    ⚠️  POST returned HTTP $post_code"
        echo "    Response: $post_body" | head -c 200
        echo ""
    fi
else
    echo "    ❌ GET endpoint failed (HTTP $http_code)"
fi
echo ""

# Check prompt stages endpoints
echo "5. Checking Prompt Stage endpoints..."
echo "  Testing GET /api/v3/stages/prompt/stages/..."
response=$(curl -s -w "\n%{http_code}" -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/stages/prompt/stages/")
http_code=$(echo "$response" | tail -n1)
if [ "$http_code" = "200" ]; then
    echo "    ✅ GET endpoint works"
    count=$(echo "$response" | sed '$d' | jq -r '.results | length' 2>/dev/null || echo "0")
    echo "    Found $count existing prompt stages"
    
    # Check structure of existing stages
    if [ "$count" -gt 0 ]; then
        first_stage=$(echo "$response" | sed '$d' | jq -r '.results[0]' 2>/dev/null)
        echo "    Example stage structure:"
        echo "$first_stage" | jq '{name, fields: (.fields | length), validation_policies: (.validation_policies | length)}' 2>/dev/null || echo "      (could not parse)"
    fi
else
    echo "    ❌ GET endpoint failed (HTTP $http_code)"
fi
echo ""

# Check OAuth2 provider endpoints
echo "6. Checking OAuth2 Provider endpoints..."
echo "  Testing GET /api/v3/providers/oauth2/..."
response=$(curl -s -w "\n%{http_code}" -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/providers/oauth2/")
http_code=$(echo "$response" | tail -n1)
if [ "$http_code" = "200" ]; then
    echo "    ✅ GET endpoint works"
    count=$(echo "$response" | sed '$d' | jq -r '.results | length' 2>/dev/null || echo "0")
    echo "    Found $count existing OAuth2 providers"
    
    # Check structure of existing provider
    if [ "$count" -gt 0 ]; then
        first_provider=$(echo "$response" | sed '$d' | jq -r '.results[0]' 2>/dev/null)
        echo "    Example provider structure:"
        echo "$first_provider" | jq '{name, client_id, property_mappings: (.property_mappings | length), authorization_flow}' 2>/dev/null || echo "      (could not parse)"
    fi
else
    echo "    ❌ GET endpoint failed (HTTP $http_code)"
fi
echo ""

# Extract schema information if available
if [ -f "$schema_file" ]; then
    echo "7. Analyzing OpenAPI Schema..."
    
    # Check property mappings paths
    echo "  Property Mappings endpoints in schema:"
    jq -r '.paths | keys[] | select(contains("propertymapping"))' "$schema_file" 2>/dev/null | head -10 || echo "    (could not extract)"
    echo ""
    
    # Check prompt paths
    echo "  Prompt/Stage endpoints in schema:"
    jq -r '.paths | keys[] | select(contains("prompt"))' "$schema_file" 2>/dev/null | head -10 || echo "    (could not extract)"
    echo ""
    
    # Check provider paths
    echo "  Provider endpoints in schema:"
    jq -r '.paths | keys[] | select(contains("provider"))' "$schema_file" 2>/dev/null | head -10 || echo "    (could not extract)"
    echo ""
    
    rm -f "$schema_file"
fi

# Summary
echo "========================================"
echo "Summary"
echo "========================================"
echo ""
echo "Next steps:"
echo "1. Review the endpoint test results above"
echo "2. If property mappings POST returns 405, you may need to:"
echo "   - Create mappings manually in Admin UI"
echo "   - Use a different endpoint (check schema)"
echo "   - Check if your Authentik version supports API creation"
echo ""
echo "3. If prompt creation fails, check the error response"
echo "   for required fields and valid type values"
echo ""
echo "4. Schema file was saved to: $schema_file (if fetched)"
echo "   You can inspect it with: jq . $schema_file"
echo ""
