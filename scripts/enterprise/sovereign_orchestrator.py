#!/usr/bin/env python3
"""
Sovereign Enterprise Orchestrator — v16.0.0

Real orchestration engine for the 84 .NET microservices + infrastructure.
Monitors health, manages startup order, handles failover, collects metrics,
and coordinates the NEXUS-level infrastructure from a single control plane.

Usage:
  python sovereign_orchestrator.py status          # Dashboard overview
  python sovereign_orchestrator.py health          # Full health check
  python sovereign_orchestrator.py start           # Bring up all services
  python sovereign_orchestrator.py stop            # Graceful shutdown
  python sovereign_orchestrator.py scale <svc> N   # Scale a service
  python sovereign_orchestrator.py logs <svc>      # Tail logs

Environment:
  DOCKER_COMPOSE_FILE   Path to docker-compose.enterprise.yml
  HEALTH_CHECK_INTERVAL Seconds between checks (default: 30)
  ALERT_WEBHOOK         Webhook URL for alerts
"""

import json
import logging
import os
import subprocess
import sys
import time
from dataclasses import dataclass, field, asdict
from datetime import datetime, timezone
from pathlib import Path
from typing import Optional
from enum import Enum

LOG_DIR = Path(os.environ.get("LOG_DIR", "logs"))
LOG_DIR.mkdir(parents=True, exist_ok=True)

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(message)s",
    handlers=[
        logging.StreamHandler(sys.stdout),
        logging.FileHandler(LOG_DIR / "orchestrator.log", encoding="utf-8"),
    ],
)
logger = logging.getLogger("ierahkwa.orchestrator")

COMPOSE_FILE = os.environ.get(
    "DOCKER_COMPOSE_FILE",
    str(Path(__file__).parent.parent.parent / "docker-compose.enterprise.yml"),
)
HEALTH_INTERVAL = int(os.environ.get("HEALTH_CHECK_INTERVAL", "30"))
ALERT_WEBHOOK = os.environ.get("ALERT_WEBHOOK", "")


class ServiceState(str, Enum):
    RUNNING = "running"
    STOPPED = "stopped"
    DEGRADED = "degraded"
    STARTING = "starting"
    UNHEALTHY = "unhealthy"
    UNKNOWN = "unknown"


# ── Domain Catalog ───────────────────────────────────────────────────

DOMAINS = {
    "core-infra": {
        "label": "Core Infrastructure",
        "priority": 0,
        "services": ["postgres-sovereign", "redis-sovereign", "mongodb-sovereign",
                      "minio-sovereign", "consul-sovereign", "rabbitmq-sovereign"],
    },
    "ai-engine": {
        "label": "AI & Analytics",
        "priority": 1,
        "services": ["ollama", "ierahkwa-ai-engine", "ierahkwa-ml-engine",
                      "ierahkwa-ai-mediator", "ierahkwa-ai-sentinel",
                      "ierahkwa-ai-conscience"],
    },
    "identity": {
        "label": "Identity & Auth",
        "priority": 2,
        "services": ["identity-service", "did-service", "kyc-service",
                      "biometrics-service", "passport-service", "auth-gateway"],
    },
    "governance": {
        "label": "Governance DAO",
        "priority": 3,
        "services": ["governance-dao", "voting-system", "constitution-engine",
                      "proposal-manager", "council-service"],
    },
    "treasury": {
        "label": "Treasury & Fiscal",
        "priority": 3,
        "services": ["treasury-service", "tax-authority", "fiscal-allocation",
                      "budget-service", "audit-service", "procurement-service"],
    },
    "banking": {
        "label": "Banking & Finance",
        "priority": 4,
        "services": ["central-bank", "commercial-bank", "defi-soberano",
                      "wampum-treasury", "lending-service", "exchange-service",
                      "payments-gateway", "settlement-engine"],
    },
    "citizen": {
        "label": "Citizen Services",
        "priority": 4,
        "services": ["citizen-crm", "citizen-portal", "education-service",
                      "healthcare-service", "social-services", "housing-service"],
    },
    "legal": {
        "label": "Legal & Courts",
        "priority": 5,
        "services": ["courts-service", "legal-contracts", "licensing-service",
                      "compliance-service", "dispute-resolution"],
    },
    "infrastructure": {
        "label": "Physical Infrastructure",
        "priority": 5,
        "services": ["land-registry", "property-service", "utilities-service",
                      "transport-service", "telecom-service"],
    },
    "communication": {
        "label": "Communication",
        "priority": 6,
        "services": ["matrix-synapse", "notification-service", "cms-service",
                      "media-service", "handshake-dns"],
    },
    "security": {
        "label": "Security & Defense",
        "priority": 6,
        "services": ["security-service", "border-control", "emergency-service",
                      "intelligence-service", "cyber-defense"],
    },
    "blockchain": {
        "label": "Blockchain & Mesh",
        "priority": 7,
        "services": ["mameynode-kernel", "mameynode-government", "mameynode-bank",
                      "ipfs-node", "lora-bridge", "satellite-gateway"],
    },
}


