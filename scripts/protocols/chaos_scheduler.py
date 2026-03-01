#!/usr/bin/env python3
"""
Ierahkwa Chaos Scheduler -- Monthly Drill System
Designed to run daily via cron. Each day there is a 5% probability of
triggering a drill, with a hard limit of one drill per calendar month.
Drills are announced with the [SIMULACRO] prefix, guardian response times
are measured, and a post-drill report is generated.

Environment variables
---------------------
DRILL_PROBABILITY   Daily probability of drill (default: 0.05)
LOG_DIR             Directory for logs and reports (default: logs/)
LAST_DRILL_FILE     Path to last-drill state file (default: logs/last_drill.txt)
NTFY_URL            ntfy server URL (default: https://ntfy.sh)
NTFY_TOPIC          ntfy topic for drill alerts (default: ierahkwa-drill)
NTFY_AUTH_TOKEN     Optional bearer token
RESPONSE_TIMEOUT    Seconds to wait for guardian responses (default: 1800 = 30 min)
GUARDIAN_LIST_PATH  Path to guardian list JSON (optional)
RESPONSE_LOG_PATH   Path to response log file (default: logs/drill_responses.jsonl)
SIGNING_KEY         Hex-encoded HMAC-SHA256 key (optional)
"""

import hashlib
import hmac
import json
import logging
import os
import random
import sys
import time
from datetime import datetime, timezone, date
from pathlib import Path

try:
    import requests
except ImportError:
    sys.exit("requests is required: pip install requests")

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------

DRILL_PROBABILITY = float(os.environ.get("DRILL_PROBABILITY", "0.05"))
LOG_DIR = Path(os.environ.get("LOG_DIR", "logs"))
LAST_DRILL_FILE = Path(os.environ.get("LAST_DRILL_FILE", "logs/last_drill.txt"))
NTFY_URL = os.environ.get("NTFY_URL", "https://ntfy.sh")
NTFY_TOPIC = os.environ.get("NTFY_TOPIC", "ierahkwa-drill")
NTFY_AUTH_TOKEN = os.environ.get("NTFY_AUTH_TOKEN", "")
RESPONSE_TIMEOUT = int(os.environ.get("RESPONSE_TIMEOUT", "1800"))
GUARDIAN_LIST_PATH = os.environ.get("GUARDIAN_LIST_PATH", "")
RESPONSE_LOG_PATH = Path(os.environ.get("RESPONSE_LOG_PATH", "logs/drill_responses.jsonl"))
SIGNING_KEY = os.environ.get("SIGNING_KEY", "")

LOG_DIR.mkdir(parents=True, exist_ok=True)
LAST_DRILL_FILE.parent.mkdir(parents=True, exist_ok=True)
RESPONSE_LOG_PATH.parent.mkdir(parents=True, exist_ok=True)

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(name)s: %(message)s",
    handlers=[
        logging.StreamHandler(sys.stdout),
        logging.FileHandler(LOG_DIR / "chaos_scheduler.log", encoding="utf-8"),
    ],
)
logger = logging.getLogger("ierahkwa.chaos")

# ---------------------------------------------------------------------------
# Drill state management
# ---------------------------------------------------------------------------


def read_last_drill() -> dict:
    """Read the last drill state from disk.

    Returns a dict with keys: date (str YYYY-MM-DD), month (str YYYY-MM),
    drill_id (str), or empty dict if no state exists.
    """
    if not LAST_DRILL_FILE.exists():
        return {}
    try:
        with open(LAST_DRILL_FILE, "r", encoding="utf-8") as fh:
            return json.load(fh)
    except (json.JSONDecodeError, OSError) as exc:
        logger.warning("Could not read last drill state: %s", exc)
        return {}


def write_last_drill(state: dict) -> None:
    """Persist the drill state to disk."""
    with open(LAST_DRILL_FILE, "w", encoding="utf-8") as fh:
        json.dump(state, fh, indent=2)


def already_drilled_this_month() -> bool:
    """Return True if a drill has already occurred in the current calendar month."""
    state = read_last_drill()
    if not state:
        return False
    current_month = date.today().strftime("%Y-%m")
    return state.get("month", "") == current_month


