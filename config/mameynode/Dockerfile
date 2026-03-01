# MameyNode Runtime — Ierahkwa Platform v3.3.0
# Sovereign Node.js runtime for 574 tribal nations

FROM node:20.11-alpine AS builder
WORKDIR /app
COPY package*.json ./
RUN npm ci --only=production && npm cache clean --force

FROM node:20.11-alpine AS runtime
LABEL maintainer="Ierahkwa Platform <dev@ierahkwa.io>"
LABEL version="3.3.0"
LABEL description="MameyNode — Sovereign Node.js Runtime"

RUN apk add --no-cache dumb-init curl && \
    addgroup -g 1001 -S ierahkwa && \
    adduser -S mameynode -u 1001 -G ierahkwa

WORKDIR /app
COPY --from=builder --chown=mameynode:ierahkwa /app/node_modules ./node_modules
COPY --chown=mameynode:ierahkwa . .

ENV NODE_ENV=production
ENV PORT=3000

USER mameynode
EXPOSE 3000 9090

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:3000/health || exit 1

ENTRYPOINT ["dumb-init", "--"]
CMD ["node", "src/server.js"]
