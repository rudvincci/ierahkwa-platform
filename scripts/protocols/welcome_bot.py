#!/usr/bin/env python3
"""
Ierahkwa Welcome Bot -- Matrix + LoRa Guardian Greeter
Monitors sovereign Matrix rooms for new member joins, sends bilingual
welcome messages, broadcasts arrival over LoRa mesh, verifies Guardian
SBT on MameyNode, and delivers an onboarding checklist via DM.

Rate-limited batch processing prevents spam when many guardians join
simultaneously.  All events are logged to JSONL for audit.

Environment variables
---------------------
MATRIX_HOMESERVER       Matrix server URL         (default: https://matrix.ierahkwa.org)
MATRIX_USER             Bot user ID               (e.g. @welcome:ierahkwa.org)
MATRIX_PASSWORD         Bot password
MATRIX_WELCOME_ROOM     Room to monitor for joins (e.g. !abc:ierahkwa.org)
MESHTASTIC_SERIAL       Serial port for device    (default: /dev/ttyUSB0)
MAMEYNODE_RPC           JSON-RPC endpoint         (default: http://mameynode:8545)
NTFY_SERVER             ntfy server URL           (default: https://ntfy.sh)
MESHTASTIC_CHANNEL      LoRa channel index        (default: 0)
WELCOME_BATCH_SECONDS   Batch window for joins    (default: 15)
LOG_DIR                 Logging directory          (default: logs/)
"""

import asyncio
import json
import logging
import os
import sys
import time
from datetime import datetime, timezone
from pathlib import Path
from collections import deque

try:
    from nio import AsyncClient, LoginResponse, RoomMemberEvent
except ImportError:
    sys.exit("matrix-nio is required: pip install matrix-nio")

try:
    import aiohttp
except ImportError:
    sys.exit("aiohttp is required: pip install aiohttp")

try:
    import meshtastic
    import meshtastic.serial_interface
except ImportError:
    sys.exit("meshtastic is required: pip install meshtastic")

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------

MATRIX_HOMESERVER = os.environ.get("MATRIX_HOMESERVER", "https://matrix.ierahkwa.org")
MATRIX_USER = os.environ.get("MATRIX_USER", "@welcome:ierahkwa.org")
MATRIX_PASSWORD = os.environ.get("MATRIX_PASSWORD", "")
MATRIX_WELCOME_ROOM = os.environ.get("MATRIX_WELCOME_ROOM", "")
MESHTASTIC_SERIAL = os.environ.get("MESHTASTIC_SERIAL", "/dev/ttyUSB0")
MESHTASTIC_CHANNEL = int(os.environ.get("MESHTASTIC_CHANNEL", "0"))
MAMEYNODE_RPC = os.environ.get("MAMEYNODE_RPC", "http://mameynode:8545")
NTFY_SERVER = os.environ.get("NTFY_SERVER", "https://ntfy.sh")
WELCOME_BATCH_SECONDS = int(os.environ.get("WELCOME_BATCH_SECONDS", "15"))
LOG_DIR = Path(os.environ.get("LOG_DIR", "logs"))
DATA_DIR = Path("data")

LOG_DIR.mkdir(parents=True, exist_ok=True)
DATA_DIR.mkdir(parents=True, exist_ok=True)

EVENTS_LOG = DATA_DIR / "welcome_events.jsonl"

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(name)s: %(message)s",
    handlers=[
        logging.StreamHandler(sys.stdout),
        logging.FileHandler(LOG_DIR / "welcome_bot.log", encoding="utf-8"),
    ],
)
logger = logging.getLogger("ierahkwa.welcome_bot")

# ---------------------------------------------------------------------------
# State
# ---------------------------------------------------------------------------

_pending_joins: deque[dict] = deque(maxlen=200)
_welcomed_users: set[str] = set()
_mesh_interface = None
_startup_ts = int(time.time() * 1000)

# ---------------------------------------------------------------------------
# Guardian SBT contract ABI (minimal)
# ---------------------------------------------------------------------------

GUARDIAN_SBT_ADDRESS = "0x0000000000000000000000000000000000001337"

# ---------------------------------------------------------------------------
# Logging helpers
# ---------------------------------------------------------------------------


def log_event(event_type: str, data: dict) -> None:
    """Append a JSONL record to the welcome events log."""
    record = {
        "ts": datetime.now(timezone.utc).isoformat(),
        "event": event_type,
        **data,
    }
    try:
        with open(EVENTS_LOG, "a", encoding="utf-8") as fh:
            fh.write(json.dumps(record, ensure_ascii=False) + "\n")
    except OSError as exc:
        logger.error("Failed to write event log: %s", exc)

