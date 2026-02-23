#!/usr/bin/env python3
"""Fix dependencies for all Pupitre Agent Task Files"""

import json
import os
import glob

OUTPUT_DIR = "/Volumes/Barracuda/mamey-io/code-final/Pupitre/agent-plans"

def load_task(task_id):
    filepath = os.path.join(OUTPUT_DIR, f"{task_id}.json")
    with open(filepath, 'r') as f:
        return json.load(f)

def save_task(task):
    filepath = os.path.join(OUTPUT_DIR, f"{task['plan_task_id']}.json")
    with open(filepath, 'w') as f:
        json.dump(task, f, indent=2)

# ============================================================================
# PHASE 0: PROJECT SETUP (P-001 to P-050)
# ============================================================================
phase0_deps = {
    "P-001": [],  # Root - no dependencies
    "P-002": ["P-001"],  # CI/CD needs repo
    "P-003": ["P-001"],  # Docker Compose needs repo
    "P-004": ["P-003"],  # K8s needs Docker
    "P-005": ["P-004"],  # Vault needs K8s
    "P-006": ["P-001"],  # Contracts needs repo
    "P-007": ["P-004"],  # Observability needs K8s
    "P-008": ["P-001", "P-006"],  # API Gateway needs repo and contracts
    "P-009": ["P-008"],  # Gateway routing needs gateway
    "P-010": ["P-004"],  # Consul needs K8s
    "P-011": ["P-001", "P-006"],  # AI Core needs repo and contracts
    "P-012": ["P-004"],  # Qdrant needs K8s
    "P-013": ["P-004"],  # Kafka needs K8s
    "P-014": ["P-004"],  # RabbitMQ needs K8s
    "P-015": ["P-004"],  # MinIO needs K8s
    "P-016": ["P-006"],  # Types needs contracts
    "P-017": ["P-004"],  # TimescaleDB needs K8s
    "P-018": ["P-003", "P-012", "P-013", "P-014", "P-015", "P-017"],  # Seed data needs all DBs
    "P-019": ["P-003"],  # Test infra needs Docker
    "P-020": ["P-019"],  # Perf tests need test infra
}

# P-021 to P-050 depend on P-001 and relevant earlier tasks
for i in range(21, 51):
    tid = f"P-{i:03d}"
    if i <= 25:
        phase0_deps[tid] = ["P-004"]  # K8s related
    elif i <= 30:
        phase0_deps[tid] = ["P-003", "P-004"]  # Docker/K8s related
    elif i <= 35:
        phase0_deps[tid] = ["P-005"]  # Security related (Vault)
    elif i <= 40:
        phase0_deps[tid] = ["P-006"]  # Shared libraries
    elif i <= 45:
        phase0_deps[tid] = ["P-007"]  # Observability related
    else:
        phase0_deps[tid] = ["P-002", "P-004"]  # DevOps related

# ============================================================================
# PHASE 1: FOUNDATION SERVICES (P-051 to P-250)
# 10 services, 20 tasks each
# ============================================================================

foundation_services = [
    ("Users", 51),
    ("GLEs", 71),
    ("Curricula", 91),
    ("Lessons", 111),
    ("Assessments", 131),
    ("IEPs", 151),
    ("Rewards", 171),
    ("Notifications", 191),
    ("Credentials", 211),
    ("Analytics", 231),
]

