#!/usr/bin/env python3
"""Generate Pupitre Agent Task Files - Phase 3-6"""

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
        "required_resources": {"cpus": 2, "ram_gb": 4},
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

# ============================================================================
# PHASE 3: SUPPORT SERVICES (P-501 to P-700)
# ============================================================================

support_services = [
    ("Parents", "Parent", 60021, "Parent/guardian portal and engagement"),
    ("Educators", "Educator", 60022, "Teacher tools and lesson builder"),
    ("Fundraising", "Campaign", 60023, "Donation and campaign management"),
    ("Bookstore", "Book", 60024, "Sovereign textbook and materials store"),
    ("Aftercare", "AftercarePlan", 60025, "After-school programs and safety"),
    ("Accessibility", "AccessibilityProfile", 60026, "Accessibility profiles and assistive tech"),
    ("Compliance", "ComplianceRecord", 60027, "Audit trails and legal compliance"),
    ("Ministries", "MinistryData", 60028, "Federation with education ministries"),
    ("Operations", "OperationMetric", 60029, "Platform operations and monitoring"),
]

task_num = 501
for service_name, entity, port, desc in support_services:
    base_num = task_num
    
    tasks_for_service = [
        (f"P-{task_num:03d}", f"Generate Pupitre.{service_name} microservice", "Generate microservice", 1, []),
        (f"P-{task_num+1:03d}", f"Implement {service_name} domain entities", "Domain layer", 3, [f"P-{task_num:03d}"]),
        (f"P-{task_num+2:03d}", f"Implement {service_name} command handlers", "Commands", 3, [f"P-{task_num+1:03d}"]),
        (f"P-{task_num+3:03d}", f"Implement {service_name} query handlers", "Queries", 2, [f"P-{task_num+2:03d}"]),
        (f"P-{task_num+4:03d}", f"Implement {service_name} PostgreSQL repository", "Postgres", 2, [f"P-{task_num+3:03d}"]),
        (f"P-{task_num+5:03d}", f"Implement {service_name} MongoDB repository", "MongoDB", 2, [f"P-{task_num+4:03d}"]),
        (f"P-{task_num+6:03d}", f"Implement {service_name} Redis caching", "Redis", 1, [f"P-{task_num+5:03d}"]),
        (f"P-{task_num+7:03d}", f"Implement {service_name} composite repository", "Composite", 1, [f"P-{task_num+6:03d}"]),
        (f"P-{task_num+8:03d}", f"Implement {service_name} API routes", "API", 2, [f"P-{task_num+7:03d}"]),
        (f"P-{task_num+9:03d}", f"Implement {service_name} event handlers", "Events", 2, [f"P-{task_num+8:03d}"]),
        (f"P-{task_num+10:03d}", f"Write {service_name} unit tests", "Unit tests", 3, [f"P-{task_num+9:03d}"]),
        (f"P-{task_num+11:03d}", f"Write {service_name} integration tests", "Integration tests", 2, [f"P-{task_num+10:03d}"]),
        (f"P-{task_num+12:03d}", f"Configure {service_name} appsettings", "Config", 1, [f"P-{task_num+11:03d}"]),
        (f"P-{task_num+13:03d}", f"Create {service_name} Dockerfile", "Docker", 1, [f"P-{task_num+12:03d}"]),
        (f"P-{task_num+14:03d}", f"Document {service_name} API", "Docs", 1, [f"P-{task_num+13:03d}"]),
        (f"P-{task_num+15:03d}", f"Configure {service_name} message bus", "Message bus", 1, [f"P-{task_num+14:03d}"]),
        (f"P-{task_num+16:03d}", f"Configure {service_name} security", "Security", 2, [f"P-{task_num+15:03d}"]),
        (f"P-{task_num+17:03d}", f"Configure {service_name} metrics", "Metrics", 1, [f"P-{task_num+16:03d}"]),
        (f"P-{task_num+18:03d}", f"Implement {service_name} gRPC service", "gRPC", 2, [f"P-{task_num+17:03d}"]),
        (f"P-{task_num+19:03d}", f"Write {service_name} performance tests", "Perf tests", 2, [f"P-{task_num+18:03d}"]),
    ]
    
    for tid, title, detail, days, deps in tasks_for_service:
        desc_text = f"From plan:\nImplement {detail} for Pupitre.{service_name}.\nService: {desc}\n\nEffort: {days} days\nOwner: Backend Engineer\nDependencies: {', '.join(deps) if deps else 'P-001'}"
        owner_type = "devops" if "Docker" in title or "appsettings" in title else ("security" if "security" in title.lower() else ("docs" if "Document" in title else "backend"))
        all_tasks.append(create_task(tid, title, desc_text, service_name, port, 3, deps if deps else ["P-001"], "high" if days >= 2 else "medium", owner_type))
    
    task_num += 20

