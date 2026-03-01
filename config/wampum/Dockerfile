# WAMPUM Blockchain Engine — Ierahkwa Platform v3.3.0
# Sovereign blockchain for 574 tribal nations

FROM golang:1.22-alpine AS builder
RUN apk add --no-cache gcc musl-dev linux-headers git make
WORKDIR /build
COPY go.mod go.sum ./
RUN go mod download
COPY . .
RUN CGO_ENABLED=1 GOOS=linux go build -ldflags="-s -w -X main.Version=3.3.0" -o /build/wampum ./cmd/wampum

FROM alpine:3.19 AS runtime
LABEL maintainer="Ierahkwa Platform <dev@ierahkwa.io>"
LABEL version="3.3.0"
LABEL description="WAMPUM Blockchain Engine — Sovereign Digital Currency"

RUN apk add --no-cache ca-certificates curl && \
    addgroup -g 1001 -S ierahkwa && \
    adduser -S wampum -u 1001 -G ierahkwa && \
    mkdir -p /data/wampum/state /data/wampum/blocks /etc/ierahkwa && \
    chown -R wampum:ierahkwa /data/wampum /etc/ierahkwa

COPY --from=builder --chown=wampum:ierahkwa /build/wampum /usr/local/bin/wampum
COPY --chown=wampum:ierahkwa wampum-blockchain.config.json /etc/ierahkwa/config.json

USER wampum
WORKDIR /data/wampum

EXPOSE 8545 8546 8547 30303 9090

VOLUME ["/data/wampum"]

HEALTHCHECK --interval=30s --timeout=5s --start-period=30s --retries=3 \
  CMD curl -f http://localhost:8545/health || exit 1

ENTRYPOINT ["wampum"]
CMD ["--config", "/etc/ierahkwa/config.json", "--datadir", "/data/wampum"]
