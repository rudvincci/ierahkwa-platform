#!/usr/bin/env python3
"""Generate Pupitre Agent Task Files - Additional Services (P-1001 to P-1500)"""

import json
import os

OUTPUT_DIR = "/Volumes/Barracuda/mamey-io/code-final/Pupitre/agent-plans"
PLAN_FILE = ".cursor/plans/Pupitre/PupitreTDD.plan.md"

def create_task(task_id, title, description, service, port, phase, dependencies, priority="high", owner="backend"):
    return {
        "plan_task_id": task_id,
        "title": title,
        "description": description,
        "submodule_repo": f"Pupitre.{service}" if service else "Pupitre",
        "submodule_path": f"Pupitre/Pupitre.{service}" if service else "Pupitre",
        "estimated_time": 28800,
        "required_resources": {"cpus": 2, "ram_gb": 8 if "AI" in str(service) else 4},
        "priority": priority,
        "artifacts": [],
        "dependencies": dependencies,
        "owner_plan": owner,
        "phase": phase,
        "status": "pending",
        "plan_file": PLAN_FILE,
        "acceptance_criteria": f"Run the task's commands locally; ensure unit/integration tests pass; create a PR with changes under branch feature/{task_id}-${{AGENT_ID}}; attach artifacts listed in 'artifacts' field.",
        "success_metrics": ["Tests pass locally", "Artifacts collected", "PR opened"]
    }

def save_task(task):
    filepath = os.path.join(OUTPUT_DIR, f"{task['plan_task_id']}.json")
    with open(filepath, 'w') as f:
        json.dump(task, f, indent=2)

all_tasks = []
all_deps = {}

# ============================================================================
# PHASE 7: ADDITIONAL FOUNDATION SERVICES (P-1001 to P-1200)
# 10 services × 20 tasks each = 200 tasks
# ============================================================================

additional_foundation = [
    ("Attendance", "Attendance", 60031, "Daily attendance tracking, tardies, absences, pattern analysis"),
    ("Scheduling", "Schedule", 60032, "Class schedules, school calendar, timetables, room assignments"),
    ("Classrooms", "Classroom", 60033, "Virtual classrooms, live sessions, breakout rooms, screen sharing"),
    ("Collaboration", "StudyGroup", 60034, "Study groups, peer tutoring, group projects, shared workspaces"),
    ("Enrollment", "Enrollment", 60035, "Student registration, transfers, withdrawals, waitlists"),
    ("Schools", "School", 60036, "Multi-tenant school/district management, campus hierarchy"),
    ("Health", "HealthRecord", 60037, "Student health records, nurse visits, immunizations, allergies"),
    ("Incidents", "Incident", 60038, "Behavioral incidents, bullying reports, disciplinary actions"),
    ("SEL", "SELProgress", 60039, "Social-Emotional Learning curriculum, competency tracking"),
    ("Library", "LibraryItem", 60044, "Digital library, e-books, audiobooks, resource checkout"),
]