# ---------------------------------------------------------------------------
# LoRa mesh announcement
# ---------------------------------------------------------------------------


def init_meshtastic() -> meshtastic.serial_interface.SerialInterface:
    """Connect to the Meshtastic device."""
    global _mesh_interface
    logger.info("Connecting to Meshtastic on %s ...", MESHTASTIC_SERIAL)
    _mesh_interface = meshtastic.serial_interface.SerialInterface(MESHTASTIC_SERIAL)
    logger.info("Meshtastic connected.")
    return _mesh_interface


def broadcast_lora(message: str) -> bool:
    """Send a text announcement over the LoRa mesh."""
    if _mesh_interface is None:
        logger.warning("Meshtastic not initialised, skipping LoRa broadcast.")
        return False
    try:
        payload = message.encode("utf-8")[:230]
        _mesh_interface.sendData(
            payload,
            portNum=meshtastic.portnums_pb2.PortNum.TEXT_MESSAGE_APP,
            channelIndex=MESHTASTIC_CHANNEL,
        )
        logger.info("LoRa broadcast: %s", message[:80])
        return True
    except Exception as exc:
        logger.error("LoRa broadcast failed: %s", exc)
        return False

# ---------------------------------------------------------------------------
# SBT verification via MameyNode JSON-RPC
# ---------------------------------------------------------------------------


async def check_guardian_sbt(session: aiohttp.ClientSession, user_id: str) -> dict:
    """Query MameyNode for Guardian SBT ownership.

    Returns a dict with 'has_sbt' (bool) and 'mattr_score' (int).
    Falls back to defaults on error.
    """
    # Derive a deterministic address from the Matrix user ID
    user_hash = user_id.encode("utf-8").hex()[:40]
    address = f"0x{user_hash}"

    # eth_call to balanceOf(address) on the SBT contract
    call_data = (
        "0x70a08231"
        + "000000000000000000000000"
        + user_hash
    )
    rpc_payload = {
        "jsonrpc": "2.0",
        "method": "eth_call",
        "params": [
            {"to": GUARDIAN_SBT_ADDRESS, "data": call_data},
            "latest",
        ],
        "id": 1,
    }

    try:
        async with session.post(
            MAMEYNODE_RPC,
            json=rpc_payload,
            timeout=aiohttp.ClientTimeout(total=10),
        ) as resp:
            body = await resp.json()
            result = body.get("result", "0x0")
            balance = int(result, 16) if result else 0
            has_sbt = balance > 0
    except Exception as exc:
        logger.warning("SBT check failed for %s: %s", user_id, exc)
        has_sbt = False

    # Query MATTR score (custom RPC extension)
    mattr_score = 50  # default initial score
    try:
        score_payload = {
            "jsonrpc": "2.0",
            "method": "mattr_getScore",
            "params": [address],
            "id": 2,
        }
        async with session.post(
            MAMEYNODE_RPC,
            json=score_payload,
            timeout=aiohttp.ClientTimeout(total=10),
        ) as resp:
            body = await resp.json()
            raw = body.get("result")
            if raw is not None:
                mattr_score = int(raw, 16) if isinstance(raw, str) else int(raw)
    except Exception:
        pass

    return {"has_sbt": has_sbt, "mattr_score": mattr_score, "address": address}

# ---------------------------------------------------------------------------
# Welcome messages
# ---------------------------------------------------------------------------

GUARDIAN_OATH_EXCERPT = (
    "Juro proteger la soberania digital de mi nacion, servir con honor, "
    "y defender la red con verdad y transparencia."
)

DASHBOARD_URL = "https://ierahkwa.org/dashboard"
GOVERNANCE_URL = "https://ierahkwa.org/governance"
COMMS_URL = "https://ierahkwa.org/comms"


def build_welcome_message(display_name: str, mattr_score: int, has_sbt: bool) -> str:
    """Build a bilingual welcome message for a new Guardian."""
    sbt_status = "Verificado" if has_sbt else "Pendiente (mint tu SBT en el Dashboard)"

    msg = (
        f"Bienvenido/a, {display_name}!\n"
        f"Welcome, {display_name}!\n"
        f"\n"
        f"Te damos la bienvenida a la Red Soberana Ierahkwa.\n"
        f"Welcome to the Ierahkwa Sovereign Network.\n"
        f"\n"
        f"--- Enlaces rapidos / Quick links ---\n"
        f"  Dashboard: {DASHBOARD_URL}\n"
        f"  Gobernanza / Governance: {GOVERNANCE_URL}\n"
        f"  Comunicaciones / Comms: {COMMS_URL}\n"
        f"\n"
        f"--- Tu estado / Your status ---\n"
        f"  $MATTR Score: {mattr_score}\n"
        f"  Guardian SBT: {sbt_status}\n"
        f"\n"
        f"--- Recordatorio / Reminder ---\n"
        f"  Configura tu nodo LoRa / Set up your LoRa node\n"
        f"  Registra tu dominio Handshake DNS / Register your Handshake DNS\n"
        f"\n"
        f"--- Juramento del Guardian / Guardian Oath ---\n"
        f'  "{GUARDIAN_OATH_EXCERPT}"\n'
    )
    return msg