# Additional support service tasks
additional_support = [
    ("P-681", "Implement cross-service parent notifications", "Parent notifications across services", 3, ["P-521"]),
    ("P-682", "Implement educator dashboard aggregation", "Dashboard data aggregation", 3, ["P-541"]),
    ("P-683", "Implement fundraising payment integration", "FutureWampumPay integration", 4, ["P-561"]),
    ("P-684", "Implement bookstore inventory management", "Inventory system", 3, ["P-581"]),
    ("P-685", "Implement aftercare GPS tracking", "GPS tracking with consent", 4, ["P-601"]),
    ("P-686", "Implement accessibility audit reporting", "Accessibility reports", 2, ["P-621"]),
    ("P-687", "Implement compliance GDPR exports", "GDPR data export", 3, ["P-641"]),
    ("P-688", "Implement ministry data federation", "Data federation", 4, ["P-661"]),
    ("P-689", "Implement operations alerting", "Alert management", 2, ["P-679"]),
    ("P-690", "Implement support service integration tests", "Cross-service integration tests", 5, ["P-681", "P-682", "P-683"]),
    ("P-691", "Create support services monitoring dashboard", "Grafana dashboard", 2, ["P-690"]),
    ("P-692", "Document support services architecture", "Architecture docs", 2, ["P-691"]),
    ("P-693", "Create support services runbooks", "Operational runbooks", 2, ["P-692"]),
    ("P-694", "Implement support service health aggregation", "Health check aggregation", 2, ["P-693"]),
    ("P-695", "Configure support services auto-scaling", "K8s HPA configuration", 2, ["P-694"]),
    ("P-696", "Implement support services backup procedures", "Backup automation", 2, ["P-695"]),
    ("P-697", "Create support services disaster recovery", "DR procedures", 3, ["P-696"]),
    ("P-698", "Security audit for support services", "Security audit", 3, ["P-697"]),
    ("P-699", "Performance optimization for support services", "Performance tuning", 3, ["P-698"]),
    ("P-700", "Final support services validation", "End-to-end validation", 2, ["P-699"]),
]

for tid, title, detail, days, deps in additional_support:
    all_tasks.append(create_task(tid, title, f"From plan:\n{detail}\n\nEffort: {days} days\nOwner: Backend Engineer\nDependencies: {', '.join(deps)}", None, None, 3, deps, "high" if days >= 3 else "medium"))

print(f"Generated {len(all_tasks)} Phase 3 tasks")

# ============================================================================
# PHASE 4: FRONTEND (P-701 to P-850)
# ============================================================================

phase4_tasks = []

# BlazorWasm Services
blazor_services = [
    ("Users", 30), ("GLEs", 15), ("Curricula", 20), ("Lessons", 25), ("Assessments", 30),
    ("IEPs", 25), ("Rewards", 15), ("Notifications", 15), ("Credentials", 20), ("Analytics", 25),
    ("AITutors", 35), ("Parents", 20), ("Educators", 30),
]

