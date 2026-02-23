# Pupitre Foundation Services Policy
# Grants access to secrets for foundation microservices

# Database credentials - Foundation services
path "secret/data/pupitre/postgres/users" {
  capabilities = ["read"]
}

path "secret/data/pupitre/postgres/gles" {
  capabilities = ["read"]
}

path "secret/data/pupitre/postgres/curricula" {
  capabilities = ["read"]
}

path "secret/data/pupitre/postgres/lessons" {
  capabilities = ["read"]
}

path "secret/data/pupitre/postgres/assessments" {
  capabilities = ["read"]
}

path "secret/data/pupitre/postgres/ieps" {
  capabilities = ["read"]
}

path "secret/data/pupitre/postgres/rewards" {
  capabilities = ["read"]
}

path "secret/data/pupitre/postgres/notifications" {
  capabilities = ["read"]
}

path "secret/data/pupitre/postgres/credentials" {
  capabilities = ["read"]
}

path "secret/data/pupitre/postgres/analytics" {
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