def build_onboarding_checklist(display_name: str) -> str:
    """Build a DM onboarding checklist for a new Guardian."""
    return (
        f"Hola {display_name}, aqui esta tu lista de tareas del primer dia:\n"
        f"Hi {display_name}, here is your first-day checklist:\n"
        f"\n"
        f"[ ] 1. Verifica tu identidad en el Dashboard / Verify identity\n"
        f"[ ] 2. Mint tu Guardian SBT en MameyNode / Mint your SBT\n"
        f"[ ] 3. Configura tu nodo LoRa Meshtastic / Set up LoRa node\n"
        f"[ ] 4. Registra tu dominio .ierahkwa en Handshake / Register DNS\n"
        f"[ ] 5. Unete al canal de tu NEXUS regional / Join your NEXUS channel\n"
        f"[ ] 6. Presenta tu propuesta inicial en Gobernanza / Submit proposal\n"
        f"[ ] 7. Sincroniza tu wallet con el vault comunitario / Sync wallet\n"
        f"[ ] 8. Revisa el WHITEPAPER de tu NEXUS / Read your NEXUS whitepaper\n"
        f"\n"
        f"Si necesitas ayuda escribe !ayuda / For help type !help\n"
    )

# ---------------------------------------------------------------------------
# ntfy push notification
# ---------------------------------------------------------------------------


async def send_ntfy(session: aiohttp.ClientSession, title: str, message: str) -> bool:
    """Send a push notification via ntfy."""
    url = f"{NTFY_SERVER}/ierahkwa-welcome"
    try:
        async with session.post(
            url,
            data=message.encode("utf-8"),
            headers={
                "Title": title,
                "Priority": "default",
                "Tags": "wave,shield",
            },
            timeout=aiohttp.ClientTimeout(total=10),
        ) as resp:
            return resp.status == 200
    except Exception as exc:
        logger.warning("ntfy notification failed: %s", exc)
        return False

# ---------------------------------------------------------------------------
# Core welcome helpers
# ---------------------------------------------------------------------------


async def _dm_checklist(client: AsyncClient, user_id: str, display_name: str) -> None:
    """Send onboarding checklist via DM."""
    checklist = build_onboarding_checklist(display_name)
    try:
        dm_resp = await client.room_create(invite=[user_id], is_direct=True)
        if hasattr(dm_resp, "room_id"):
            await client.room_send(
                dm_resp.room_id, "m.room.message",
                {"msgtype": "m.text", "body": checklist},
            )
            logger.info("Onboarding checklist sent to %s via DM.", user_id)
        else:
            logger.warning("Could not create DM for %s: %s", user_id, dm_resp)
    except Exception as exc:
        logger.error("DM failed for %s: %s", user_id, exc)


async def _welcome_single(client, session, user_id, display_name) -> None:
    """Full welcome for one Guardian: room msg, LoRa, DM, ntfy, log."""
    sbt = await check_guardian_sbt(session, user_id)
    welcome = build_welcome_message(display_name, sbt["mattr_score"], sbt["has_sbt"])
    try:
        await client.room_send(
            MATRIX_WELCOME_ROOM, "m.room.message",
            {"msgtype": "m.text", "body": welcome},
        )
    except Exception as exc:
        logger.error("Room welcome failed: %s", exc)
    broadcast_lora(f"NUEVO GUARDIAN: {display_name} -- Bienvenido a la red")
    await _dm_checklist(client, user_id, display_name)
    await send_ntfy(session, f"Nuevo Guardian: {display_name}",
                    f"{display_name} se unio. SBT: {'Si' if sbt['has_sbt'] else 'No'}")
    log_event("guardian_welcomed", {"user_id": user_id, "display_name": display_name,
              "has_sbt": sbt["has_sbt"], "mattr_score": sbt["mattr_score"],
              "address": sbt["address"]})


