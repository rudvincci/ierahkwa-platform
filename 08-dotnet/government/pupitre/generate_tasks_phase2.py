#!/usr/bin/env python3
"""Generate Pupitre Agent Task Files - Phase 2: AI Services"""

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

tasks = []

# ============================================================================
# PHASE 2: AI SERVICES (P-251 to P-500)
# ============================================================================

ai_services = [
    ("AITutors", "AITutor", 60011, "Conversational AI tutoring engine"),
    ("AIAssessments", "AIAssessment", 60012, "AI-powered adaptive testing and grading"),
    ("AIContent", "AIContent", 60013, "AI content generation and curation"),
    ("AISpeech", "AISpeech", 60014, "Speech-to-text and text-to-speech services"),
    ("AIAdaptive", "AdaptivePath", 60015, "Learning path optimization and adaptation"),
    ("AIBehavior", "BehaviorAnalysis", 60016, "Emotional and behavioral analysis"),
    ("AISafety", "SafetyCheck", 60017, "Content moderation and child safety"),
    ("AIRecommendations", "Recommendation", 60018, "Content and activity recommendations"),
    ("AITranslation", "Translation", 60019, "Multilingual support and translation"),
    ("AIVision", "VisionAnalysis", 60020, "Visual content analysis"),
]

task_num = 251
for service_name, entity, port, desc in ai_services:
    base_num = task_num
    
    # Generate microservice
    tasks.append(create_task(f"P-{task_num:03d}", f"Generate Pupitre.{service_name} microservice",
        f"From plan:\nGenerate the Pupitre.{service_name} AI microservice using Mamey.TemplateEngine.\ncd /Volumes/Barracuda/mamey-io/code-final/Pupitre\ndotnet new mamey-microservice --service {service_name} --entity {entity} --port {port}\n\nService: {desc}\n\nEffort: 1 day\nOwner: AI Engineer\nDependencies: P-011",
        service_name, port, 2, ["P-011"], "high", "ai"))
    task_num += 1
    
    # Domain layer
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} domain entities",
        f"From plan:\nImplement domain entities for Pupitre.{service_name}:\n- {entity} aggregate root\n- AI-specific value objects\n- Domain events\n- Repository interfaces\n\nEffort: 3 days\nOwner: AI Engineer\nDependencies: P-{base_num:03d}",
        service_name, port, 2, [f"P-{base_num:03d}"], "high", "ai"))
    task_num += 1
    
    # AI orchestration layer
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} AI orchestration",
        f"From plan:\nImplement AI orchestration for Pupitre.{service_name}:\n- Semantic Kernel integration\n- LLM routing (LiteLLM)\n- Prompt templates\n- Agent configuration\n\nEffort: 5 days\nOwner: AI Engineer\nDependencies: P-{base_num+1:03d}",
        service_name, port, 2, [f"P-{base_num+1:03d}"], "high", "ai"))
    task_num += 1
    
    # Vector database integration
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} vector storage",
        f"From plan:\nImplement Qdrant vector storage for Pupitre.{service_name}:\n- Embedding generation\n- Vector collections\n- Semantic search\n- RAG pipeline\n\nEffort: 3 days\nOwner: AI Engineer\nDependencies: P-{base_num+2:03d}",
        service_name, port, 2, [f"P-{base_num+2:03d}"], "high", "ai"))
    task_num += 1
    
    # Command handlers
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} command handlers",
        f"From plan:\nImplement AI-specific command handlers for Pupitre.{service_name}:\n- StartSession, ProcessInput, EndSession\n- AI-specific domain commands\n\nEffort: 4 days\nOwner: AI Engineer\nDependencies: P-{base_num+3:03d}",
        service_name, port, 2, [f"P-{base_num+3:03d}"], "high", "ai"))
    task_num += 1
    
    # Query handlers
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} query handlers",
        f"From plan:\nImplement query handlers for Pupitre.{service_name}:\n- GetSession, GetHistory, GetRecommendations\n- AI inference queries\n\nEffort: 3 days\nOwner: AI Engineer\nDependencies: P-{base_num+4:03d}",
        service_name, port, 2, [f"P-{base_num+4:03d}"], "high", "ai"))
    task_num += 1
    
    # PostgreSQL repository
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} PostgreSQL repository",
        f"From plan:\nImplement PostgreSQL persistence for Pupitre.{service_name}:\n- Session storage\n- Configuration storage\n- Audit logging\n\nEffort: 2 days\nOwner: Backend Engineer\nDependencies: P-{base_num+5:03d}",
        service_name, port, 2, [f"P-{base_num+5:03d}"], "high"))
    task_num += 1
    
    # MongoDB repository
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} MongoDB repository",
        f"From plan:\nImplement MongoDB persistence for Pupitre.{service_name}:\n- Conversation history\n- AI response caching\n- Embeddings metadata\n\nEffort: 2 days\nOwner: Backend Engineer\nDependencies: P-{base_num+6:03d}",
        service_name, port, 2, [f"P-{base_num+6:03d}"], "high"))
    task_num += 1
    
    # Redis caching
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} Redis caching",
        f"From plan:\nImplement Redis caching for Pupitre.{service_name}:\n- Session context caching\n- Response caching\n- Rate limiting counters\n\nEffort: 2 days\nOwner: Backend Engineer\nDependencies: P-{base_num+7:03d}",
        service_name, port, 2, [f"P-{base_num+7:03d}"], "high"))
    task_num += 1
    
    # API routes
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} API routes",
        f"From plan:\nImplement REST API routes for Pupitre.{service_name}:\n- Session endpoints\n- Inference endpoints\n- Streaming endpoints\n\nEffort: 3 days\nOwner: Backend Engineer\nDependencies: P-{base_num+8:03d}",
        service_name, port, 2, [f"P-{base_num+8:03d}"], "high"))
    task_num += 1
    
    # Streaming support
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} streaming",
        f"From plan:\nImplement streaming support for Pupitre.{service_name}:\n- Server-Sent Events\n- WebSocket integration\n- Token streaming\n\nEffort: 2 days\nOwner: Backend Engineer\nDependencies: P-{base_num+9:03d}",
        service_name, port, 2, [f"P-{base_num+9:03d}"], "high"))
    task_num += 1
    
    # Safety filters
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} safety filters",
        f"From plan:\nImplement safety filters for Pupitre.{service_name}:\n- Content moderation\n- Guardrails integration\n- Hallucination detection\n- Age-appropriate filtering\n\nEffort: 3 days\nOwner: AI Engineer\nDependencies: P-{base_num+10:03d}",
        service_name, port, 2, [f"P-{base_num+10:03d}"], "high", "ai"))
    task_num += 1
    
    # Unit tests
    tasks.append(create_task(f"P-{task_num:03d}", f"Write {service_name} unit tests",
        f"From plan:\nWrite comprehensive unit tests for Pupitre.{service_name}:\n- AI orchestration tests\n- Handler tests\n- Safety filter tests\n\nTarget: 85% coverage\n\nEffort: 4 days\nOwner: QA Engineer\nDependencies: P-{base_num+11:03d}",
        service_name, port, 2, [f"P-{base_num+11:03d}"], "high"))
    task_num += 1
    
    # Integration tests
    tasks.append(create_task(f"P-{task_num:03d}", f"Write {service_name} integration tests",
        f"From plan:\nWrite integration tests for Pupitre.{service_name}:\n- LLM integration tests\n- Vector DB tests\n- End-to-end AI flow tests\n\nEffort: 3 days\nOwner: QA Engineer\nDependencies: P-{base_num+12:03d}",
        service_name, port, 2, [f"P-{base_num+12:03d}"], "high"))
    task_num += 1
    
    # AI quality tests
    tasks.append(create_task(f"P-{task_num:03d}", f"Write {service_name} AI quality tests",
        f"From plan:\nWrite AI quality evaluation tests for Pupitre.{service_name}:\n- Response accuracy tests\n- Prompt regression tests\n- Bias detection tests\n- Latency benchmarks\n\nEffort: 4 days\nOwner: AI Engineer\nDependencies: P-{base_num+13:03d}",
        service_name, port, 2, [f"P-{base_num+13:03d}"], "high", "ai"))
    task_num += 1
    
    # Configuration
    tasks.append(create_task(f"P-{task_num:03d}", f"Configure {service_name} appsettings",
        f"From plan:\nConfigure appsettings.json for Pupitre.{service_name}:\n- LLM provider settings\n- Vector DB connection\n- Rate limiting\n- Model parameters\n\nEffort: 1 day\nOwner: DevOps Engineer\nDependencies: P-{base_num+14:03d}",
        service_name, port, 2, [f"P-{base_num+14:03d}"], "medium", "devops"))
    task_num += 1
    
    # Dockerfile
    tasks.append(create_task(f"P-{task_num:03d}", f"Create {service_name} Dockerfile",
        f"From plan:\nCreate Dockerfile for Pupitre.{service_name}:\n- GPU support (optional)\n- Model caching\n- Health checks\n\nEffort: 1 day\nOwner: DevOps Engineer\nDependencies: P-{base_num+15:03d}",
        service_name, port, 2, [f"P-{base_num+15:03d}"], "medium", "devops"))
    task_num += 1
    
    # Metrics
    tasks.append(create_task(f"P-{task_num:03d}", f"Configure {service_name} AI metrics",
        f"From plan:\nConfigure AI-specific metrics for Pupitre.{service_name}:\n- Inference latency\n- Token usage\n- Model accuracy\n- Safety filter triggers\n\nEffort: 2 days\nOwner: AI Engineer\nDependencies: P-{base_num+16:03d}",
        service_name, port, 2, [f"P-{base_num+16:03d}"], "medium", "ai"))
    task_num += 1
    
    # Documentation
    tasks.append(create_task(f"P-{task_num:03d}", f"Document {service_name} API",
        f"From plan:\nCreate documentation for Pupitre.{service_name}:\n- API documentation\n- AI capabilities\n- Prompt templates\n- Integration guide\n\nEffort: 2 days\nOwner: Technical Writer\nDependencies: P-{base_num+17:03d}",
        service_name, port, 2, [f"P-{base_num+17:03d}"], "low", "docs"))
    task_num += 1
    
    # Model management
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} model management",
        f"From plan:\nImplement model management for Pupitre.{service_name}:\n- Model versioning\n- A/B testing\n- Fallback strategies\n- Sovereign model support (Ollama)\n\nEffort: 3 days\nOwner: AI Engineer\nDependencies: P-{base_num+18:03d}",
        service_name, port, 2, [f"P-{base_num+18:03d}"], "high", "ai"))
    task_num += 1
    
    # Event handlers
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} event handlers",
        f"From plan:\nImplement event handlers for Pupitre.{service_name}:\n- Cross-service AI events\n- Analytics events\n- Audit events\n\nEffort: 2 days\nOwner: Backend Engineer\nDependencies: P-{base_num+19:03d}",
        service_name, port, 2, [f"P-{base_num+19:03d}"], "high"))
    task_num += 1
    
    # Message bus
    tasks.append(create_task(f"P-{task_num:03d}", f"Configure {service_name} message bus",
        f"From plan:\nConfigure Kafka/RabbitMQ for Pupitre.{service_name}:\n- AI inference queue\n- Event streaming\n- Dead letter handling\n\nEffort: 2 days\nOwner: Backend Engineer\nDependencies: P-{base_num+20:03d}",
        service_name, port, 2, [f"P-{base_num+20:03d}"], "high"))
    task_num += 1
    
    # Performance tests
    tasks.append(create_task(f"P-{task_num:03d}", f"Write {service_name} performance tests",
        f"From plan:\nWrite performance tests for Pupitre.{service_name}:\n- Throughput tests\n- Concurrent user tests\n- Memory usage tests\n\nEffort: 2 days\nOwner: QA Engineer\nDependencies: P-{base_num+21:03d}",
        service_name, port, 2, [f"P-{base_num+21:03d}"], "medium"))
    task_num += 1
    
    # Security
    tasks.append(create_task(f"P-{task_num:03d}", f"Configure {service_name} security",
        f"From plan:\nConfigure security for Pupitre.{service_name}:\n- API key management\n- Rate limiting\n- Input sanitization\n- PII protection\n\nEffort: 2 days\nOwner: Security Engineer\nDependencies: P-{base_num+22:03d}",
        service_name, port, 2, [f"P-{base_num+22:03d}"], "high", "security"))
    task_num += 1
    
    # gRPC service
    tasks.append(create_task(f"P-{task_num:03d}", f"Implement {service_name} gRPC service",
        f"From plan:\nImplement gRPC service for Pupitre.{service_name}:\n- Proto definitions\n- Streaming support\n- Service-to-service calls\n\nEffort: 2 days\nOwner: Backend Engineer\nDependencies: P-{base_num+23:03d}",
        service_name, port, 2, [f"P-{base_num+23:03d}"], "medium"))
    task_num += 1

# Save all Phase 2 tasks
for task in tasks:
    save_task(task)

print(f"Generated {len(tasks)} Phase 2 AI Service tasks")

# Update queue.json with Phase 2
queue_path = os.path.join(OUTPUT_DIR, "queue.json")
with open(queue_path, 'r') as f:
    queue = json.load(f)

queue["phases"]["2"] = {"name": "AI Services", "tasks": [t["plan_task_id"] for t in tasks]}
for t in tasks:
    queue["task_index"][t["plan_task_id"]] = {"status": t["status"], "phase": t["phase"]}
queue["total_tasks"] = len(queue["task_index"])

with open(queue_path, 'w') as f:
    json.dump(queue, f, indent=2)

print(f"Updated queue.json with {len(tasks)} Phase 2 tasks")














