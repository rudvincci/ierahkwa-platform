#!/usr/bin/env python3
"""Generate Pupitre Agent Task Files"""

import json
import os

OUTPUT_DIR = "/Volumes/Barracuda/mamey-io/code-final/Pupitre/agent-plans"
PLAN_FILE = ".cursor/plans/Pupitre/PupitreTDD.plan.md"

def create_task(task_id, title, description, service, port, phase, dependencies, priority="high", owner="backend"):
    """Create a task dictionary"""
    return {
        "plan_task_id": task_id,
        "title": title,
        "description": description,
        "submodule_repo": f"Pupitre.{service}" if service else "Pupitre",
        "submodule_path": f"Pupitre/Pupitre.{service}" if service else "Pupitre",
        "estimated_time": 28800,
        "required_resources": {"cpus": 2, "ram_gb": 4},
        "priority": priority,
        "artifacts": [],
        "dependencies": dependencies,
        "owner_plan": owner,
        "phase": phase,
        "status": "pending",
        "plan_file": PLAN_FILE,
        "acceptance_criteria": f"Run the task's commands locally; ensure unit/integration tests pass; create a PR with changes under branch feature/{task_id}-${{AGENT_ID}}; attach artifacts listed in 'artifacts' field; update task status in Pupitre/agent-plans/queue.json.",
        "success_metrics": ["Tests pass locally", "Artifacts collected", "PR opened"]
    }

def save_task(task):
    """Save task to JSON file"""
    filepath = os.path.join(OUTPUT_DIR, f"{task['plan_task_id']}.json")
    with open(filepath, 'w') as f:
        json.dump(task, f, indent=2)
    return filepath

# Ensure output directory exists
os.makedirs(OUTPUT_DIR, exist_ok=True)

tasks = []

# ============================================================================
# PHASE 0: PROJECT SETUP (P-001 to P-050)
# ============================================================================

phase0_tasks = [
    ("P-001", "Initialize Pupitre repository structure", "Create root directory structure, global.json (.NET 9), solution file, and directory organization for all microservices.", None, None),
    ("P-002", "Configure CI/CD pipeline for Pupitre", "Set up GitHub Actions workflows for build, test, and deploy. Configure branch protection and PR requirements.", None, None),
    ("P-003", "Create Pupitre Docker Compose environment", "Create docker-compose.yml with all dependencies: PostgreSQL, MongoDB, Redis, RabbitMQ, Kafka, Qdrant, MinIO.", None, None),
    ("P-004", "Set up Kubernetes namespace structure", "Create K8s namespace definitions: pupitre-foundation, pupitre-ai, pupitre-support, pupitre-data, pupitre-observability.", None, None),
    ("P-005", "Configure Vault secrets for Pupitre", "Set up HashiCorp Vault with PKI, database credentials, and service secrets for all microservices.", None, None),
    ("P-006", "Create shared Pupitre contracts library", "Create Pupitre.Contracts library with shared DTOs, interfaces, and contract definitions used across services.", None, None),
    ("P-007", "Set up Pupitre observability stack", "Configure Prometheus, Grafana, Jaeger, and Seq for centralized logging and monitoring.", None, None),
    ("P-008", "Create Pupitre API Gateway project", "Generate Pupitre.ApiGateway microservice using mamey-microservice template with YARP configuration.", "ApiGateway", 60000),
    ("P-009", "Configure API Gateway routing rules", "Define routing rules for all 30 microservices with rate limiting, authentication, and load balancing.", "ApiGateway", 60000),
    ("P-010", "Set up Consul service discovery", "Configure Consul cluster for service registration and health checks across all Pupitre services.", None, None),
    ("P-011", "Create shared AI orchestration library", "Create Pupitre.AI.Core library with Semantic Kernel, LangChain integration, and LLM routing.", None, None),
    ("P-012", "Configure Qdrant vector database", "Set up Qdrant cluster with collections for curriculum embeddings, lesson content, and student profiles.", None, None),
    ("P-013", "Set up Kafka event streaming", "Configure Kafka cluster with topics for domain events, analytics, and AI inference requests.", None, None),
    ("P-014", "Create RabbitMQ exchanges and queues", "Configure RabbitMQ with exchanges for commands, events, and rejected events across all services.", None, None),
    ("P-015", "Set up MinIO object storage", "Configure MinIO buckets for media, SCORM packages, documents, and AI model artifacts.", None, None),
    ("P-016", "Create Pupitre.Types shared library", "Create shared value objects library extending Mamey.Types with education-specific types.", None, None),
    ("P-017", "Configure TimescaleDB for analytics", "Set up TimescaleDB for time-series analytics data and learning behavior tracking.", None, None),
    ("P-018", "Create development seed data scripts", "Create scripts for seeding development data: sample users, GLEs, lessons, and assessments.", None, None),
    ("P-019", "Set up integration test infrastructure", "Configure Testcontainers and test fixtures for all database and message broker dependencies.", None, None),
    ("P-020", "Create performance test framework", "Set up k6 and NBomber test infrastructure for load testing all microservices.", None, None),
]

