#!/usr/bin/env python3
"""
Ierahkwa Health Sentinel -- 24h Auto-Diagnostic System
Performs comprehensive health checks on all Ierahkwa sovereign infrastructure
services, generates a Markdown report, delivers it to Matrix and ntfy, and
logs results as JSON for historical tracking.

Exit codes: 0=healthy, 1=degraded, 2=critical

Environment variables
---------------------
MATRIX_HOMESERVER       Matrix server URL          (default: https://matrix.ierahkwa.org)
MATRIX_USER             Bot Matrix user ID         (e.g. @sentinel:ierahkwa.org)
MATRIX_PASSWORD         Bot password
MATRIX_SENTINEL_ROOM    Private sentinel room      (e.g. !sentinel:ierahkwa.org)
MAMEYNODE_RPC           MameyNode JSON-RPC         (default: http://mameynode:8545)
PULSE_ADDRESS           IerahkwaPulse contract     (default: 0x...placeholder)
IPFS_API_URL            IPFS Kubo API endpoint     (default: http://127.0.0.1:5101)
NTFY_URL                ntfy server URL            (default: https://ntfy.sh)
NTFY_TOPIC              ntfy topic for sentinel    (default: ierahkwa-sentinel)
LOG_DIR                 Logging directory          (default: logs/)
SENTINEL_LOG_PATH       JSON report path           (default: /var/log/ierahkwa/sentinel.json)
LORA_SERIAL_PORT        LoRa device port           (default: /dev/ttyUSB0)
BIO_SERIAL_PORT         Bio sensor port            (default: /dev/ttyUSB1)
RESOURCE_WARN_PCT       Resource warning threshold (default: 80)
"""

import asyncio
import json
import logging
import os
import socket
import subprocess
import sys
import time
from datetime import datetime, timezone
from pathlib import Path
from typing import Optional

try:
    import aiohttp
except ImportError:
    sys.exit("aiohttp is required: pip install aiohttp")

try:
    from nio import AsyncClient, LoginResponse
except ImportError:
    sys.exit("matrix-nio is required: pip install matrix-nio")

try:
    import psutil
except ImportError:
    psutil = None

try:
    from web3 import Web3
except ImportError:
    Web3 = None

try:
    import requests as http_requests
except ImportError:
    sys.exit("requests is required: pip install requests")

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------

MATRIX_HOMESERVER = os.environ.get("MATRIX_HOMESERVER", "https://matrix.ierahkwa.org")
MATRIX_USER = os.environ.get("MATRIX_USER", "@sentinel:ierahkwa.org")
MATRIX_PASSWORD = os.environ.get("MATRIX_PASSWORD", "")
MATRIX_SENTINEL_ROOM = os.environ.get("MATRIX_SENTINEL_ROOM", "")
MAMEYNODE_RPC = os.environ.get("MAMEYNODE_RPC", "http://mameynode:8545")
PULSE_ADDRESS = os.environ.get(
    "PULSE_ADDRESS", "0x0000000000000000000000000000000000000000"
)
IPFS_API_URL = os.environ.get("IPFS_API_URL", "http://127.0.0.1:5101")
NTFY_URL = os.environ.get("NTFY_URL", "https://ntfy.sh")
NTFY_TOPIC = os.environ.get("NTFY_TOPIC", "ierahkwa-sentinel")
LOG_DIR = Path(os.environ.get("LOG_DIR", "logs"))
SENTINEL_LOG_PATH = Path(
    os.environ.get("SENTINEL_LOG_PATH", "/var/log/ierahkwa/sentinel.json")
)
LORA_SERIAL_PORT = os.environ.get("LORA_SERIAL_PORT", "/dev/ttyUSB0")
BIO_SERIAL_PORT = os.environ.get("BIO_SERIAL_PORT", "/dev/ttyUSB1")
RESOURCE_WARN_PCT = int(os.environ.get("RESOURCE_WARN_PCT", "80"))

LOG_DIR.mkdir(parents=True, exist_ok=True)
SENTINEL_LOG_PATH.parent.mkdir(parents=True, exist_ok=True)

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(name)s: %(message)s",
    handlers=[
        logging.StreamHandler(sys.stdout),
        logging.FileHandler(LOG_DIR / "ierahkwa_sentinel.log", encoding="utf-8"),
    ],
)
logger = logging.getLogger("ierahkwa.sentinel")