task_num = 1001
for service_name, entity, port, desc in additional_foundation:
    base_num = task_num
    
    # Dependencies for generating microservice
    gen_deps = ["P-001", "P-003", "P-006", "P-016"]
    if service_name == "Schools":
        gen_deps.append("P-070")  # Depends on Users
    elif service_name == "Enrollment":
        gen_deps.append("P-070")  # Depends on Users
    elif service_name == "Attendance":
        gen_deps.append("P-070")  # Depends on Users
    elif service_name == "Health":
        gen_deps.append("P-070")  # Depends on Users
    
    # Task pattern for each service (20 tasks)
    service_tasks = [
        (f"Generate Pupitre.{service_name} microservice", f"Generate the Pupitre.{service_name} microservice using Mamey.TemplateEngine.\ncd /Volumes/Barracuda/mamey-io/code-final/Pupitre\ndotnet new mamey-microservice --service {service_name} --entity {entity} --port {port}\n\nService: {desc}", gen_deps),
        (f"Implement {service_name} domain entities", f"Implement domain entities for Pupitre.{service_name}:\n- {entity} aggregate root\n- Value objects\n- Domain events\n- Repository interfaces", [f"P-{base_num:04d}"]),
        (f"Implement {service_name} command handlers", f"Implement command handlers for Pupitre.{service_name}:\n- Create{entity}Handler\n- Update{entity}Handler\n- Delete{entity}Handler\n- Custom domain commands", [f"P-{base_num+1:04d}"]),
        (f"Implement {service_name} query handlers", f"Implement query handlers for Pupitre.{service_name}:\n- Get{entity}ByIdHandler\n- Browse{service_name}Handler\n- Search{service_name}Handler", [f"P-{base_num+2:04d}"]),
        (f"Implement {service_name} PostgreSQL repository", f"Implement PostgreSQL persistence for Pupitre.{service_name}:\n- {entity}DbContext\n- {entity}PostgresRepository\n- Entity configurations\n- Migrations", [f"P-{base_num+3:04d}", "P-003"]),
        (f"Implement {service_name} MongoDB repository", f"Implement MongoDB persistence for Pupitre.{service_name}:\n- {entity}Document\n- {entity}MongoRepository\n- Sync service", [f"P-{base_num+4:04d}"]),
        (f"Implement {service_name} Redis caching", f"Implement Redis caching for Pupitre.{service_name}:\n- {entity}RedisRepository\n- Cache invalidation\n- Sync service", [f"P-{base_num+5:04d}"]),
        (f"Implement {service_name} composite repository", f"Implement composite repository pattern for Pupitre.{service_name}:\n- Composite{entity}Repository\n- Fallback strategy (Redis → Mongo → Postgres)", [f"P-{base_num+6:04d}"]),
        (f"Implement {service_name} API routes", f"Implement REST API routes for Pupitre.{service_name}:\n- CRUD endpoints\n- Search and filter endpoints\n- Pagination\n- Authentication", [f"P-{base_num+7:04d}"]),
        (f"Implement {service_name} event handlers", f"Implement domain event handlers for Pupitre.{service_name}:\n- {entity}CreatedHandler\n- {entity}UpdatedHandler\n- Integration event publishing", [f"P-{base_num+8:04d}", "P-014"]),
        (f"Implement {service_name} gRPC service", f"Implement gRPC service for Pupitre.{service_name}:\n- Proto file definitions\n- gRPC service implementation\n- Interceptors", [f"P-{base_num+9:04d}"]),
        (f"Write {service_name} unit tests", f"Write comprehensive unit tests for Pupitre.{service_name}:\n- Domain entity tests\n- Command handler tests\n- Query handler tests\n- Service tests\n\nTarget: 90% coverage", [f"P-{base_num+10:04d}", "P-019"]),
        (f"Write {service_name} integration tests", f"Write integration tests for Pupitre.{service_name}:\n- Repository tests\n- API endpoint tests\n- Database tests", [f"P-{base_num+11:04d}"]),
        (f"Configure {service_name} appsettings", f"Configure appsettings.json for Pupitre.{service_name}:\n- Connection strings\n- Vault integration\n- Logging\n- Sync intervals", [f"P-{base_num+12:04d}", "P-005"]),
        (f"Create {service_name} Dockerfile", f"Create Dockerfile and docker-compose for Pupitre.{service_name}:\n- Multi-stage build\n- Health checks\n- Environment configuration", [f"P-{base_num+13:04d}", "P-003"]),
        (f"Document {service_name} API", f"Create documentation for Pupitre.{service_name}:\n- API documentation (Swagger)\n- README.md\n- Postman collection\n- .http file", [f"P-{base_num+14:04d}"]),
        (f"Configure {service_name} message bus", f"Configure RabbitMQ integration for Pupitre.{service_name}:\n- MessageBusSubscriber\n- Event subscriptions\n- Command routing", [f"P-{base_num+15:04d}", "P-014"]),
        (f"Write {service_name} performance tests", f"Write performance tests for Pupitre.{service_name}:\n- Load tests\n- Stress tests\n- Latency benchmarks", [f"P-{base_num+16:04d}", "P-020"]),
        (f"Configure {service_name} security", f"Configure security for Pupitre.{service_name}:\n- Permission validator\n- Role-based access\n- Field-level encryption", [f"P-{base_num+17:04d}", "P-005"]),
        (f"Configure {service_name} metrics", f"Configure metrics for Pupitre.{service_name}:\n- Custom metrics middleware\n- Prometheus counters\n- Grafana dashboard", [f"P-{base_num+18:04d}", "P-007"]),
    ]
    
    for offset, (title, detail, deps) in enumerate(service_tasks):
        tid = f"P-{base_num + offset:04d}"
        owner = "devops" if "Docker" in title or "appsettings" in title else ("security" if "security" in title.lower() else ("docs" if "Document" in title else "backend"))
        task = create_task(tid, title, f"From plan:\n{detail}\n\nEffort: 2 days\nOwner: Backend Engineer\nDependencies: {', '.join(deps)}", service_name, port, 7, deps, "high", owner)
        all_tasks.append(task)
        all_deps[tid] = deps
    
    task_num += 20