# ---------------------------------------------------------------------------
# Batch welcome processor (rate limiting)
# ---------------------------------------------------------------------------


async def batch_welcome_processor(client: AsyncClient, session: aiohttp.ClientSession) -> None:
    """Process pending joins in batches to avoid spamming."""
    while True:
        await asyncio.sleep(WELCOME_BATCH_SECONDS)
        if not _pending_joins:
            continue
        batch = []
        while _pending_joins and len(batch) < 10:
            batch.append(_pending_joins.popleft())
        if not batch:
            continue

        if len(batch) == 1:
            j = batch[0]
            if j["user_id"] not in _welcomed_users and j["user_id"] != client.user_id:
                _welcomed_users.add(j["user_id"])
                await _welcome_single(client, session, j["user_id"], j["display_name"])
        else:
            names = [j["display_name"] for j in batch]
            summary = (f"Bienvenidos nuevos Guardianes! / Welcome new Guardians!\n"
                       f"{', '.join(names)}\nTotal: {len(batch)} nuevos miembros")
            try:
                await client.room_send(
                    MATRIX_WELCOME_ROOM, "m.room.message",
                    {"msgtype": "m.text", "body": summary},
                )
            except Exception as exc:
                logger.error("Batch welcome failed: %s", exc)
            broadcast_lora(f"NUEVOS GUARDIANES: {len(batch)} -- Bienvenidos a la red")
            for j in batch:
                uid, dname = j["user_id"], j["display_name"]
                if uid in _welcomed_users or uid == client.user_id:
                    continue
                _welcomed_users.add(uid)
                sbt = await check_guardian_sbt(session, uid)
                await _dm_checklist(client, uid, dname)
                log_event("guardian_welcomed", {"user_id": uid, "display_name": dname,
                          "has_sbt": sbt["has_sbt"], "mattr_score": sbt["mattr_score"],
                          "address": sbt["address"]})
            await send_ntfy(session, f"{len(batch)} Nuevos Guardianes",
                           f"Se unieron: {', '.join(names)}")
        logger.info("Batch processed: %d joins.", len(batch))

# ---------------------------------------------------------------------------
# Matrix membership event callback
# ---------------------------------------------------------------------------


def make_membership_callback(client: AsyncClient):
    """Create a callback for room membership events."""

    async def callback(room, event):
        if event.server_timestamp < _startup_ts:
            return
        if not isinstance(event, RoomMemberEvent):
            return
        if room.room_id != MATRIX_WELCOME_ROOM:
            return

        # Detect new joins (membership changed to "join")
        if event.membership == "join" and event.prev_membership != "join":
            user_id = event.state_key
            display_name = event.content.get("displayname") or user_id.split(":")[0].lstrip("@")

            logger.info("New join detected: %s (%s)", display_name, user_id)
            _pending_joins.append({
                "user_id": user_id,
                "display_name": display_name,
                "joined_at": datetime.now(timezone.utc).isoformat(),
            })

    return callback

# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------


async def main() -> None:
    if not MATRIX_PASSWORD:
        sys.exit("MATRIX_PASSWORD environment variable is required.")
    if not MATRIX_WELCOME_ROOM:
        sys.exit("MATRIX_WELCOME_ROOM environment variable is required.")

    # Initialise LoRa
    try:
        init_meshtastic()
    except Exception as exc:
        logger.warning("Meshtastic init failed (LoRa disabled): %s", exc)

    # Initialise Matrix client
    client = AsyncClient(MATRIX_HOMESERVER, MATRIX_USER)
    session = aiohttp.ClientSession()

    try:
        logger.info("Logging in to Matrix as %s ...", MATRIX_USER)
        login_resp = await client.login(MATRIX_PASSWORD)
        if not isinstance(login_resp, LoginResponse):
            sys.exit(f"Matrix login failed: {login_resp}")
        logger.info("Matrix login successful. Monitoring room %s", MATRIX_WELCOME_ROOM)

        # Register membership event callback
        client.add_event_callback(make_membership_callback(client), RoomMemberEvent)

        # Launch concurrent tasks
        await asyncio.gather(
            batch_welcome_processor(client, session),
            client.sync_forever(timeout=30000, full_state=True),
        )

    except KeyboardInterrupt:
        logger.info("Shutting down on keyboard interrupt.")
    except Exception as exc:
        logger.exception("Unhandled exception: %s", exc)
    finally:
        await session.close()
        await client.close()
        if _mesh_interface:
            _mesh_interface.close()
        logger.info("Welcome Bot stopped.")


if __name__ == "__main__":
    asyncio.run(main())