task_num = 701
for service_name, complexity in blazor_services:
    tasks_for_blazor = [
        (f"P-{task_num:03d}", f"Create {service_name} BlazorWasm RouteService", f"RouteService for {service_name}", 1),
        (f"P-{task_num+1:03d}", f"Create {service_name} BlazorWasm API Client", f"Refit API client", 1),
        (f"P-{task_num+2:03d}", f"Create {service_name} BlazorWasm Index page", f"List/index page", 2),
        (f"P-{task_num+3:03d}", f"Create {service_name} BlazorWasm Details page", f"Details view page", 2),
        (f"P-{task_num+4:03d}", f"Create {service_name} BlazorWasm Create form", f"Create form", 2),
        (f"P-{task_num+5:03d}", f"Create {service_name} BlazorWasm Edit form", f"Edit form", 2),
        (f"P-{task_num+6:03d}", f"Create {service_name} BlazorWasm shared components", f"Shared components", 2),
    ]
    
    deps = [f"P-{task_num+i-1:03d}" if i > 0 else f"P-{50 + blazor_services.index((service_name, complexity)) * 20 + 8:03d}" for i in range(len(tasks_for_blazor))]
    
    for i, (tid, title, detail, days) in enumerate(tasks_for_blazor):
        phase4_tasks.append(create_task(tid, title, f"From plan:\n{detail} for Pupitre.{service_name}.BlazorWasm\n\nEffort: {days} days\nOwner: Frontend Engineer", f"{service_name}.BlazorWasm", None, 4, [deps[i]] if i > 0 else ["P-008"], "high" if days >= 2 else "medium", "frontend"))
    
    task_num += 7