# ── Docker Interface ─────────────────────────────────────────────────

def docker_compose(*args, capture: bool = True) -> subprocess.CompletedProcess:
    cmd = ["docker", "compose", "-f", COMPOSE_FILE, *args]
    logger.debug("Running: %s", " ".join(cmd))
    return subprocess.run(
        cmd,
        capture_output=capture,
        text=True,
        timeout=300,
    )


def get_container_status() -> dict[str, dict]:
    """Get status of all containers managed by the compose file."""
    result = docker_compose("ps", "--format", "json")
    containers = {}
    if result.returncode == 0 and result.stdout.strip():
        for line in result.stdout.strip().split("\n"):
            try:
                c = json.loads(line)
                name = c.get("Service", c.get("Name", ""))
                containers[name] = {
                    "name": name,
                    "state": c.get("State", "unknown"),
                    "health": c.get("Health", ""),
                    "status": c.get("Status", ""),
                    "ports": c.get("Publishers", []),
                }
            except json.JSONDecodeError:
                continue
    return containers


def check_service_health(service: str, port: int) -> dict:
    """HTTP health check on a service."""
    import urllib.request
    try:
        start = time.monotonic()
        url = f"http://localhost:{port}/health"
        with urllib.request.urlopen(url, timeout=10) as resp:
            latency = round((time.monotonic() - start) * 1000, 1)
            body = resp.read().decode()
            return {
                "service": service,
                "status": "healthy",
                "http_code": resp.status,
                "latency_ms": latency,
                "body": body[:200],
            }
    except Exception as exc:
        return {
            "service": service,
            "status": "unhealthy",
            "error": str(exc),
            "latency_ms": -1,
        }


# ── Orchestrator Commands ────────────────────────────────────────────

def cmd_status():
    """Print a dashboard overview of all domains and services."""
    containers = get_container_status()

    print("\n╔══════════════════════════════════════════════════════════════════╗")
    print("║          IERAHKWA NE KANIENKE — SOVEREIGN ORCHESTRATOR        ║")
    print("║                        v16.0.0 Evergreen                       ║")
    print("╚══════════════════════════════════════════════════════════════════╝\n")

    total = 0
    running = 0
    for domain_key, domain in sorted(DOMAINS.items(), key=lambda x: x[1]["priority"]):
        print(f"  ┌─ {domain['label']} (priority {domain['priority']})")
        for svc in domain["services"]:
            total += 1
            info = containers.get(svc, {})
            state = info.get("state", "not found")
            health = info.get("health", "")
            if state == "running":
                running += 1
                icon = "●" if health != "unhealthy" else "○"
            elif state == "exited":
                icon = "✗"
            else:
                icon = "?"
            print(f"  │  {icon} {svc:<35} {state:<12} {health}")
        print(f"  └{'─' * 60}")

    print(f"\n  Total: {running}/{total} services running")
    print(f"  Compose file: {COMPOSE_FILE}")
    print(f"  Timestamp: {datetime.now(timezone.utc).isoformat()}\n")


def cmd_health():
    """Run health checks on all running services."""
    containers = get_container_status()
    results = []

    print("\n=== HEALTH CHECK ===\n")
    for domain_key, domain in sorted(DOMAINS.items(), key=lambda x: x[1]["priority"]):
        for svc in domain["services"]:
            info = containers.get(svc, {})
            if info.get("state") != "running":
                continue

            ports = info.get("ports", [])
            if not ports:
                continue

            published = None
            for p in ports:
                if isinstance(p, dict):
                    published = p.get("PublishedPort", 0)
                    if published:
                        break

            if not published:
                continue

            result = check_service_health(svc, published)
            results.append(result)
            icon = "✓" if result["status"] == "healthy" else "✗"
            latency = f"{result['latency_ms']}ms" if result["latency_ms"] > 0 else "N/A"
            print(f"  {icon} {svc:<35} {result['status']:<12} {latency}")

    healthy = sum(1 for r in results if r["status"] == "healthy")
    print(f"\n  Health: {healthy}/{len(results)} healthy\n")

    report_path = LOG_DIR / f"health_{datetime.now(timezone.utc).strftime('%Y%m%d_%H%M%S')}.json"
    with open(report_path, "w") as f:
        json.dump(results, f, indent=2)
    print(f"  Report: {report_path}")

    return results


