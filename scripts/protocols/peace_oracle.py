#!/usr/bin/env python3
"""
Ierahkwa Peace Oracle -- ACLED Conflict Data Monitor
Fetches conflict events for the Americas region from the ACLED API, classifies
each event by severity (GREEN / YELLOW / RED), stores them in a local SQLite
database for offline access, and pushes ntfy notifications when RED-level
events are detected.

Environment variables
---------------------
ACLED_API_KEY       ACLED API key (required)
ACLED_EMAIL         Email registered with ACLED (required)
ACLED_BASE_URL      ACLED API base URL (default: https://api.acleddata.com/acled/read)
POLL_INTERVAL       Seconds between polls (default: 3600 = 1 hour)
DB_PATH             SQLite database path (default: data/peace_oracle.db)
NTFY_URL            ntfy server URL (default: https://ntfy.sh)
NTFY_TOPIC          ntfy topic for alerts (default: ierahkwa-peace)
LOG_DIR             Logging directory (default: logs/)
MAX_RETRIES         Retry attempts for HTTP calls (default: 3)
RETRY_DELAY         Seconds between retries (default: 10)
RATE_LIMIT_DELAY    Minimum seconds between API calls (default: 5)
"""

import json
import logging
import os
import sqlite3
import sys
import time
from datetime import datetime, timezone
from pathlib import Path

try:
    import requests
except ImportError:
    sys.exit("requests is required: pip install requests")

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------

ACLED_API_KEY = os.environ.get("ACLED_API_KEY", "")
ACLED_EMAIL = os.environ.get("ACLED_EMAIL", "")
ACLED_BASE_URL = os.environ.get("ACLED_BASE_URL", "https://api.acleddata.com/acled/read")
POLL_INTERVAL = int(os.environ.get("POLL_INTERVAL", "3600"))
DB_PATH = Path(os.environ.get("DB_PATH", "data/peace_oracle.db"))
NTFY_URL = os.environ.get("NTFY_URL", "https://ntfy.sh")
NTFY_TOPIC = os.environ.get("NTFY_TOPIC", "ierahkwa-peace")
LOG_DIR = Path(os.environ.get("LOG_DIR", "logs"))
MAX_RETRIES = int(os.environ.get("MAX_RETRIES", "3"))
RETRY_DELAY = float(os.environ.get("RETRY_DELAY", "10"))
RATE_LIMIT_DELAY = float(os.environ.get("RATE_LIMIT_DELAY", "5"))

LOG_DIR.mkdir(parents=True, exist_ok=True)
DB_PATH.parent.mkdir(parents=True, exist_ok=True)

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(name)s: %(message)s",
    handlers=[
        logging.StreamHandler(sys.stdout),
        logging.FileHandler(LOG_DIR / "peace_oracle.log", encoding="utf-8"),
    ],
)
logger = logging.getLogger("ierahkwa.peace_oracle")

# ---------------------------------------------------------------------------
# Severity classification
# ---------------------------------------------------------------------------

# ACLED event types mapped to severity levels.
# See https://acleddata.com/acleddatanew/wp-content/uploads/2021/11/ACLED_Codebook_2021.pdf
RED_TYPES = {
    "Battles",
    "Explosions/Remote violence",
    "Violence against civilians",
}
YELLOW_TYPES = {
    "Riots",
    "Strategic developments",
}
GREEN_TYPES = {
    "Protests",
}

# Sub-event overrides that escalate severity regardless of main type
RED_SUB_EVENTS = {
    "Armed clash",
    "Air/drone strike",
    "Shelling/artillery/missile attack",
    "Chemical weapon",
    "Attack",
    "Abduction/forced disappearance",
    "Sexual violence",
}


def classify_severity(event_type: str, sub_event_type: str, fatalities: int) -> str:
    """Return 'RED', 'YELLOW', or 'GREEN' based on event characteristics."""
    if sub_event_type in RED_SUB_EVENTS:
        return "RED"
    if fatalities > 0:
        return "RED"
    if event_type in RED_TYPES:
        return "RED"
    if event_type in YELLOW_TYPES:
        return "YELLOW"
    return "GREEN"

# ---------------------------------------------------------------------------
# SQLite database
# ---------------------------------------------------------------------------


def init_db(db_path: Path) -> sqlite3.Connection:
    """Create the events table if it does not exist and return a connection."""
    conn = sqlite3.connect(str(db_path))
    conn.execute("PRAGMA journal_mode=WAL")
    conn.execute(
        """
        CREATE TABLE IF NOT EXISTS events (
            data_id         TEXT PRIMARY KEY,
            event_date      TEXT NOT NULL,
            event_type      TEXT NOT NULL,
            sub_event_type  TEXT,
            country         TEXT,
            admin1          TEXT,
            location        TEXT,
            fatalities      INTEGER DEFAULT 0,
            severity        TEXT NOT NULL,
            notes           TEXT,
            source          TEXT,
            fetched_at      TEXT NOT NULL
        )
        """
    )
    conn.execute(
        "CREATE INDEX IF NOT EXISTS idx_events_severity ON events (severity)"
    )
    conn.execute(
        "CREATE INDEX IF NOT EXISTS idx_events_date ON events (event_date DESC)"
    )
    conn.commit()
    return conn


