#!/bin/bash
# Initialize Redis Cluster for Pupitre

REDIS_NODES=${REDIS_NODES:-"redis-1:6379 redis-2:6379 redis-3:6379 redis-4:6379 redis-5:6379 redis-6:6379"}
REDIS_PASSWORD=${REDIS_PASSWORD:-"pupitre_dev_password"}

echo "🚀 Initializing Redis Cluster for Pupitre..."

# Wait for Redis nodes to be ready
sleep 5

# Create cluster (3 masters, 3 replicas)
redis-cli --cluster create $REDIS_NODES \
  --cluster-replicas 1 \
  -a $REDIS_PASSWORD \
  --cluster-yes

# Wait for cluster to be ready
sleep 3

# Check cluster status
redis-cli -a $REDIS_PASSWORD cluster info

echo "✅ Redis Cluster initialized successfully!"

# Create key prefixes/namespaces for different services
redis-cli -a $REDIS_PASSWORD <<EOF
# Session keys (TTL: 24 hours)
SET pupitre:sessions:config:ttl 86400

# Cache keys (TTL: 1 hour)
SET pupitre:cache:config:ttl 3600

# Rate limiting keys (TTL: 1 minute)
SET pupitre:ratelimit:config:ttl 60

# Real-time presence (TTL: 5 minutes)
SET pupitre:presence:config:ttl 300

# AI conversation context (TTL: 2 hours)
SET pupitre:ai:context:config:ttl 7200
EOF

echo "📋 Redis namespaces configured successfully!"
