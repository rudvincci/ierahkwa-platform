FROM node:25-alpine
WORKDIR /app
COPY . .
EXPOSE 3004
HEALTHCHECK --interval=30s --timeout=10s --retries=3 \
  CMD wget -qO- http://localhost:3004/health || exit 1
USER node
CMD ["node", "server.js"]
