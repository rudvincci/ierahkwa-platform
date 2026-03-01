#!/usr/bin/env python3
"""
Ierahkwa Guardian Notification System -- ntfy.sh Push Notifications
Provides a reusable send_alert() function and a CLI entry point for sending
prioritised, rate-limited, digitally-signed push notifications to guardians
via ntfy (self-hosted or ntfy.sh).

Environment variables
---------------------
NTFY_URL              ntfy server URL              (default: https://ntfy.sh)
NTFY_TOPIC_PREFIX     Base topic prefix            (default: ierahkwa)
NTFY_AUTH_TOKEN       Optional bearer token for authenticated servers
SIGNING_KEY           Hex-encoded HMAC-SHA256 key  (optional, for signature verification)
RATE_LIMIT_RED        Min seconds between RED alerts per region (default: 3600)
RATE_LIMIT_WARNING    Min seconds between WARNING alerts per region (default: 600)
GUARDIAN_LIST_PATH    Path to JSON file listing guardians and regions (optional)
MAX_RETRIES           Retry count for HTTP failures (default: 3)
RETRY_DELAY           Seconds between retries      (default: 5)
LOG_DIR               Logging directory             (default: logs/)
"""

import hashlib
import hmac
import json
import logging
import os
import sys
import time
from datetime import datetime, timezone
from pathlib import Path
from typing import Optional

try:
    import requests
except ImportError:
    sys.exit("requests is required: pip install requests")

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------

NTFY_URL = os.environ.get("NTFY_URL", "https://ntfy.sh")
NTFY_TOPIC_PREFIX = os.environ.get("NTFY_TOPIC_PREFIX", "ierahkwa")
NTFY_AUTH_TOKEN = os.environ.get("NTFY_AUTH_TOKEN", "")
SIGNING_KEY = os.environ.get("SIGNING_KEY", "")
RATE_LIMIT_RED = int(os.environ.get("RATE_LIMIT_RED", "3600"))
RATE_LIMIT_WARNING = int(os.environ.get("RATE_LIMIT_WARNING", "600"))
GUARDIAN_LIST_PATH = os.environ.get("GUARDIAN_LIST_PATH", "")
MAX_RETRIES = int(os.environ.get("MAX_RETRIES", "3"))
RETRY_DELAY = float(os.environ.get("RETRY_DELAY", "5"))
LOG_DIR = Path(os.environ.get("LOG_DIR", "logs"))

LOG_DIR.mkdir(parents=True, exist_ok=True)

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(name)s: %(message)s",
    handlers=[
        logging.StreamHandler(sys.stdout),
        logging.FileHandler(LOG_DIR / "notify_guardians.log", encoding="utf-8"),
    ],
)
logger = logging.getLogger("ierahkwa.notify")

# ---------------------------------------------------------------------------
# Rate limiter state
# ---------------------------------------------------------------------------

# Key: (level, region) -> timestamp of last alert
_rate_limit_tracker: dict[tuple[int, str], float] = {}


def _is_rate_limited(level: int, region: str) -> bool:
    """Check whether an alert at the given level/region is rate-limited."""
    key = (level, region)
    now = time.time()
    last = _rate_limit_tracker.get(key)
    if last is None:
        return False
    if level >= 5:
        return (now - last) < RATE_LIMIT_RED
    if level >= 4:
        return (now - last) < RATE_LIMIT_WARNING
    # Info-level alerts are not rate-limited
    return False


def _record_alert(level: int, region: str) -> None:
    _rate_limit_tracker[(level, region)] = time.time()

# ---------------------------------------------------------------------------
# Digital signature
# ---------------------------------------------------------------------------


def compute_signature(message: str, timestamp: str) -> str:
    """Compute HMAC-SHA256 signature of <timestamp>:<message>."""
    if not SIGNING_KEY:
        return ""
    key_bytes = bytes.fromhex(SIGNING_KEY)
    data = f"{timestamp}:{message}".encode("utf-8")
    return hmac.new(key_bytes, data, hashlib.sha256).hexdigest()


def verify_signature(message: str, timestamp: str, signature: str) -> bool:
    """Verify an HMAC-SHA256 signature."""
    if not SIGNING_KEY:
        return True  # No key configured, skip verification
    expected = compute_signature(message, timestamp)
    return hmac.compare_digest(expected, signature)

# ---------------------------------------------------------------------------
# Guardian list
# ---------------------------------------------------------------------------


def load_guardians() -> list[dict]:
    """Load guardian list from JSON file.

    Expected format:
    [
        {"name": "Guardian One", "region": "MX-OAX", "topic": "guardian-one"},
        ...
    ]
    """
    if not GUARDIAN_LIST_PATH:
        return []
    path = Path(GUARDIAN_LIST_PATH)
    if not path.exists():
        logger.warning("Guardian list not found at %s", path)
        return []
    with open(path, "r", encoding="utf-8") as fh:
        data = json.load(fh)
    if not isinstance(data, list):
        logger.error("Guardian list must be a JSON array.")
        return []
    return data