print(f"Generated {len(all_tasks)} Phase 7 (Additional Foundation) tasks")

# ============================================================================
# PHASE 8: ADDITIONAL AI & SPECIALIZED SERVICES (P-1201 to P-1450)
# ============================================================================

additional_ai_services = [
    ("AIHomework", "HomeworkHelp", 60040, "AI homework helper with step-by-step guidance and explanations"),
    ("AIStudyPlanner", "StudyPlan", 60041, "AI-powered study schedule optimization and spaced repetition"),
    ("AIParentInsights", "ParentInsight", 60042, "AI-generated insights and recommendations for parents"),
]

task_num = 1201
for service_name, entity, port, desc in additional_ai_services:
    base_num = task_num
    
    # AI service dependencies
    gen_deps = ["P-001", "P-003", "P-006", "P-011", "P-012"]
    
    service_tasks = [
        (f"Generate Pupitre.{service_name} microservice", f"Generate the Pupitre.{service_name} AI microservice using Mamey.TemplateEngine.\ncd /Volumes/Barracuda/mamey-io/code-final/Pupitre\ndotnet new mamey-microservice --service {service_name} --entity {entity} --port {port}\n\nService: {desc}", gen_deps),
        (f"Implement {service_name} domain entities", f"Implement domain entities for Pupitre.{service_name}:\n- {entity} aggregate root\n- AI-specific value objects\n- Domain events", [f"P-{base_num:04d}"]),
        (f"Implement {service_name} AI orchestration", f"Implement AI orchestration for Pupitre.{service_name}:\n- Semantic Kernel integration\n- LLM routing (LiteLLM)\n- Prompt templates\n- Agent configuration", [f"P-{base_num+1:04d}", "P-011"]),
        (f"Implement {service_name} vector storage", f"Implement Qdrant vector storage for Pupitre.{service_name}:\n- Embedding generation\n- Vector collections\n- Semantic search\n- RAG pipeline", [f"P-{base_num+2:04d}", "P-012"]),
        (f"Implement {service_name} command handlers", f"Implement AI-specific command handlers for Pupitre.{service_name}:\n- StartSession, ProcessInput, EndSession\n- AI-specific domain commands", [f"P-{base_num+3:04d}"]),
        (f"Implement {service_name} query handlers", f"Implement query handlers for Pupitre.{service_name}:\n- GetSession, GetHistory, GetRecommendations\n- AI inference queries", [f"P-{base_num+4:04d}"]),
        (f"Implement {service_name} PostgreSQL repository", f"Implement PostgreSQL persistence for Pupitre.{service_name}:\n- Session storage\n- Configuration storage\n- Audit logging", [f"P-{base_num+5:04d}", "P-003"]),
        (f"Implement {service_name} MongoDB repository", f"Implement MongoDB persistence for Pupitre.{service_name}:\n- Conversation history\n- AI response caching\n- Embeddings metadata", [f"P-{base_num+6:04d}"]),
        (f"Implement {service_name} Redis caching", f"Implement Redis caching for Pupitre.{service_name}:\n- Session context caching\n- Response caching\n- Rate limiting counters", [f"P-{base_num+7:04d}"]),
        (f"Implement {service_name} API routes", f"Implement REST API routes for Pupitre.{service_name}:\n- Session endpoints\n- Inference endpoints\n- Streaming endpoints", [f"P-{base_num+8:04d}"]),
        (f"Implement {service_name} streaming", f"Implement streaming support for Pupitre.{service_name}:\n- Server-Sent Events\n- WebSocket integration\n- Token streaming", [f"P-{base_num+9:04d}"]),
        (f"Implement {service_name} safety filters", f"Implement safety filters for Pupitre.{service_name}:\n- Content moderation\n- Guardrails integration\n- Age-appropriate filtering", [f"P-{base_num+10:04d}"]),
        (f"Write {service_name} unit tests", f"Write comprehensive unit tests for Pupitre.{service_name}:\n- AI orchestration tests\n- Handler tests\n- Safety filter tests\n\nTarget: 85% coverage", [f"P-{base_num+11:04d}", "P-019"]),
        (f"Write {service_name} integration tests", f"Write integration tests for Pupitre.{service_name}:\n- LLM integration tests\n- Vector DB tests\n- End-to-end AI flow tests", [f"P-{base_num+12:04d}"]),
        (f"Write {service_name} AI quality tests", f"Write AI quality evaluation tests for Pupitre.{service_name}:\n- Response accuracy tests\n- Prompt regression tests\n- Bias detection tests", [f"P-{base_num+13:04d}"]),
        (f"Configure {service_name} appsettings", f"Configure appsettings.json for Pupitre.{service_name}:\n- LLM provider settings\n- Vector DB connection\n- Rate limiting", [f"P-{base_num+14:04d}", "P-005"]),
        (f"Create {service_name} Dockerfile", f"Create Dockerfile for Pupitre.{service_name}:\n- GPU support (optional)\n- Model caching\n- Health checks", [f"P-{base_num+15:04d}", "P-003"]),
        (f"Configure {service_name} AI metrics", f"Configure AI-specific metrics for Pupitre.{service_name}:\n- Inference latency\n- Token usage\n- Model accuracy", [f"P-{base_num+16:04d}", "P-007"]),
        (f"Document {service_name} API", f"Create documentation for Pupitre.{service_name}:\n- API documentation\n- AI capabilities\n- Integration guide", [f"P-{base_num+17:04d}"]),
        (f"Implement {service_name} model management", f"Implement model management for Pupitre.{service_name}:\n- Model versioning\n- A/B testing\n- Sovereign model support", [f"P-{base_num+18:04d}"]),
        (f"Configure {service_name} message bus", f"Configure Kafka/RabbitMQ for Pupitre.{service_name}:\n- AI inference queue\n- Event streaming", [f"P-{base_num+19:04d}", "P-013", "P-014"]),
        (f"Configure {service_name} security", f"Configure security for Pupitre.{service_name}:\n- API key management\n- Rate limiting\n- PII protection", [f"P-{base_num+20:04d}", "P-005"]),
    ]
    
    for offset, (title, detail, deps) in enumerate(service_tasks):
        tid = f"P-{base_num + offset:04d}"
        owner = "ai" if "AI" in title or "orchestration" in title.lower() or "vector" in title.lower() else ("devops" if "Docker" in title else "backend")
        task = create_task(tid, title, f"From plan:\n{detail}\n\nEffort: 3 days\nOwner: AI Engineer\nDependencies: {', '.join(deps)}", service_name, port, 8, deps, "high", owner)
        all_tasks.append(task)
        all_deps[tid] = deps
    
    task_num += 21