# Additional frontend tasks
additional_frontend = [
    ("P-792", "Create Student Portal RCL", "Student-facing RCL aggregation"),
    ("P-793", "Create Parent Portal RCL", "Parent-facing RCL aggregation"),
    ("P-794", "Create Educator Portal RCL", "Educator-facing RCL aggregation"),
    ("P-795", "Create Admin Portal RCL", "Admin-facing RCL aggregation"),
    ("P-796", "Implement Student Portal layout", "Student portal main layout"),
    ("P-797", "Implement Parent Portal layout", "Parent portal main layout"),
    ("P-798", "Implement Educator Portal layout", "Educator portal main layout"),
    ("P-799", "Implement Admin Portal layout", "Admin portal main layout"),
    ("P-800", "Create shared UI component library", "MameyPro component usage"),
    ("P-801", "Implement responsive design", "Mobile-first responsive design"),
    ("P-802", "Implement accessibility features", "WCAG 2.1 AA compliance"),
    ("P-803", "Implement dark mode", "Dark mode theme support"),
    ("P-804", "Implement offline support", "PWA offline capabilities"),
    ("P-805", "Create .NET MAUI shared project", "MAUI shared code"),
    ("P-806", "Create MAUI iOS app", "iOS application"),
    ("P-807", "Create MAUI Android app", "Android application"),
    ("P-808", "Implement MAUI push notifications", "Mobile push notifications"),
    ("P-809", "Implement MAUI offline sync", "Mobile offline support"),
    ("P-810", "Create Unity gamification module", "Unity WebGL games"),
    ("P-811", "Implement game progression system", "Game progress tracking"),
    ("P-812", "Create reward animations", "Animation library"),
    ("P-813", "Implement achievement unlocks", "Achievement UI"),
    ("P-814", "Create AI tutor chat interface", "Chat UI for AI tutor"),
    ("P-815", "Implement voice input UI", "Voice input component"),
    ("P-816", "Create drawing canvas", "Drawing input component"),
    ("P-817", "Implement handwriting input", "Handwriting recognition UI"),
    ("P-818", "Create assessment player", "Quiz/test player UI"),
    ("P-819", "Implement adaptive assessment UI", "Adaptive testing UI"),
    ("P-820", "Create progress visualization", "Progress charts and graphs"),
    ("P-821", "Implement leaderboard UI", "Leaderboard component"),
    ("P-822", "Create IEP dashboard", "IEP management UI"),
    ("P-823", "Implement parent communication UI", "Parent messaging UI"),
    ("P-824", "Create educator gradebook", "Gradebook interface"),
    ("P-825", "Implement lesson builder UI", "Lesson creation interface"),
    ("P-826", "Create analytics dashboard", "Analytics visualization"),
    ("P-827", "Implement credential viewer", "Credential display/export"),
    ("P-828", "Create notification center", "Notification management UI"),
    ("P-829", "Implement settings pages", "User settings interface"),
    ("P-830", "Create onboarding flow", "First-time user onboarding"),
    ("P-831", "Implement help/support UI", "Help and support interface"),
    ("P-832", "Create feedback collection", "User feedback UI"),
    ("P-833", "Implement search functionality", "Global search UI"),
    ("P-834", "Create filtering components", "Filter and sort components"),
    ("P-835", "Implement pagination", "Pagination components"),
    ("P-836", "Create loading states", "Loading indicators"),
    ("P-837", "Implement error handling UI", "Error display components"),
    ("P-838", "Create form validation UI", "Form validation display"),
    ("P-839", "Implement file upload UI", "File upload components"),
    ("P-840", "Create media player", "Video/audio player"),
    ("P-841", "Implement localization", "Multi-language support"),
    ("P-842", "Create RTL support", "Right-to-left layout"),
    ("P-843", "Implement print styles", "Print-friendly styles"),
    ("P-844", "Create PDF export UI", "PDF generation UI"),
    ("P-845", "Implement keyboard navigation", "Full keyboard support"),
    ("P-846", "Create screen reader support", "ARIA labels and roles"),
    ("P-847", "Implement color contrast", "High contrast mode"),
    ("P-848", "Create frontend unit tests", "Jest/Playwright tests"),
    ("P-849", "Implement E2E frontend tests", "Playwright E2E tests"),
    ("P-850", "Final frontend validation", "Frontend QA validation"),
]

for tid, title, detail in additional_frontend:
    deps = [f"P-{int(tid.split('-')[1])-1:03d}"] if int(tid.split('-')[1]) > 792 else ["P-791"]
    phase4_tasks.append(create_task(tid, title, f"From plan:\n{detail}\n\nEffort: 2 days\nOwner: Frontend Engineer", None, None, 4, deps, "high", "frontend"))

all_tasks.extend(phase4_tasks)
print(f"Generated {len(phase4_tasks)} Phase 4 tasks")

# ============================================================================
# PHASE 5: INTEGRATION & TESTING (P-851 to P-950)
# ============================================================================

phase5_tasks = []

