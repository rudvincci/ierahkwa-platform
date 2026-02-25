#!/bin/bash
# Generate production secrets for Ierahkwa Platform
# Run once, seal with kubeseal, then delete the raw file

set -euo pipefail

echo "=== Ierahkwa Production Secret Generator ==="

SQL_PASSWORD=$(openssl rand -base64 32 | tr -d '/+=' | head -c 40)
JWT_KEY=$(openssl rand -base64 48 | tr -d '/+=' | head -c 64)
REDIS_PASSWORD=$(openssl rand -base64 24 | tr -d '/+=' | head -c 32)

cat > secrets-raw.yml << EOF
apiVersion: v1
kind: Secret
metadata:
  name: ierahkwa-production-secrets
  namespace: ierahkwa
type: Opaque
stringData:
  sql-password: "${SQL_PASSWORD}"
  jwt-key: "${JWT_KEY}"
  redis-password: "${REDIS_PASSWORD}"
EOF

echo "Generated secrets-raw.yml"
echo "SQL Password: ${SQL_PASSWORD:0:4}****"
echo "JWT Key:      ${JWT_KEY:0:4}****"
echo "Redis Pass:   ${REDIS_PASSWORD:0:4}****"
echo ""
echo "Next steps:"
echo "  1. kubeseal --format yaml < secrets-raw.yml > k8s/sealed-secrets.yml"
echo "  2. rm secrets-raw.yml"
echo "  3. git add k8s/sealed-secrets.yml && git commit"