# Additional specialized services
specialized_services = [
    ("STEMLabs", "LabSession", 60043, "Virtual science labs, coding sandboxes, simulations"),
    ("Events", "Event", 60045, "Field trips, school events, permissions, chaperones"),
    ("Transportation", "Route", 60046, "School bus routes, GPS tracking, pickup/dropoff"),
    ("Extracurriculars", "Activity", 60047, "Clubs, sports teams, activities tracking"),
    ("CareerGuidance", "CareerPath", 60048, "Career exploration, college prep, internships"),
    ("Billing", "Invoice", 60049, "Tuition, fees, payment processing, financial aid"),
    ("Communications", "Announcement", 60050, "Announcements, newsletters, emergency alerts, broadcast"),
]

for service_name, entity, port, desc in specialized_services:
    base_num = task_num
    
    gen_deps = ["P-001", "P-003", "P-006", "P-016"]
    if service_name == "Billing":
        gen_deps.append("P-070")  # Users
    elif service_name == "Transportation":
        gen_deps.append("P-070")  # Users
        gen_deps.append("P-1061")  # Schools
    elif service_name == "Events":
        gen_deps.append("P-1061")  # Schools
    elif service_name == "Extracurriculars":
        gen_deps.append("P-070")  # Users
        gen_deps.append("P-1061")  # Schools
    
    service_tasks = [
        (f"Generate Pupitre.{service_name} microservice", f"Generate the Pupitre.{service_name} microservice using Mamey.TemplateEngine.\ncd /Volumes/Barracuda/mamey-io/code-final/Pupitre\ndotnet new mamey-microservice --service {service_name} --entity {entity} --port {port}\n\nService: {desc}", gen_deps),
        (f"Implement {service_name} domain entities", f"Implement domain entities for Pupitre.{service_name}:\n- {entity} aggregate root\n- Value objects\n- Domain events\n- Repository interfaces", [f"P-{base_num:04d}"]),
        (f"Implement {service_name} command handlers", f"Implement command handlers for Pupitre.{service_name}:\n- Create{entity}Handler\n- Update{entity}Handler\n- Delete{entity}Handler", [f"P-{base_num+1:04d}"]),
        (f"Implement {service_name} query handlers", f"Implement query handlers for Pupitre.{service_name}:\n- Get{entity}ByIdHandler\n- Browse{service_name}Handler\n- Search{service_name}Handler", [f"P-{base_num+2:04d}"]),
        (f"Implement {service_name} PostgreSQL repository", f"Implement PostgreSQL persistence for Pupitre.{service_name}:\n- {entity}DbContext\n- {entity}PostgresRepository\n- Migrations", [f"P-{base_num+3:04d}", "P-003"]),
        (f"Implement {service_name} MongoDB repository", f"Implement MongoDB persistence for Pupitre.{service_name}:\n- {entity}Document\n- {entity}MongoRepository", [f"P-{base_num+4:04d}"]),
        (f"Implement {service_name} Redis caching", f"Implement Redis caching for Pupitre.{service_name}:\n- {entity}RedisRepository\n- Cache invalidation", [f"P-{base_num+5:04d}"]),
        (f"Implement {service_name} composite repository", f"Implement composite repository for Pupitre.{service_name}", [f"P-{base_num+6:04d}"]),
        (f"Implement {service_name} API routes", f"Implement REST API routes for Pupitre.{service_name}:\n- CRUD endpoints\n- Search endpoints\n- Pagination", [f"P-{base_num+7:04d}"]),
        (f"Implement {service_name} event handlers", f"Implement event handlers for Pupitre.{service_name}:\n- Domain event handlers\n- Integration events", [f"P-{base_num+8:04d}", "P-014"]),
        (f"Implement {service_name} gRPC service", f"Implement gRPC service for Pupitre.{service_name}", [f"P-{base_num+9:04d}"]),
        (f"Write {service_name} unit tests", f"Write unit tests for Pupitre.{service_name}\nTarget: 90% coverage", [f"P-{base_num+10:04d}", "P-019"]),
        (f"Write {service_name} integration tests", f"Write integration tests for Pupitre.{service_name}", [f"P-{base_num+11:04d}"]),
        (f"Configure {service_name} appsettings", f"Configure appsettings for Pupitre.{service_name}", [f"P-{base_num+12:04d}", "P-005"]),
        (f"Create {service_name} Dockerfile", f"Create Dockerfile for Pupitre.{service_name}", [f"P-{base_num+13:04d}", "P-003"]),
        (f"Document {service_name} API", f"Document API for Pupitre.{service_name}", [f"P-{base_num+14:04d}"]),
        (f"Configure {service_name} message bus", f"Configure message bus for Pupitre.{service_name}", [f"P-{base_num+15:04d}", "P-014"]),
        (f"Write {service_name} performance tests", f"Write performance tests for Pupitre.{service_name}", [f"P-{base_num+16:04d}", "P-020"]),
        (f"Configure {service_name} security", f"Configure security for Pupitre.{service_name}", [f"P-{base_num+17:04d}", "P-005"]),
        (f"Configure {service_name} metrics", f"Configure metrics for Pupitre.{service_name}", [f"P-{base_num+18:04d}", "P-007"]),
    ]
    
    for offset, (title, detail, deps) in enumerate(service_tasks):
        tid = f"P-{base_num + offset:04d}"
        owner = "devops" if "Docker" in title or "appsettings" in title else ("docs" if "Document" in title else "backend")
        task = create_task(tid, title, f"From plan:\n{detail}\n\nEffort: 2 days\nOwner: Backend Engineer\nDependencies: {', '.join(deps)}", service_name, port, 8, deps, "high", owner)
        all_tasks.append(task)
        all_deps[tid] = deps
    
    task_num += 19

