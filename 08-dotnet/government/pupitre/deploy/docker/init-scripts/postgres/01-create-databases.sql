-- Pupitre Platform Database Initialization
-- Creates databases for all microservices

-- Foundation Services
CREATE DATABASE pupitre_users;
CREATE DATABASE pupitre_gles;
CREATE DATABASE pupitre_curricula;
CREATE DATABASE pupitre_lessons;
CREATE DATABASE pupitre_assessments;
CREATE DATABASE pupitre_ieps;
CREATE DATABASE pupitre_rewards;
CREATE DATABASE pupitre_notifications;
CREATE DATABASE pupitre_credentials;
CREATE DATABASE pupitre_analytics;

-- AI Services
CREATE DATABASE pupitre_ai_tutors;
CREATE DATABASE pupitre_ai_assessments;
CREATE DATABASE pupitre_ai_content;
CREATE DATABASE pupitre_ai_speech;
CREATE DATABASE pupitre_ai_adaptive;
CREATE DATABASE pupitre_ai_behavior;
CREATE DATABASE pupitre_ai_safety;
CREATE DATABASE pupitre_ai_recommendations;
CREATE DATABASE pupitre_ai_translation;
CREATE DATABASE pupitre_ai_vision;

-- Support Services
CREATE DATABASE pupitre_parents;
CREATE DATABASE pupitre_educators;
CREATE DATABASE pupitre_fundraising;
CREATE DATABASE pupitre_bookstore;
CREATE DATABASE pupitre_aftercare;
CREATE DATABASE pupitre_accessibility;
CREATE DATABASE pupitre_compliance;
CREATE DATABASE pupitre_ministries;
CREATE DATABASE pupitre_operations;

-- Grant permissions
GRANT ALL PRIVILEGES ON DATABASE pupitre_users TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_gles TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_curricula TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_lessons TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_assessments TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_ieps TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_rewards TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_notifications TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_credentials TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_analytics TO pupitre;

GRANT ALL PRIVILEGES ON DATABASE pupitre_ai_tutors TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_ai_assessments TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_ai_content TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_ai_speech TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_ai_adaptive TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_ai_behavior TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_ai_safety TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_ai_recommendations TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_ai_translation TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_ai_vision TO pupitre;

GRANT ALL PRIVILEGES ON DATABASE pupitre_parents TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_educators TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_fundraising TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_bookstore TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_aftercare TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_accessibility TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_compliance TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_ministries TO pupitre;
GRANT ALL PRIVILEGES ON DATABASE pupitre_operations TO pupitre;