phase1_deps = {}
for service_name, base_num in foundation_services:
    # Task pattern for each service (20 tasks):
    # 0: Generate microservice - depends on P-001, P-003, P-006, P-016
    # 1: Domain entities - depends on generate
    # 2: Command handlers - depends on domain
    # 3: Query handlers - depends on commands
    # 4: PostgreSQL repo - depends on queries, P-003
    # 5: MongoDB repo - depends on postgres
    # 6: Redis caching - depends on mongo
    # 7: Composite repo - depends on redis
    # 8: API routes - depends on composite
    # 9: Event handlers - depends on API, P-014
    # 10: gRPC service - depends on events
    # 11: Unit tests - depends on gRPC, P-019
    # 12: Integration tests - depends on unit tests
    # 13: Config - depends on integration, P-005
    # 14: Docker - depends on config, P-003
    # 15: Documentation - depends on docker
    # 16: Message bus - depends on docs, P-014
    # 17: Performance tests - depends on message bus, P-020
    # 18: Security - depends on perf tests, P-005
    # 19: Metrics - depends on security, P-007
    
    for offset in range(20):
        tid = f"P-{base_num + offset:03d}"
        if offset == 0:
            phase1_deps[tid] = ["P-001", "P-003", "P-006", "P-016"]
        elif offset == 1:
            phase1_deps[tid] = [f"P-{base_num:03d}"]
        elif offset == 2:
            phase1_deps[tid] = [f"P-{base_num + 1:03d}"]
        elif offset == 3:
            phase1_deps[tid] = [f"P-{base_num + 2:03d}"]
        elif offset == 4:
            phase1_deps[tid] = [f"P-{base_num + 3:03d}", "P-003"]
        elif offset == 5:
            phase1_deps[tid] = [f"P-{base_num + 4:03d}"]
        elif offset == 6:
            phase1_deps[tid] = [f"P-{base_num + 5:03d}"]
        elif offset == 7:
            phase1_deps[tid] = [f"P-{base_num + 6:03d}"]
        elif offset == 8:
            phase1_deps[tid] = [f"P-{base_num + 7:03d}"]
        elif offset == 9:
            phase1_deps[tid] = [f"P-{base_num + 8:03d}", "P-014"]
        elif offset == 10:
            phase1_deps[tid] = [f"P-{base_num + 9:03d}"]
        elif offset == 11:
            phase1_deps[tid] = [f"P-{base_num + 10:03d}", "P-019"]
        elif offset == 12:
            phase1_deps[tid] = [f"P-{base_num + 11:03d}"]
        elif offset == 13:
            phase1_deps[tid] = [f"P-{base_num + 12:03d}", "P-005"]
        elif offset == 14:
            phase1_deps[tid] = [f"P-{base_num + 13:03d}", "P-003"]
        elif offset == 15:
            phase1_deps[tid] = [f"P-{base_num + 14:03d}"]
        elif offset == 16:
            phase1_deps[tid] = [f"P-{base_num + 15:03d}", "P-014"]
        elif offset == 17:
            phase1_deps[tid] = [f"P-{base_num + 16:03d}", "P-020"]
        elif offset == 18:
            phase1_deps[tid] = [f"P-{base_num + 17:03d}", "P-005"]
        elif offset == 19:
            phase1_deps[tid] = [f"P-{base_num + 18:03d}", "P-007"]

# ============================================================================
# PHASE 2: AI SERVICES (P-251 to P-500)
# 10 services, 25 tasks each
# ============================================================================

ai_services = [
    ("AITutors", 251),
    ("AIAssessments", 276),
    ("AIContent", 301),
    ("AISpeech", 326),
    ("AIAdaptive", 351),
    ("AIBehavior", 376),
    ("AISafety", 401),
    ("AIRecommendations", 426),
    ("AITranslation", 451),
    ("AIVision", 476),
]

phase2_deps = {}
for service_name, base_num in ai_services:
    for offset in range(25):
        tid = f"P-{base_num + offset:03d}"
        if offset == 0:
            # Generate AI microservice - needs P-001, P-003, P-006, P-011 (AI Core), P-012 (Qdrant)
            phase2_deps[tid] = ["P-001", "P-003", "P-006", "P-011", "P-012"]
        elif offset == 1:
            phase2_deps[tid] = [f"P-{base_num:03d}"]
        elif offset == 2:
            # AI orchestration needs domain + P-011
            phase2_deps[tid] = [f"P-{base_num + 1:03d}", "P-011"]
        elif offset == 3:
            # Vector storage needs orchestration + P-012
            phase2_deps[tid] = [f"P-{base_num + 2:03d}", "P-012"]
        elif offset == 4:
            phase2_deps[tid] = [f"P-{base_num + 3:03d}"]
        elif offset == 5:
            phase2_deps[tid] = [f"P-{base_num + 4:03d}"]
        elif offset == 6:
            phase2_deps[tid] = [f"P-{base_num + 5:03d}", "P-003"]
        elif offset == 7:
            phase2_deps[tid] = [f"P-{base_num + 6:03d}"]
        elif offset == 8:
            phase2_deps[tid] = [f"P-{base_num + 7:03d}"]
        elif offset == 9:
            phase2_deps[tid] = [f"P-{base_num + 8:03d}"]
        elif offset == 10:
            phase2_deps[tid] = [f"P-{base_num + 9:03d}"]
        elif offset == 11:
            # Safety filters
            phase2_deps[tid] = [f"P-{base_num + 10:03d}"]
        elif offset == 12:
            phase2_deps[tid] = [f"P-{base_num + 11:03d}", "P-019"]
        elif offset == 13:
            phase2_deps[tid] = [f"P-{base_num + 12:03d}"]
        elif offset == 14:
            phase2_deps[tid] = [f"P-{base_num + 13:03d}"]
        elif offset == 15:
            phase2_deps[tid] = [f"P-{base_num + 14:03d}", "P-005"]
        elif offset == 16:
            phase2_deps[tid] = [f"P-{base_num + 15:03d}", "P-003"]
        elif offset == 17:
            phase2_deps[tid] = [f"P-{base_num + 16:03d}", "P-007"]
        elif offset == 18:
            phase2_deps[tid] = [f"P-{base_num + 17:03d}"]
        elif offset == 19:
            phase2_deps[tid] = [f"P-{base_num + 18:03d}"]
        elif offset == 20:
            phase2_deps[tid] = [f"P-{base_num + 19:03d}", "P-014"]
        elif offset == 21:
            phase2_deps[tid] = [f"P-{base_num + 20:03d}", "P-013"]
        elif offset == 22:
            phase2_deps[tid] = [f"P-{base_num + 21:03d}", "P-020"]
        elif offset == 23:
            phase2_deps[tid] = [f"P-{base_num + 22:03d}", "P-005"]
        elif offset == 24:
            phase2_deps[tid] = [f"P-{base_num + 23:03d}"]