integration_tasks = [
    ("P-851", "Create end-to-end test framework", "Set up Playwright test infrastructure"),
    ("P-852", "Implement student registration flow test", "Full registration E2E test"),
    ("P-853", "Implement lesson completion flow test", "Lesson consumption E2E test"),
    ("P-854", "Implement assessment flow test", "Assessment taking E2E test"),
    ("P-855", "Implement AI tutor conversation test", "AI tutor E2E test"),
    ("P-856", "Implement IEP workflow test", "IEP creation/approval E2E test"),
    ("P-857", "Implement reward earning test", "Gamification E2E test"),
    ("P-858", "Implement credential issuance test", "Credential flow E2E test"),
    ("P-859", "Implement parent portal test", "Parent dashboard E2E test"),
    ("P-860", "Implement educator workflow test", "Teacher workflow E2E test"),
    ("P-861", "Create cross-service integration tests", "Service-to-service tests"),
    ("P-862", "Implement event flow tests", "Event propagation tests"),
    ("P-863", "Create saga workflow tests", "Saga orchestration tests"),
    ("P-864", "Implement authentication flow tests", "Auth/authz E2E tests"),
    ("P-865", "Create API contract tests", "Pact contract tests"),
    ("P-866", "Implement load testing suite", "k6 load tests"),
    ("P-867", "Create stress testing suite", "Stress test scenarios"),
    ("P-868", "Implement soak testing", "Long-running stability tests"),
    ("P-869", "Create chaos engineering tests", "Chaos Monkey scenarios"),
    ("P-870", "Implement failover tests", "DR failover testing"),
    ("P-871", "Create security penetration tests", "Pen testing suite"),
    ("P-872", "Implement OWASP compliance tests", "OWASP Top 10 tests"),
    ("P-873", "Create accessibility testing suite", "WCAG compliance tests"),
    ("P-874", "Implement AI response testing", "AI quality testing"),
    ("P-875", "Create bias detection tests", "AI bias testing"),
    ("P-876", "Implement safety filter tests", "Content safety tests"),
    ("P-877", "Create performance benchmark suite", "Performance baselines"),
    ("P-878", "Implement latency tests", "API latency testing"),
    ("P-879", "Create throughput tests", "System throughput tests"),
    ("P-880", "Implement concurrency tests", "Concurrent user tests"),
    ("P-881", "Create database performance tests", "DB query performance"),
    ("P-882", "Implement cache effectiveness tests", "Cache hit rate tests"),
    ("P-883", "Create message queue tests", "Queue performance tests"),
    ("P-884", "Implement observability tests", "Logging/tracing tests"),
    ("P-885", "Create monitoring alert tests", "Alert rule testing"),
    ("P-886", "Implement data integrity tests", "Data consistency tests"),
    ("P-887", "Create backup restore tests", "Backup/restore validation"),
    ("P-888", "Implement migration tests", "Schema migration tests"),
    ("P-889", "Create rollback tests", "Deployment rollback tests"),
    ("P-890", "Implement canary deployment tests", "Canary validation"),
    ("P-891", "Create blue-green deployment tests", "Blue-green validation"),
    ("P-892", "Implement configuration tests", "Config validation tests"),
    ("P-893", "Create secret rotation tests", "Vault rotation tests"),
    ("P-894", "Implement certificate tests", "TLS cert validation"),
    ("P-895", "Create rate limiting tests", "Rate limit validation"),
    ("P-896", "Implement quota tests", "Resource quota tests"),
    ("P-897", "Create multi-tenant tests", "Tenant isolation tests"),
    ("P-898", "Implement data sovereignty tests", "Data residency tests"),
    ("P-899", "Create compliance report generation", "Compliance reports"),
    ("P-900", "Implement test result aggregation", "Test reporting dashboard"),
]

for i, (tid, title, detail) in enumerate(integration_tasks):
    deps = [f"P-{int(tid.split('-')[1])-1:03d}"] if i > 0 else ["P-850"]
    phase5_tasks.append(create_task(tid, title, f"From plan:\n{detail}\n\nEffort: 2 days\nOwner: QA Engineer", None, None, 5, deps, "high"))

