#!/bin/bash
# Initialize MongoDB Replica Set for Pupitre

MONGO_PRIMARY=${MONGO_PRIMARY:-"mongo-primary:27017"}
MONGO_SECONDARY1=${MONGO_SECONDARY1:-"mongo-secondary1:27017"}
MONGO_SECONDARY2=${MONGO_SECONDARY2:-"mongo-secondary2:27017"}
MONGO_USER=${MONGO_USER:-"pupitre"}
MONGO_PASSWORD=${MONGO_PASSWORD:-"pupitre_dev_password"}

echo "🚀 Initializing MongoDB Replica Set for Pupitre..."

# Wait for MongoDB to be ready
sleep 10

# Initialize replica set
mongosh --host $MONGO_PRIMARY --eval '
rs.initiate({
  _id: "pupitre-rs",
  members: [
    { _id: 0, host: "'$MONGO_PRIMARY'", priority: 2 },
    { _id: 1, host: "'$MONGO_SECONDARY1'", priority: 1 },
    { _id: 2, host: "'$MONGO_SECONDARY2'", priority: 1, slaveDelay: 3600, hidden: true }
  ],
  settings: {
    electionTimeoutMillis: 10000,
    heartbeatTimeoutSecs: 10
  }
})
'

# Wait for replica set to be ready
sleep 5

# Create admin user
mongosh --host $MONGO_PRIMARY --eval '
use admin
db.createUser({
  user: "'$MONGO_USER'",
  pwd: "'$MONGO_PASSWORD'",
  roles: [
    { role: "root", db: "admin" },
    { role: "readWriteAnyDatabase", db: "admin" }
  ]
})
'

# Create application databases with users
mongosh --host $MONGO_PRIMARY -u $MONGO_USER -p $MONGO_PASSWORD --authenticationDatabase admin --eval '
// Users service database
use pupitre_users
db.createCollection("users")
db.users.createIndex({ "email": 1 }, { unique: true })
db.users.createIndex({ "role": 1 })

// GLEs service database
use pupitre_gles
db.createCollection("gles")
db.gles.createIndex({ "subject": 1, "gradeLevel": 1 })
db.gles.createIndex({ "code": 1 }, { unique: true })

// Lessons service database
use pupitre_lessons
db.createCollection("lessons")
db.lessons.createIndex({ "curriculumId": 1 })
db.lessons.createIndex({ "gleIds": 1 })

// Assessments service database  
use pupitre_assessments
db.createCollection("assessments")
db.assessments.createIndex({ "lessonId": 1 })
db.assessments.createIndex({ "userId": 1, "createdAt": -1 })

// AI services database
use pupitre_ai
db.createCollection("tutor_sessions")
db.tutor_sessions.createIndex({ "userId": 1, "createdAt": -1 })
db.tutor_sessions.createIndex({ "status": 1 })

print("✅ All databases and indexes created successfully!")
'

echo "✅ MongoDB Replica Set initialized successfully!"
