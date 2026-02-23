#!/bin/bash
# Initialize Qdrant collections for Pupitre

QDRANT_HOST=${QDRANT_HOST:-"localhost"}
QDRANT_PORT=${QDRANT_PORT:-"6333"}
BASE_URL="http://${QDRANT_HOST}:${QDRANT_PORT}"

echo "🚀 Initializing Qdrant collections for Pupitre..."

# Curriculum Embeddings Collection
echo "📚 Creating curriculum_embeddings collection..."
curl -X PUT "${BASE_URL}/collections/curriculum_embeddings" \
  -H "Content-Type: application/json" \
  -d '{
    "vectors": {
      "size": 1536,
      "distance": "Cosine"
    },
    "hnsw_config": {
      "m": 16,
      "ef_construct": 100
    },
    "optimizers_config": {
      "default_segment_number": 2
    },
    "replication_factor": 1
  }'

echo ""

# Lesson Content Collection
echo "📝 Creating lesson_content collection..."
curl -X PUT "${BASE_URL}/collections/lesson_content" \
  -H "Content-Type: application/json" \
  -d '{
    "vectors": {
      "size": 1536,
      "distance": "Cosine"
    },
    "hnsw_config": {
      "m": 16,
      "ef_construct": 100
    },
    "optimizers_config": {
      "default_segment_number": 2
    },
    "replication_factor": 1
  }'

echo ""

# Student Profiles Collection (for personalization)
echo "👤 Creating student_profiles collection..."
curl -X PUT "${BASE_URL}/collections/student_profiles" \
  -H "Content-Type: application/json" \
  -d '{
    "vectors": {
      "size": 1536,
      "distance": "Cosine"
    },
    "hnsw_config": {
      "m": 16,
      "ef_construct": 100
    },
    "optimizers_config": {
      "default_segment_number": 2
    },
    "replication_factor": 1
  }'

echo ""

# Assessment Questions Collection
echo "❓ Creating assessment_questions collection..."
curl -X PUT "${BASE_URL}/collections/assessment_questions" \
  -H "Content-Type: application/json" \
  -d '{
    "vectors": {
      "size": 1536,
      "distance": "Cosine"
    },
    "hnsw_config": {
      "m": 16,
      "ef_construct": 100
    },
    "optimizers_config": {
      "default_segment_number": 2
    },
    "replication_factor": 1
  }'

echo ""

# Tutor Conversation History
echo "💬 Creating tutor_conversations collection..."
curl -X PUT "${BASE_URL}/collections/tutor_conversations" \
  -H "Content-Type: application/json" \
  -d '{
    "vectors": {
      "size": 1536,
      "distance": "Cosine"
    },
    "hnsw_config": {
      "m": 16,
      "ef_construct": 100
    },
    "optimizers_config": {
      "default_segment_number": 2
    },
    "replication_factor": 1
  }'

echo ""

# GLEs (Grade Level Expectations) Collection
echo "📊 Creating gles collection..."
curl -X PUT "${BASE_URL}/collections/gles" \
  -H "Content-Type: application/json" \
  -d '{
    "vectors": {
      "size": 1536,
      "distance": "Cosine"
    },
    "hnsw_config": {
      "m": 16,
      "ef_construct": 100
    },
    "optimizers_config": {
      "default_segment_number": 2
    },
    "replication_factor": 1
  }'

echo ""
echo "✅ Qdrant collections initialized successfully!"
echo ""
echo "📋 Collections created:"
curl -s "${BASE_URL}/collections" | python3 -m json.tool 2>/dev/null || curl -s "${BASE_URL}/collections"