def store_events(conn: sqlite3.Connection, events: list[dict]) -> list[dict]:
    """Insert new events into the database. Return list of newly inserted events."""
    new_events = []
    now = datetime.now(timezone.utc).isoformat()
    for ev in events:
        data_id = ev.get("data_id", "")
        if not data_id:
            continue
        try:
            conn.execute(
                """
                INSERT OR IGNORE INTO events
                    (data_id, event_date, event_type, sub_event_type, country,
                     admin1, location, fatalities, severity, notes, source, fetched_at)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
                """,
                (
                    data_id,
                    ev.get("event_date", ""),
                    ev.get("event_type", ""),
                    ev.get("sub_event_type", ""),
                    ev.get("country", ""),
                    ev.get("admin1", ""),
                    ev.get("location", ""),
                    int(ev.get("fatalities", 0)),
                    ev["_severity"],
                    ev.get("notes", ""),
                    ev.get("source", ""),
                    now,
                ),
            )
            if conn.total_changes:
                new_events.append(ev)
        except sqlite3.IntegrityError:
            pass
    conn.commit()
    return new_events

# ---------------------------------------------------------------------------
# ACLED API
# ---------------------------------------------------------------------------


def fetch_acled_events(page: int = 1, page_size: int = 500) -> list[dict]:
    """Fetch a page of conflict events for the Americas from ACLED."""
    params = {
        "key": ACLED_API_KEY,
        "email": ACLED_EMAIL,
        "region": 7,  # Americas
        "limit": page_size,
        "page": page,
    }
    for attempt in range(1, MAX_RETRIES + 1):
        try:
            resp = requests.get(ACLED_BASE_URL, params=params, timeout=60)
            if resp.status_code == 429:
                wait = RATE_LIMIT_DELAY * attempt
                logger.warning("Rate limited. Waiting %.0f seconds...", wait)
                time.sleep(wait)
                continue
            resp.raise_for_status()
            payload = resp.json()
            data = payload.get("data", [])
            logger.info("Fetched %d events from ACLED (page %d)", len(data), page)
            return data
        except requests.RequestException as exc:
            logger.warning("ACLED fetch attempt %d/%d failed: %s", attempt, MAX_RETRIES, exc)
            if attempt < MAX_RETRIES:
                time.sleep(RETRY_DELAY)
    logger.error("All ACLED fetch attempts failed.")
    return []

# ---------------------------------------------------------------------------
# Notifications via ntfy
# ---------------------------------------------------------------------------

_last_red_alert: dict[str, float] = {}  # region -> timestamp


def send_ntfy_alert(event: dict) -> None:
    """Send a push notification via ntfy for a RED-level event."""
    region = event.get("country", "Unknown")
    now = time.time()

    # Rate limit: max 1 RED alert per hour per region
    if region in _last_red_alert and (now - _last_red_alert[region]) < 3600:
        logger.info("RED alert for %s suppressed (rate limit).", region)
        return

    title = f"[ALERTA ROJA] {event.get('event_type', 'Conflicto')} en {region}"
    body = (
        f"Ubicacion: {event.get('location', 'N/A')}, {event.get('admin1', '')}\n"
        f"Fecha: {event.get('event_date', 'N/A')}\n"
        f"Fatalidades: {event.get('fatalities', 0)}\n"
        f"Fuente: {event.get('source', 'N/A')}\n"
        f"Detalles: {(event.get('notes', '') or '')[:300]}"
    )
    headers = {
        "Title": title,
        "Priority": "5",
        "Tags": "warning,skull",
    }
    url = f"{NTFY_URL}/{NTFY_TOPIC}"
    for attempt in range(1, MAX_RETRIES + 1):
        try:
            resp = requests.post(url, data=body.encode("utf-8"), headers=headers, timeout=15)
            resp.raise_for_status()
            _last_red_alert[region] = now
            logger.info("RED alert sent for %s via ntfy.", region)
            return
        except requests.RequestException as exc:
            logger.warning("ntfy attempt %d/%d failed: %s", attempt, MAX_RETRIES, exc)
            if attempt < MAX_RETRIES:
                time.sleep(RETRY_DELAY)
    logger.error("Failed to send ntfy alert for %s.", region)

# ---------------------------------------------------------------------------
# Main loop
# ---------------------------------------------------------------------------


def run_cycle(conn: sqlite3.Connection) -> dict:
    """Run a single fetch-classify-store-notify cycle. Return summary dict."""
    raw_events = fetch_acled_events()
    if not raw_events:
        return {"fetched": 0, "new": 0, "red": 0, "yellow": 0, "green": 0}

    for ev in raw_events:
        ev["_severity"] = classify_severity(
            ev.get("event_type", ""),
            ev.get("sub_event_type", ""),
            int(ev.get("fatalities", 0)),
        )

    new_events = store_events(conn, raw_events)

    counts = {"fetched": len(raw_events), "new": len(new_events), "red": 0, "yellow": 0, "green": 0}
    for ev in new_events:
        sev = ev["_severity"]
        if sev == "RED":
            counts["red"] += 1
            send_ntfy_alert(ev)
        elif sev == "YELLOW":
            counts["yellow"] += 1
        else:
            counts["green"] += 1

    logger.info(
        "Cycle complete -- fetched=%d new=%d RED=%d YELLOW=%d GREEN=%d",
        counts["fetched"],
        counts["new"],
        counts["red"],
        counts["yellow"],
        counts["green"],
    )
    return counts


def main() -> None:
    if not ACLED_API_KEY or not ACLED_EMAIL:
        sys.exit("ACLED_API_KEY and ACLED_EMAIL environment variables are required.")

    conn = init_db(DB_PATH)
    logger.info(
        "Peace Oracle started. Polling every %d seconds. DB: %s",
        POLL_INTERVAL,
        DB_PATH,
    )

    try:
        while True:
            try:
                run_cycle(conn)
            except Exception as exc:
                logger.exception("Error during cycle: %s", exc)
            time.sleep(POLL_INTERVAL)
    except KeyboardInterrupt:
        logger.info("Shutting down on keyboard interrupt.")
    finally:
        conn.close()
        logger.info("Peace Oracle stopped.")


if __name__ == "__main__":
    main()