# ---------------------------------------------------------------------------
# Health check status constants
# ---------------------------------------------------------------------------

STATUS_OK = "OK"
STATUS_WARN = "WARN"
STATUS_FAIL = "FAIL"
STATUS_SKIP = "SKIP"

# ---------------------------------------------------------------------------
# Service definitions
# ---------------------------------------------------------------------------

TCP_SERVICES = [
    {"name": "Matrix Synapse", "host": "127.0.0.1", "port": 8008},
    {"name": "Ollama AI", "host": "127.0.0.1", "port": 11434},
    {"name": "IPFS Kubo", "host": "127.0.0.1", "port": 5101},
    {"name": "PostgreSQL", "host": "127.0.0.1", "port": 5432},
    {"name": "Redis", "host": "127.0.0.1", "port": 6379},
    {"name": "MameyNode", "host": "127.0.0.1", "port": 8545},
    {"name": "Tor SOCKS5 Proxy", "host": "127.0.0.1", "port": 9050},
    {"name": "Handshake DNS", "host": "127.0.0.1", "port": 12037},
    {"name": "ntfy", "host": "127.0.0.1", "port": 2586},
    {"name": "sovereign-core", "host": "127.0.0.1", "port": 3050},
    {"name": "Nginx HTTP", "host": "127.0.0.1", "port": 80},
    {"name": "Nginx HTTPS", "host": "127.0.0.1", "port": 443},
]

SERIAL_DEVICES = [
    {"name": "LoRa Meshtastic", "path": LORA_SERIAL_PORT},
    {"name": "Bio Sensors", "path": BIO_SERIAL_PORT},
]


# ---------------------------------------------------------------------------
# Individual check functions
# ---------------------------------------------------------------------------


def check_tcp_service(host: str, port: int, timeout: float = 3.0) -> dict:
    """Check if a TCP port is accepting connections."""
    try:
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        sock.settimeout(timeout)
        result = sock.connect_ex((host, port))
        sock.close()
        if result == 0:
            return {"status": STATUS_OK, "detail": f"Port {port} open"}
        return {"status": STATUS_FAIL, "detail": f"Port {port} refused (code {result})"}
    except socket.timeout:
        return {"status": STATUS_FAIL, "detail": f"Port {port} timeout"}
    except Exception as exc:
        return {"status": STATUS_FAIL, "detail": str(exc)}


def check_serial_device(path: str) -> dict:
    """Check if a serial device file exists."""
    if Path(path).exists():
        return {"status": STATUS_OK, "detail": f"Device {path} present"}
    return {"status": STATUS_FAIL, "detail": f"Device {path} not found"}


def check_meshtastic_gateway() -> dict:
    """Check if the Meshtastic Python package can detect a device."""
    try:
        result = subprocess.run(
            ["meshtastic", "--info"],
            capture_output=True,
            text=True,
            timeout=15,
        )
        if result.returncode == 0:
            return {"status": STATUS_OK, "detail": "Meshtastic gateway responsive"}
        return {"status": STATUS_WARN, "detail": f"meshtastic exited {result.returncode}"}
    except FileNotFoundError:
        return {"status": STATUS_SKIP, "detail": "meshtastic CLI not installed"}
    except subprocess.TimeoutExpired:
        return {"status": STATUS_WARN, "detail": "meshtastic --info timed out"}
    except Exception as exc:
        return {"status": STATUS_FAIL, "detail": str(exc)}


