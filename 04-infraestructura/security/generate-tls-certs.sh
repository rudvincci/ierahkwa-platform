#!/usr/bin/env bash
# generate-tls-certs.sh — Genera CA soberana + certificados TLS
# Plataforma Soberana Ierahkwa · Chain 777777
set -euo pipefail

TLS_DIR="/etc/mamey/tls"
echo "Generando certificados TLS soberanos..."

# Crear directorio
sudo mkdir -p "$TLS_DIR"
sudo chown "$(whoami)" "$TLS_DIR"

# 1. Generar CA soberana (RSA-4096, 10 anos)
if [ ! -f "$TLS_DIR/sovereign-ca.key" ]; then
    openssl genrsa -out "$TLS_DIR/sovereign-ca.key" 4096 2>/dev/null
    chmod 600 "$TLS_DIR/sovereign-ca.key"
    openssl req -x509 -new -nodes -key "$TLS_DIR/sovereign-ca.key" -sha256 -days 3650 \
        -out "$TLS_DIR/sovereign-ca.crt" \
        -subj "/C=XX/ST=Sovereign Territory/L=Ierahkwa/O=Gobierno Soberano de Ierahkwa/CN=Ierahkwa Sovereign CA" 2>/dev/null
    echo "CA Soberana generada"
else
    echo "CA ya existe"
fi

# 2. Generar certificado servidor con SAN
cat > "$TLS_DIR/_san.cnf" << SANEOF2
[req]
default_bits = 4096
prompt = no
default_md = sha256
distinguished_name = dn
req_extensions = v3_req
[dn]
C = XX
ST = Sovereign Territory
L = Ierahkwa Ne Kanienke
O = Gobierno Soberano de Ierahkwa
CN = *.soberano.local
[v3_req]
basicConstraints = CA:FALSE
keyUsage = digitalSignature, keyEncipherment
extendedKeyUsage = serverAuth
subjectAltName = @alt_names
[alt_names]
DNS.1 = soberano.local
DNS.2 = *.soberano.local
DNS.3 = ierahkwa.sovereign
DNS.4 = *.ierahkwa.sovereign
DNS.5 = mamey.sovereign
DNS.6 = localhost
IP.1 = 127.0.0.1
IP.2 = ::1
SANEOF2

if [ ! -f "$TLS_DIR/soberano.local.key" ]; then
    openssl genrsa -out "$TLS_DIR/soberano.local.key" 4096 2>/dev/null
    chmod 600 "$TLS_DIR/soberano.local.key"
    openssl req -new -key "$TLS_DIR/soberano.local.key" -out "$TLS_DIR/soberano.local.csr" \
        -config "$TLS_DIR/_san.cnf" 2>/dev/null
    openssl x509 -req -in "$TLS_DIR/soberano.local.csr" \
        -CA "$TLS_DIR/sovereign-ca.crt" -CAkey "$TLS_DIR/sovereign-ca.key" \
        -CAcreateserial -out "$TLS_DIR/soberano.local.crt" \
        -days 365 -sha256 -extfile "$TLS_DIR/_san.cnf" -extensions v3_req 2>/dev/null
    rm -f "$TLS_DIR/soberano.local.csr" "$TLS_DIR/_san.cnf" "$TLS_DIR"/*.srl
    echo "Certificado servidor generado"
else
    echo "Certificado ya existe"
fi

echo ""
echo "Archivos en $TLS_DIR:"
ls -la "$TLS_DIR"
echo ""
echo "Para confiar en la CA:"
echo "  macOS: sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain $TLS_DIR/sovereign-ca.crt"
echo "  Linux: sudo cp $TLS_DIR/sovereign-ca.crt /usr/local/share/ca-certificates/ && sudo update-ca-certificates"
