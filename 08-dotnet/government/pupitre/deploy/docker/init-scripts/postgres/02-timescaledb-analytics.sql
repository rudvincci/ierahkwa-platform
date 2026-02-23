-- Enable TimescaleDB extension for analytics database
\c pupitre_analytics;

CREATE EXTENSION IF NOT EXISTS timescaledb;

-- Learning Progress Events (time-series)
CREATE TABLE IF NOT EXISTS learning_progress_events (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    lesson_id UUID NOT NULL,
    curriculum_id UUID NOT NULL,
    event_type VARCHAR(50) NOT NULL,
    progress_percentage DECIMAL(5,2),
    time_spent_seconds INTEGER,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

SELECT create_hypertable('learning_progress_events', 'created_at', if_not_exists => TRUE);

-- Assessment Results (time-series)
CREATE TABLE IF NOT EXISTS assessment_results (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    assessment_id UUID NOT NULL,
    score DECIMAL(5,2),
    max_score DECIMAL(5,2),
    time_taken_seconds INTEGER,
    attempt_number INTEGER DEFAULT 1,
    submitted_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

SELECT create_hypertable('assessment_results', 'submitted_at', if_not_exists => TRUE);

-- User Engagement Metrics (time-series)
CREATE TABLE IF NOT EXISTS engagement_metrics (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    session_id UUID NOT NULL,
    metric_type VARCHAR(50) NOT NULL,
    metric_value DECIMAL(10,2),
    recorded_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

SELECT create_hypertable('engagement_metrics', 'recorded_at', if_not_exists => TRUE);

-- AI Tutor Interactions (time-series)
CREATE TABLE IF NOT EXISTS tutor_interactions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    session_id UUID NOT NULL,
    interaction_type VARCHAR(50) NOT NULL,
    prompt_tokens INTEGER,
    completion_tokens INTEGER,
    latency_ms INTEGER,
    model_used VARCHAR(100),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

SELECT create_hypertable('tutor_interactions', 'created_at', if_not_exists => TRUE);

-- Create indexes for common queries
CREATE INDEX IF NOT EXISTS idx_learning_progress_user ON learning_progress_events(user_id, created_at DESC);
CREATE INDEX IF NOT EXISTS idx_learning_progress_lesson ON learning_progress_events(lesson_id, created_at DESC);
CREATE INDEX IF NOT EXISTS idx_assessment_results_user ON assessment_results(user_id, submitted_at DESC);
CREATE INDEX IF NOT EXISTS idx_engagement_user ON engagement_metrics(user_id, recorded_at DESC);
CREATE INDEX IF NOT EXISTS idx_tutor_user ON tutor_interactions(user_id, created_at DESC);

-- Create continuous aggregates for common analytics
CREATE MATERIALIZED VIEW IF NOT EXISTS daily_learning_stats
WITH (timescaledb.continuous) AS
SELECT
    user_id,
    time_bucket('1 day', created_at) AS day,
    COUNT(*) AS total_events,
    AVG(progress_percentage) AS avg_progress,
    SUM(time_spent_seconds) AS total_time_spent
FROM learning_progress_events
GROUP BY user_id, time_bucket('1 day', created_at);

CREATE MATERIALIZED VIEW IF NOT EXISTS daily_assessment_stats
WITH (timescaledb.continuous) AS
SELECT
    user_id,
    time_bucket('1 day', submitted_at) AS day,
    COUNT(*) AS assessments_taken,
    AVG(score / NULLIF(max_score, 0) * 100) AS avg_score_pct
FROM assessment_results
GROUP BY user_id, time_bucket('1 day', submitted_at);

-- Refresh policy for continuous aggregates (every hour)
SELECT add_continuous_aggregate_policy('daily_learning_stats',
    start_offset => INTERVAL '3 days',
    end_offset => INTERVAL '1 hour',
    schedule_interval => INTERVAL '1 hour',
    if_not_exists => TRUE);

SELECT add_continuous_aggregate_policy('daily_assessment_stats',
    start_offset => INTERVAL '3 days',
    end_offset => INTERVAL '1 hour',
    schedule_interval => INTERVAL '1 hour',
    if_not_exists => TRUE);

-- Data retention policy (keep raw data for 90 days)
SELECT add_retention_policy('learning_progress_events', INTERVAL '90 days', if_not_exists => TRUE);
SELECT add_retention_policy('engagement_metrics', INTERVAL '90 days', if_not_exists => TRUE);
SELECT add_retention_policy('tutor_interactions', INTERVAL '90 days', if_not_exists => TRUE);
-- Keep assessment results longer (1 year)
SELECT add_retention_policy('assessment_results', INTERVAL '365 days', if_not_exists => TRUE);