# Additional integration tasks
more_integration = [
    ("P-901", "Validate all foundation services integration", 3),
    ("P-902", "Validate all AI services integration", 4),
    ("P-903", "Validate all support services integration", 3),
    ("P-904", "Validate frontend-backend integration", 3),
    ("P-905", "Validate mobile app integration", 3),
    ("P-906", "Validate external system integration", 4),
    ("P-907", "Performance optimization round 1", 5),
    ("P-908", "Security hardening round 1", 5),
    ("P-909", "Accessibility remediation", 4),
    ("P-910", "AI quality improvement", 5),
    ("P-911", "Load testing at scale", 4),
    ("P-912", "Stress testing at scale", 4),
    ("P-913", "Final security audit", 5),
    ("P-914", "Final performance audit", 4),
    ("P-915", "Final accessibility audit", 3),
    ("P-916", "User acceptance testing prep", 2),
    ("P-917", "UAT execution round 1", 5),
    ("P-918", "UAT bug fixes", 5),
    ("P-919", "UAT execution round 2", 3),
    ("P-920", "Final UAT sign-off", 2),
    ("P-921", "Documentation review", 3),
    ("P-922", "API documentation finalization", 2),
    ("P-923", "Architecture documentation", 3),
    ("P-924", "Operations documentation", 3),
    ("P-925", "Training material creation", 5),
    ("P-926", "Training video production", 5),
    ("P-927", "Help content creation", 4),
    ("P-928", "FAQ compilation", 2),
    ("P-929", "Knowledge base setup", 3),
    ("P-930", "Support workflow setup", 3),
    ("P-931", "Incident response procedures", 3),
    ("P-932", "Escalation procedures", 2),
    ("P-933", "On-call rotation setup", 2),
    ("P-934", "Monitoring dashboard finalization", 3),
    ("P-935", "Alert tuning", 3),
    ("P-936", "Log aggregation optimization", 2),
    ("P-937", "Tracing optimization", 2),
    ("P-938", "Metrics collection optimization", 2),
    ("P-939", "Cost optimization review", 3),
    ("P-940", "Resource right-sizing", 3),
    ("P-941", "Auto-scaling configuration", 2),
    ("P-942", "Cache optimization", 2),
    ("P-943", "Database optimization", 3),
    ("P-944", "Query optimization", 3),
    ("P-945", "Index optimization", 2),
    ("P-946", "Network optimization", 2),
    ("P-947", "CDN configuration", 2),
    ("P-948", "Edge caching setup", 2),
    ("P-949", "Final integration validation", 3),
    ("P-950", "Go-live readiness checklist", 2),
]

for tid, title, days in more_integration:
    deps = [f"P-{int(tid.split('-')[1])-1:03d}"]
    phase5_tasks.append(create_task(tid, title, f"From plan:\n{title}\n\nEffort: {days} days\nOwner: QA/DevOps Engineer", None, None, 5, deps, "high" if days >= 3 else "medium"))

all_tasks.extend(phase5_tasks)
print(f"Generated {len(phase5_tasks)} Phase 5 tasks")

# ============================================================================
# PHASE 6: DEPLOYMENT (P-951 to P-1000)
# ============================================================================

phase6_tasks = []

