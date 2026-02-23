#!/bin/bash

# Generate HTTPS development certificate for Mamey Government Portal
# This script creates a self-signed PFX certificate for Docker HTTPS

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
CERTS_DIR="${SCRIPT_DIR}/../certs"
CERT_PASSWORD="${CERT_PASSWORD:-MameyDev123!}"
CERT_NAME="aspnetapp"

echo "🔐 Generating HTTPS development certificate..."

# Create certs directory
mkdir -p "${CERTS_DIR}"

# Check if dotnet is available
if command -v dotnet &> /dev/null; then
    echo "Using dotnet dev-certs..."
    
    # Clean existing dev cert
    dotnet dev-certs https --clean 2>/dev/null || true
    
    # Generate new dev cert and export to PFX
    dotnet dev-certs https -ep "${CERTS_DIR}/${CERT_NAME}.pfx" -p "${CERT_PASSWORD}" --trust
    
    echo "✅ Certificate generated at: ${CERTS_DIR}/${CERT_NAME}.pfx"
else
    echo "Using OpenSSL..."
    
    # Generate private key and certificate
    openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
        -keyout "${CERTS_DIR}/${CERT_NAME}.key" \
        -out "${CERTS_DIR}/${CERT_NAME}.crt" \
        -subj "/CN=localhost/O=Mamey Government/C=US" \
        -addext "subjectAltName=DNS:localhost,DNS:government.mamey.local,IP:127.0.0.1"
    
    # Convert to PFX
    openssl pkcs12 -export -out "${CERTS_DIR}/${CERT_NAME}.pfx" \
        -inkey "${CERTS_DIR}/${CERT_NAME}.key" \
        -in "${CERTS_DIR}/${CERT_NAME}.crt" \
        -passout pass:"${CERT_PASSWORD}"
    
    # Clean up intermediate files
    rm -f "${CERTS_DIR}/${CERT_NAME}.key" "${CERTS_DIR}/${CERT_NAME}.crt"
    
    echo "✅ Certificate generated at: ${CERTS_DIR}/${CERT_NAME}.pfx"
fi

echo ""
echo "📋 Certificate Details:"
echo "   Path: ${CERTS_DIR}/${CERT_NAME}.pfx"
echo "   Password: ${CERT_PASSWORD}"
echo ""
echo "🚀 You can now run: docker compose -f docker-compose.app.yml up -d"