for i, (tid, title, desc, service, port) in enumerate(phase0_tasks):
    deps = [phase0_tasks[i-1][0]] if i > 0 and i < 5 else []
    tasks.append(create_task(tid, title, f"From plan:\n{desc}\n\nEffort: 1 week\nOwner: DevOps Engineer\nDependencies: {', '.join(deps) if deps else 'None'}", 
                            service, port, 0, deps, "high", "devops" if "CI/CD" in title or "Docker" in title or "Kubernetes" in title else "backend"))

# Additional Phase 0 tasks
more_phase0 = [
    ("P-021", "Create Helm charts for Pupitre", "Create Helm charts for all microservices with configurable values for different environments."),
    ("P-022", "Set up ArgoCD for GitOps", "Configure ArgoCD applications for declarative deployments of all Pupitre services."),
    ("P-023", "Create database migration strategy", "Define EF Core migration strategy and automated migration scripts for all PostgreSQL databases."),
    ("P-024", "Configure MongoDB replica sets", "Set up MongoDB replica sets with read preferences for high availability."),
    ("P-025", "Create Redis cluster configuration", "Configure Redis Cluster for session management, caching, and real-time features."),
    ("P-026", "Set up SSL/TLS certificates", "Configure certificate management with Vault PKI for all service-to-service communication."),
    ("P-027", "Create shared exception handling", "Create Pupitre.Exceptions library with education-specific exceptions and error codes."),
    ("P-028", "Configure OpenTelemetry", "Set up OpenTelemetry for distributed tracing across all microservices."),
    ("P-029", "Create shared validation library", "Create Pupitre.Validation library with FluentValidation rules for common DTOs."),
    ("P-030", "Set up feature flags system", "Configure feature flag management for gradual rollout of new features."),
    ("P-031", "Create documentation site", "Set up documentation site with API documentation, architecture diagrams, and guides."),
    ("P-032", "Configure backup strategies", "Set up automated backup procedures for all databases and object storage."),
    ("P-033", "Create disaster recovery plan", "Document and implement disaster recovery procedures for all services."),
    ("P-034", "Set up security scanning", "Configure SonarQube, Snyk, and Trivy for continuous security scanning."),
    ("P-035", "Create shared health checks", "Create standardized health check endpoints for all microservices."),
    ("P-036", "Configure rate limiting policies", "Define rate limiting policies for API Gateway based on user roles."),
    ("P-037", "Set up A/B testing framework", "Configure A/B testing infrastructure for frontend experiments."),
    ("P-038", "Create shared middleware", "Create Pupitre.Middleware library with common middleware components."),
    ("P-039", "Configure CORS policies", "Define CORS policies for all frontend applications."),
    ("P-040", "Set up WebSocket infrastructure", "Configure SignalR hubs for real-time notifications and collaboration."),
    ("P-041", "Create shared caching strategies", "Define Redis caching strategies and cache invalidation patterns."),
    ("P-042", "Configure message retry policies", "Set up dead letter queues and retry policies for message processing."),
    ("P-043", "Create shared audit logging", "Create audit logging infrastructure for compliance requirements."),
    ("P-044", "Set up metrics dashboards", "Create Grafana dashboards for all microservices."),
    ("P-045", "Configure alerting rules", "Set up Prometheus alerting rules and PagerDuty integration."),
    ("P-046", "Create deployment scripts", "Create shell scripts for manual deployment procedures."),
    ("P-047", "Set up load balancer configuration", "Configure Fabio load balancing for all services."),
    ("P-048", "Create network policies", "Define Kubernetes network policies for service isolation."),
    ("P-049", "Configure pod security policies", "Set up pod security standards for all deployments."),
    ("P-050", "Create runbook documentation", "Document operational runbooks for common scenarios."),
]