# ---------------------------------------------------------------------------
# Core alert function
# ---------------------------------------------------------------------------

# ntfy priority mapping
LEVEL_MAP = {
    1: ("min", "Diagnostico"),
    2: ("low", "Informativo"),
    3: ("default", "Informativo"),
    4: ("high", "Advertencia"),
    5: ("urgent", "ALERTA CRITICA"),
}


def send_alert(
    level: int,
    message: str,
    region: str = "global",
    topic_override: Optional[str] = None,
) -> bool:
    """Send a prioritised push notification via ntfy.

    Parameters
    ----------
    level : int
        Priority level: 3=info, 4=warning, 5=critical.
    message : str
        Notification body text.
    region : str
        Geographic region code (used for rate limiting and routing).
    topic_override : str, optional
        Send to a specific topic instead of the default.

    Returns
    -------
    bool
        True if the notification was delivered successfully.
    """
    level = max(1, min(5, level))

    if _is_rate_limited(level, region):
        logger.info(
            "Alert suppressed (rate limit) -- level=%d region=%s", level, region
        )
        return False

    priority_str, label = LEVEL_MAP.get(level, ("default", "Info"))
    timestamp = datetime.now(timezone.utc).isoformat()
    signature = compute_signature(message, timestamp)

    topic = topic_override or f"{NTFY_TOPIC_PREFIX}-{region}"
    url = f"{NTFY_URL}/{topic}"

    headers = {
        "Title": f"[{label}] Ierahkwa -- {region}",
        "Priority": priority_str,
        "Tags": _level_tags(level),
        "X-Timestamp": timestamp,
    }
    if signature:
        headers["X-Signature"] = signature
    if NTFY_AUTH_TOKEN:
        headers["Authorization"] = f"Bearer {NTFY_AUTH_TOKEN}"

    body = message.encode("utf-8")

    for attempt in range(1, MAX_RETRIES + 1):
        try:
            resp = requests.post(url, data=body, headers=headers, timeout=15)
            resp.raise_for_status()
            _record_alert(level, region)
            logger.info(
                "Alert sent -- level=%d region=%s topic=%s", level, region, topic
            )
            return True
        except requests.RequestException as exc:
            logger.warning(
                "ntfy attempt %d/%d failed: %s", attempt, MAX_RETRIES, exc
            )
            if attempt < MAX_RETRIES:
                time.sleep(RETRY_DELAY)

    logger.error("Failed to send alert after %d attempts.", MAX_RETRIES)
    return False


def _level_tags(level: int) -> str:
    if level >= 5:
        return "rotating_light,skull"
    if level >= 4:
        return "warning"
    return "information_source"

# ---------------------------------------------------------------------------
# Batch notification
# ---------------------------------------------------------------------------


def notify_all_guardians(level: int, message: str, region_filter: Optional[str] = None) -> dict:
    """Send an alert to all guardians, optionally filtered by region.

    Returns a dict with counts: {"sent": N, "skipped": N, "failed": N}.
    """
    guardians = load_guardians()
    if not guardians:
        # Fall back to default topic
        success = send_alert(level, message)
        return {"sent": int(success), "skipped": 0, "failed": int(not success)}

    stats = {"sent": 0, "skipped": 0, "failed": 0}
    for guardian in guardians:
        g_region = guardian.get("region", "global")
        g_topic = guardian.get("topic", "")
        if region_filter and g_region != region_filter:
            stats["skipped"] += 1
            continue
        if not g_topic:
            stats["skipped"] += 1
            continue

        full_topic = f"{NTFY_TOPIC_PREFIX}-{g_topic}"
        success = send_alert(level, message, region=g_region, topic_override=full_topic)
        if success:
            stats["sent"] += 1
        else:
            stats["failed"] += 1

    logger.info(
        "Batch notification complete -- sent=%d skipped=%d failed=%d",
        stats["sent"],
        stats["skipped"],
        stats["failed"],
    )
    return stats

# ---------------------------------------------------------------------------
# CLI
# ---------------------------------------------------------------------------


def main() -> None:
    import argparse

    parser = argparse.ArgumentParser(
        description="Ierahkwa Guardian Notification System"
    )
    parser.add_argument(
        "level", type=int, choices=[1, 2, 3, 4, 5], help="Priority level (3=info, 4=warn, 5=critical)"
    )
    parser.add_argument("message", help="Notification message body")
    parser.add_argument(
        "--region", default="global", help="Target region code (default: global)"
    )
    parser.add_argument(
        "--batch", action="store_true", help="Send to all guardians in the list"
    )
    parser.add_argument(
        "--topic", default=None, help="Override the ntfy topic"
    )
    args = parser.parse_args()

    if args.batch:
        result = notify_all_guardians(args.level, args.message, region_filter=args.region if args.region != "global" else None)
        print(json.dumps(result, indent=2))
    else:
        success = send_alert(args.level, args.message, args.region, topic_override=args.topic)
        sys.exit(0 if success else 1)


if __name__ == "__main__":
    main()
