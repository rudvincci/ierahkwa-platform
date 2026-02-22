#!/bin/bash
# 🔑 ROTATE ALL CREDENTIALS
set -euo pipefail
DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo "🔑 Rotating credentials..."
mkdir -p "$DIR/.security/auth/archive"

TS=$(date '+%Y%m%d-%H%M%S')

# Archive old
for f in "$DIR/.security/auth/jwt-secret.key" "$DIR/.security/auth/api-keys.env"; do
    [ -f "$f" ] && cp "$f" "$DIR/.security/auth/archive/$(basename $f).$TS"
done

# New JWT
openssl rand -hex 64 > "$DIR/.security/auth/jwt-secret.key"
chmod 600 "$DIR/.security/auth/jwt-secret.key"

# New API keys
cat > "$DIR/.security/auth/api-keys.env" << EOF
# Rotated $TS
IDENTITY_API_KEY=$(openssl rand -hex 32)
ZKP_API_KEY=$(openssl rand -hex 32)
TREASURY_API_KEY=$(openssl rand -hex 32)
INTER_SERVICE_KEY=$(openssl rand -hex 32)
ADMIN_API_KEY=$(openssl rand -hex 32)
EOF
chmod 600 "$DIR/.security/auth/api-keys.env"

echo "✅ Credentials rotated. Restart services!"