deployment_tasks = [
    ("P-951", "Create production Kubernetes cluster", "Production K8s setup"),
    ("P-952", "Configure production namespaces", "Namespace configuration"),
    ("P-953", "Deploy production databases", "PostgreSQL, MongoDB, Redis"),
    ("P-954", "Deploy production message brokers", "RabbitMQ, Kafka"),
    ("P-955", "Deploy production object storage", "MinIO production"),
    ("P-956", "Deploy production vector database", "Qdrant production"),
    ("P-957", "Configure production secrets", "Vault production setup"),
    ("P-958", "Deploy foundation services", "Phase 1 services deployment"),
    ("P-959", "Deploy AI services", "Phase 2 services deployment"),
    ("P-960", "Deploy support services", "Phase 3 services deployment"),
    ("P-961", "Deploy API Gateway", "Production gateway"),
    ("P-962", "Deploy frontend applications", "Blazor/MAUI deployment"),
    ("P-963", "Configure production DNS", "DNS records setup"),
    ("P-964", "Configure production SSL", "TLS certificates"),
    ("P-965", "Configure production CDN", "CDN setup"),
    ("P-966", "Configure production WAF", "WAF rules"),
    ("P-967", "Configure production load balancer", "Load balancer setup"),
    ("P-968", "Configure production auto-scaling", "HPA configuration"),
    ("P-969", "Configure production monitoring", "Prometheus/Grafana"),
    ("P-970", "Configure production logging", "ELK/Loki setup"),
    ("P-971", "Configure production tracing", "Jaeger production"),
    ("P-972", "Configure production alerting", "Alert rules"),
    ("P-973", "Production smoke tests", "Basic functionality"),
    ("P-974", "Production integration tests", "Integration validation"),
    ("P-975", "Production performance tests", "Performance validation"),
    ("P-976", "Production security scan", "Security validation"),
    ("P-977", "Production data migration", "Data migration"),
    ("P-978", "Production seed data", "Initial data setup"),
    ("P-979", "Production backup configuration", "Backup setup"),
    ("P-980", "Production DR configuration", "DR setup"),
    ("P-981", "Production runbook validation", "Runbook testing"),
    ("P-982", "Production on-call setup", "On-call rotation"),
    ("P-983", "Production documentation", "Deployment docs"),
    ("P-984", "Soft launch preparation", "Soft launch prep"),
    ("P-985", "Soft launch execution", "Limited user launch"),
    ("P-986", "Soft launch monitoring", "Initial monitoring"),
    ("P-987", "Soft launch bug fixes", "Early bug fixes"),
    ("P-988", "Soft launch optimization", "Performance tuning"),
    ("P-989", "Full launch preparation", "Full launch prep"),
    ("P-990", "Full launch execution", "Production launch"),
    ("P-991", "Post-launch monitoring", "Intensive monitoring"),
    ("P-992", "Post-launch optimization", "Performance tuning"),
    ("P-993", "Post-launch bug fixes", "Bug resolution"),
    ("P-994", "User feedback collection", "Feedback gathering"),
    ("P-995", "Iteration planning", "Next iteration planning"),
    ("P-996", "Knowledge transfer", "Team knowledge transfer"),
    ("P-997", "Retrospective", "Project retrospective"),
    ("P-998", "Documentation finalization", "Final documentation"),
    ("P-999", "Project closure", "Project closure activities"),
    ("P-1000", "Handover to operations", "Operations handover"),
]

for i, (tid, title, detail) in enumerate(deployment_tasks):
    deps = [f"P-{int(tid.split('-')[1])-1:03d}"] if i > 0 else ["P-950"]
    owner = "devops" if any(x in title.lower() for x in ["deploy", "configure", "kubernetes", "production"]) else "backend"
    phase6_tasks.append(create_task(tid, title, f"From plan:\n{detail}\n\nEffort: 2 days\nOwner: DevOps Engineer", None, None, 6, deps, "high", owner))

all_tasks.extend(phase6_tasks)
print(f"Generated {len(phase6_tasks)} Phase 6 tasks")

# Save all tasks
for task in all_tasks:
    save_task(task)

# Update queue.json
queue_path = os.path.join(OUTPUT_DIR, "queue.json")
with open(queue_path, 'r') as f:
    queue = json.load(f)

queue["phases"]["3"] = {"name": "Support Services", "tasks": [t["plan_task_id"] for t in all_tasks if t["phase"] == 3]}
queue["phases"]["4"] = {"name": "Frontend", "tasks": [t["plan_task_id"] for t in all_tasks if t["phase"] == 4]}
queue["phases"]["5"] = {"name": "Integration & Testing", "tasks": [t["plan_task_id"] for t in all_tasks if t["phase"] == 5]}
queue["phases"]["6"] = {"name": "Deployment", "tasks": [t["plan_task_id"] for t in all_tasks if t["phase"] == 6]}

for t in all_tasks:
    queue["task_index"][t["plan_task_id"]] = {"status": t["status"], "phase": t["phase"]}
queue["total_tasks"] = len(queue["task_index"])

with open(queue_path, 'w') as f:
    json.dump(queue, f, indent=2)

print(f"\nTotal tasks generated: {len(all_tasks)}")
print(f"Queue file updated with all phases")
print(f"Total tasks in queue: {queue['total_tasks']}")