print(f"Generated {len(all_tasks)} Phase 7+8 tasks so far")

# ============================================================================
# PHASE 9: FRONTEND FOR NEW SERVICES (P-1401 to P-1470)
# ============================================================================

# BlazorWasm for new services
new_blazor_services = [
    ("Attendance", "P-1020", 1401),
    ("Scheduling", "P-1040", 1408),
    ("Classrooms", "P-1060", 1415),
    ("Enrollment", "P-1100", 1422),
    ("Schools", "P-1120", 1429),
    ("Health", "P-1140", 1436),
    ("Library", "P-1200", 1443),
    ("STEMLabs", "P-1283", 1450),
    ("Events", "P-1302", 1457),
    ("Communications", "P-1378", 1464),
]

for service_name, backend_dep, base_num in new_blazor_services:
    for offset in range(7):
        tid = f"P-{base_num + offset:04d}"
        if offset == 0:
            deps = [backend_dep, "P-008"]
            title = f"Create {service_name} BlazorWasm RouteService"
        elif offset == 1:
            deps = [f"P-{base_num:04d}"]
            title = f"Create {service_name} BlazorWasm API Client"
        elif offset == 2:
            deps = [f"P-{base_num + 1:04d}"]
            title = f"Create {service_name} BlazorWasm Index page"
        elif offset == 3:
            deps = [f"P-{base_num + 2:04d}"]
            title = f"Create {service_name} BlazorWasm Details page"
        elif offset == 4:
            deps = [f"P-{base_num + 3:04d}"]
            title = f"Create {service_name} BlazorWasm Create form"
        elif offset == 5:
            deps = [f"P-{base_num + 4:04d}"]
            title = f"Create {service_name} BlazorWasm Edit form"
        else:
            deps = [f"P-{base_num + 5:04d}"]
            title = f"Create {service_name} BlazorWasm shared components"
        
        task = create_task(tid, title, f"From plan:\n{title} for Pupitre.{service_name}.BlazorWasm\n\nEffort: 2 days\nOwner: Frontend Engineer\nDependencies: {', '.join(deps)}", f"{service_name}.BlazorWasm", None, 9, deps, "high", "frontend")
        all_tasks.append(task)
        all_deps[tid] = deps