def check_system_resources() -> dict:
    """Check CPU, memory, and disk usage."""
    if psutil is None:
        return {
            "status": STATUS_SKIP,
            "detail": "psutil not installed",
            "cpu_pct": None,
            "memory_pct": None,
            "disk_pct": None,
        }

    cpu_pct = psutil.cpu_percent(interval=1)
    mem = psutil.virtual_memory()
    disk = psutil.disk_usage("/")

    warnings = []
    if cpu_pct > RESOURCE_WARN_PCT:
        warnings.append(f"CPU at {cpu_pct:.1f}%")
    if mem.percent > RESOURCE_WARN_PCT:
        warnings.append(f"Memory at {mem.percent:.1f}%")
    if disk.percent > RESOURCE_WARN_PCT:
        warnings.append(f"Disk at {disk.percent:.1f}%")

    status = STATUS_WARN if warnings else STATUS_OK
    detail = "; ".join(warnings) if warnings else "All resources within limits"

    return {
        "status": status,
        "detail": detail,
        "cpu_pct": round(cpu_pct, 1),
        "memory_pct": round(mem.percent, 1),
        "memory_used_gb": round(mem.used / (1024 ** 3), 2),
        "memory_total_gb": round(mem.total / (1024 ** 3), 2),
        "disk_pct": round(disk.percent, 1),
        "disk_used_gb": round(disk.used / (1024 ** 3), 2),
        "disk_total_gb": round(disk.total / (1024 ** 3), 2),
    }


def check_blockchain_health() -> dict:
    """Check the latest block and time since last Pulse heartbeat."""
    if Web3 is None:
        return {"status": STATUS_SKIP, "detail": "web3.py not installed"}

    try:
        w3 = Web3(Web3.HTTPProvider(MAMEYNODE_RPC))
        if not w3.is_connected():
            return {"status": STATUS_FAIL, "detail": "MameyNode not connected"}

        latest_block = w3.eth.block_number
        block = w3.eth.get_block("latest")
        block_timestamp = block.get("timestamp", 0)
        block_age = int(time.time()) - block_timestamp

        result = {
            "status": STATUS_OK,
            "latest_block": latest_block,
            "block_age_seconds": block_age,
            "chain_id": w3.eth.chain_id,
        }

        # Check if block is stale (> 5 minutes old)
        if block_age > 300:
            result["status"] = STATUS_WARN
            result["detail"] = f"Latest block is {block_age}s old"
        else:
            result["detail"] = f"Block {latest_block}, {block_age}s ago"

        # Check Pulse heartbeat
        pulse_result = _check_pulse_heartbeat(w3)
        result["pulse"] = pulse_result

        return result

    except Exception as exc:
        return {"status": STATUS_FAIL, "detail": str(exc)}


# Minimal ABI for IerahkwaPulse
PULSE_ABI_FRAGMENT = [
    {
        "inputs": [],
        "name": "getActiveGuardianCount",
        "outputs": [{"name": "", "type": "uint256"}],
        "stateMutability": "view",
        "type": "function",
    },
    {
        "inputs": [],
        "name": "lastHeartbeat",
        "outputs": [{"name": "", "type": "uint256"}],
        "stateMutability": "view",
        "type": "function",
    },
]


def _check_pulse_heartbeat(w3) -> dict:
    """Check IerahkwaPulse contract for last heartbeat and guardian count."""
    try:
        contract = w3.eth.contract(
            address=Web3.to_checksum_address(PULSE_ADDRESS),
            abi=PULSE_ABI_FRAGMENT,
        )

        last_heartbeat = contract.functions.lastHeartbeat().call()
        heartbeat_age = int(time.time()) - last_heartbeat

        active_guardians = contract.functions.getActiveGuardianCount().call()

        status = STATUS_OK
        detail = f"{active_guardians} guardians, heartbeat {heartbeat_age}s ago"

        # Heartbeat older than 30 days is a warning
        if heartbeat_age > 30 * 86400:
            status = STATUS_WARN
            detail = f"Heartbeat stale ({heartbeat_age // 86400}d), {active_guardians} guardians"

        # Heartbeat older than 60 days is critical (pre-Purge territory)
        if heartbeat_age > 60 * 86400:
            status = STATUS_FAIL
            detail = f"FLATLINE: No heartbeat for {heartbeat_age // 86400}d! Pre-Purge imminent."

        return {
            "status": status,
            "detail": detail,
            "last_heartbeat": last_heartbeat,
            "heartbeat_age_seconds": heartbeat_age,
            "active_guardians": active_guardians,
        }

    except Exception as exc:
        return {"status": STATUS_SKIP, "detail": f"Pulse check failed: {exc}"}


