#!/bin/bash
# Authentik OIDC Setup Script for Mamey Government Portal
# Enhanced for Hierarchical Multi-Tenant Support (B2B/B2C per Government)
# This script creates tenants, brands, flows, stages, prompts, providers, and applications
# for multiple government tenants, each with B2B (employees) and B2C (citizens) support

# Don't exit on error - we want to continue and report all issues
set +e

AUTHENTIK_URL="${AUTHENTIK_URL:-http://localhost:9100}"
PORTAL_URL="${PORTAL_URL:-https://localhost:7295}"
ENABLE_TENANCY="${ENABLE_TENANCY:-true}"
TENANT_SOURCE="${TENANT_SOURCE:-config}"  # database, config, api, env
DEBUG="${DEBUG:-false}"  # Set to "true" for verbose output

echo "=== Mamey Government Portal - Authentik Multi-Tenant Setup ==="
echo ""
echo "This script requires an Authentik API token."
echo "To get one:"
echo "  1. Log into Authentik Admin: $AUTHENTIK_URL/if/admin/"
echo "  2. Go to: Directory → Tokens and App passwords"
echo "  3. Click 'Create' and select 'App Password' with appropriate permissions"
echo "  4. Copy the token"
echo ""

if [ -z "$AUTHENTIK_TOKEN" ]; then
    read -p "Enter your Authentik API token: " AUTHENTIK_TOKEN
fi

if [ -z "$AUTHENTIK_TOKEN" ]; then
    echo "Error: No token provided"
    exit 1
fi

# Validate token format (should be reasonably long)
if [ ${#AUTHENTIK_TOKEN} -lt 20 ]; then
    echo "⚠️  Warning: Token seems too short. Authentik App Passwords are typically 40+ characters."
    echo "   Make sure you're using the full token from Authentik Admin → Directory → Tokens and App passwords"
    read -p "Continue anyway? (y/N): " continue_anyway
    if [ "$continue_anyway" != "y" ] && [ "$continue_anyway" != "Y" ]; then
        exit 1
    fi
fi

AUTH_HEADER="Authorization: Bearer $AUTHENTIK_TOKEN"
CONTENT_TYPE="Content-Type: application/json"

echo ""
echo "Testing API connection..."
echo "Authentik URL: $AUTHENTIK_URL"

# Check Authentik version first
echo "Checking Authentik version..."
VERSION_RESPONSE=$(curl -s "$AUTHENTIK_URL/api/v3/core/version/" 2>/dev/null || curl -s "$AUTHENTIK_URL/api/version/" 2>/dev/null)
if echo "$VERSION_RESPONSE" | grep -q "version"; then
    VERSION=$(echo "$VERSION_RESPONSE" | jq -r '.version // .version_current // "unknown"' 2>/dev/null || echo "unknown")
    echo "  Authentik version: $VERSION"
else
    echo "  Could not determine Authentik version"
fi

# Test multiple endpoints to find one that works
TEST_ENDPOINTS=(
    "/api/v3/core/users/me/"
    "/api/v3/core/users/"
    "/api/v3/core/groups/"
    "/api/v3/flows/instances/"
    "/api/core/users/"  # Fallback for older versions
)

API_WORKING=false
for endpoint in "${TEST_ENDPOINTS[@]}"; do
    if [ "$DEBUG" = "true" ]; then
        echo "  Testing endpoint: $endpoint"
    fi
    
    response=$(curl -s -w "\n%{http_code}" -H "$AUTH_HEADER" "$AUTHENTIK_URL$endpoint")
    http_code=$(echo "$response" | tail -n1)
    body=$(echo "$response" | sed '$d')
    
    if [ "$DEBUG" = "true" ]; then
        echo "  HTTP Code: $http_code"
        echo "  Response preview: $(echo "$body" | head -c 200)"
    fi
    
    if [ "$http_code" = "200" ]; then
        # Check if response contains valid JSON (has pk, results, or id field)
        if echo "$body" | grep -qE '"(pk|id|results|username)"'; then
            echo "✅ API connection successful (tested: $endpoint)"
            API_WORKING=true
            break
        fi
    elif [ "$http_code" = "401" ] || [ "$http_code" = "403" ]; then
        if [ "$DEBUG" = "true" ] || [ "$endpoint" = "${TEST_ENDPOINTS[0]}" ]; then
            echo "⚠️  Authentication failed (HTTP $http_code) on $endpoint"
            if [ "$DEBUG" = "true" ]; then
                echo "Full response: $body"
            else
                echo "Response preview: $(echo "$body" | head -c 200)"
            fi
        fi
    elif [ "$http_code" = "404" ]; then
        if [ "$DEBUG" = "true" ]; then
            echo "  Endpoint not found (404): $endpoint"
        fi
    fi
done

if [ "$API_WORKING" = false ]; then
    echo ""
    echo "❌ Error: Failed to authenticate with Authentik API"
    echo ""
    echo "Troubleshooting steps:"
    echo ""
    echo "1. Verify Authentik is running:"
    echo "   curl $AUTHENTIK_URL/if/admin/"
    echo "   docker compose -f docker-compose.authentik.console.yml ps"
    echo ""
    echo "2. Check token format:"
    echo "   - Should be 40+ characters long"
    echo "   - Get from: Authentik Admin → Directory → Tokens and App passwords"
    echo "   - Create new 'App Password' if needed"
    echo "   - Make sure to copy the FULL token (not truncated)"
    echo ""
    echo "3. Verify token permissions:"
    echo "   - Token needs: Core, Flows, Providers, Property Mappings permissions"
    echo "   - Check in Authentik Admin → Directory → Tokens"
    echo ""
    echo "4. Test token manually:"
    echo "   curl -H 'Authorization: Bearer YOUR_TOKEN' $AUTHENTIK_URL/api/v3/core/users/"
    echo ""
    echo "5. Check Authentik version compatibility:"
    echo "   - This script targets Authentik 2025.10.0"
    echo "   - Older versions may use different API endpoints"
    echo ""
    echo "6. Enable debug mode for more details:"
    echo "   DEBUG=true ./scripts/setup-authentik.sh"
    echo ""
    echo "Testing basic connectivity..."
    if curl -s -o /dev/null -w "%{http_code}" "$AUTHENTIK_URL/if/admin/" | grep -q "200\|302"; then
        echo "✅ Authentik is accessible at $AUTHENTIK_URL"
    else
        echo "❌ Cannot reach Authentik at $AUTHENTIK_URL"
        echo "   Check if Authentik is running: docker compose -f docker-compose.authentik.console.yml ps"
        echo "   Check logs: docker compose -f docker-compose.authentik.console.yml logs authentik-server"
    fi
    exit 1
fi

# Function to read government tenants as JSON array
read_government_tenants() {
    case "$TENANT_SOURCE" in
        env)
            if [ -z "$GOVERNMENT_TENANTS" ]; then
                echo "Error: GOVERNMENT_TENANTS environment variable not set"
                exit 1
            fi
            echo "$GOVERNMENT_TENANTS" | jq '.'
            ;;
        config)
            local config_file="${TENANT_CONFIG_FILE:-config/government-tenants.json}"
            if [ ! -f "$config_file" ]; then
                echo "Warning: Config file $config_file not found, using default tenants"
                echo '[{"id":"default","name":"Default Government","domain":"localhost"}]'
            else
                jq '.' "$config_file"
            fi
            ;;
        database)
            # Query PostgreSQL for tenants
            local pg_conn="${PG_CONNECTION_STRING:-Host=localhost;Port=5432;Database=mamey_government;Username=admin;Password=secret}"
            # Extract connection details (simplified - would need proper parsing)
            echo "Error: Database source not yet implemented. Use 'config' or 'env' source."
            exit 1
            ;;
        api)
            # Query tenant API
            local api_url="${TENANT_API_URL:-http://localhost:5000/api/tenants}"
            curl -s "$api_url" | jq '.'
            ;;
        *)
            echo "Error: Unknown tenant source: $TENANT_SOURCE"
            exit 1
            ;;
    esac
}