# ============================================================================
# PHASE 3: SUPPORT SERVICES (P-501 to P-700)
# 9 services, 20 tasks each + 20 integration tasks
# ============================================================================

support_services = [
    ("Parents", 501),
    ("Educators", 521),
    ("Fundraising", 541),
    ("Bookstore", 561),
    ("Aftercare", 581),
    ("Accessibility", 601),
    ("Compliance", 621),
    ("Ministries", 641),
    ("Operations", 661),
]

phase3_deps = {}
for service_name, base_num in support_services:
    for offset in range(20):
        tid = f"P-{base_num + offset:03d}"
        if offset == 0:
            # Support services depend on foundation services being done
            phase3_deps[tid] = ["P-001", "P-003", "P-006", "P-016", "P-070"]  # After Users service
        elif offset == 1:
            phase3_deps[tid] = [f"P-{base_num:03d}"]
        else:
            phase3_deps[tid] = [f"P-{base_num + offset - 1:03d}"]

# Cross-service integration tasks (P-681 to P-700)
phase3_deps["P-681"] = ["P-520", "P-191"]  # Parent notifications need Parents + Notifications
phase3_deps["P-682"] = ["P-540", "P-231"]  # Educator dashboard needs Educators + Analytics
phase3_deps["P-683"] = ["P-560"]  # Fundraising payment
phase3_deps["P-684"] = ["P-580"]  # Bookstore inventory
phase3_deps["P-685"] = ["P-600"]  # Aftercare GPS
phase3_deps["P-686"] = ["P-620"]  # Accessibility audit
phase3_deps["P-687"] = ["P-640"]  # Compliance GDPR
phase3_deps["P-688"] = ["P-660"]  # Ministry federation
phase3_deps["P-689"] = ["P-680"]  # Operations alerting
phase3_deps["P-690"] = ["P-681", "P-682", "P-683", "P-684", "P-685"]  # Integration tests
phase3_deps["P-691"] = ["P-690", "P-007"]  # Monitoring dashboard
phase3_deps["P-692"] = ["P-691"]
phase3_deps["P-693"] = ["P-692"]
phase3_deps["P-694"] = ["P-693"]
phase3_deps["P-695"] = ["P-694", "P-004"]
phase3_deps["P-696"] = ["P-695"]
phase3_deps["P-697"] = ["P-696"]
phase3_deps["P-698"] = ["P-697", "P-005"]
phase3_deps["P-699"] = ["P-698"]
phase3_deps["P-700"] = ["P-699"]

# ============================================================================
# PHASE 4: FRONTEND (P-701 to P-850)
# ============================================================================

# BlazorWasm services (7 tasks each for 13 services = 91 tasks)
blazor_services = [
    ("Users", 701, "P-070"),
    ("GLEs", 708, "P-090"),
    ("Curricula", 715, "P-110"),
    ("Lessons", 722, "P-130"),
    ("Assessments", 729, "P-150"),
    ("IEPs", 736, "P-170"),
    ("Rewards", 743, "P-190"),
    ("Notifications", 750, "P-210"),
    ("Credentials", 757, "P-230"),
    ("Analytics", 764, "P-250"),
    ("AITutors", 771, "P-275"),
    ("Parents", 778, "P-520"),
    ("Educators", 785, "P-540"),
]

phase4_deps = {}
for service_name, base_num, backend_dep in blazor_services:
    for offset in range(7):
        tid = f"P-{base_num + offset:03d}"
        if offset == 0:
            phase4_deps[tid] = [backend_dep, "P-008"]  # Backend service + API Gateway
        else:
            phase4_deps[tid] = [f"P-{base_num + offset - 1:03d}"]

# Additional frontend tasks (P-792 to P-850)
phase4_deps["P-792"] = ["P-707", "P-728", "P-735", "P-742", "P-777"]  # Student Portal RCL
phase4_deps["P-793"] = ["P-784", "P-749"]  # Parent Portal RCL
phase4_deps["P-794"] = ["P-791", "P-763"]  # Educator Portal RCL
phase4_deps["P-795"] = ["P-792", "P-793", "P-794"]  # Admin Portal RCL
for i in range(796, 851):
    tid = f"P-{i:03d}"
    phase4_deps[tid] = [f"P-{i-1:03d}"]