def check_guardian_count() -> dict:
    """Standalone check for active Guardian count from IerahkwaPulse."""
    if Web3 is None:
        return {"status": STATUS_SKIP, "detail": "web3.py not installed"}

    try:
        w3 = Web3(Web3.HTTPProvider(MAMEYNODE_RPC))
        if not w3.is_connected():
            return {"status": STATUS_FAIL, "detail": "MameyNode not connected"}

        contract = w3.eth.contract(
            address=Web3.to_checksum_address(PULSE_ADDRESS),
            abi=PULSE_ABI_FRAGMENT,
        )
        count = contract.functions.getActiveGuardianCount().call()

        status = STATUS_OK
        if count < 34:
            status = STATUS_WARN
        if count < 10:
            status = STATUS_FAIL

        return {
            "status": status,
            "active_guardians": count,
            "detail": f"{count} active guardians (minimum quorum: 34)",
        }

    except Exception as exc:
        return {"status": STATUS_FAIL, "detail": str(exc)}


def check_ipfs_storage() -> dict:
    """Check IPFS pinned objects count and repo size."""
    try:
        # Pin count
        resp = http_requests.post(
            f"{IPFS_API_URL}/api/v0/pin/ls",
            params={"type": "recursive"},
            timeout=15,
        )
        if resp.status_code != 200:
            return {"status": STATUS_FAIL, "detail": f"IPFS pin/ls returned {resp.status_code}"}

        keys = resp.json().get("Keys", {})
        pin_count = len(keys)

        # Repo stat
        resp = http_requests.post(f"{IPFS_API_URL}/api/v0/repo/stat", timeout=15)
        repo_size = 0
        if resp.status_code == 200:
            repo_size = resp.json().get("RepoSize", 0)

        # Human-readable
        size_human = _human_bytes(repo_size)

        return {
            "status": STATUS_OK,
            "detail": f"{pin_count} pinned objects, {size_human} total",
            "pin_count": pin_count,
            "repo_size_bytes": repo_size,
            "repo_size_human": size_human,
        }

    except Exception as exc:
        return {"status": STATUS_FAIL, "detail": str(exc)}


def _human_bytes(n: int) -> str:
    for unit in ("B", "KB", "MB", "GB", "TB"):
        if n < 1024:
            return f"{n:.1f} {unit}"
        n /= 1024
    return f"{n:.1f} PB"


# ---------------------------------------------------------------------------
# Report generation
# ---------------------------------------------------------------------------


def run_all_checks() -> dict:
    """Execute all health checks and return structured results."""
    timestamp = datetime.now(timezone.utc).isoformat()
    results = {
        "timestamp": timestamp,
        "services": {},
        "serial_devices": {},
        "meshtastic": {},
        "resources": {},
        "blockchain": {},
        "guardians": {},
        "ipfs": {},
    }

    # TCP services
    for svc in TCP_SERVICES:
        check = check_tcp_service(svc["host"], svc["port"])
        results["services"][svc["name"]] = check

    # Serial devices
    for dev in SERIAL_DEVICES:
        check = check_serial_device(dev["path"])
        results["serial_devices"][dev["name"]] = check

    # Meshtastic gateway
    results["meshtastic"] = check_meshtastic_gateway()

    # System resources
    results["resources"] = check_system_resources()

    # Blockchain health
    results["blockchain"] = check_blockchain_health()

    # Guardian count
    results["guardians"] = check_guardian_count()

    # IPFS storage
    results["ipfs"] = check_ipfs_storage()

    # Compute overall status
    all_statuses = []
    for category in results.values():
        if isinstance(category, dict):
            if "status" in category:
                all_statuses.append(category["status"])
            else:
                for v in category.values():
                    if isinstance(v, dict) and "status" in v:
                        all_statuses.append(v["status"])

    if STATUS_FAIL in all_statuses:
        results["overall"] = "CRITICAL"
    elif STATUS_WARN in all_statuses:
        results["overall"] = "DEGRADED"
    else:
        results["overall"] = "HEALTHY"

    return results


