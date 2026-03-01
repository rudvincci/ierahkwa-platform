#!/usr/bin/env python3
"""
Ierahkwa AI Mediator -- Matrix-Nio + Ollama Bridge
Connects to a Matrix homeserver, listens for messages across all joined rooms,
and runs empathy / toxicity analysis through a local Ollama model.
When toxicity exceeds the configured threshold the bot suggests a reformulated,
non-violent version of the message and logs every mediation for transparency.

Environment variables
---------------------
MATRIX_HOMESERVER   Matrix server URL          (default: https://matrix.ierahkwa.org)
MATRIX_USER         Full Matrix user ID        (e.g. @mediator:ierahkwa.org)
MATRIX_PASSWORD     Password for the bot user
OLLAMA_URL          Ollama API endpoint        (default: http://localhost:11434)
OLLAMA_MODEL        Model to use               (default: mistral)
TOXICITY_THRESHOLD  Float 0-1                  (default: 0.55)
LOG_DIR             Directory for mediation logs (default: logs/)
MAX_RETRIES         Retry count for transient failures (default: 3)
RETRY_DELAY         Seconds between retries    (default: 5)
"""

import asyncio
import json
import logging
import os
import sys
import time
from datetime import datetime, timezone
from pathlib import Path

try:
    from nio import AsyncClient, MatrixRoom, RoomMessageText, LoginResponse
except ImportError:
    sys.exit("matrix-nio is required: pip install matrix-nio")

try:
    import aiohttp
except ImportError:
    sys.exit("aiohttp is required: pip install aiohttp")

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------

MATRIX_HOMESERVER = os.environ.get("MATRIX_HOMESERVER", "https://matrix.ierahkwa.org")
MATRIX_USER = os.environ.get("MATRIX_USER", "@mediator:ierahkwa.org")
MATRIX_PASSWORD = os.environ.get("MATRIX_PASSWORD", "")
OLLAMA_URL = os.environ.get("OLLAMA_URL", "http://localhost:11434")
OLLAMA_MODEL = os.environ.get("OLLAMA_MODEL", "mistral")
TOXICITY_THRESHOLD = float(os.environ.get("TOXICITY_THRESHOLD", "0.55"))
LOG_DIR = Path(os.environ.get("LOG_DIR", "logs"))
MAX_RETRIES = int(os.environ.get("MAX_RETRIES", "3"))
RETRY_DELAY = float(os.environ.get("RETRY_DELAY", "5"))

LOG_DIR.mkdir(parents=True, exist_ok=True)

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(name)s: %(message)s",
    handlers=[
        logging.StreamHandler(sys.stdout),
        logging.FileHandler(LOG_DIR / "mediator.log", encoding="utf-8"),
    ],
)
logger = logging.getLogger("ierahkwa.mediator")

# ---------------------------------------------------------------------------
# Ollama helpers
# ---------------------------------------------------------------------------

ANALYSIS_PROMPT = (
    "You are an empathy and conflict-resolution analyst. "
    "Given the following message from a community discussion, respond ONLY with "
    "valid JSON containing exactly two keys:\n"
    '  "toxicity": a float between 0.0 (completely peaceful) and 1.0 (extremely toxic),\n'
    '  "reformulation": a non-violent, empathetic version of the message that preserves '
    "the speaker's intent while removing hostility.\n\n"
    "Message:\n{message}\n\n"
    "Respond ONLY with the JSON object, no markdown, no extra text."
)


async def _ollama_generate(session: aiohttp.ClientSession, prompt: str) -> str:
    """Call Ollama /api/generate and return the assembled response text."""
    payload = {
        "model": OLLAMA_MODEL,
        "prompt": prompt,
        "stream": False,
        "options": {"temperature": 0.3},
    }
    url = f"{OLLAMA_URL}/api/generate"
    async with session.post(url, json=payload, timeout=aiohttp.ClientTimeout(total=120)) as resp:
        resp.raise_for_status()
        data = await resp.json()
        return data.get("response", "")


async def analyse_message(session: aiohttp.ClientSession, message: str) -> dict:
    """Return {"toxicity": float, "reformulation": str} or raise on failure."""
    prompt = ANALYSIS_PROMPT.format(message=message)
    raw = await _ollama_generate(session, prompt)
    raw = raw.strip()
    # Strip markdown fences if the model wraps in ```json ... ```
    if raw.startswith("```"):
        raw = raw.split("\n", 1)[-1].rsplit("```", 1)[0].strip()
    result = json.loads(raw)
    toxicity = float(result["toxicity"])
    reformulation = str(result["reformulation"])
    return {"toxicity": toxicity, "reformulation": reformulation}