for i, (tid, title, desc) in enumerate(more_phase0):
    tasks.append(create_task(tid, title, f"From plan:\n{desc}\n\nEffort: 3 days\nOwner: DevOps Engineer\nDependencies: P-001", 
                            None, None, 0, ["P-001"], "medium", "devops"))

# Save Phase 0 tasks
for task in tasks:
    save_task(task)

print(f"Generated {len(tasks)} Phase 0 tasks")

# ============================================================================
# PHASE 1: FOUNDATION SERVICES (P-051 to P-250)
# ============================================================================

foundation_services = [
    ("Users", "User", 60001, "Identity, authentication, and user profile management"),
    ("GLEs", "GLE", 60002, "Grade-Level Expectations management"),
    ("Curricula", "Curriculum", 60003, "Curriculum mapping and learning track management"),
    ("Lessons", "Lesson", 60004, "Lesson content and multimedia management"),
    ("Assessments", "Assessment", 60005, "Quiz engine, grading, and assessment management"),
    ("IEPs", "IEP", 60006, "Individualized Education Plan management"),
    ("Rewards", "Reward", 60007, "Gamification, points, badges, and achievements"),
    ("Notifications", "Notification", 60008, "Alerts, messaging, and communication"),
    ("Credentials", "Credential", 60009, "Verifiable credentials and certificates"),
    ("Analytics", "Analytic", 60010, "Progress tracking, dashboards, and reporting"),
]

