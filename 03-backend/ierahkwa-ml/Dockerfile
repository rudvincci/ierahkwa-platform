# ============================================================
# Ierahkwa ML Engine — Dockerfile
# Sovereign AI/ML: Trust Scoring, Anomaly Detection, Pattern Analysis
# ============================================================

FROM node:22-alpine AS builder
WORKDIR /app
COPY package*.json ./
RUN npm ci --ignore-scripts --production
COPY . .

FROM node:22-alpine
RUN apk add --no-cache tini \
 && addgroup -g 1001 appgroup \
 && adduser -u 1001 -G appgroup -D appuser
WORKDIR /app
COPY --from=builder --chown=appuser:appgroup /app .
USER appuser
EXPOSE 3400
ENV PORT=3400 HOST=0.0.0.0 NODE_ENV=production
HEALTHCHECK --interval=30s --timeout=10s --start-period=15s --retries=3 \
  CMD wget -qO- http://localhost:3400/health || exit 1
ENTRYPOINT ["tini", "--"]
CMD ["node", "server.js"]
