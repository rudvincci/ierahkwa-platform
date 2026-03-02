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
EXPOSE 3095
ENV PORT=3095
HEALTHCHECK --interval=30s --timeout=10s --start-period=15s --retries=3 \
  CMD wget -qO- http://localhost:3095/health || exit 1
CMD ["node", "conciencia_captcha.js"]