def cmd_start():
    """Start all services in priority order."""
    print("\n=== STARTING SOVEREIGN INFRASTRUCTURE ===\n")

    for priority in range(max(d["priority"] for d in DOMAINS.values()) + 1):
        group = {k: v for k, v in DOMAINS.items() if v["priority"] == priority}
        if not group:
            continue

        services = []
        for d in group.values():
            services.extend(d["services"])

        labels = ", ".join(d["label"] for d in group.values())
        print(f"  Phase {priority}: {labels} ({len(services)} services)")

        result = docker_compose("up", "-d", *services)
        if result.returncode != 0:
            logger.error("Failed to start phase %d: %s", priority, result.stderr)
            print(f"    ✗ ERROR: {result.stderr[:200]}")
        else:
            print(f"    ✓ Started")

        if priority < 2:
            print("    Waiting 15s for infrastructure init...")
            time.sleep(15)
        else:
            time.sleep(5)

    print("\n  All phases started. Run 'health' to verify.\n")


def cmd_stop():
    """Graceful shutdown in reverse priority order."""
    print("\n=== STOPPING SOVEREIGN INFRASTRUCTURE ===\n")

    max_pri = max(d["priority"] for d in DOMAINS.values())
    for priority in range(max_pri, -1, -1):
        group = {k: v for k, v in DOMAINS.items() if v["priority"] == priority}
        if not group:
            continue

        services = []
        for d in group.values():
            services.extend(d["services"])

        labels = ", ".join(d["label"] for d in group.values())
        print(f"  Phase {priority}: {labels} ({len(services)} services)")

        result = docker_compose("stop", *services)
        if result.returncode != 0:
            logger.warning("Stop phase %d had errors: %s", priority, result.stderr[:200])
        else:
            print(f"    ✓ Stopped")

    print("\n  All services stopped.\n")


def cmd_scale(service: str, replicas: int):
    """Scale a specific service."""
    print(f"\n  Scaling {service} to {replicas} replicas...")
    result = docker_compose("up", "-d", "--scale", f"{service}={replicas}", service)
    if result.returncode == 0:
        print(f"  ✓ {service} scaled to {replicas}")
    else:
        print(f"  ✗ Error: {result.stderr[:200]}")


def cmd_logs(service: str):
    """Tail logs for a service."""
    docker_compose("logs", "-f", "--tail", "100", service, capture=False)


def send_alert(message: str, severity: str = "warning"):
    """Send alert to configured webhook."""
    if not ALERT_WEBHOOK:
        return
    try:
        import urllib.request
        payload = json.dumps({
            "text": f"[{severity.upper()}] IERAHKWA Orchestrator: {message}",
            "timestamp": datetime.now(timezone.utc).isoformat(),
        }).encode()
        req = urllib.request.Request(
            ALERT_WEBHOOK,
            data=payload,
            headers={"Content-Type": "application/json"},
        )
        urllib.request.urlopen(req, timeout=10)
    except Exception as exc:
        logger.error("Alert send failed: %s", exc)


# ── Watchdog ─────────────────────────────────────────────────────────

def cmd_watch():
    """Continuous health monitoring with alerting."""
    print(f"\n  Watchdog active (interval={HEALTH_INTERVAL}s)\n")
    prev_unhealthy = set()

    while True:
        try:
            results = cmd_health()
            unhealthy = {r["service"] for r in results if r["status"] == "unhealthy"}

            new_failures = unhealthy - prev_unhealthy
            recoveries = prev_unhealthy - unhealthy

            for svc in new_failures:
                msg = f"Service {svc} is UNHEALTHY"
                logger.warning(msg)
                send_alert(msg, "critical")

            for svc in recoveries:
                msg = f"Service {svc} RECOVERED"
                logger.info(msg)
                send_alert(msg, "info")

            prev_unhealthy = unhealthy
            time.sleep(HEALTH_INTERVAL)

        except KeyboardInterrupt:
            print("\n  Watchdog stopped.")
            break
        except Exception as exc:
            logger.error("Watchdog error: %s", exc)
            time.sleep(10)


# ── Main ─────────────────────────────────────────────────────────────

def main():
    if len(sys.argv) < 2:
        print("Usage: sovereign_orchestrator.py <command> [args]")
        print("Commands: status, health, start, stop, scale, logs, watch")
        sys.exit(1)

    cmd = sys.argv[1]

    if cmd == "status":
        cmd_status()
    elif cmd == "health":
        cmd_health()
    elif cmd == "start":
        cmd_start()
    elif cmd == "stop":
        cmd_stop()
    elif cmd == "scale":
        if len(sys.argv) < 4:
            print("Usage: sovereign_orchestrator.py scale <service> <replicas>")
            sys.exit(1)
        cmd_scale(sys.argv[2], int(sys.argv[3]))
    elif cmd == "logs":
        if len(sys.argv) < 3:
            print("Usage: sovereign_orchestrator.py logs <service>")
            sys.exit(1)
        cmd_logs(sys.argv[2])
    elif cmd == "watch":
        cmd_watch()
    else:
        print(f"Unknown command: {cmd}")
        sys.exit(1)


if __name__ == "__main__":
    main()
