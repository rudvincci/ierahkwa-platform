# ============================================================
# WiFi Soberano — Dockerfile
# Internet Satelital Soberano — Ierahkwa Ne Kanienke
# Port: 3095
# ============================================================

# Stage 1: Dependencies
FROM node:22-alpine AS deps
WORKDIR /app
COPY package.json package-lock.json* ./
RUN npm ci --only=production && npm cache clean --force

# Stage 2: Production
FROM node:22-alpine AS production
LABEL maintainer="Ierahkwa Ne Kanienke <soberano@ierahkwa.org>"
LABEL description="WiFi Soberano — Internet Satelital Soberano"
LABEL version="1.0.0"

# Install tini for proper PID 1
RUN apk add --no-cache tini curl

# Create non-root user
RUN addgroup -g 1001 -S soberano && \
    adduser -S soberano -u 1001 -G soberano

WORKDIR /app

# Copy dependencies from stage 1
COPY --from=deps /app/node_modules ./node_modules

# Copy application code
COPY package.json ./
COPY server.js ./
COPY routes/ ./routes/
COPY middleware/ ./middleware/
COPY models/ ./models/

# Set ownership
RUN chown -R soberano:soberano /app

# Switch to non-root user
USER soberano

# Environment
ENV NODE_ENV=production
ENV PORT=3095

# Expose port
EXPOSE 3095

# Health check
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:3095/health || exit 1

# Use tini as PID 1
ENTRYPOINT ["/sbin/tini", "--"]

# Start server
CMD ["node", "server.js"]
