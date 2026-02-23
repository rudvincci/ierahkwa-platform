FROM node:22-alpine
WORKDIR /app
COPY . .
EXPOSE 3001
HEALTHCHECK --interval=30s --timeout=10s --retries=3 \
  CMD wget -qO- http://localhost:3001/health || exit 1
USER node
CMD ["node", "server.js"]
