# Pupitre AI Services Policy
# Grants access to secrets for AI microservices

# Database credentials - AI services
path "secret/data/pupitre/postgres/ai-*" {
  capabilities = ["read"]
}

# AI API keys
path "secret/data/pupitre/ai/*" {
  capabilities = ["read"]
}

# Shared services
path "secret/data/pupitre/mongodb/*" {
  capabilities = ["read"]
}

path "secret/data/pupitre/redis/*" {
  capabilities = ["read"]
}

path "secret/data/pupitre/rabbitmq/*" {
  capabilities = ["read"]
}

path "secret/data/pupitre/jwt/*" {
  capabilities = ["read"]
}

# Vector DB credentials
path "secret/data/pupitre/qdrant/*" {
  capabilities = ["read"]
}
