FROM node:22-alpine AS builder
WORKDIR /app
COPY package*.json ./
RUN npm ci --ignore-scripts --production
COPY . .

FROM node:22-alpine
RUN addgroup -g 1001 appgroup && adduser -u 1001 -G appgroup -D appuser
WORKDIR /app
COPY --from=builder /app .
USER appuser
EXPOSE 3091
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD node -e "require('http').get('http://localhost:3091/health', r => r.statusCode === 200 ? process.exit(0) : process.exit(1))"
CMD ["node", "server.js"]
