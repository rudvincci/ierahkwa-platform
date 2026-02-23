#!/bin/bash
# Initialize MinIO buckets for Pupitre

MINIO_ENDPOINT=${MINIO_ENDPOINT:-"http://localhost:9000"}
MINIO_ACCESS_KEY=${MINIO_ACCESS_KEY:-"pupitre"}
MINIO_SECRET_KEY=${MINIO_SECRET_KEY:-"pupitre_dev_password"}

echo "🚀 Initializing MinIO buckets for Pupitre..."

# Configure mc alias
mc alias set pupitre $MINIO_ENDPOINT $MINIO_ACCESS_KEY $MINIO_SECRET_KEY

# Media Buckets
echo "📸 Creating media buckets..."
mc mb --ignore-existing pupitre/pupitre-media-images
mc mb --ignore-existing pupitre/pupitre-media-videos
mc mb --ignore-existing pupitre/pupitre-media-audio
mc mb --ignore-existing pupitre/pupitre-media-thumbnails

# Content Buckets
echo "📚 Creating content buckets..."
mc mb --ignore-existing pupitre/pupitre-lessons-content
mc mb --ignore-existing pupitre/pupitre-scorm-packages
mc mb --ignore-existing pupitre/pupitre-assessments-assets
mc mb --ignore-existing pupitre/pupitre-curricula-documents

# User Content Buckets
echo "👤 Creating user content buckets..."
mc mb --ignore-existing pupitre/pupitre-user-avatars
mc mb --ignore-existing pupitre/pupitre-user-submissions
mc mb --ignore-existing pupitre/pupitre-user-portfolios

# AI Model Artifacts
echo "🤖 Creating AI model buckets..."
mc mb --ignore-existing pupitre/pupitre-ai-models
mc mb --ignore-existing pupitre/pupitre-ai-embeddings
mc mb --ignore-existing pupitre/pupitre-ai-cache

# Credentials and Documents
echo "📄 Creating document buckets..."
mc mb --ignore-existing pupitre/pupitre-credentials-templates
mc mb --ignore-existing pupitre/pupitre-credentials-issued
mc mb --ignore-existing pupitre/pupitre-iep-documents

# Backup Buckets
echo "💾 Creating backup buckets..."
mc mb --ignore-existing pupitre/pupitre-backups
mc mb --ignore-existing pupitre/pupitre-exports

# Set bucket policies
echo "🔒 Setting bucket policies..."

# Public read for media thumbnails
mc anonymous set download pupitre/pupitre-media-thumbnails

# Public read for avatars
mc anonymous set download pupitre/pupitre-user-avatars

echo ""
echo "✅ MinIO buckets initialized successfully!"
echo ""
echo "📋 Buckets created:"
mc ls pupitre
