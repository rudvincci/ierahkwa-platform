FROM node:22-alpine
WORKDIR /app
COPY . .
EXPOSE 3007
HEALTHCHECK --interval=30s --timeout=10s --retries=3 \
  CMD wget -qO- http://localhost:3007/health || exit 1
USER node
CMD ["node", "index.js"]