# ============================================================================
# PHASE 5: INTEGRATION & TESTING (P-851 to P-950)
# ============================================================================

phase5_deps = {}
phase5_deps["P-851"] = ["P-850", "P-019"]  # E2E framework needs frontend + test infra
phase5_deps["P-852"] = ["P-851", "P-070"]  # Student reg test needs Users service
phase5_deps["P-853"] = ["P-852", "P-130"]  # Lesson test needs Lessons service
phase5_deps["P-854"] = ["P-853", "P-150"]  # Assessment test needs Assessments
phase5_deps["P-855"] = ["P-854", "P-275"]  # AI tutor test needs AITutors
phase5_deps["P-856"] = ["P-855", "P-170"]  # IEP test needs IEPs
phase5_deps["P-857"] = ["P-856", "P-190"]  # Reward test needs Rewards
phase5_deps["P-858"] = ["P-857", "P-230"]  # Credential test needs Credentials
phase5_deps["P-859"] = ["P-858", "P-520"]  # Parent test needs Parents
phase5_deps["P-860"] = ["P-859", "P-540"]  # Educator test needs Educators

for i in range(861, 951):
    tid = f"P-{i:03d}"
    phase5_deps[tid] = [f"P-{i-1:03d}"]

# Add cross-dependencies for key testing tasks
phase5_deps["P-866"] = ["P-865", "P-020"]  # Load testing needs perf framework
phase5_deps["P-871"] = ["P-870", "P-005"]  # Security testing needs Vault
phase5_deps["P-884"] = ["P-883", "P-007"]  # Observability tests need observability stack

# ============================================================================
# PHASE 6: DEPLOYMENT (P-951 to P-1000)
# ============================================================================

phase6_deps = {}
phase6_deps["P-951"] = ["P-950", "P-004"]  # Prod K8s needs Phase 5 complete + K8s setup
phase6_deps["P-952"] = ["P-951"]
phase6_deps["P-953"] = ["P-952", "P-003"]  # Prod DBs need namespaces + Docker
phase6_deps["P-954"] = ["P-953", "P-013", "P-014"]  # Prod brokers need DBs + Kafka/RabbitMQ
phase6_deps["P-955"] = ["P-954", "P-015"]  # MinIO needs brokers + MinIO setup
phase6_deps["P-956"] = ["P-955", "P-012"]  # Qdrant needs MinIO + Qdrant setup
phase6_deps["P-957"] = ["P-956", "P-005"]  # Vault needs Qdrant + Vault setup
phase6_deps["P-958"] = ["P-957", "P-250"]  # Foundation deploy needs secrets + Phase 1 services
phase6_deps["P-959"] = ["P-958", "P-500"]  # AI deploy needs foundation + Phase 2 services
phase6_deps["P-960"] = ["P-959", "P-700"]  # Support deploy needs AI + Phase 3 services
phase6_deps["P-961"] = ["P-960", "P-008"]  # Gateway deploy needs support + API Gateway
phase6_deps["P-962"] = ["P-961", "P-850"]  # Frontend deploy needs gateway + Phase 4

for i in range(963, 1001):
    tid = f"P-{i:03d}"
    phase6_deps[tid] = [f"P-{i-1:03d}"]

# ============================================================================
# APPLY ALL DEPENDENCIES
# ============================================================================

all_deps = {}
all_deps.update(phase0_deps)
all_deps.update(phase1_deps)
all_deps.update(phase2_deps)
all_deps.update(phase3_deps)
all_deps.update(phase4_deps)
all_deps.update(phase5_deps)
all_deps.update(phase6_deps)

# Update all task files
updated = 0
for task_id, deps in all_deps.items():
    try:
        task = load_task(task_id)
        task["dependencies"] = deps
        # Also update description to include dependencies
        desc = task["description"]
        if "Dependencies:" in desc:
            # Update dependencies in description
            lines = desc.split("\n")
            new_lines = []
            for line in lines:
                if line.startswith("Dependencies:"):
                    new_lines.append(f"Dependencies: {', '.join(deps) if deps else 'None'}")
                else:
                    new_lines.append(line)
            task["description"] = "\n".join(new_lines)
        save_task(task)
        updated += 1
    except FileNotFoundError:
        print(f"Warning: Task {task_id} not found")
    except Exception as e:
        print(f"Error updating {task_id}: {e}")

print(f"Updated {updated} tasks with proper dependencies")

# Update queue.json with dependency graph
queue_path = os.path.join(OUTPUT_DIR, "queue.json")
with open(queue_path, 'r') as f:
    queue = json.load(f)

queue["dependency_graph"] = all_deps

with open(queue_path, 'w') as f:
    json.dump(queue, f, indent=2)

print("Updated queue.json with dependency graph")














