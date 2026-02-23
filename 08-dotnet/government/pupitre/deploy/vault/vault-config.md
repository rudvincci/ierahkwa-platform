# Pupitre Vault Configuration

## Overview

This document describes the Vault secrets management configuration for the Pupitre platform.

## Secret Paths

### Database Credentials

```
secret/pupitre/postgres/users
secret/pupitre/postgres/gles
secret/pupitre/postgres/curricula
secret/pupitre/postgres/lessons
secret/pupitre/postgres/assessments
secret/pupitre/postgres/ieps
secret/pupitre/postgres/rewards
secret/pupitre/postgres/notifications
secret/pupitre/postgres/credentials
secret/pupitre/postgres/analytics
secret/pupitre/postgres/ai-tutors
secret/pupitre/postgres/ai-assessments
secret/pupitre/postgres/ai-content
secret/pupitre/postgres/ai-speech
secret/pupitre/postgres/ai-adaptive
secret/pupitre/postgres/ai-behavior
secret/pupitre/postgres/ai-safety
secret/pupitre/postgres/ai-recommendations
secret/pupitre/postgres/ai-translation
secret/pupitre/postgres/ai-vision
secret/pupitre/postgres/parents
secret/pupitre/postgres/educators
secret/pupitre/postgres/fundraising
secret/pupitre/postgres/bookstore
secret/pupitre/postgres/aftercare
secret/pupitre/postgres/accessibility
secret/pupitre/postgres/compliance
secret/pupitre/postgres/ministries
secret/pupitre/postgres/operations
```

### MongoDB Credentials

```
secret/pupitre/mongodb/connection-string
```

### Redis Configuration

```
secret/pupitre/redis/connection-string
```

### RabbitMQ Credentials

```
secret/pupitre/rabbitmq/connection-string
```

### AI Service API Keys

```
secret/pupitre/ai/openai-api-key
secret/pupitre/ai/azure-openai-endpoint
secret/pupitre/ai/azure-openai-key
secret/pupitre/ai/anthropic-api-key
```

### JWT Configuration

```
secret/pupitre/jwt/signing-key
secret/pupitre/jwt/issuer
secret/pupitre/jwt/audience
```

## Lease Types

| Type | Duration | Max TTL |
|------|----------|---------|
| postgres | 1h | 24h |
| mongodb | 1h | 24h |
| rabbitmq | 1h | 24h |
| jwt | 30m | 2h |

## Policies

See `policies/` directory for Vault ACL policies.