def should_trigger_drill() -> bool:
    """Decide whether to trigger a drill today."""
    if already_drilled_this_month():
        logger.info("Drill already completed this month. Skipping.")
        return False
    roll = random.random()
    logger.info("Drill probability roll: %.4f (threshold: %.4f)", roll, DRILL_PROBABILITY)
    return roll < DRILL_PROBABILITY

# ---------------------------------------------------------------------------
# Notification
# ---------------------------------------------------------------------------


def compute_signature(message: str, timestamp: str) -> str:
    if not SIGNING_KEY:
        return ""
    key_bytes = bytes.fromhex(SIGNING_KEY)
    data = f"{timestamp}:{message}".encode("utf-8")
    return hmac.new(key_bytes, data, hashlib.sha256).hexdigest()


def send_drill_notification(drill_id: str, message: str) -> bool:
    """Send the drill notification via ntfy."""
    timestamp = datetime.now(timezone.utc).isoformat()
    signature = compute_signature(message, timestamp)

    url = f"{NTFY_URL}/{NTFY_TOPIC}"
    headers = {
        "Title": f"[SIMULACRO] Drill {drill_id}",
        "Priority": "high",
        "Tags": "loudspeaker,test_tube",
        "X-Timestamp": timestamp,
        "X-Drill-ID": drill_id,
    }
    if signature:
        headers["X-Signature"] = signature
    if NTFY_AUTH_TOKEN:
        headers["Authorization"] = f"Bearer {NTFY_AUTH_TOKEN}"

    try:
        resp = requests.post(
            url, data=message.encode("utf-8"), headers=headers, timeout=15
        )
        resp.raise_for_status()
        logger.info("Drill notification sent: %s", drill_id)
        return True
    except requests.RequestException as exc:
        logger.error("Failed to send drill notification: %s", exc)
        return False

# ---------------------------------------------------------------------------
# Guardian response tracking
# ---------------------------------------------------------------------------


def load_guardians() -> list[dict]:
    """Load guardian list if available."""
    if not GUARDIAN_LIST_PATH:
        return []
    path = Path(GUARDIAN_LIST_PATH)
    if not path.exists():
        return []
    try:
        with open(path, "r", encoding="utf-8") as fh:
            return json.load(fh)
    except (json.JSONDecodeError, OSError):
        return []


def collect_responses(drill_id: str, start_time: float) -> list[dict]:
    """Collect guardian responses from the response log.

    In a production system this would poll a Matrix room or webhook endpoint.
    Here we read from a JSONL file that guardians (or an automated system)
    append to when they acknowledge the drill.

    Each line: {"drill_id": "...", "guardian": "...", "ack_time": "ISO8601"}
    """
    responses = []
    if not RESPONSE_LOG_PATH.exists():
        return responses

    try:
        with open(RESPONSE_LOG_PATH, "r", encoding="utf-8") as fh:
            for line in fh:
                line = line.strip()
                if not line:
                    continue
                try:
                    record = json.loads(line)
                    if record.get("drill_id") == drill_id:
                        ack_iso = record.get("ack_time", "")
                        if ack_iso:
                            ack_dt = datetime.fromisoformat(ack_iso)
                            response_seconds = (ack_dt.timestamp() - start_time)
                            record["response_seconds"] = max(0, response_seconds)
                        responses.append(record)
                except json.JSONDecodeError:
                    continue
    except OSError as exc:
        logger.warning("Could not read response log: %s", exc)

    return responses

# ---------------------------------------------------------------------------
# Post-drill report
# ---------------------------------------------------------------------------


