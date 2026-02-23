#!/bin/bash
# ═══════════════════════════════════════════════════════════════════════════════
#  IERAHKWA SSL/TLS SETUP SCRIPT
#  Configures Let's Encrypt certificates for production
# ═══════════════════════════════════════════════════════════════════════════════

set -e

# Configuration
DOMAIN="${DOMAIN:-ierahkwa.gov}"
EMAIL="${EMAIL:-admin@ierahkwa.gov}"
ROOT="$(cd "$(dirname "$0")/.." && pwd)"

echo "╔══════════════════════════════════════════════════════════════════════════════╗"
echo "║     🔐 IERAHKWA SSL/TLS SETUP                                                ║"
echo "╚══════════════════════════════════════════════════════════════════════════════╝"
echo ""
echo "Domain: $DOMAIN"
echo "Email: $EMAIL"
echo ""

# Create directories
mkdir -p "$ROOT/ssl/certs"
mkdir -p "$ROOT/certbot/www"
mkdir -p "$ROOT/certbot/conf"

# ─────────────────────────────────────────────────────────────────────────────────
#  OPTION 1: Let's Encrypt with Docker (Recommended for Production)
# ─────────────────────────────────────────────────────────────────────────────────

setup_letsencrypt_docker() {
    echo "📜 Setting up Let's Encrypt with Docker..."
    
    # First, start nginx to serve ACME challenge
    docker-compose -f "$ROOT/docker-compose.production.yml" up -d nginx
    
    # Get certificate
    docker-compose -f "$ROOT/docker-compose.production.yml" run --rm certbot certonly \
        --webroot \
        --webroot-path=/var/www/certbot \
        --email "$EMAIL" \
        --agree-tos \
        --no-eff-email \
        -d "$DOMAIN" \
        -d "www.$DOMAIN" \
        -d "api.$DOMAIN" \
        -d "monitor.$DOMAIN"
    
    # Restart nginx to load new certificates
    docker-compose -f "$ROOT/docker-compose.production.yml" restart nginx
    
    echo "✅ Let's Encrypt certificates obtained!"
}

# ─────────────────────────────────────────────────────────────────────────────────
#  OPTION 2: Let's Encrypt with Standalone (No Docker)
# ─────────────────────────────────────────────────────────────────────────────────

setup_letsencrypt_standalone() {
    echo "📜 Setting up Let's Encrypt standalone..."
    
    # Check if certbot is installed
    if ! command -v certbot &> /dev/null; then
        echo "Installing certbot..."
        if [[ "$OSTYPE" == "darwin"* ]]; then
            brew install certbot
        else
            sudo apt-get update
            sudo apt-get install -y certbot
        fi
    fi
    
    # Stop any running services on port 80
    echo "⚠️  Make sure port 80 is free!"
    
    # Get certificate
    sudo certbot certonly \
        --standalone \
        --email "$EMAIL" \
        --agree-tos \
        --no-eff-email \
        -d "$DOMAIN" \
        -d "www.$DOMAIN" \
        -d "api.$DOMAIN"
    
    # Copy certificates
    sudo cp /etc/letsencrypt/live/$DOMAIN/fullchain.pem "$ROOT/ssl/certs/"
    sudo cp /etc/letsencrypt/live/$DOMAIN/privkey.pem "$ROOT/ssl/certs/"
    sudo cp /etc/letsencrypt/live/$DOMAIN/chain.pem "$ROOT/ssl/certs/"
    sudo chmod 644 "$ROOT/ssl/certs/"*.pem
    
    echo "✅ Let's Encrypt certificates obtained!"
}

# ─────────────────────────────────────────────────────────────────────────────────
#  OPTION 3: Self-Signed Certificate (Development Only)
# ─────────────────────────────────────────────────────────────────────────────────

setup_self_signed() {
    echo "🔑 Generating self-signed certificate (for development only)..."
    
    openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
        -keyout "$ROOT/ssl/certs/privkey.pem" \
        -out "$ROOT/ssl/certs/fullchain.pem" \
        -subj "/C=CA/ST=Ontario/L=Akwesasne/O=Ierahkwa Government/CN=$DOMAIN"
    
    # Create chain (same as fullchain for self-signed)
    cp "$ROOT/ssl/certs/fullchain.pem" "$ROOT/ssl/certs/chain.pem"
    
    echo "⚠️  Self-signed certificate created. Use only for development!"
    echo "✅ Certificates saved to $ROOT/ssl/certs/"
}

# ─────────────────────────────────────────────────────────────────────────────────
#  SETUP AUTO-RENEWAL
# ─────────────────────────────────────────────────────────────────────────────────

setup_auto_renewal() {
    echo "⏰ Setting up auto-renewal..."
    
    # Create renewal script
    cat > "$ROOT/scripts/renew-ssl.sh" << 'RENEWEOF'
#!/bin/bash
# SSL Certificate Auto-Renewal Script

# For Docker setup
docker-compose -f /path/to/docker-compose.production.yml run --rm certbot renew --quiet
docker-compose -f /path/to/docker-compose.production.yml restart nginx

# For standalone setup (uncomment if not using Docker)
# certbot renew --quiet
# systemctl reload nginx
RENEWEOF

    chmod +x "$ROOT/scripts/renew-ssl.sh"
    
    # Add to crontab (runs twice daily as recommended by Let's Encrypt)
    CRON_JOB="0 0,12 * * * $ROOT/scripts/renew-ssl.sh >> $ROOT/logs/ssl-renewal.log 2>&1"
    
    if crontab -l 2>/dev/null | grep -q "renew-ssl.sh"; then
        echo "Auto-renewal cron job already exists."
    else
        (crontab -l 2>/dev/null; echo "$CRON_JOB") | crontab -
        echo "✅ Auto-renewal cron job added (runs at 00:00 and 12:00 daily)"
    fi
}

# ─────────────────────────────────────────────────────────────────────────────────
#  MAIN
# ─────────────────────────────────────────────────────────────────────────────────

echo "Select SSL setup method:"
echo "  1) Let's Encrypt with Docker (recommended for production)"
echo "  2) Let's Encrypt standalone (no Docker)"
echo "  3) Self-signed certificate (development only)"
echo ""
read -p "Choice [1-3]: " choice

case $choice in
    1)
        setup_letsencrypt_docker
        setup_auto_renewal
        ;;
    2)
        setup_letsencrypt_standalone
        setup_auto_renewal
        ;;
    3)
        setup_self_signed
        ;;
    *)
        echo "Invalid choice. Exiting."
        exit 1
        ;;
esac

echo ""
echo "═══════════════════════════════════════════════════════════════════════════════"
echo "🔐 SSL Setup Complete!"
echo ""
echo "Certificates location: $ROOT/ssl/certs/"
echo ""
echo "Next steps:"
echo "  1. Update nginx.conf with your domain"
echo "  2. Start/restart nginx: docker-compose up -d nginx"
echo "  3. Test HTTPS: curl -I https://$DOMAIN"
echo "═══════════════════════════════════════════════════════════════════════════════"