print(f"Generated {len(all_tasks)} tasks including Phase 9 frontend")

# ============================================================================
# PHASE 10: INTEGRATION OF NEW SERVICES (P-1471 to P-1500)
# ============================================================================

integration_tasks = [
    ("P-1471", "Integrate Attendance with Analytics", ["P-1020", "P-250"], "Cross-service attendance analytics"),
    ("P-1472", "Integrate Scheduling with Classrooms", ["P-1040", "P-1060"], "Schedule-classroom integration"),
    ("P-1473", "Integrate Enrollment with Schools", ["P-1100", "P-1120"], "Enrollment-school integration"),
    ("P-1474", "Integrate Health with Parents portal", ["P-1140", "P-520"], "Health records for parents"),
    ("P-1475", "Integrate Library with Lessons", ["P-1200", "P-130"], "Library resources in lessons"),
    ("P-1476", "Integrate STEMLabs with Curricula", ["P-1283", "P-110"], "STEM labs curriculum mapping"),
    ("P-1477", "Integrate Events with Notifications", ["P-1302", "P-210"], "Event notifications"),
    ("P-1478", "Integrate Transportation with Parents", ["P-1340", "P-520"], "Bus tracking for parents"),
    ("P-1479", "Integrate Billing with Parents portal", ["P-1359", "P-520"], "Billing in parent portal"),
    ("P-1480", "Integrate Communications with all portals", ["P-1378", "P-792", "P-793", "P-794"], "System-wide communications"),
    ("P-1481", "Integrate AIHomework with Lessons", ["P-1221", "P-130"], "Homework help for lessons"),
    ("P-1482", "Integrate AIStudyPlanner with Curricula", ["P-1242", "P-110"], "AI study planning"),
    ("P-1483", "Integrate AIParentInsights with Analytics", ["P-1263", "P-250"], "AI insights for parents"),
    ("P-1484", "Integrate SEL with AIBehavior", ["P-1180", "P-400"], "SEL behavior integration"),
    ("P-1485", "Integrate Incidents with Compliance", ["P-1160", "P-640"], "Incident compliance tracking"),
    ("P-1486", "Integrate CareerGuidance with Credentials", ["P-1321", "P-230"], "Career credentials"),
    ("P-1487", "Integrate Extracurriculars with Rewards", ["P-1340", "P-190"], "Activity rewards"),
    ("P-1488", "Create unified student dashboard", ["P-1471", "P-1472", "P-1473"], "Student dashboard integration"),
    ("P-1489", "Create unified parent dashboard", ["P-1474", "P-1478", "P-1479"], "Parent dashboard integration"),
    ("P-1490", "Create unified educator dashboard", ["P-1475", "P-1476", "P-1477"], "Educator dashboard integration"),
    ("P-1491", "Write cross-service integration tests", ["P-1488", "P-1489", "P-1490"], "Integration testing"),
    ("P-1492", "Performance test new services", ["P-1491", "P-020"], "Performance validation"),
    ("P-1493", "Security audit new services", ["P-1492", "P-005"], "Security audit"),
    ("P-1494", "Update API Gateway routes", ["P-1493", "P-008"], "Gateway routing update"),
    ("P-1495", "Update Consul service discovery", ["P-1494", "P-010"], "Service discovery update"),
    ("P-1496", "Create Helm charts for new services", ["P-1495", "P-021"], "Helm chart creation"),
    ("P-1497", "Update monitoring dashboards", ["P-1496", "P-007"], "Dashboard updates"),
    ("P-1498", "Update documentation", ["P-1497"], "Documentation update"),
    ("P-1499", "Deploy new services to staging", ["P-1498", "P-004"], "Staging deployment"),
    ("P-1500", "Final validation of new services", ["P-1499"], "Final validation"),
]