def generate_markdown_report(results: dict) -> str:
    """Generate a Markdown-formatted health report."""
    overall = results.get("overall", "UNKNOWN")
    ts = results.get("timestamp", "")

    indicator = {"HEALTHY": "[OK]", "DEGRADED": "[!!]", "CRITICAL": "[XX]"}.get(
        overall, "[??]"
    )

    lines = [
        f"# Ierahkwa Sentinel Report {indicator}",
        "",
        f"**Timestamp:** {ts}",
        f"**Overall status:** {overall}",
        "",
        "---",
        "",
        "## Service Checks",
        "",
        "| Service | Status | Detail |",
        "|---------|--------|--------|",
    ]

    for name, check in results.get("services", {}).items():
        status_icon = _status_icon(check.get("status", ""))
        detail = check.get("detail", "")
        lines.append(f"| {name} | {status_icon} | {detail} |")

    lines.extend(["", "## Serial Devices", ""])
    lines.extend(["| Device | Status | Detail |", "|--------|--------|--------|"])
    for name, check in results.get("serial_devices", {}).items():
        lines.append(
            f"| {name} | {_status_icon(check.get('status', ''))} | {check.get('detail', '')} |"
        )

    # Meshtastic
    mesh = results.get("meshtastic", {})
    lines.extend([
        "",
        "## Meshtastic Gateway",
        "",
        f"- **Status:** {_status_icon(mesh.get('status', ''))} {mesh.get('detail', '')}",
    ])

    # System resources
    res = results.get("resources", {})
    lines.extend([
        "",
        "## System Resources",
        "",
        f"- **Status:** {_status_icon(res.get('status', ''))} {res.get('detail', '')}",
    ])
    if res.get("cpu_pct") is not None:
        lines.append(f"- **CPU:** {res['cpu_pct']}%")
    if res.get("memory_pct") is not None:
        lines.append(
            f"- **Memory:** {res['memory_pct']}% "
            f"({res.get('memory_used_gb', '?')} / {res.get('memory_total_gb', '?')} GB)"
        )
    if res.get("disk_pct") is not None:
        lines.append(
            f"- **Disk:** {res['disk_pct']}% "
            f"({res.get('disk_used_gb', '?')} / {res.get('disk_total_gb', '?')} GB)"
        )

    # Blockchain
    bc = results.get("blockchain", {})
    lines.extend([
        "",
        "## Blockchain (MameyNode)",
        "",
        f"- **Status:** {_status_icon(bc.get('status', ''))} {bc.get('detail', '')}",
    ])
    if bc.get("latest_block"):
        lines.append(f"- **Latest block:** {bc['latest_block']}")
    if bc.get("chain_id"):
        lines.append(f"- **Chain ID:** {bc['chain_id']}")

    pulse = bc.get("pulse", {})
    if pulse:
        lines.extend([
            "",
            "### Pulse Heartbeat",
            "",
            f"- **Status:** {_status_icon(pulse.get('status', ''))} {pulse.get('detail', '')}",
        ])
        if pulse.get("active_guardians") is not None:
            lines.append(f"- **Active Guardians:** {pulse['active_guardians']}")

    # Guardian count
    gc = results.get("guardians", {})
    lines.extend([
        "",
        "## Guardian Count",
        "",
        f"- **Status:** {_status_icon(gc.get('status', ''))} {gc.get('detail', '')}",
    ])

    # IPFS
    ipfs = results.get("ipfs", {})
    lines.extend([
        "",
        "## IPFS Storage",
        "",
        f"- **Status:** {_status_icon(ipfs.get('status', ''))} {ipfs.get('detail', '')}",
    ])
    if ipfs.get("pin_count") is not None:
        lines.append(f"- **Pinned objects:** {ipfs['pin_count']}")
    if ipfs.get("repo_size_human"):
        lines.append(f"- **Repo size:** {ipfs['repo_size_human']}")

    lines.extend(["", "---", "", "*Ierahkwa Sentinel -- Red Soberana*"])

    return "\n".join(lines)


def _status_icon(status: str) -> str:
    return {
        STATUS_OK: "[OK]",
        STATUS_WARN: "[!!]",
        STATUS_FAIL: "[XX]",
        STATUS_SKIP: "[--]",
    }.get(status, "[??]")


# ---------------------------------------------------------------------------
# Report delivery
# ---------------------------------------------------------------------------