# Create default government tenants if none provided
if [ "$TENANT_SOURCE" = "config" ] && [ ! -f "${TENANT_CONFIG_FILE:-config/government-tenants.json}" ]; then
    echo ""
    echo "Creating default government tenants config..."
    mkdir -p config
    cat > config/government-tenants.json <<'EOF'
[
  {
    "id": "ink",
    "name": "Ierahkwa ne Kanienke Government",
    "domain": "ink.gov"
  }
]
EOF
    echo "✅ Created default config/government-tenants.json"
fi

# Read government tenants
echo ""
echo "Reading government tenants from source: $TENANT_SOURCE"
TENANTS_ARRAY=$(read_government_tenants)
if [ -z "$TENANTS_ARRAY" ]; then
    echo "Error: No government tenants found"
    exit 1
fi

TENANT_COUNT=$(echo "$TENANTS_ARRAY" | jq 'length')
echo "Found $TENANT_COUNT government tenant(s)"

# Process each government tenant
process_government_tenant() {
    local gov_id=$(echo "$1" | jq -r '.id')
    local gov_name=$(echo "$1" | jq -r '.name')
    local gov_domain=$(echo "$1" | jq -r '.domain // ""')
    
    echo ""
    echo "========================================"
    echo "Processing Government: $gov_name ($gov_id)"
    echo "========================================"
    
    # Create Authentik tenant (if tenancy enabled)
    local tenant_pk=""
    if [ "$ENABLE_TENANCY" = "true" ]; then
        echo ""
        echo "Creating Authentik tenant..."
        local schema_name="t_gov_${gov_id}"
        local tenant_data=$(cat <<EOF
{
    "domain": "$gov_domain",
    "default": false,
    "branding_logo": "",
    "branding_favicon": "",
    "flow_authentication": null,
    "flow_invalidation": null,
    "flow_recovery": null,
    "flow_unenrollment": null,
    "flow_user_settings": null,
    "event_retention": "days=365"
}
EOF
        )
        
        # Check if tenancy API is available first
        local tenancy_check=$(curl -s -w "\n%{http_code}" -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/tenancy/tenants/" | tail -n1)
        if [ "$tenancy_check" = "404" ] || [ "$tenancy_check" = "405" ]; then
            echo "  ℹ️  Authentik tenancy API not available (HTTP $tenancy_check)"
            echo "  ℹ️  Tenancy may not be enabled or API endpoint differs"
            echo "  ℹ️  Continuing with brands/flows instead (this is fine)"
        else
            local tenant_result=$(curl -s -w "\n%{http_code}" -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
                "$AUTHENTIK_URL/api/v3/tenancy/tenants/" \
                -d "$tenant_data")
            
            local tenant_http_code=$(echo "$tenant_result" | tail -n1)
            local tenant_body=$(echo "$tenant_result" | sed '$d')
            
            if [ "$tenant_http_code" = "201" ] || [ "$tenant_http_code" = "200" ]; then
                if echo "$tenant_body" | grep -q '"pk"'; then
                    tenant_pk=$(echo "$tenant_body" | jq -r '.pk' 2>/dev/null)
                    if [ -n "$tenant_pk" ] && [ "$tenant_pk" != "null" ]; then
                        echo "  ✅ Created tenant: $gov_name (PK: $tenant_pk)"
                    else
                        echo "  ⚠️  Tenant created but could not extract PK"
                    fi
                else
                    echo "  ⚠️  Unexpected response format: $tenant_body"
                fi
            elif [ "$tenant_http_code" = "400" ]; then
                # Check if tenant already exists
                existing_tenant=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/tenancy/tenants/?domain=$gov_domain" | jq -r '.results[0].pk // empty' 2>/dev/null)
                if [ -n "$existing_tenant" ] && [ "$existing_tenant" != "null" ]; then
                    tenant_pk="$existing_tenant"
                    echo "  ⏭️  Tenant already exists: $gov_name (PK: $tenant_pk)"
                else
                    echo "  ⚠️  Could not create tenant (HTTP $tenant_http_code): $tenant_body"
                    if [ "$DEBUG" = "true" ]; then
                        echo "  Request data: $tenant_data"
                    fi
                    echo "  ℹ️  Continuing without native tenant (using brands/flows instead)"
                fi
            else
                echo "  ⚠️  Could not create tenant (HTTP $tenant_http_code)"
                if [ "$DEBUG" = "true" ]; then
                    echo "  Response: $tenant_body"
                    echo "  Request data: $tenant_data"
                else
                    echo "  Response: $(echo "$tenant_body" | head -c 200)"
                fi
                echo "  ℹ️  Continuing without native tenant (using brands/flows instead)"
            fi
        fi
    fi
    
    # Create groups for this government
    echo ""
    echo "Creating groups for $gov_name..."

create_group() {
    local name=$1
        local result=$(curl -s -w "\n%{http_code}" -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
        "$AUTHENTIK_URL/api/v3/core/groups/" \
        -d "{\"name\": \"$name\", \"is_superuser\": false}")
    
        local http_code=$(echo "$result" | tail -n1)
        local body=$(echo "$result" | sed '$d')
        
        if [ "$http_code" = "201" ] || [ "$http_code" = "200" ]; then
            if echo "$body" | grep -q '"pk"'; then
                local pk=$(echo "$body" | jq -r '.pk' 2>/dev/null)
                if [ -n "$pk" ] && [ "$pk" != "null" ]; then
        echo "  ✅ Created group: $name"
                    echo "$pk"
                    return
                fi
            fi
        elif [ "$http_code" = "400" ]; then
            # Check if already exists
            local existing=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/core/groups/?name=$name" | jq -r '.results[0].pk // empty' 2>/dev/null)
            if [ -n "$existing" ] && [ "$existing" != "null" ]; then
        echo "  ⏭️  Group already exists: $name"
                echo "$existing"
                return
            fi
        fi
        
        # Try to find existing
        local existing=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/core/groups/?name=$name" | jq -r '.results[0].pk // empty' 2>/dev/null)
        if [ -n "$existing" ] && [ "$existing" != "null" ]; then
            echo "  ⏭️  Using existing group: $name"
            echo "$existing"
        else
            echo "  ⚠️  Could not create group: $name (HTTP $http_code)"
            echo ""
        fi
    }
    
    local b2b_admin_group=$(create_group "${gov_id}-admin")
    local b2b_agent_group=$(create_group "${gov_id}-agent")
    local b2b_manager_group=$(create_group "${gov_id}-manager")
    local b2c_citizen_group=$(create_group "${gov_id}-citizen")
    
    # Create global groups if first government
    if [ "$gov_id" = "ink" ] || [ "$gov_id" = "default" ]; then
        create_group "portal-admin" > /dev/null
        create_group "portal-agent" > /dev/null
        create_group "citizen" > /dev/null
    fi
    
    # Create prompt stages
    echo ""
    echo "Creating prompt stages..."
    
    # Create Terms Acceptance Prompt
    # First create the prompt, then create the stage with it
    echo "  Creating terms acceptance prompt..."
    local terms_prompt_data=$(cat <<EOF
{
    "name": "Terms Acceptance Prompt",
    "field_key": "terms_accepted",
    "label": "I accept the Terms and Conditions",
    "type": "checkbox",
    "required": true,
    "placeholder": "",
    "initial_value": "false",
    "order": 0
}
EOF
    )
    
    local terms_prompt_result=$(curl -s -w "\n%{http_code}" -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
        "$AUTHENTIK_URL/api/v3/stages/prompt/prompts/" \
        -d "$terms_prompt_data")
    
    local terms_prompt_http_code=$(echo "$terms_prompt_result" | tail -n1)
    local terms_prompt_body=$(echo "$terms_prompt_result" | sed '$d')
    
    local terms_prompt_pk=""
    if [ "$terms_prompt_http_code" = "201" ] || [ "$terms_prompt_http_code" = "200" ]; then
        terms_prompt_pk=$(echo "$terms_prompt_body" | jq -r '.pk // empty' 2>/dev/null)
        if [ -n "$terms_prompt_pk" ] && [ "$terms_prompt_pk" != "null" ]; then
            echo "  ✅ Created terms prompt"
        else
            echo "  ⚠️  Prompt created but could not extract PK"
        fi
    elif [ "$terms_prompt_http_code" = "400" ]; then
        # Check if already exists
        terms_prompt_pk=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/stages/prompt/prompts/?field_key=terms_accepted" | jq -r '.results[0].pk // empty' 2>/dev/null)
        if [ -n "$terms_prompt_pk" ] && [ "$terms_prompt_pk" != "null" ]; then
            echo "  ⏭️  Terms prompt already exists"
        else
            echo "  ⚠️  Could not create terms prompt (HTTP $terms_prompt_http_code)"
            echo "  Response: $terms_prompt_body" | head -c 500
            echo ""
        fi
    else
        # Try to find existing
        terms_prompt_pk=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/stages/prompt/prompts/?field_key=terms_accepted" | jq -r '.results[0].pk // empty' 2>/dev/null)
        if [ -n "$terms_prompt_pk" ] && [ "$terms_prompt_pk" != "null" ]; then
            echo "  ⏭️  Using existing terms prompt"
        else
            echo "  ⚠️  Could not create terms prompt (HTTP $terms_prompt_http_code)"
            if [ "$DEBUG" = "true" ]; then
                echo "  Response: $terms_prompt_body"
            fi
        fi
    fi
    
    # Now create the prompt stage with the prompt in fields array
    local terms_stage_pk=""
    if [ -n "$terms_prompt_pk" ] && [ "$terms_prompt_pk" != "null" ]; then
        echo "  Creating terms acceptance stage..."
        # Ensure fields array is properly formatted JSON
        local fields_array="[$terms_prompt_pk]"
        if [ -z "$terms_prompt_pk" ] || [ "$terms_prompt_pk" = "null" ]; then
            fields_array="[]"
        fi
        
        local terms_stage_data=$(cat <<EOF
{
    "name": "Terms Acceptance",
    "fields": $fields_array
}
EOF
        )
        
        local terms_stage_result=$(curl -s -w "\n%{http_code}" -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
            "$AUTHENTIK_URL/api/v3/stages/prompt/stages/" \
            -d "$terms_stage_data")
        
        local terms_stage_http_code=$(echo "$terms_stage_result" | tail -n1)
        local terms_stage_body=$(echo "$terms_stage_result" | sed '$d')
        
        if [ "$terms_stage_http_code" = "201" ] || [ "$terms_stage_http_code" = "200" ]; then
            terms_stage_pk=$(echo "$terms_stage_body" | jq -r '.pk // empty' 2>/dev/null)
            if [ -n "$terms_stage_pk" ] && [ "$terms_stage_pk" != "null" ]; then
                echo "  ✅ Created terms acceptance stage"
            else
                echo "  ⚠️  Stage created but could not extract PK"
            fi
        elif [ "$terms_stage_http_code" = "400" ]; then
            # Check if already exists
            terms_stage_pk=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/stages/prompt/stages/?name=Terms%20Acceptance" | jq -r '.results[0].pk // empty' 2>/dev/null)
            if [ -n "$terms_stage_pk" ] && [ "$terms_stage_pk" != "null" ]; then
                echo "  ⏭️  Terms stage already exists"
            else
                echo "  ⚠️  Could not create terms stage (HTTP $terms_stage_http_code)"
                echo "  Response: $terms_stage_body" | head -c 500
                echo ""
            fi
        else
            # Try to find existing
            terms_stage_pk=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/stages/prompt/stages/?name=Terms%20Acceptance" | jq -r '.results[0].pk // empty' 2>/dev/null)
            if [ -n "$terms_stage_pk" ] && [ "$terms_stage_pk" != "null" ]; then
                echo "  ⏭️  Using existing terms stage"
            else
                echo "  ⚠️  Could not create terms stage (HTTP $terms_stage_http_code)"
                if [ "$DEBUG" = "true" ]; then
                    echo "  Response: $terms_stage_body"
                fi
            fi
        fi
    fi
    
    # Use stage_pk for flow binding
    local terms_prompt_stage_pk="$terms_stage_pk"
    
    # B2B profile prompts - create prompts first, then stage
    local b2b_prompt_name="${gov_name} B2B Profile"
    local b2b_prompt_pks=()
    
    echo "  Creating B2B profile prompts..."
    local order=0
    for field in "job_title:Job Title" "department:Department" "phone:Phone Number"; do
        IFS=':' read -r field_key field_label <<< "$field"
        local prompt_data=$(cat <<EOF
{
    "name": "${gov_id}_$field_key Prompt",
    "field_key": "${gov_id}_$field_key",
    "label": "$field_label",
    "type": "text",
    "required": false,
    "placeholder": "",
    "initial_value": "",
    "order": $order
}
EOF
        )
        
        local prompt_result=$(curl -s -w "\n%{http_code}" -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
            "$AUTHENTIK_URL/api/v3/stages/prompt/prompts/" \
            -d "$prompt_data")
        
        local prompt_http_code=$(echo "$prompt_result" | tail -n1)
        local prompt_body=$(echo "$prompt_result" | sed '$d')
        
        if [ "$prompt_http_code" = "201" ] || [ "$prompt_http_code" = "200" ]; then
            local prompt_pk=$(echo "$prompt_body" | jq -r '.pk // empty' 2>/dev/null)
            if [ -n "$prompt_pk" ] && [ "$prompt_pk" != "null" ]; then
                b2b_prompt_pks+=("$prompt_pk")
            fi
        elif [ "$prompt_http_code" = "400" ]; then
            # Check if already exists
            local existing_pk=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/stages/prompt/prompts/?field_key=${gov_id}_$field_key" | jq -r '.results[0].pk // empty' 2>/dev/null)
            if [ -n "$existing_pk" ] && [ "$existing_pk" != "null" ]; then
                b2b_prompt_pks+=("$existing_pk")
            fi
        fi
        order=$((order + 1))
    done
    
    # Now create the stage with the prompts
    local b2b_stage_pk=""
    if [ ${#b2b_prompt_pks[@]} -gt 0 ]; then
        echo "  Creating B2B profile stage..."
        local fields_array=$(printf '%s\n' "${b2b_prompt_pks[@]}" | jq -R . | jq -s .)
        local b2b_stage_data=$(cat <<EOF
{
    "name": "$b2b_prompt_name",
    "fields": $fields_array
}
EOF
        )
        
        local b2b_stage_result=$(curl -s -w "\n%{http_code}" -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
            "$AUTHENTIK_URL/api/v3/stages/prompt/stages/" \
            -d "$b2b_stage_data")
        
        local b2b_http_code=$(echo "$b2b_stage_result" | tail -n1)
        local b2b_body=$(echo "$b2b_stage_result" | sed '$d')
        
        if [ "$b2b_http_code" = "201" ] || [ "$b2b_http_code" = "200" ]; then
            b2b_stage_pk=$(echo "$b2b_body" | jq -r '.pk // empty' 2>/dev/null)
            if [ -n "$b2b_stage_pk" ] && [ "$b2b_stage_pk" != "null" ]; then
                echo "  ✅ Created B2B profile stage"
            fi
        elif [ "$b2b_http_code" = "400" ]; then
            # Check if already exists
            b2b_stage_pk=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/stages/prompt/stages/?name=$(echo "$b2b_prompt_name" | jq -sRr @uri)" | jq -r '.results[0].pk // empty' 2>/dev/null)
            if [ -n "$b2b_stage_pk" ] && [ "$b2b_stage_pk" != "null" ]; then
                echo "  ⏭️  B2B stage already exists"
            else
                echo "  ⚠️  Could not create B2B stage (HTTP $b2b_http_code)"
                echo "  Response: $b2b_body" | head -c 500
                echo ""
            fi
        else
            # Try to find existing
            b2b_stage_pk=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/stages/prompt/stages/?name=$(echo "$b2b_prompt_name" | jq -sRr @uri)" | jq -r '.results[0].pk // empty' 2>/dev/null)
            if [ -n "$b2b_stage_pk" ] && [ "$b2b_stage_pk" != "null" ]; then
                echo "  ⏭️  Using existing B2B stage"
            else
                echo "  ⚠️  Could not create B2B stage (HTTP $b2b_http_code)"
                if [ "$DEBUG" = "true" ]; then
                    echo "  Response: $b2b_body"
                fi
            fi
        fi
    fi
    
    # Get default stages
    echo "  Getting default stages..."
    local identification_stage=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/stages/identification/" | jq -r '.results[0].pk // empty' 2>/dev/null)
    local password_stage=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/stages/password/" | jq -r '.results[0].pk // empty' 2>/dev/null)
    local user_login_stage=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/stages/user_login/" | jq -r '.results[0].pk // empty' 2>/dev/null)
    
    if [ -z "$identification_stage" ] || [ "$identification_stage" = "null" ]; then
        echo "  ⚠️  Warning: Could not find identification stage"
    fi
    if [ -z "$password_stage" ] || [ "$password_stage" = "null" ]; then
        echo "  ⚠️  Warning: Could not find password stage"
    fi
    if [ -z "$user_login_stage" ] || [ "$user_login_stage" = "null" ]; then
        echo "  ⚠️  Warning: Could not find user login stage"
    fi
    
    # Create B2B Authentication Flow
    echo ""
    echo "Creating B2B Authentication Flow for $gov_name..."
    
    local b2b_auth_flow_data=$(cat <<EOF
{
    "name": "${gov_name} B2B Authentication",
    "slug": "${gov_id}-b2b-auth",
    "title": "${gov_name} Employee Login",
    "designation": "authentication",
    "background": "",
    "compatibility_mode": false
}
EOF
    )
    
    local b2b_auth_flow_result=$(curl -s -w "\n%{http_code}" -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
        "$AUTHENTIK_URL/api/v3/flows/instances/" \
        -d "$b2b_auth_flow_data")
    
    local b2b_flow_http_code=$(echo "$b2b_auth_flow_result" | tail -n1)
    local b2b_flow_body=$(echo "$b2b_auth_flow_result" | sed '$d')
    
    local b2b_auth_flow_pk=""
    if [ "$b2b_flow_http_code" = "201" ] || [ "$b2b_flow_http_code" = "200" ]; then
        if echo "$b2b_flow_body" | grep -q '"pk"'; then
            b2b_auth_flow_pk=$(echo "$b2b_flow_body" | jq -r '.pk' 2>/dev/null)
            if [ -n "$b2b_auth_flow_pk" ] && [ "$b2b_auth_flow_pk" != "null" ]; then
                echo "  ✅ Created B2B auth flow"
                
                # Add stages to flow using flow bindings endpoint
                local stage_order=0
        if [ -n "$identification_stage" ] && [ "$identification_stage" != "null" ]; then
            curl -s -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
                "$AUTHENTIK_URL/api/v3/flows/bindings/" \
                -d "{\"target\": $b2b_auth_flow_pk, \"stage\": $identification_stage, \"order\": $stage_order}" > /dev/null 2>&1
            stage_order=$((stage_order + 1))
        fi
        if [ -n "$password_stage" ] && [ "$password_stage" != "null" ]; then
            curl -s -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
                "$AUTHENTIK_URL/api/v3/flows/bindings/" \
                -d "{\"target\": $b2b_auth_flow_pk, \"stage\": $password_stage, \"order\": $stage_order}" > /dev/null 2>&1
            stage_order=$((stage_order + 1))
        fi
        if [ -n "$terms_prompt_stage_pk" ] && [ "$terms_prompt_stage_pk" != "null" ]; then
            curl -s -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
                "$AUTHENTIK_URL/api/v3/flows/bindings/" \
                -d "{\"target\": $b2b_auth_flow_pk, \"stage\": $terms_prompt_stage_pk, \"order\": $stage_order}" > /dev/null 2>&1
            stage_order=$((stage_order + 1))
        fi
                if [ -n "$user_login_stage" ] && [ "$user_login_stage" != "null" ]; then
                    curl -s -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
                        "$AUTHENTIK_URL/api/v3/flows/bindings/" \
                        -d "{\"target\": $b2b_auth_flow_pk, \"stage\": $user_login_stage, \"order\": $stage_order}" > /dev/null 2>&1
                fi
            else
                echo "  ⚠️  B2B flow created but could not extract PK"
            fi
        else
            echo "  ⚠️  Unexpected response format"
            if [ "$DEBUG" = "true" ]; then
                echo "  Response: $b2b_flow_body"
            fi
        fi
    elif [ "$b2b_flow_http_code" = "400" ]; then
        # Check if already exists
        b2b_auth_flow_pk=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/flows/instances/?slug=${gov_id}-b2b-auth" | jq -r '.results[0].pk // empty' 2>/dev/null)
        if [ -n "$b2b_auth_flow_pk" ] && [ "$b2b_auth_flow_pk" != "null" ]; then
            echo "  ⏭️  B2B auth flow already exists"
        else
            echo "  ⚠️  Could not create B2B flow (HTTP $b2b_flow_http_code)"
            if [ "$DEBUG" = "true" ]; then
                echo "  Response: $b2b_flow_body"
            fi
        fi
    else
        # Try to find existing
        b2b_auth_flow_pk=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/flows/instances/?slug=${gov_id}-b2b-auth" | jq -r '.results[0].pk // empty' 2>/dev/null)
        if [ -n "$b2b_auth_flow_pk" ] && [ "$b2b_auth_flow_pk" != "null" ]; then
            echo "  ⏭️  Using existing B2B auth flow"
        else
            echo "  ⚠️  Could not create B2B flow (HTTP $b2b_flow_http_code)"
            if [ "$DEBUG" = "true" ]; then
                echo "  Response: $b2b_flow_body"
            fi
        fi
    fi
    
    # Create B2C Authentication Flow
    echo ""
    echo "Creating B2C Authentication Flow for $gov_name..."
    
    local b2c_auth_flow_data=$(cat <<EOF
{
    "name": "${gov_name} B2C Authentication",
    "slug": "${gov_id}-b2c-auth",
    "title": "${gov_name} Citizen Login",
    "designation": "authentication",
    "background": "",
    "compatibility_mode": false
}
EOF
    )
    
    local b2c_auth_flow_result=$(curl -s -w "\n%{http_code}" -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
        "$AUTHENTIK_URL/api/v3/flows/instances/" \
        -d "$b2c_auth_flow_data")
    
    local b2c_flow_http_code=$(echo "$b2c_auth_flow_result" | tail -n1)
    local b2c_flow_body=$(echo "$b2c_auth_flow_result" | sed '$d')
    
    local b2c_auth_flow_pk=""
    if [ "$b2c_flow_http_code" = "201" ] || [ "$b2c_flow_http_code" = "200" ]; then
        if echo "$b2c_flow_body" | grep -q '"pk"'; then
            b2c_auth_flow_pk=$(echo "$b2c_flow_body" | jq -r '.pk' 2>/dev/null)
            if [ -n "$b2c_auth_flow_pk" ] && [ "$b2c_auth_flow_pk" != "null" ]; then
                echo "  ✅ Created B2C auth flow"
                
                # Add stages to flow using flow bindings endpoint
                local stage_order=0
                if [ -n "$identification_stage" ] && [ "$identification_stage" != "null" ]; then
                    curl -s -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
                        "$AUTHENTIK_URL/api/v3/flows/bindings/" \
                        -d "{\"target\": $b2c_auth_flow_pk, \"stage\": $identification_stage, \"order\": $stage_order}" > /dev/null 2>&1
                    stage_order=$((stage_order + 1))
                fi
                if [ -n "$password_stage" ] && [ "$password_stage" != "null" ]; then
                    curl -s -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
                        "$AUTHENTIK_URL/api/v3/flows/bindings/" \
                        -d "{\"target\": $b2c_auth_flow_pk, \"stage\": $password_stage, \"order\": $stage_order}" > /dev/null 2>&1
                    stage_order=$((stage_order + 1))
                fi
                if [ -n "$terms_prompt_stage_pk" ] && [ "$terms_prompt_stage_pk" != "null" ]; then
                    curl -s -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
                        "$AUTHENTIK_URL/api/v3/flows/bindings/" \
                        -d "{\"target\": $b2c_auth_flow_pk, \"stage\": $terms_prompt_stage_pk, \"order\": $stage_order}" > /dev/null 2>&1
                    stage_order=$((stage_order + 1))
                fi
                if [ -n "$user_login_stage" ] && [ "$user_login_stage" != "null" ]; then
                    curl -s -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
                        "$AUTHENTIK_URL/api/v3/flows/bindings/" \
                        -d "{\"target\": $b2c_auth_flow_pk, \"stage\": $user_login_stage, \"order\": $stage_order}" > /dev/null 2>&1
                fi
            else
                echo "  ⚠️  B2C flow created but could not extract PK"
            fi
        else
            echo "  ⚠️  Unexpected response format"
            if [ "$DEBUG" = "true" ]; then
                echo "  Response: $b2c_flow_body"
            fi
        fi
    elif [ "$b2c_flow_http_code" = "400" ]; then
        # Check if already exists
        b2c_auth_flow_pk=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/flows/instances/?slug=${gov_id}-b2c-auth" | jq -r '.results[0].pk // empty' 2>/dev/null)
        if [ -n "$b2c_auth_flow_pk" ] && [ "$b2c_auth_flow_pk" != "null" ]; then
            echo "  ⏭️  B2C auth flow already exists"
        else
            echo "  ⚠️  Could not create B2C flow (HTTP $b2c_flow_http_code)"
            if [ "$DEBUG" = "true" ]; then
                echo "  Response: $b2c_flow_body"
            fi
        fi
    else
        # Try to find existing
        b2c_auth_flow_pk=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/flows/instances/?slug=${gov_id}-b2c-auth" | jq -r '.results[0].pk // empty' 2>/dev/null)
        if [ -n "$b2c_auth_flow_pk" ] && [ "$b2c_auth_flow_pk" != "null" ]; then
            echo "  ⏭️  Using existing B2C auth flow"
        else
            echo "  ⚠️  Could not create B2C flow (HTTP $b2c_flow_http_code)"
            if [ "$DEBUG" = "true" ]; then
                echo "  Response: $b2c_flow_body"
            fi
        fi
    fi
    
    # Create Brands
    echo ""
    echo "Creating brands for $gov_name..."
    
    # Brands might not be available in all Authentik versions
    # Skip brand creation if endpoint doesn't exist
    local b2b_brand_result=""
    local b2b_brand_http_code="404"
    
    # Try to create brand if we have a flow
    if [ -n "$b2b_auth_flow_pk" ] && [ "$b2b_auth_flow_pk" != "null" ]; then
        local b2b_brand_data=$(cat <<EOF
{
    "domain": "$gov_domain",
    "default": false,
    "flow_authentication": $b2b_auth_flow_pk
}
EOF
        )
        
        b2b_brand_result=$(curl -s -w "\n%{http_code}" -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
            "$AUTHENTIK_URL/api/v3/brands/" \
            -d "$b2b_brand_data" 2>/dev/null || echo -e "\n404")
    else
        echo "  ⚠️  Skipping B2B brand creation - B2B flow not available"
    fi
    
    local b2b_brand_body=""
    local b2b_brand_pk=""
    
    if [ -n "$b2b_brand_result" ]; then
        b2b_brand_http_code=$(echo "$b2b_brand_result" | tail -n1)
        b2b_brand_body=$(echo "$b2b_brand_result" | sed '$d')
    fi
    
    if [ "$b2b_brand_http_code" = "201" ] || [ "$b2b_brand_http_code" = "200" ]; then
        b2b_brand_pk=$(echo "$b2b_brand_body" | jq -r '.pk' 2>/dev/null)
        if [ -n "$b2b_brand_pk" ] && [ "$b2b_brand_pk" != "null" ]; then
            echo "  ✅ Created B2B brand"
        else
            echo "  ⚠️  B2B brand created but could not extract PK"
        fi
    elif [ "$b2b_brand_http_code" = "400" ]; then
        # Check if already exists
        b2b_brand_pk=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/brands/?domain=$gov_domain" | jq -r '.results[0].pk // empty' 2>/dev/null)
        if [ -n "$b2b_brand_pk" ] && [ "$b2b_brand_pk" != "null" ]; then
            echo "  ⏭️  B2B brand already exists"
        else
            echo "  ⚠️  Could not create B2B brand (HTTP $b2b_brand_http_code)"
            if [ "$DEBUG" = "true" ]; then
                echo "  Response: $b2b_brand_body"
            fi
        fi
    elif [ "$b2b_brand_http_code" = "404" ] || [ "$b2b_brand_http_code" = "405" ]; then
        echo "  ℹ️  Brands API not available (HTTP $b2b_brand_http_code) - skipping brand creation"
    else
        echo "  ⚠️  Could not create B2B brand (HTTP $b2b_brand_http_code)"
        if [ "$DEBUG" = "true" ]; then
            echo "  Response: $b2b_brand_body"
        fi
    fi
    
    # Brands might not be available in all Authentik versions
    local b2c_brand_result=""
    local b2c_brand_http_code="404"
    
    # Try to create brand if we have a flow
    if [ -n "$b2c_auth_flow_pk" ] && [ "$b2c_auth_flow_pk" != "null" ]; then
        local b2c_brand_data=$(cat <<EOF
{
    "domain": "citizens.$gov_domain",
    "default": false,
    "flow_authentication": $b2c_auth_flow_pk
}
EOF
        )
        
        b2c_brand_result=$(curl -s -w "\n%{http_code}" -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
            "$AUTHENTIK_URL/api/v3/brands/" \
            -d "$b2c_brand_data" 2>/dev/null || echo -e "\n404")
    else
        echo "  ⚠️  Skipping B2C brand creation - B2C flow not available"
    fi
    
    local b2c_brand_body=""
    local b2c_brand_pk=""
    
    if [ -n "$b2c_brand_result" ]; then
        b2c_brand_http_code=$(echo "$b2c_brand_result" | tail -n1)
        b2c_brand_body=$(echo "$b2c_brand_result" | sed '$d')
    fi
    
    if [ "$b2c_brand_http_code" = "201" ] || [ "$b2c_brand_http_code" = "200" ]; then
        b2c_brand_pk=$(echo "$b2c_brand_body" | jq -r '.pk' 2>/dev/null)
        if [ -n "$b2c_brand_pk" ] && [ "$b2c_brand_pk" != "null" ]; then
            echo "  ✅ Created B2C brand"
        else
            echo "  ⚠️  B2C brand created but could not extract PK"
        fi
    elif [ "$b2c_brand_http_code" = "400" ]; then
        # Check if already exists
        b2c_brand_pk=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/brands/?domain=citizens.$gov_domain" | jq -r '.results[0].pk // empty' 2>/dev/null)
        if [ -n "$b2c_brand_pk" ] && [ "$b2c_brand_pk" != "null" ]; then
            echo "  ⏭️  B2C brand already exists"
        else
            echo "  ⚠️  Could not create B2C brand (HTTP $b2c_brand_http_code)"
            if [ "$DEBUG" = "true" ]; then
                echo "  Response: $b2c_brand_body"
            fi
        fi
    elif [ "$b2c_brand_http_code" = "404" ] || [ "$b2c_brand_http_code" = "405" ]; then
        echo "  ℹ️  Brands API not available (HTTP $b2c_brand_http_code) - skipping brand creation"
    else
        echo "  ⚠️  Could not create B2C brand (HTTP $b2c_brand_http_code)"
        if [ "$DEBUG" = "true" ]; then
            echo "  Response: $b2c_brand_body"
        fi
    fi
    
    # Create property mappings
    # Note: Property mappings may need to be created via a different endpoint
    # or may not be available via API in all Authentik versions
    echo ""
    echo "Creating property mappings for $gov_name..."
    echo "  ℹ️  Note: Property mappings may need to be created manually in Authentik Admin UI"
    echo "  ℹ️  Or the API endpoint may differ for your Authentik version"
    
    local tenant_mapping_expr="return \"$gov_id\""
    local tenant_mapping_data=$(cat <<EOF
{
    "name": "${gov_name} Tenant Mapping",
    "scope_name": "tenant",
    "expression": $(echo "$tenant_mapping_expr" | jq -Rs .)
}
EOF
    )
    
    # Try creating scope property mapping
    # According to api.goauthentik.io, the endpoint should be /api/v3/propertymappings/scope/
    local tenant_mapping_result=$(curl -s -w "\n%{http_code}" -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
        "$AUTHENTIK_URL/api/v3/propertymappings/scope/" \
        -d "$tenant_mapping_data" 2>/dev/null || echo -e "\n405")
    
    local tenant_mapping_http_code=$(echo "$tenant_mapping_result" | tail -n1)
    local tenant_mapping_body=$(echo "$tenant_mapping_result" | sed '$d')
    
    local tenant_mapping_pk=""
    if [ "$tenant_mapping_http_code" = "201" ] || [ "$tenant_mapping_http_code" = "200" ]; then
        tenant_mapping_pk=$(echo "$tenant_mapping_body" | jq -r '.pk' 2>/dev/null)
        if [ -n "$tenant_mapping_pk" ] && [ "$tenant_mapping_pk" != "null" ]; then
            echo "  ✅ Created tenant mapping"
        else
            echo "  ⚠️  Tenant mapping created but could not extract PK"
        fi
    elif [ "$tenant_mapping_http_code" = "400" ]; then
        # Check if already exists
        tenant_mapping_pk=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/propertymappings/scope/?name=$(echo "${gov_name} Tenant Mapping" | jq -sRr @uri)" | jq -r '.results[0].pk // empty' 2>/dev/null)
        if [ -n "$tenant_mapping_pk" ] && [ "$tenant_mapping_pk" != "null" ]; then
            echo "  ⏭️  Tenant mapping already exists"
        else
            echo "  ⚠️  Could not create tenant mapping (HTTP $tenant_mapping_http_code)"
            if [ "$DEBUG" = "true" ]; then
                echo "  Response: $tenant_mapping_body"
            fi
        fi
    else
        echo "  ⚠️  Could not create tenant mapping (HTTP $tenant_mapping_http_code)"
        echo "  Response: $tenant_mapping_body" | head -c 500
        echo ""
        if [ "$DEBUG" = "true" ]; then
            echo "  Request: $tenant_mapping_data"
        fi
    fi
    
    # Create roles mapping for this government
    local roles_mapping_expr=$(cat <<EOF
# Return user's groups as roles for ${gov_name}
group_to_role = {
    "${gov_id}-admin": "Admin",
    "${gov_id}-agent": "GovernmentAgent",
    "${gov_id}-manager": "Manager",
    "${gov_id}-citizen": "Citizen",
    "portal-admin": "Admin",
    "portal-agent": "GovernmentAgent",
    "citizen": "Citizen"
}

roles = []
for group in user.ak_groups.all():
    if group.name in group_to_role:
        roles.append(group_to_role[group.name])

return roles
EOF
)

    local roles_mapping_data=$(cat <<EOF
{
    "name": "${gov_name} Roles Mapping",
    "scope_name": "roles",
    "expression": $(echo "$roles_mapping_expr" | jq -Rs .)
}
EOF
    )
    
    # Try creating scope property mapping
    # According to api.goauthentik.io, the endpoint should be /api/v3/propertymappings/scope/
    local roles_mapping_result=$(curl -s -w "\n%{http_code}" -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
        "$AUTHENTIK_URL/api/v3/propertymappings/scope/" \
        -d "$roles_mapping_data" 2>/dev/null || echo -e "\n405")
    
    local roles_mapping_http_code=$(echo "$roles_mapping_result" | tail -n1)
    local roles_mapping_body=$(echo "$roles_mapping_result" | sed '$d')
    
    local roles_mapping_pk=""
    if [ "$roles_mapping_http_code" = "201" ] || [ "$roles_mapping_http_code" = "200" ]; then
        roles_mapping_pk=$(echo "$roles_mapping_body" | jq -r '.pk' 2>/dev/null)
        if [ -n "$roles_mapping_pk" ] && [ "$roles_mapping_pk" != "null" ]; then
            echo "  ✅ Created roles mapping"
        else
            echo "  ⚠️  Roles mapping created but could not extract PK"
        fi
    elif [ "$roles_mapping_http_code" = "400" ]; then
        # Check if already exists
        roles_mapping_pk=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/propertymappings/scope/?name=$(echo "${gov_name} Roles Mapping" | jq -sRr @uri)" | jq -r '.results[0].pk // empty' 2>/dev/null)
        if [ -n "$roles_mapping_pk" ] && [ "$roles_mapping_pk" != "null" ]; then
            echo "  ⏭️  Roles mapping already exists"
        else
            echo "  ⚠️  Could not create roles mapping (HTTP $roles_mapping_http_code)"
            if [ "$DEBUG" = "true" ]; then
                echo "  Response: $roles_mapping_body"
            fi
        fi
    else
        echo "  ⚠️  Could not create roles mapping (HTTP $roles_mapping_http_code)"
        echo "  Response: $roles_mapping_body" | head -c 500
        echo ""
        if [ "$DEBUG" = "true" ]; then
            echo "  Request: $roles_mapping_data"
        fi
    fi
    
    # Get default OpenID mappings
    local default_mappings_response=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/propertymappings/scope/?managed__startswith=goauthentik.io/providers/oauth2/scope-" 2>/dev/null)
    local default_mappings=""
    if echo "$default_mappings_response" | jq -e '.results' > /dev/null 2>&1; then
        default_mappings=$(echo "$default_mappings_response" | jq -r '.results[].pk // empty' | grep -v '^$' | tr '\n' ',' | sed 's/,$//')
    fi

    # Build property mappings array - collect all PKs
    local mapping_pks=()
    if [ -n "$default_mappings" ] && [ "$default_mappings" != "," ]; then
        IFS=',' read -ra DEFAULT_PKS <<< "$default_mappings"
        for pk in "${DEFAULT_PKS[@]}"; do
            if [ -n "$pk" ] && [ "$pk" != "null" ]; then
                mapping_pks+=("$pk")
            fi
        done
    fi
    if [ -n "$roles_mapping_pk" ] && [ "$roles_mapping_pk" != "null" ]; then
        mapping_pks+=("$roles_mapping_pk")
    fi
    if [ -n "$tenant_mapping_pk" ] && [ "$tenant_mapping_pk" != "null" ]; then
        mapping_pks+=("$tenant_mapping_pk")
    fi
    
    # Build JSON array from collected PKs
    local property_mappings_array="[]"
    if [ ${#mapping_pks[@]} -gt 0 ]; then
        property_mappings_array=$(printf '%s\n' "${mapping_pks[@]}" | jq -R . | jq -s .)
    fi

# Get signing key
    local signing_key=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/crypto/certificatekeypairs/?name=authentik%20Self-signed%20Certificate" | jq -r '.results[0].pk')
    if [ -z "$signing_key" ] || [ "$signing_key" = "null" ]; then
        signing_key=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/crypto/certificatekeypairs/" | jq -r '.results[0].pk')
    fi
    
    # Create OAuth2 Provider (single provider with tenant-aware mapping)
    echo ""
    echo "Creating OAuth2/OIDC Provider for $gov_name..."
    
    # Use B2B flow if available, otherwise try B2C, otherwise use default
    local auth_flow_pk="$b2b_auth_flow_pk"
    if [ -z "$auth_flow_pk" ] || [ "$auth_flow_pk" = "null" ]; then
        auth_flow_pk="$b2c_auth_flow_pk"
    fi
    if [ -z "$auth_flow_pk" ] || [ "$auth_flow_pk" = "null" ]; then
        # Get default authorization flow
        auth_flow_pk=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/flows/instances/?slug=default-provider-authorization-implicit-consent" | jq -r '.results[0].pk // empty' 2>/dev/null)
        if [ -z "$auth_flow_pk" ] || [ "$auth_flow_pk" = "null" ]; then
            auth_flow_pk=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/flows/instances/?designation=authorization" | jq -r '.results[0].pk // empty' 2>/dev/null)
        fi
        if [ -z "$auth_flow_pk" ] || [ "$auth_flow_pk" = "null" ]; then
            echo "  ❌ Error: No authorization flow available. Cannot create provider."
            echo "  ℹ️  Please create flows manually in Authentik Admin UI"
            return
        else
            echo "  ⚠️  Using default authorization flow (custom flows not created)"
        fi
    fi

    local client_id="mamey-government-${gov_id}"
    
    # Build redirect_uris as array
    local redirect_uris_array="[\"$PORTAL_URL/signin-oidc\"]"
    
    # Handle signing_key - use null if not available
    local signing_key_value="null"
    if [ -n "$signing_key" ] && [ "$signing_key" != "null" ]; then
        signing_key_value="$signing_key"
    fi
    
    # Ensure property_mappings_array is valid JSON array
    if [ -z "$property_mappings_array" ] || [ "$property_mappings_array" = "[]" ] || [ "$property_mappings_array" = "null" ]; then
        property_mappings_array="[]"
    fi
    
    # Build provider data - ensure all JSON values are properly formatted
    # Handle signing_key - use null if not available, otherwise use the PK number
    local signing_key_json="null"
    if [ -n "$signing_key" ] && [ "$signing_key" != "null" ]; then
        signing_key_json="$signing_key"
    fi
    
    # Validate property_mappings_array is valid JSON
    if ! echo "$property_mappings_array" | jq . > /dev/null 2>&1; then
        property_mappings_array="[]"
    fi
    
    # Build provider data using jq to ensure valid JSON
    # Handle signing_key properly - it can be null or a number
    local signing_key_arg="null"
    if [ -n "$signing_key" ] && [ "$signing_key" != "null" ]; then
        signing_key_arg="$signing_key"
    fi
    
    # Build provider data with jq to avoid JSON parse errors
    local provider_data=$(jq -n \
        --arg name "mamey-government-${gov_id}" \
        --argjson auth_flow "$auth_flow_pk" \
        --arg client_type "confidential" \
        --arg client_id "$client_id" \
        --argjson redirect_uris "$redirect_uris_array" \
        --argjson property_mappings "$property_mappings_array" \
        --arg access_token_validity "minutes=5" \
        --arg refresh_token_validity "days=30" \
        --argjson signing_key "$signing_key_arg" \
        '{
            name: $name,
            authorization_flow: ($auth_flow | tonumber),
            client_type: $client_type,
            client_id: $client_id,
            redirect_uris: $redirect_uris,
            property_mappings: $property_mappings,
            access_token_validity: $access_token_validity,
            refresh_token_validity: $refresh_token_validity,
            include_claims_in_id_token: true,
            sub_mode: "user_id",
            signing_key: (if $signing_key == "null" then null else ($signing_key | tonumber) end)
        }' 2>/dev/null)
    
    # Fallback if jq fails - build JSON manually with proper escaping
    if [ -z "$provider_data" ] || [ "$provider_data" = "null" ] || ! echo "$provider_data" | jq . > /dev/null 2>&1; then
        # Ensure all JSON values are properly formatted
        local signing_key_val="null"
        if [ -n "$signing_key" ] && [ "$signing_key" != "null" ]; then
            signing_key_val="$signing_key"
        fi
        
        # Use printf to build JSON safely
        provider_data=$(printf '{
    "name": "mamey-government-%s",
    "authorization_flow": %s,
    "client_type": "confidential",
    "client_id": "%s",
    "redirect_uris": %s,
    "signing_key": %s,
    "property_mappings": %s,
    "access_token_validity": "minutes=5",
    "refresh_token_validity": "days=30",
    "include_claims_in_id_token": true,
    "sub_mode": "user_id"
}' "$gov_id" "$auth_flow_pk" "$client_id" "$redirect_uris_array" "$signing_key_val" "$property_mappings_array")
    fi

    local provider_result=$(curl -s -w "\n%{http_code}" -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
        "$AUTHENTIK_URL/api/v3/providers/oauth2/" \
        -d "$provider_data")
    
    local provider_http_code=$(echo "$provider_result" | tail -n1)
    local provider_body=$(echo "$provider_result" | sed '$d')
    
    local provider_pk=""
    local client_secret=""
    if [ "$provider_http_code" = "201" ] || [ "$provider_http_code" = "200" ]; then
        if echo "$provider_body" | grep -q '"pk"'; then
            provider_pk=$(echo "$provider_body" | jq -r '.pk' 2>/dev/null)
            client_secret=$(echo "$provider_body" | jq -r '.client_secret' 2>/dev/null)
            if [ -n "$provider_pk" ] && [ "$provider_pk" != "null" ]; then
                echo "  ✅ Created OAuth2 provider: $client_id"
            else
                echo "  ⚠️  Provider created but could not extract PK"
            fi
        else
            echo "  ⚠️  Unexpected response format"
            if [ "$DEBUG" = "true" ]; then
                echo "  Response: $provider_body"
            fi
        fi
    elif [ "$provider_http_code" = "400" ]; then
        # Check if already exists
        provider_pk=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/providers/oauth2/?name=mamey-government-${gov_id}" | jq -r '.results[0].pk // empty' 2>/dev/null)
        if [ -n "$provider_pk" ] && [ "$provider_pk" != "null" ]; then
            client_secret=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/providers/oauth2/$provider_pk/" | jq -r '.client_secret' 2>/dev/null)
            echo "  ⏭️  Provider already exists"
        else
            echo "  ⚠️  Could not create provider (HTTP $provider_http_code)"
            echo "  Response: $provider_body" | head -c 500
            echo ""
            if [ "$DEBUG" = "true" ]; then
                echo "  Request: $provider_data"
            fi
        fi
    else
        # Try to find existing
        provider_pk=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/providers/oauth2/?name=mamey-government-${gov_id}" | jq -r '.results[0].pk // empty' 2>/dev/null)
        if [ -n "$provider_pk" ] && [ "$provider_pk" != "null" ]; then
            client_secret=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/providers/oauth2/$provider_pk/" | jq -r '.client_secret' 2>/dev/null)
            echo "  ⏭️  Using existing provider"
        else
            echo "  ⚠️  Could not create provider (HTTP $provider_http_code)"
            if [ "$DEBUG" = "true" ]; then
                echo "  Response: $provider_body"
            fi
        fi
    fi
    
    # Create Applications
    echo ""
    echo "Creating applications for $gov_name..."
    
    if [ -z "$provider_pk" ] || [ "$provider_pk" = "null" ]; then
        echo "  ⚠️  Skipping application creation - provider not available"
    else
        local b2b_app_data=$(cat <<EOF
{
    "name": "${gov_name} Portal (B2B)",
    "slug": "${gov_id}-portal-b2b",
    "provider": $provider_pk,
    "meta_launch_url": "$PORTAL_URL/?tenant=${gov_id}&type=b2b"
}
EOF
        )
        
        local b2b_app_result=$(curl -s -w "\n%{http_code}" -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
            "$AUTHENTIK_URL/api/v3/core/applications/" \
            -d "$b2b_app_data")
        
        local b2b_app_http_code=$(echo "$b2b_app_result" | tail -n1)
        if [ "$b2b_app_http_code" = "201" ] || [ "$b2b_app_http_code" = "200" ]; then
            echo "  ✅ Created B2B application"
        elif [ "$b2b_app_http_code" = "400" ]; then
            echo "  ⏭️  B2B application may already exist"
        else
            echo "  ⚠️  Could not create B2B application (HTTP $b2b_app_http_code)"
        fi
        
        local b2c_app_data=$(cat <<EOF
{
    "name": "${gov_name} Citizen Portal (B2C)",
    "slug": "${gov_id}-portal-b2c",
    "provider": $provider_pk,
    "meta_launch_url": "$PORTAL_URL/?tenant=${gov_id}&type=b2c"
}
EOF
        )
        
        local b2c_app_result=$(curl -s -w "\n%{http_code}" -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
            "$AUTHENTIK_URL/api/v3/core/applications/" \
            -d "$b2c_app_data")
        
        local b2c_app_http_code=$(echo "$b2c_app_result" | tail -n1)
        if [ "$b2c_app_http_code" = "201" ] || [ "$b2c_app_http_code" = "200" ]; then
            echo "  ✅ Created B2C application"
        elif [ "$b2c_app_http_code" = "400" ]; then
            echo "  ⏭️  B2C application may already exist"
        else
            echo "  ⚠️  Could not create B2C application (HTTP $b2c_app_http_code)"
        fi
    fi

# Create test users
echo ""
    echo "Creating test users for $gov_name..."

create_user() {
    local username=$1
    local name=$2
    local email=$3
    local groups=$4
    
    local group_pks=""
    for group in $groups; do
            local gpk=$(curl -s -H "$AUTH_HEADER" "$AUTHENTIK_URL/api/v3/core/groups/?name=$group" | jq -r '.results[0].pk // empty')
        if [ -n "$gpk" ] && [ "$gpk" != "null" ]; then
            if [ -n "$group_pks" ]; then
                group_pks="$group_pks,$gpk"
            else
                group_pks="$gpk"
            fi
        fi
    done
    
    local user_data=$(cat <<EOF
{
    "username": "$username",
    "name": "$name",
    "email": "$email",
    "is_active": true,
    "groups": [$(echo "$group_pks" | sed 's/,/","/g' | sed 's/^/"/' | sed 's/$/"/')]
}
EOF
)
    
    local result=$(curl -s -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
        "$AUTHENTIK_URL/api/v3/core/users/" \
        -d "$user_data")
    
    if echo "$result" | grep -q '"pk"'; then
        local user_pk=$(echo "$result" | jq -r '.pk')
        curl -s -X POST -H "$AUTH_HEADER" -H "$CONTENT_TYPE" \
            "$AUTHENTIK_URL/api/v3/core/users/$user_pk/set_password/" \
            -d '{"password": "Test123!"}' > /dev/null
        echo "  ✅ Created user: $username (password: Test123!)"
    elif echo "$result" | grep -q "already exists"; then
        echo "  ⏭️  User already exists: $username"
        fi
    }
    
    if [ -n "$gov_domain" ] && [ "$gov_domain" != "null" ] && [ "$gov_domain" != "localhost" ]; then
        create_user "${gov_id}-admin" "${gov_name} Admin" "admin@${gov_domain}" "${gov_id}-admin portal-admin"
        create_user "${gov_id}-agent" "${gov_name} Agent" "agent@${gov_domain}" "${gov_id}-agent portal-agent"
        create_user "${gov_id}-citizen1" "${gov_name} Citizen 1" "citizen1@example.com" "${gov_id}-citizen citizen"
        create_user "${gov_id}-citizen2" "${gov_name} Citizen 2" "citizen2@example.com" "${gov_id}-citizen citizen"
    else
        create_user "${gov_id}-admin" "${gov_name} Admin" "admin@${gov_id}.local" "${gov_id}-admin portal-admin"
        create_user "${gov_id}-agent" "${gov_name} Agent" "agent@${gov_id}.local" "${gov_id}-agent portal-agent"
        create_user "${gov_id}-citizen1" "${gov_name} Citizen 1" "citizen1@${gov_id}.local" "${gov_id}-citizen citizen"
    fi
    
    # Store provider info for output
    echo "${gov_id}|${client_id}|${client_secret}|${b2b_auth_flow_pk}|${b2c_auth_flow_pk}" >> /tmp/authentik_providers.txt
}

# Process all government tenants
PROVIDERS_FILE="/tmp/authentik_providers.txt"
> "$PROVIDERS_FILE"

# Process each tenant
for i in $(seq 0 $((TENANT_COUNT - 1))); do
    tenant_json=$(echo "$TENANTS_ARRAY" | jq -c ".[$i]")
    process_government_tenant "$tenant_json"
done

# Output summary
echo ""
echo "========================================"
echo "✅ Authentik Multi-Tenant Setup Complete!"
echo "========================================"
echo ""
echo "OIDC Configuration Summary:"
echo ""

if [ -f "$PROVIDERS_FILE" ]; then
    while IFS='|' read -r gov_id client_id client_secret b2b_flow b2c_flow; do
        echo "Government: $gov_id"
        echo "  Authority: $AUTHENTIK_URL/application/o/$client_id/"
        echo "  Client ID: $client_id"
        echo "  Client Secret: $client_secret"
        echo ""
    done < "$PROVIDERS_FILE"
fi

echo "Update your appsettings.Development.json:"
echo ""
if [ -f "$PROVIDERS_FILE" ] && [ -s "$PROVIDERS_FILE" ]; then
    # Get first provider info
    first_provider=$(head -n1 "$PROVIDERS_FILE")
    if [ -n "$first_provider" ]; then
        IFS='|' read -r gov_id client_id client_secret b2b_flow b2c_flow <<< "$first_provider"
echo '{
  "Auth": {
    "Mode": "Oidc",
    "TenantClaimType": "tenant",
    "Oidc": {
      "Authority": "'$AUTHENTIK_URL'/application/o/'$client_id'/",
      "ClientId": "'$client_id'",
      "ClientSecret": "'$client_secret'",
      "RoleClaimType": "roles",
      "NameClaimType": "preferred_username",
      "RequireHttpsMetadata": false
    }
  }
}'
    else
        echo "  (Provider information not available)"
    fi
else
    echo "  (No providers created - check errors above)"
fi
echo ""
echo "Test Users (password: Test123!):"
echo "  - {gov-id}-admin    (Admin role)"
echo "  - {gov-id}-agent    (GovernmentAgent role)"
echo "  - {gov-id}-citizen1 (Citizen role)"
echo ""
