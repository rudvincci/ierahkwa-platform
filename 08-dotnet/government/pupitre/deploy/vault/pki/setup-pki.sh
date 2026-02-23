#!/bin/bash
# Setup Vault PKI for Pupitre service-to-service TLS

VAULT_ADDR=${VAULT_ADDR:-"http://localhost:8200"}
VAULT_TOKEN=${VAULT_TOKEN:-"root"}

echo "🔐 Setting up Vault PKI for Pupitre..."

# Enable PKI secrets engine for root CA
vault secrets enable -path=pki pki

# Configure root CA with 10 year max TTL
vault secrets tune -max-lease-ttl=87600h pki

# Generate root CA certificate
vault write -field=certificate pki/root/generate/internal \
  common_name="Pupitre Root CA" \
  issuer_name="pupitre-root-ca" \
  ttl=87600h > /tmp/pupitre_root_ca.crt

# Configure CA and CRL URLs
vault write pki/config/urls \
  issuing_certificates="$VAULT_ADDR/v1/pki/ca" \
  crl_distribution_points="$VAULT_ADDR/v1/pki/crl"

# Enable intermediate PKI for service certificates
vault secrets enable -path=pki_int pki

# Configure intermediate CA
vault secrets tune -max-lease-ttl=43800h pki_int

# Generate intermediate CA CSR
vault write -format=json pki_int/intermediate/generate/internal \
  common_name="Pupitre Intermediate CA" \
  issuer_name="pupitre-intermediate-ca" \
  | jq -r '.data.csr' > /tmp/pki_intermediate.csr

# Sign intermediate with root CA
vault write -format=json pki/root/sign-intermediate \
  issuer_ref="pupitre-root-ca" \
  csr=@/tmp/pki_intermediate.csr \
  format=pem_bundle \
  ttl="43800h" \
  | jq -r '.data.certificate' > /tmp/intermediate.cert.pem

# Set signed intermediate certificate
vault write pki_int/intermediate/set-signed certificate=@/tmp/intermediate.cert.pem

# Create role for Foundation services
vault write pki_int/roles/pupitre-foundation \
  allowed_domains="pupitre-foundation.svc.cluster.local,localhost" \
  allow_subdomains=true \
  allow_localhost=true \
  max_ttl="720h"

# Create role for AI services
vault write pki_int/roles/pupitre-ai \
  allowed_domains="pupitre-ai.svc.cluster.local,localhost" \
  allow_subdomains=true \
  allow_localhost=true \
  max_ttl="720h"

# Create role for Support services
vault write pki_int/roles/pupitre-support \
  allowed_domains="pupitre-support.svc.cluster.local,localhost" \
  allow_subdomains=true \
  allow_localhost=true \
  max_ttl="720h"

# Create policy for certificate issuance
vault policy write pupitre-pki - <<EOF
path "pki_int/issue/pupitre-foundation" {
  capabilities = ["create", "update"]
}
path "pki_int/issue/pupitre-ai" {
  capabilities = ["create", "update"]
}
path "pki_int/issue/pupitre-support" {
  capabilities = ["create", "update"]
}
path "pki/ca/pem" {
  capabilities = ["read"]
}
path "pki_int/ca/pem" {
  capabilities = ["read"]
}
EOF

echo "✅ Vault PKI setup complete!"
echo "📋 Available roles:"
echo "  - pupitre-foundation"
echo "  - pupitre-ai"
echo "  - pupitre-support"
