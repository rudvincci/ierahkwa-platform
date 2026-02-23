# Maestro Docker Image
# Multi-stage build for optimized image size

# Stage 1: Build
FROM node:20-alpine AS builder

WORKDIR /app

# Copy package files
COPY package*.json ./
COPY tsconfig.json ./

# Install dependencies
RUN npm ci

# Copy source code
COPY src/ ./src/
COPY config/ ./config/

# Build TypeScript
RUN npm run build

# Stage 2: Runtime
FROM node:20-alpine

# Install git (needed for private sync and git operations)
RUN apk add --no-cache git openssh-client curl

# Note: cursor-agent should be available on host system or mounted as volume
# Or install it here if there's a public installation method
# For now, users should mount it from host or install in their own Dockerfile

# Create app directory
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install production dependencies only
RUN npm ci --only=production

# Copy built files from builder
COPY --from=builder /app/dist ./dist

# Copy config templates
COPY config/ ./config/
COPY templates/ ./templates/
COPY scripts/ ./scripts/
COPY docs/ ./docs/

# Copy documentation
COPY *.md ./

# Create directories for user data (will be mounted as volumes)
RUN mkdir -p /workspace/.maestro/{checkpoints,cache,reports,logs,config}

# Set working directory to workspace (where user's repo will be mounted)
WORKDIR /workspace

# Expose dashboard port
EXPOSE 3000

# Expose MCP server port
EXPOSE 3001

# Default command (can be overridden)
# Use docker-compose or override CMD to run specific commands
CMD ["node", "/app/dist/cli/index.js", "--help"]

# Health check (optional, uncomment for production)
# HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
#   CMD node -e "require('http').get('http://localhost:3000/api/health', (r) => {process.exit(r.statusCode === 200 ? 0 : 1)})"

# Labels
LABEL maintainer="Mamey Technologies"
LABEL description="Maestro - Orchestrate AI agents and workflows"
LABEL version="1.0.0"