async def send_matrix_report(report: str) -> bool:
    """Send the health report to the sentinel Matrix room."""
    if not MATRIX_PASSWORD or not MATRIX_SENTINEL_ROOM:
        logger.warning("Matrix credentials or room not configured. Skipping Matrix delivery.")
        return False

    client = AsyncClient(MATRIX_HOMESERVER, MATRIX_USER)
    try:
        login_resp = await client.login(MATRIX_PASSWORD)
        if not isinstance(login_resp, LoginResponse):
            logger.error("Matrix login failed: %s", login_resp)
            return False

        await client.room_send(
            MATRIX_SENTINEL_ROOM,
            "m.room.message",
            {
                "msgtype": "m.text",
                "body": report,
                "format": "org.matrix.custom.html",
                "formatted_body": f"<pre>{report}</pre>",
            },
        )
        logger.info("Report sent to Matrix room %s", MATRIX_SENTINEL_ROOM)
        return True

    except Exception as exc:
        logger.error("Matrix report delivery failed: %s", exc)
        return False
    finally:
        await client.close()


def send_ntfy_alert(overall: str, summary: str) -> bool:
    """Push critical alerts to ntfy as a backup notification channel."""
    if overall == "HEALTHY":
        # Only push on degraded or critical
        return True

    priority = "5" if overall == "CRITICAL" else "4"
    title = f"[SENTINEL] Ierahkwa: {overall}"

    try:
        resp = http_requests.post(
            f"{NTFY_URL}/{NTFY_TOPIC}",
            data=summary.encode("utf-8"),
            headers={
                "Title": title,
                "Priority": priority,
                "Tags": "rotating_light,stethoscope" if overall == "CRITICAL" else "warning",
            },
            timeout=15,
        )
        resp.raise_for_status()
        logger.info("ntfy alert sent: %s", overall)
        return True
    except Exception as exc:
        logger.error("ntfy alert failed: %s", exc)
        return False


def save_json_log(results: dict) -> None:
    """Append the results as a JSON line to the sentinel log file."""
    try:
        with open(SENTINEL_LOG_PATH, "a", encoding="utf-8") as fh:
            fh.write(json.dumps(results, ensure_ascii=False) + "\n")
        logger.info("JSON log written to %s", SENTINEL_LOG_PATH)
    except OSError as exc:
        logger.error("Failed to write sentinel JSON log: %s", exc)


def build_ntfy_summary(results: dict) -> str:
    """Build a short text summary for ntfy notifications."""
    lines = [f"Estado: {results.get('overall', 'UNKNOWN')}"]

    # Count failures
    fail_services = []
    for name, check in results.get("services", {}).items():
        if check.get("status") == STATUS_FAIL:
            fail_services.append(name)
    if fail_services:
        lines.append(f"Servicios caidos: {', '.join(fail_services)}")

    res = results.get("resources", {})
    if res.get("status") == STATUS_WARN:
        lines.append(f"Recursos: {res.get('detail', '')}")

    bc = results.get("blockchain", {})
    if bc.get("status") in (STATUS_WARN, STATUS_FAIL):
        lines.append(f"Blockchain: {bc.get('detail', '')}")

    gc = results.get("guardians", {})
    if gc.get("status") in (STATUS_WARN, STATUS_FAIL):
        lines.append(f"Guardianes: {gc.get('detail', '')}")

    return "\n".join(lines)


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------


async def async_main() -> int:
    """Run all checks, generate report, deliver, and return exit code."""
    logger.info("=" * 60)
    logger.info("  Ierahkwa Health Sentinel starting diagnostic")
    logger.info("=" * 60)

    results = run_all_checks()
    overall = results["overall"]

    # Generate Markdown report
    report = generate_markdown_report(results)
    logger.info("Report generated. Overall status: %s", overall)

    # Deliver to Matrix
    await send_matrix_report(report)

    # Deliver critical alerts to ntfy
    summary = build_ntfy_summary(results)
    send_ntfy_alert(overall, summary)

    # Save JSON log
    save_json_log(results)

    # Print report to stdout
    print(report)

    # Return exit code
    if overall == "CRITICAL":
        return 2
    elif overall == "DEGRADED":
        return 1
    return 0


def main() -> None:
    exit_code = asyncio.run(async_main())
    sys.exit(exit_code)


if __name__ == "__main__":
    main()
