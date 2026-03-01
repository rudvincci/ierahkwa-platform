#!/usr/bin/env python3
"""
Ierahkwa LoRa Mesh Bridge -- Meshtastic + Matrix
Bridges messages between a Meshtastic LoRa mesh network and Matrix rooms.
When internet is available, critical Matrix messages are forwarded to the
LoRa mesh, and incoming LoRa messages are queued for Matrix delivery.
If internet goes down the bridge switches to LoRa-only mode automatically.

Environment variables
---------------------
MATRIX_HOMESERVER     Matrix server URL        (default: https://matrix.ierahkwa.org)
MATRIX_USER           Full Matrix user ID      (e.g. @bridge:ierahkwa.org)
MATRIX_PASSWORD       Password for the bot user
MATRIX_ROOM           Default room to bridge   (e.g. !abc:ierahkwa.org)
MESHTASTIC_SERIAL     Serial port for device   (default: /dev/ttyUSB0)
MESHTASTIC_CHANNEL    LoRa channel index       (default: 0)
LORA_MAX_BYTES        Max payload bytes        (default: 230)
PRIORITY_KEYWORDS     Comma-separated keywords that mark a message as critical
INTERNET_CHECK_URL    URL to probe connectivity (default: https://matrix.ierahkwa.org/_matrix/client/versions)
INTERNET_CHECK_INTERVAL  Seconds between connectivity checks (default: 30)
QUEUE_DIR             Directory for offline message queue (default: data/lora_queue)
LOG_DIR               Logging directory        (default: logs/)
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
    import meshtastic
    import meshtastic.serial_interface
except ImportError:
    sys.exit("meshtastic is required: pip install meshtastic")

try:
    from nio import AsyncClient, RoomMessageText, LoginResponse
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
MATRIX_USER = os.environ.get("MATRIX_USER", "@bridge:ierahkwa.org")
MATRIX_PASSWORD = os.environ.get("MATRIX_PASSWORD", "")
MATRIX_ROOM = os.environ.get("MATRIX_ROOM", "")
MESHTASTIC_SERIAL = os.environ.get("MESHTASTIC_SERIAL", "/dev/ttyUSB0")
MESHTASTIC_CHANNEL = int(os.environ.get("MESHTASTIC_CHANNEL", "0"))
LORA_MAX_BYTES = int(os.environ.get("LORA_MAX_BYTES", "230"))
PRIORITY_KEYWORDS = [
    kw.strip().lower()
    for kw in os.environ.get(
        "PRIORITY_KEYWORDS", "ALERTA,EMERGENCIA,RED,SIMULACRO,URGENTE"
    ).split(",")
    if kw.strip()
]
INTERNET_CHECK_URL = os.environ.get(
    "INTERNET_CHECK_URL", "https://matrix.ierahkwa.org/_matrix/client/versions"
)
INTERNET_CHECK_INTERVAL = int(os.environ.get("INTERNET_CHECK_INTERVAL", "30"))
QUEUE_DIR = Path(os.environ.get("QUEUE_DIR", "data/lora_queue"))
LOG_DIR = Path(os.environ.get("LOG_DIR", "logs"))

QUEUE_DIR.mkdir(parents=True, exist_ok=True)
LOG_DIR.mkdir(parents=True, exist_ok=True)

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(name)s: %(message)s",
    handlers=[
        logging.StreamHandler(sys.stdout),
        logging.FileHandler(LOG_DIR / "lora_mesh_bridge.log", encoding="utf-8"),
    ],
)
logger = logging.getLogger("ierahkwa.lora_bridge")

# ---------------------------------------------------------------------------
# State
# ---------------------------------------------------------------------------

_internet_available = True
_matrix_to_lora_queue: deque = deque(maxlen=500)
_lora_to_matrix_queue: deque = deque(maxlen=500)
_known_nodes: set[str] = set()
_node_sender_map: dict[str, str] = {}  # node_id -> first-seen sender address

# ---------------------------------------------------------------------------
# Message optimization for LoRa
# ---------------------------------------------------------------------------


def compress_for_lora(sender: str, body: str) -> bytes:
    """Compress a message to fit within LoRa payload limits.

    Format: <sender_short>|<truncated_body>
    The total encoded size must be <= LORA_MAX_BYTES.
    """
    sender_short = sender.split(":")[0].lstrip("@")[:12]
    prefix = f"{sender_short}|"
    prefix_bytes = prefix.encode("utf-8")
    remaining = LORA_MAX_BYTES - len(prefix_bytes)
    if remaining <= 0:
        return prefix_bytes[:LORA_MAX_BYTES]

    body_bytes = body.encode("utf-8")
    if len(body_bytes) > remaining:
        # Truncate at character boundary
        truncated = body.encode("utf-8")[:remaining]
        # Ensure we don't split a multi-byte character
        truncated = truncated.decode("utf-8", errors="ignore").encode("utf-8")
        body_bytes = truncated

    return prefix_bytes + body_bytes


def is_critical(message: str) -> bool:
    """Determine whether a message qualifies as critical based on keywords."""
    lower = message.lower()
    return any(kw in lower for kw in PRIORITY_KEYWORDS)

# ---------------------------------------------------------------------------
# Internet connectivity check
# ---------------------------------------------------------------------------


async def check_internet(session: aiohttp.ClientSession) -> bool:
    """Return True if the Matrix homeserver is reachable."""
    try:
        async with session.get(INTERNET_CHECK_URL, timeout=aiohttp.ClientTimeout(total=10)) as resp:
            return resp.status == 200
    except Exception:
        return False


async def connectivity_monitor(session: aiohttp.ClientSession) -> None:
    """Continuously monitor internet connectivity and update global state."""
    global _internet_available
    while True:
        was_available = _internet_available
        _internet_available = await check_internet(session)
        if was_available and not _internet_available:
            logger.warning("Internet DOWN -- switching to LoRa-only mode.")
        elif not was_available and _internet_available:
            logger.info("Internet UP -- resuming Matrix bridge.")
        await asyncio.sleep(INTERNET_CHECK_INTERVAL)

# ---------------------------------------------------------------------------
# Offline queue persistence
# ---------------------------------------------------------------------------


def persist_queue_item(direction: str, data: dict) -> None:
    """Write a queued message to disk for crash recovery."""
    ts = datetime.now(timezone.utc).strftime("%Y%m%d%H%M%S%f")
    path = QUEUE_DIR / f"{direction}_{ts}.json"
    with open(path, "w", encoding="utf-8") as fh:
        json.dump(data, fh, ensure_ascii=False)


def load_persisted_queue(direction: str) -> list[dict]:
    """Load and delete queued messages from disk."""
    items = []
    pattern = f"{direction}_*.json"
    for path in sorted(QUEUE_DIR.glob(pattern)):
        try:
            with open(path, "r", encoding="utf-8") as fh:
                items.append(json.load(fh))
            path.unlink()
        except Exception as exc:
            logger.warning("Failed to load queued item %s: %s", path, exc)
    return items

# ---------------------------------------------------------------------------
# Meshtastic interface
# ---------------------------------------------------------------------------

_mesh_interface = None


def init_meshtastic() -> meshtastic.serial_interface.SerialInterface:
    """Connect to the Meshtastic device."""
    global _mesh_interface
    logger.info("Connecting to Meshtastic device on %s ...", MESHTASTIC_SERIAL)
    _mesh_interface = meshtastic.serial_interface.SerialInterface(MESHTASTIC_SERIAL)
    logger.info("Meshtastic device connected.")
    return _mesh_interface


def send_to_lora(payload: bytes) -> bool:
    """Send a payload to the LoRa mesh. Return True on success."""
    if _mesh_interface is None:
        logger.error("Meshtastic interface not initialized.")
        return False
    try:
        _mesh_interface.sendData(
            payload,
            portNum=meshtastic.portnums_pb2.PortNum.TEXT_MESSAGE_APP,
            channelIndex=MESHTASTIC_CHANNEL,
        )
        logger.info("Sent %d bytes to LoRa mesh.", len(payload))
        return True
    except Exception as exc:
        logger.error("Failed to send to LoRa: %s", exc)
        return False


def on_lora_receive(packet: dict, interface) -> None:
    """Callback for incoming LoRa messages."""
    # --- New node discovery ---
    _check_new_node(packet)

    decoded = packet.get("decoded", {})
    text = decoded.get("text", "")
    if not text:
        # Try payload bytes
        payload = decoded.get("payload", b"")
        if isinstance(payload, bytes):
            text = payload.decode("utf-8", errors="replace")
    if not text:
        return

    sender_id = packet.get("fromId", "unknown")
    logger.info("LoRa message received from %s: %s", sender_id, text[:80])

    msg_data = {
        "sender": sender_id,
        "body": text,
        "received_at": datetime.now(timezone.utc).isoformat(),
    }
    _lora_to_matrix_queue.append(msg_data)

    if not _internet_available:
        persist_queue_item("lora2matrix", msg_data)

# ---------------------------------------------------------------------------
# Matrix -> LoRa forwarder
# ---------------------------------------------------------------------------


async def matrix_to_lora_forwarder() -> None:
    """Drain the Matrix-to-LoRa queue and send messages over the mesh."""
    while True:
        while _matrix_to_lora_queue:
            item = _matrix_to_lora_queue.popleft()
            payload = compress_for_lora(item["sender"], item["body"])
            if not send_to_lora(payload):
                persist_queue_item("matrix2lora", item)
        await asyncio.sleep(1)

# ---------------------------------------------------------------------------
# LoRa -> Matrix forwarder
# ---------------------------------------------------------------------------


async def lora_to_matrix_forwarder(client: AsyncClient) -> None:
    """Drain the LoRa-to-Matrix queue when internet is available."""
    # First, load any persisted items from a previous crash
    for item in load_persisted_queue("lora2matrix"):
        _lora_to_matrix_queue.append(item)

    while True:
        if _internet_available and MATRIX_ROOM:
            while _lora_to_matrix_queue:
                item = _lora_to_matrix_queue.popleft()
                body = f"[LoRa:{item['sender']}] {item['body']}"
                try:
                    await client.room_send(
                        MATRIX_ROOM,
                        "m.room.message",
                        {"msgtype": "m.text", "body": body},
                    )
                    logger.info("Forwarded LoRa message to Matrix room %s.", MATRIX_ROOM)
                except Exception as exc:
                    logger.error("Failed to forward to Matrix: %s", exc)
                    _lora_to_matrix_queue.appendleft(item)
                    break
        await asyncio.sleep(2)

# ---------------------------------------------------------------------------
# Matrix event callback
# ---------------------------------------------------------------------------

_startup_ts = int(time.time() * 1000)


def make_matrix_callback(client: AsyncClient):
    """Create a Matrix message callback that queues critical messages for LoRa."""

    async def callback(room, event):
        if event.server_timestamp < _startup_ts:
            return
        if event.sender == client.user_id:
            return
        body = event.body.strip()
        if not body:
            return
        if is_critical(body):
            logger.info("Critical message detected, queuing for LoRa: %s", body[:60])
            _matrix_to_lora_queue.append({"sender": event.sender, "body": body})

    return callback

# ---------------------------------------------------------------------------
# Node discovery & welcome
# ---------------------------------------------------------------------------

MESH_NODES_LOG = Path("data/mesh_nodes.jsonl")
MESH_NODES_LOG.parent.mkdir(parents=True, exist_ok=True)

NTFY_SERVER = os.environ.get("NTFY_SERVER", "https://ntfy.sh")


def _check_new_node(packet: dict) -> None:
    """Detect new nodes from incoming packets and trigger welcome flow.

    Includes anti-spoofing: rejects packets where a node_id is already
    registered to a different sender address.
    """
    node_id = packet.get("fromId") or packet.get("from")
    if not node_id:
        return

    node_id = str(node_id)
    sender_addr = str(packet.get("from", ""))

    # Anti-spoofing: if we know this node_id, verify sender matches
    if node_id in _known_nodes:
        original_sender = _node_sender_map.get(node_id)
        if original_sender and sender_addr and original_sender != sender_addr:
            logger.warning(
                "SPOOF DETECTED: node %s claimed by sender %s, but originally from %s. Dropping.",
                node_id,
                sender_addr,
                original_sender,
            )
            log_mesh_event("spoof_attempt", {
                "node_id": node_id,
                "claimed_sender": sender_addr,
                "original_sender": original_sender,
            })
        return

    # New node discovered
    _known_nodes.add(node_id)
    if sender_addr:
        _node_sender_map[node_id] = sender_addr

    # Extract GPS position if available
    position = packet.get("decoded", {}).get("position", {})
    gps = None
    if position:
        lat = position.get("latitude") or position.get("latitudeI")
        lon = position.get("longitude") or position.get("longitudeI")
        if lat is not None and lon is not None:
            # Meshtastic stores as integer * 1e-7
            if isinstance(lat, int) and abs(lat) > 1000:
                lat = lat * 1e-7
                lon = lon * 1e-7
            gps = {"lat": lat, "lon": lon}

    node_info = {
        "node_id": node_id,
        "sender_addr": sender_addr,
        "gps": gps,
        "hw_model": packet.get("decoded", {}).get("hwModel"),
    }

    welcome_new_node(node_id, node_info)


def welcome_new_node(node_id: str, node_info: dict) -> None:
    """Welcome a newly discovered node to the Ierahkwa mesh.

    1. Sends LoRa welcome message
    2. Logs to data/mesh_nodes.jsonl
    3. Notifies Matrix room
    4. Sends ntfy push notification
    """
    logger.info("New mesh node discovered: %s", node_id)

    # 1. LoRa welcome broadcast
    welcome_msg = f"BIENVENIDO nodo {node_id} a la red Ierahkwa"
    send_to_lora(welcome_msg.encode("utf-8")[:LORA_MAX_BYTES])

    # 2. Log to JSONL
    log_mesh_event("node_joined", {
        "node_id": node_id,
        "sender_addr": node_info.get("sender_addr"),
        "gps": node_info.get("gps"),
        "hw_model": node_info.get("hw_model"),
    })

    # 3. Queue Matrix notification
    matrix_msg = f"[Mesh] Nuevo nodo detectado: {node_id}"
    if node_info.get("gps"):
        gps = node_info["gps"]
        matrix_msg += f" (GPS: {gps['lat']:.6f}, {gps['lon']:.6f})"
    if node_info.get("hw_model"):
        matrix_msg += f" [{node_info['hw_model']}]"

    _lora_to_matrix_queue.append({
        "sender": "mesh-discovery",
        "body": matrix_msg,
        "received_at": datetime.now(timezone.utc).isoformat(),
    })

    # 4. ntfy push notification (fire-and-forget via thread)
    _send_ntfy_sync(
        f"Nuevo nodo: {node_id}",
        matrix_msg,
    )


def log_mesh_event(event_type: str, data: dict) -> None:
    """Append a JSONL record to the mesh nodes log."""
    record = {
        "ts": datetime.now(timezone.utc).isoformat(),
        "event": event_type,
        **data,
    }
    try:
        with open(MESH_NODES_LOG, "a", encoding="utf-8") as fh:
            fh.write(json.dumps(record, ensure_ascii=False) + "\n")
    except OSError as exc:
        logger.error("Failed to write mesh node log: %s", exc)


def _send_ntfy_sync(title: str, message: str) -> None:
    """Send a push notification via ntfy (synchronous, best-effort)."""
    import threading

    def _post():
        try:
            import urllib.request
            url = f"{NTFY_SERVER}/ierahkwa-mesh"
            req = urllib.request.Request(
                url,
                data=message.encode("utf-8"),
                headers={
                    "Title": title,
                    "Priority": "default",
                    "Tags": "satellite,wave",
                },
                method="POST",
            )
            urllib.request.urlopen(req, timeout=10)
        except Exception as exc:
            logger.debug("ntfy send failed: %s", exc)

    threading.Thread(target=_post, daemon=True).start()


# ---------------------------------------------------------------------------
# Mesh topology periodic reporter
# ---------------------------------------------------------------------------


async def mesh_topology_reporter() -> None:
    """Every 5 minutes, log the count of known active nodes."""
    while True:
        await asyncio.sleep(300)  # 5 minutes
        count = len(_known_nodes)
        logger.info("Mesh topology: %d known active nodes.", count)
        log_mesh_event("topology_report", {
            "active_node_count": count,
            "node_ids": sorted(_known_nodes),
        })


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------


async def main() -> None:
    if not MATRIX_PASSWORD:
        sys.exit("MATRIX_PASSWORD environment variable is required.")

    # Initialize Meshtastic
    mesh = init_meshtastic()

    # Register LoRa receive callback using the pub-sub system
    from pubsub import pub
    pub.subscribe(on_lora_receive, "meshtastic.receive.text")

    # Initialize Matrix client
    client = AsyncClient(MATRIX_HOMESERVER, MATRIX_USER)
    http_session = aiohttp.ClientSession()

    try:
        logger.info("Logging in to Matrix as %s ...", MATRIX_USER)
        login_resp = await client.login(MATRIX_PASSWORD)
        if not isinstance(login_resp, LoginResponse):
            sys.exit(f"Matrix login failed: {login_resp}")
        logger.info("Matrix login successful.")

        client.add_event_callback(make_matrix_callback(client), RoomMessageText)

        # Load any persisted matrix2lora items
        for item in load_persisted_queue("matrix2lora"):
            _matrix_to_lora_queue.append(item)

        # Launch concurrent tasks
        await asyncio.gather(
            connectivity_monitor(http_session),
            matrix_to_lora_forwarder(),
            lora_to_matrix_forwarder(client),
            mesh_topology_reporter(),
            client.sync_forever(timeout=30000, full_state=True),
        )

    except KeyboardInterrupt:
        logger.info("Shutting down on keyboard interrupt.")
    except Exception as exc:
        logger.exception("Unhandled exception: %s", exc)
    finally:
        await http_session.close()
        await client.close()
        if _mesh_interface:
            _mesh_interface.close()
        logger.info("LoRa Mesh Bridge stopped.")


if __name__ == "__main__":
    asyncio.run(main())