# ---------------------------------------------------------------------------
# Mediation log
# ---------------------------------------------------------------------------

_mediation_log_path = LOG_DIR / "mediations.jsonl"


def log_mediation(room_id: str, sender: str, original: str, analysis: dict) -> None:
    """Append a mediation record to the JSONL log file."""
    record = {
        "timestamp": datetime.now(timezone.utc).isoformat(),
        "room_id": room_id,
        "sender": sender,
        "original_message": original,
        "toxicity": analysis["toxicity"],
        "reformulation": analysis["reformulation"],
    }
    with open(_mediation_log_path, "a", encoding="utf-8") as fh:
        fh.write(json.dumps(record, ensure_ascii=False) + "\n")
    logger.info(
        "Mediation logged -- room=%s sender=%s toxicity=%.2f",
        room_id,
        sender,
        analysis["toxicity"],
    )

# ---------------------------------------------------------------------------
# Matrix event handler
# ---------------------------------------------------------------------------

_startup_ts = int(time.time() * 1000)


async def on_room_message(
    room: MatrixRoom,
    event: RoomMessageText,
    client: AsyncClient,
    http_session: aiohttp.ClientSession,
) -> None:
    """Process every text message in every joined room."""
    # Ignore messages from before the bot started
    if event.server_timestamp < _startup_ts:
        return
    # Ignore our own messages
    if event.sender == client.user_id:
        return

    body = event.body.strip()
    if not body:
        return

    attempt = 0
    analysis = None
    while attempt < MAX_RETRIES:
        try:
            analysis = await analyse_message(http_session, body)
            break
        except (aiohttp.ClientError, json.JSONDecodeError, KeyError, ValueError) as exc:
            attempt += 1
            logger.warning(
                "Ollama analysis attempt %d/%d failed: %s", attempt, MAX_RETRIES, exc
            )
            if attempt < MAX_RETRIES:
                await asyncio.sleep(RETRY_DELAY)

    if analysis is None:
        logger.error("All analysis attempts failed for message in %s", room.room_id)
        return

    log_mediation(room.room_id, event.sender, body, analysis)

    if analysis["toxicity"] > TOXICITY_THRESHOLD:
        suggestion = (
            f"[Mediador Ierahkwa]\n"
            f"Se detecto un nivel de hostilidad elevado ({analysis['toxicity']:.0%}).\n"
            f"Reformulacion sugerida:\n\n"
            f"> {analysis['reformulation']}\n\n"
            f"Recordemos que el dialogo respetuoso fortalece a la comunidad."
        )
        try:
            await client.room_send(
                room.room_id,
                "m.room.message",
                {"msgtype": "m.text", "body": suggestion},
            )
            logger.info("Suggestion sent to %s (toxicity %.2f)", room.room_id, analysis["toxicity"])
        except Exception as exc:
            logger.error("Failed to send suggestion to %s: %s", room.room_id, exc)

# ---------------------------------------------------------------------------
# Main loop
# ---------------------------------------------------------------------------


async def main() -> None:
    if not MATRIX_PASSWORD:
        sys.exit("MATRIX_PASSWORD environment variable is required.")

    client = AsyncClient(MATRIX_HOMESERVER, MATRIX_USER)
    http_session = aiohttp.ClientSession()

    try:
        logger.info("Logging in to %s as %s ...", MATRIX_HOMESERVER, MATRIX_USER)
        login_resp = await client.login(MATRIX_PASSWORD)
        if not isinstance(login_resp, LoginResponse):
            sys.exit(f"Login failed: {login_resp}")
        logger.info("Login successful. User ID: %s", client.user_id)

        # Register the callback
        client.add_event_callback(
            lambda room, event: on_room_message(room, event, client, http_session),
            RoomMessageText,
        )

        logger.info("Starting sync loop. Toxicity threshold: %.2f", TOXICITY_THRESHOLD)
        await client.sync_forever(timeout=30000, full_state=True)

    except KeyboardInterrupt:
        logger.info("Shutting down on keyboard interrupt.")
    except Exception as exc:
        logger.exception("Unhandled exception: %s", exc)
    finally:
        await http_session.close()
        await client.close()
        logger.info("Mediator stopped.")


if __name__ == "__main__":
    asyncio.run(main())
