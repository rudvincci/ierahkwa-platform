# ============================================================
# Ierahkwa Sovereign Core — Docker Image
# Node 22 Alpine | Port 3050 | Production-ready
# ============================================================

# ── Stage 1: Install dependencies ────────────────────────────
FROM node:22-alpine AS deps

WORKDIR /app

# Install build tools for native modules (pg, bcryptjs)
RUN apk add --no-cache python3 make g++

# Copy package files first (layer caching)
COPY package.json package-lock.json* ./

# Production-only install (no devDependencies)
RUN npm ci --omit=dev && npm cache clean --force

# ── Stage 2: Production image ────────────────────────────────
FROM node:22-alpine AS production

# Security: run as non-root user
RUN addgroup -g 1001 -S ierahkwa && \
    adduser -S sovereign -u 1001 -G ierahkwa

# Install runtime dependencies only
RUN apk add --no-cache tini curl

WORKDIR /app

# Copy node_modules from deps stage
COPY --from=deps /app/node_modules ./node_modules

# Copy application code
COPY package.json ./
COPY server.js ./
COPY src/ ./src/
COPY migrations/ ./migrations/
COPY data/ ./data/

# Copy shared modules (mounted or copied at build time)
# NOTE: In docker-compose, ../shared/ is typically mounted as a volume.
# For standalone builds, copy shared into the image context.
# COPY ../shared/ ./shared/

# Create data directories
RUN mkdir -p /app/data/uploads /app/data/logs && \
    chown -R sovereign:ierahkwa /app

# Switch to non-root user
USER sovereign

# Environment defaults
ENV NODE_ENV=production \
    PORT=3050 \
    LOG_LEVEL=info

# Expose API port
EXPOSE 3050

# Health check — /health endpoint every 30s
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:3050/health || exit 1

# Use tini as PID 1 for proper signal handling
ENTRYPOINT ["/sbin/tini", "--"]

# Start the server
CMD ["node", "server.js"]
