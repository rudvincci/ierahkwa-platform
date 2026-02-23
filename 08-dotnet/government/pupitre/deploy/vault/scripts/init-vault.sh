#!/bin/bash
# Initialize Vault with Pupitre secrets structure

set -e

VAULT_ADDR=${VAULT_ADDR:-"http://localhost:8200"}
VAULT_TOKEN=${VAULT_TOKEN:-"pupitre-dev-token"}

export VAULT_ADDR
export VAULT_TOKEN

echo "🔐 Initializing Pupitre Vault secrets..."

# Enable KV secrets engine v2
vault secrets enable -path=secret -version=2 kv 2>/dev/null || true

# Create policies
echo "📜 Creating policies..."
vault policy write pupitre-foundation /vault/policies/pupitre-foundation.hcl
vault policy write pupitre-ai /vault/policies/pupitre-ai.hcl

# Initialize Foundation service secrets
echo "🏛️ Initializing Foundation service secrets..."
FOUNDATION_SERVICES=("users" "gles" "curricula" "lessons" "assessments" "ieps" "rewards" "notifications" "credentials" "analytics")

for service in "${FOUNDATION_SERVICES[@]}"; do
    vault kv put secret/pupitre/postgres/${service} \
        username="pupitre_${service}" \
        password="$(openssl rand -base64 24)" \
        database="pupitre_${service}"
done

# Initialize AI service secrets
echo "🤖 Initializing AI service secrets..."
AI_SERVICES=("tutors" "assessments" "content" "speech" "adaptive" "behavior" "safety" "recommendations" "translation" "vision")

for service in "${AI_SERVICES[@]}"; do
    vault kv put secret/pupitre/postgres/ai-${service} \
        username="pupitre_ai_${service}" \
        password="$(openssl rand -base64 24)" \
        database="pupitre_ai_${service}"
done

# Initialize shared service secrets
echo "🔗 Initializing shared service secrets..."
vault kv put secret/pupitre/mongodb/connection-string \
    value="mongodb://pupitre:pupitre_dev_password@localhost:27017"

vault kv put secret/pupitre/redis/connection-string \
    value="localhost:6379"

vault kv put secret/pupitre/rabbitmq/connection-string \
    value="amqp://pupitre:pupitre_dev_password@localhost:5672"

# Initialize JWT secrets
echo "🔑 Initializing JWT secrets..."
vault kv put secret/pupitre/jwt/signing-key \
    value="$(openssl rand -base64 64)"

vault kv put secret/pupitre/jwt/issuer \
    value="pupitre-platform"

vault kv put secret/pupitre/jwt/audience \
    value="pupitre-services"

# Initialize AI API placeholders
echo "🧠 Initializing AI API key placeholders..."
vault kv put secret/pupitre/ai/openai-api-key \
    value="sk-placeholder-replace-me"

vault kv put secret/pupitre/ai/azure-openai-endpoint \
    value="https://your-instance.openai.azure.com/"

vault kv put secret/pupitre/ai/azure-openai-key \
    value="placeholder-replace-me"

echo "✅ Vault initialization complete!"
echo ""
echo "📌 Remember to update AI API keys with real values before production use."
