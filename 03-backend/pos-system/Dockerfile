FROM node:22-alpine
WORKDIR /app
COPY package*.json ./
RUN npm ci --only=production 2>/dev/null || true
COPY . .
EXPOSE 3030
HEALTHCHECK --interval=30s --timeout=10s --retries=3 \
  CMD wget -qO- http://localhost:3030/health || exit 1
USER node
CMD ["node", "server.js"]