def generate_report(drill_id: str, start_time: float, responses: list[dict]) -> str:
    """Generate a post-drill report as a text string."""
    guardians = load_guardians()
    total_guardians = len(guardians) if guardians else "unknown"
    responded = len(responses)

    lines = [
        "=" * 60,
        f"  IERAHKWA DRILL REPORT -- {drill_id}",
        "=" * 60,
        "",
        f"Drill triggered at: {datetime.fromtimestamp(start_time, tz=timezone.utc).isoformat()}",
        f"Report generated at: {datetime.now(timezone.utc).isoformat()}",
        f"Total guardians: {total_guardians}",
        f"Responses received: {responded}",
        "",
    ]

    if responses:
        response_times = [r.get("response_seconds", 0) for r in responses if "response_seconds" in r]
        if response_times:
            avg_time = sum(response_times) / len(response_times)
            min_time = min(response_times)
            max_time = max(response_times)
            lines.append("Response Time Statistics:")
            lines.append(f"  Fastest: {min_time:.1f}s")
            lines.append(f"  Slowest: {max_time:.1f}s")
            lines.append(f"  Average: {avg_time:.1f}s")
            lines.append("")

        lines.append("Individual Responses:")
        lines.append("-" * 40)
        for r in sorted(responses, key=lambda x: x.get("response_seconds", 9999)):
            guardian = r.get("guardian", "unknown")
            rt = r.get("response_seconds")
            rt_str = f"{rt:.1f}s" if rt is not None else "N/A"
            lines.append(f"  {guardian}: {rt_str}")
        lines.append("")
    else:
        lines.append("No guardian responses were recorded.")
        lines.append("")

    if isinstance(total_guardians, int) and total_guardians > 0:
        response_rate = (responded / total_guardians) * 100
        lines.append(f"Response rate: {response_rate:.1f}%")
        if response_rate < 50:
            lines.append("STATUS: BAJO -- Response rate below 50%. Review needed.")
        elif response_rate < 80:
            lines.append("STATUS: ACEPTABLE -- Response rate between 50-80%.")
        else:
            lines.append("STATUS: OPTIMO -- Response rate above 80%.")
    else:
        lines.append("Response rate: Cannot calculate (guardian list unavailable).")

    lines.append("")
    lines.append("=" * 60)

    report_text = "\n".join(lines)
    return report_text

# ---------------------------------------------------------------------------
# Main drill execution
# ---------------------------------------------------------------------------


def execute_drill() -> None:
    """Execute a full drill cycle: notify, wait, collect, report."""
    today = date.today()
    drill_id = f"DRILL-{today.strftime('%Y%m%d')}-{random.randint(1000, 9999)}"
    start_time = time.time()

    logger.info("Triggering drill: %s", drill_id)

    message = (
        f"[SIMULACRO] Drill ID: {drill_id}\n"
        f"Fecha: {today.isoformat()}\n"
        f"Hora UTC: {datetime.now(timezone.utc).strftime('%H:%M:%S')}\n\n"
        "ESTO ES UN SIMULACRO. Responda con su codigo de guardian "
        "para confirmar recepcion.\n\n"
        "THIS IS A DRILL. Respond with your guardian code to confirm receipt."
    )

    sent = send_drill_notification(drill_id, message)
    if not sent:
        logger.error("Drill notification failed. Aborting drill %s.", drill_id)
        return

    # Record drill state
    state = {
        "date": today.isoformat(),
        "month": today.strftime("%Y-%m"),
        "drill_id": drill_id,
        "start_time": start_time,
        "notification_sent": True,
    }
    write_last_drill(state)

    # Wait for responses
    logger.info(
        "Waiting %d seconds for guardian responses...", RESPONSE_TIMEOUT
    )
    time.sleep(RESPONSE_TIMEOUT)

    # Collect and report
    responses = collect_responses(drill_id, start_time)
    report = generate_report(drill_id, start_time, responses)

    # Save report
    report_path = LOG_DIR / f"drill_report_{drill_id}.txt"
    with open(report_path, "w", encoding="utf-8") as fh:
        fh.write(report)
    logger.info("Drill report saved to %s", report_path)

    # Log to stdout
    print(report)

    # Send report summary via ntfy
    summary = (
        f"[SIMULACRO COMPLETADO] {drill_id}\n"
        f"Respuestas: {len(responses)}\n"
        f"Reporte: {report_path}"
    )
    send_drill_notification(drill_id, summary)

    # Update state
    state["completed"] = True
    state["responses"] = len(responses)
    state["report_path"] = str(report_path)
    write_last_drill(state)

    logger.info("Drill %s completed.", drill_id)


def main() -> None:
    """Entry point: decide whether to drill, then execute if yes."""
    logger.info("Chaos Scheduler running. Date: %s", date.today().isoformat())

    if should_trigger_drill():
        logger.info("Drill triggered by probability roll.")
        execute_drill()
    else:
        logger.info("No drill today.")


if __name__ == "__main__":
    main()