task_num = 51
for service_name, entity, port, desc in foundation_services:
    base_num = task_num
    
    # Generate microservice
    tasks.append(create_task(f"P-{task_num:03d}", f"Generate Pupitre.{service_name} microservice",
        f"From plan:\nGenerate the Pupitre.{service_name} microservice using Mamey.TemplateEngine.\ncd /Volumes/Barracuda/mamey-io/code-final/Pupitre\ndotnet new mamey-microservice --service {service_name} --entity {entity} --port {port}\n\nService: {desc}\n\nEffort: 1 day\nOwner: Senior Engineer\nDependencies: P-001",
        service_name, port, 1, ["P-001"], "high"))
    task_num += 1
    
    # Domain layer
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} domain entities",
        f"From plan:\nImplement domain entities for Pupitre.{service_name}:\n- {entity} aggregate root\n- Value objects\n- Domain events\n- Repository interfaces\n\nEffort: 3 days\nOwner: Domain Expert\nDependencies: P-{base_num:03d}",
        service_name, port, 1, [f"P-{base_num:03d}"], "high"))
    task_num += 1
    
    # Application layer commands
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} command handlers",
        f"From plan:\nImplement command handlers for Pupitre.{service_name}:\n- Create{entity}Handler\n- Update{entity}Handler\n- Delete{entity}Handler\n- Custom domain commands\n\nEffort: 3 days\nOwner: Backend Engineer\nDependencies: P-{base_num+1:03d}",
        service_name, port, 1, [f"P-{base_num+1:03d}"], "high"))
    task_num += 1
    
    # Application layer queries
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} query handlers",
        f"From plan:\nImplement query handlers for Pupitre.{service_name}:\n- Get{entity}ByIdHandler\n- Browse{service_name}Handler\n- Search{service_name}Handler\n- Custom queries\n\nEffort: 2 days\nOwner: Backend Engineer\nDependencies: P-{base_num+2:03d}",
        service_name, port, 1, [f"P-{base_num+2:03d}"], "high"))
    task_num += 1
    
    # Infrastructure - PostgreSQL
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} PostgreSQL repository",
        f"From plan:\nImplement PostgreSQL persistence for Pupitre.{service_name}:\n- {entity}DbContext\n- {entity}PostgresRepository\n- Entity configurations\n- Migrations\n\nEffort: 2 days\nOwner: Backend Engineer\nDependencies: P-{base_num+3:03d}",
        service_name, port, 1, [f"P-{base_num+3:03d}"], "high"))
    task_num += 1
    
    # Infrastructure - MongoDB
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} MongoDB repository",
        f"From plan:\nImplement MongoDB persistence for Pupitre.{service_name}:\n- {entity}Document\n- {entity}MongoRepository\n- Sync service\n\nEffort: 2 days\nOwner: Backend Engineer\nDependencies: P-{base_num+4:03d}",
        service_name, port, 1, [f"P-{base_num+4:03d}"], "high"))
    task_num += 1
    
    # Infrastructure - Redis
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} Redis caching",
        f"From plan:\nImplement Redis caching for Pupitre.{service_name}:\n- {entity}RedisRepository\n- Cache invalidation\n- Sync service\n\nEffort: 1 day\nOwner: Backend Engineer\nDependencies: P-{base_num+5:03d}",
        service_name, port, 1, [f"P-{base_num+5:03d}"], "medium"))
    task_num += 1
    
    # Composite repository
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} composite repository",
        f"From plan:\nImplement composite repository pattern for Pupitre.{service_name}:\n- Composite{entity}Repository\n- Fallback strategy (Redis → Mongo → Postgres)\n\nEffort: 1 day\nOwner: Backend Engineer\nDependencies: P-{base_num+6:03d}",
        service_name, port, 1, [f"P-{base_num+6:03d}"], "high"))
    task_num += 1
    
    # API routes
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} API routes",
        f"From plan:\nImplement REST API routes for Pupitre.{service_name}:\n- CRUD endpoints\n- Search and filter endpoints\n- Pagination\n- Authentication\n\nEffort: 2 days\nOwner: Backend Engineer\nDependencies: P-{base_num+7:03d}",
        service_name, port, 1, [f"P-{base_num+7:03d}"], "high"))
    task_num += 1
    
    # Event handlers
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} event handlers",
        f"From plan:\nImplement domain event handlers for Pupitre.{service_name}:\n- {entity}CreatedHandler\n- {entity}UpdatedHandler\n- Integration event publishing\n\nEffort: 2 days\nOwner: Backend Engineer\nDependencies: P-{base_num+8:03d}",
        service_name, port, 1, [f"P-{base_num+8:03d}"], "high"))
    task_num += 1
    
    # gRPC service
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} gRPC service",
        f"From plan:\nImplement gRPC service for Pupitre.{service_name}:\n- Proto file definitions\n- gRPC service implementation\n- Interceptors\n\nEffort: 2 days\nOwner: Backend Engineer\nDependencies: P-{base_num+9:03d}",
        service_name, port, 1, [f"P-{base_num+9:03d}"], "medium"))
    task_num += 1
    
    # Unit tests
    tasks.append(create_task(f"P-{task_num:03d}", f"Write {service_name} unit tests",
        f"From plan:\nWrite comprehensive unit tests for Pupitre.{service_name}:\n- Domain entity tests\n- Command handler tests\n- Query handler tests\n- Service tests\n\nTarget: 90% coverage\n\nEffort: 3 days\nOwner: QA Engineer\nDependencies: P-{base_num+10:03d}",
        service_name, port, 1, [f"P-{base_num+10:03d}"], "high"))
    task_num += 1
    
    # Integration tests
    tasks.append(create_task(f"P-{task_num:03d}", f"Write {service_name} integration tests",
        f"From plan:\nWrite integration tests for Pupitre.{service_name}:\n- Repository tests\n- API endpoint tests\n- Database tests\n\nEffort: 2 days\nOwner: QA Engineer\nDependencies: P-{base_num+11:03d}",
        service_name, port, 1, [f"P-{base_num+11:03d}"], "high"))
    task_num += 1
    
    # Configuration
    tasks.append(create_task(f"P-{task_num:03d}", f"Configure {service_name} appsettings",
        f"From plan:\nConfigure appsettings.json for Pupitre.{service_name}:\n- Connection strings\n- Vault integration\n- Logging\n- Sync intervals\n\nEffort: 1 day\nOwner: DevOps Engineer\nDependencies: P-{base_num+12:03d}",
        service_name, port, 1, [f"P-{base_num+12:03d}"], "medium", "devops"))
    task_num += 1
    
    # Docker
    tasks.append(create_task(f"P-{task_num:03d}", f"Create {service_name} Dockerfile",
        f"From plan:\nCreate Dockerfile and docker-compose for Pupitre.{service_name}:\n- Multi-stage build\n- Health checks\n- Environment configuration\n\nEffort: 1 day\nOwner: DevOps Engineer\nDependencies: P-{base_num+13:03d}",
        service_name, port, 1, [f"P-{base_num+13:03d}"], "medium", "devops"))
    task_num += 1
    
    # Documentation
    tasks.append(create_task(f"P-{task_num:03d}", f"Document {service_name} API",
        f"From plan:\nCreate documentation for Pupitre.{service_name}:\n- API documentation (Swagger)\n- README.md\n- Postman collection\n- .http file\n\nEffort: 1 day\nOwner: Technical Writer\nDependencies: P-{base_num+14:03d}",
        service_name, port, 1, [f"P-{base_num+14:03d}"], "low", "docs"))
    task_num += 1
    
    # Message bus subscriber
    tasks.append(create_task(f"P-{task_num:03d}", f"Configure {service_name} message bus",
        f"From plan:\nConfigure RabbitMQ integration for Pupitre.{service_name}:\n- MessageBusSubscriber\n- Event subscriptions\n- Command routing\n\nEffort: 1 day\nOwner: Backend Engineer\nDependencies: P-{base_num+15:03d}",
        service_name, port, 1, [f"P-{base_num+15:03d}"], "high"))
    task_num += 1
    
    # Performance tests
    tasks.append(create_task(f"P-{task_num:03d}", f"Write {service_name} performance tests",
        f"From plan:\nWrite performance tests for Pupitre.{service_name}:\n- Load tests\n- Stress tests\n- Latency benchmarks\n\nEffort: 2 days\nOwner: QA Engineer\nDependencies: P-{base_num+16:03d}",
        service_name, port, 1, [f"P-{base_num+16:03d}"], "medium"))
    task_num += 1
    
    # Security configuration
    tasks.append(create_task(f"P-{task_num:03d}", f"Configure {service_name} security",
        f"From plan:\nConfigure security for Pupitre.{service_name}:\n- Permission validator\n- Role-based access\n- Field-level encryption\n\nEffort: 2 days\nOwner: Security Engineer\nDependencies: P-{base_num+17:03d}",
        service_name, port, 1, [f"P-{base_num+17:03d}"], "high", "security"))
    task_num += 1
    
    # Metrics
    tasks.append(create_task(f"P-{task_num:03d}", f"Configure {service_name} metrics",
        f"From plan:\nConfigure metrics for Pupitre.{service_name}:\n- Custom metrics middleware\n- Prometheus counters\n- Grafana dashboard\n\nEffort: 1 day\nOwner: DevOps Engineer\nDependencies: P-{base_num+18:03d}",
        service_name, port, 1, [f"P-{base_num+18:03d}"], "medium", "devops"))
    task_num += 1

# Save Phase 1 tasks
for task in tasks[50:]:
    save_task(task)

print(f"Generated {len(tasks)} total tasks (Phase 0 + Phase 1)")

# Save queue.json
queue = {
    "version": "1.0",
    "project": "Pupitre",
    "total_tasks": len(tasks),
    "phases": {
        "0": {"name": "Project Setup", "tasks": [t["plan_task_id"] for t in tasks if t["phase"] == 0]},
        "1": {"name": "Foundation Services", "tasks": [t["plan_task_id"] for t in tasks if t["phase"] == 1]},
    },
    "task_index": {t["plan_task_id"]: {"status": t["status"], "phase": t["phase"]} for t in tasks}
}

with open(os.path.join(OUTPUT_DIR, "queue.json"), 'w') as f:
    json.dump(queue, f, indent=2)

print(f"\nQueue file saved to {OUTPUT_DIR}/queue.json")
print(f"Total tasks generated: {len(tasks)}")