for tid, title, deps, detail in integration_tasks:
    task = create_task(tid, title, f"From plan:\n{detail}\n\nEffort: 2 days\nOwner: Backend Engineer\nDependencies: {', '.join(deps)}", None, None, 10, deps, "high", "backend")
    all_tasks.append(task)
    all_deps[tid] = deps

print(f"Generated {len(all_tasks)} total new tasks")

# Save all tasks
for task in all_tasks:
    save_task(task)

print(f"Saved {len(all_tasks)} task files")

# Update queue.json
queue_path = os.path.join(OUTPUT_DIR, "queue.json")
with open(queue_path, 'r') as f:
    queue = json.load(f)

# Add new phases
queue["phases"]["7"] = {"name": "Additional Foundation Services", "tasks": [t["plan_task_id"] for t in all_tasks if t["phase"] == 7]}
queue["phases"]["8"] = {"name": "Additional AI & Specialized Services", "tasks": [t["plan_task_id"] for t in all_tasks if t["phase"] == 8]}
queue["phases"]["9"] = {"name": "Frontend for New Services", "tasks": [t["plan_task_id"] for t in all_tasks if t["phase"] == 9]}
queue["phases"]["10"] = {"name": "Integration of New Services", "tasks": [t["plan_task_id"] for t in all_tasks if t["phase"] == 10]}

# Update task index and dependency graph
for t in all_tasks:
    queue["task_index"][t["plan_task_id"]] = {"status": t["status"], "phase": t["phase"]}
    
queue["dependency_graph"].update(all_deps)
queue["total_tasks"] = len(queue["task_index"])

with open(queue_path, 'w') as f:
    json.dump(queue, f, indent=2)

print(f"\nUpdated queue.json")
print(f"Total tasks in queue: {queue['total_tasks']}")














