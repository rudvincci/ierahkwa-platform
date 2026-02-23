#!/bin/bash
# Initialize Kafka topics for Pupitre

KAFKA_BOOTSTRAP=${KAFKA_BOOTSTRAP:-"localhost:9092"}

echo "🚀 Initializing Kafka topics for Pupitre..."

# Domain Events Topics
echo "📨 Creating domain event topics..."

# Foundation Service Events
kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --create --if-not-exists \
  --topic pupitre.users.events --partitions 6 --replication-factor 1

kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --create --if-not-exists \
  --topic pupitre.gles.events --partitions 3 --replication-factor 1

kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --create --if-not-exists \
  --topic pupitre.curricula.events --partitions 3 --replication-factor 1

kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --create --if-not-exists \
  --topic pupitre.lessons.events --partitions 6 --replication-factor 1

kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --create --if-not-exists \
  --topic pupitre.assessments.events --partitions 6 --replication-factor 1

kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --create --if-not-exists \
  --topic pupitre.ieps.events --partitions 3 --replication-factor 1

kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --create --if-not-exists \
  --topic pupitre.rewards.events --partitions 3 --replication-factor 1

kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --create --if-not-exists \
  --topic pupitre.notifications.events --partitions 6 --replication-factor 1

kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --create --if-not-exists \
  --topic pupitre.credentials.events --partitions 3 --replication-factor 1

kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --create --if-not-exists \
  --topic pupitre.analytics.events --partitions 12 --replication-factor 1

# AI Service Events
echo "🤖 Creating AI service topics..."

kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --create --if-not-exists \
  --topic pupitre.ai.inference.requests --partitions 12 --replication-factor 1

kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --create --if-not-exists \
  --topic pupitre.ai.inference.responses --partitions 12 --replication-factor 1

kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --create --if-not-exists \
  --topic pupitre.ai.tutoring.sessions --partitions 6 --replication-factor 1

kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --create --if-not-exists \
  --topic pupitre.ai.assessments.grading --partitions 6 --replication-factor 1

kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --create --if-not-exists \
  --topic pupitre.ai.content.generation --partitions 3 --replication-factor 1

kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --create --if-not-exists \
  --topic pupitre.ai.speech.transcription --partitions 3 --replication-factor 1

kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --create --if-not-exists \
  --topic pupitre.ai.safety.moderation --partitions 6 --replication-factor 1

# Analytics Topics (high volume)
echo "📊 Creating analytics topics..."

kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --create --if-not-exists \
  --topic pupitre.analytics.learning.progress --partitions 12 --replication-factor 1

kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --create --if-not-exists \
  --topic pupitre.analytics.engagement.metrics --partitions 12 --replication-factor 1

kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --create --if-not-exists \
  --topic pupitre.analytics.assessment.results --partitions 6 --replication-factor 1

kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --create --if-not-exists \
  --topic pupitre.analytics.behavior.patterns --partitions 6 --replication-factor 1

# Dead Letter Topics
echo "💀 Creating dead letter topics..."

kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --create --if-not-exists \
  --topic pupitre.dlq.events --partitions 3 --replication-factor 1

kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --create --if-not-exists \
  --topic pupitre.dlq.ai.requests --partitions 3 --replication-factor 1

echo ""
echo "✅ Kafka topics initialized successfully!"
echo ""
echo "📋 Topics created:"
kafka-topics.sh --bootstrap-server $KAFKA_BOOTSTRAP --list | grep pupitre
