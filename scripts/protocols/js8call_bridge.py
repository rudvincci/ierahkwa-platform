#!/usr/bin/env python3
"""
JS8Call HF Radio Bridge -- Ierahkwa Sovereign Communications
Bridges messages between the JS8Call weak-signal HF radio mode and the
Matrix sovereign messaging network. When even LoRa is out of range,
JS8Call can bounce signals off the ionosphere for global reach.

JS8Call is built on FT8 modulation but allows free-form text messaging
at very low signal-to-noise ratios (-24 dB). This bridge connects the
JS8Call TCP API (port 2442) to Matrix rooms, enabling Guardians with
HF radios to stay connected from anywhere on Earth.

Environment variables
---------------------
JS8CALL_HOST          JS8Call TCP API host      (default: 127.0.0.1)
JS8CALL_PORT          JS8Call TCP API port      (default: 2442)
MATRIX_HOMESERVER     Matrix server URL         (default: https://matrix.ierahkwa.org)
MATRIX_USER           Bot Matrix user ID        (e.g. @hf-bridge:ierahkwa.org)
MATRIX_PASSWORD       Bot password
MATRIX_ROOM           Room to bridge            (e.g. !radio:ierahkwa.org)
CALLSIGN              Station callsign          (e.g. KD2ABC)
HF_BAND               Default band              (default: 40m)
HEARTBEAT_INTERVAL    Heartbeat seconds         (default: 21600 = 6 hours)
NTFY_URL              ntfy fallback URL         (default: https://ntfy.sh)
NTFY_TOPIC            ntfy topic                (default: ierahkwa-hf)
LORA_FALLBACK         Enable LoRa cascade       (default: true)
LOG_DIR               Logging directory          (default: logs/)
"""

import asyncio
import json
import logging
import os
import socket
import struct
import sys
import time
import zlib
from datetime import datetime, timezone
from pathlib import Path
from typing import Optional

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

JS8CALL_HOST = os.environ.get("JS8CALL_HOST", "127.0.0.1")
JS8CALL_PORT = int(os.environ.get("JS8CALL_PORT", "2442"))
MATRIX_HOMESERVER = os.environ.get("MATRIX_HOMESERVER", "https://matrix.ierahkwa.org")
MATRIX_USER = os.environ.get("MATRIX_USER", "@hf-bridge:ierahkwa.org")
MATRIX_PASSWORD = os.environ.get("MATRIX_PASSWORD", "")
MATRIX_ROOM = os.environ.get("MATRIX_ROOM", "")
CALLSIGN = os.environ.get("CALLSIGN", "KD2IER")
HF_BAND = os.environ.get("HF_BAND", "40m")
HEARTBEAT_INTERVAL = int(os.environ.get("HEARTBEAT_INTERVAL", "21600"))
NTFY_URL = os.environ.get("NTFY_URL", "https://ntfy.sh")
NTFY_TOPIC = os.environ.get("NTFY_TOPIC", "ierahkwa-hf")
LORA_FALLBACK = os.environ.get("LORA_FALLBACK", "true").lower() in ("true", "1", "yes")
LOG_DIR = Path(os.environ.get("LOG_DIR", "logs"))

LOG_DIR.mkdir(parents=True, exist_ok=True)

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(name)s: %(message)s",
    handlers=[
        logging.StreamHandler(sys.stdout),
        logging.FileHandler(LOG_DIR / "js8call_bridge.log", encoding="utf-8"),
    ],
)
logger = logging.getLogger("ierahkwa.js8call")

# ---------------------------------------------------------------------------
# HF Band definitions
# ---------------------------------------------------------------------------

HF_BANDS = {
    "20m": {"freq_hz": 14078000, "dial_khz": 14078, "description": "20 meter band (14.078 MHz)"},
    "40m": {"freq_hz": 7078000, "dial_khz": 7078, "description": "40 meter band (7.078 MHz)"},
}

# ---------------------------------------------------------------------------
# Message priority encoding
# ---------------------------------------------------------------------------

PRIORITY_TYPES = {
    "PEACE":     {"code": "P", "level": 0, "description": "Routine peace/community message"},
    "ALERT":     {"code": "A", "level": 1, "description": "Non-emergency alert or advisory"},
    "EMERGENCY": {"code": "E", "level": 2, "description": "Life-safety emergency"},
    "HEARTBEAT": {"code": "H", "level": 0, "description": "Automated heartbeat beacon"},
}

# Maximum JS8Call message length (characters)
JS8_MAX_CHARS = 50

# ---------------------------------------------------------------------------
# Callsign <-> Matrix mapping
# ---------------------------------------------------------------------------

_callsign_map: dict[str, str] = {}
_signal_log: list[dict] = []


def register_callsign(callsign: str, matrix_id: str) -> None:
    """Map a Guardian callsign to a Matrix user ID."""
    normalized = callsign.strip().upper()
    _callsign_map[normalized] = matrix_id
    logger.info("Registered callsign %s -> %s", normalized, matrix_id)


def lookup_callsign(callsign: str) -> Optional[str]:
    """Look up the Matrix user ID for a callsign."""
    return _callsign_map.get(callsign.strip().upper())


def lookup_matrix(matrix_id: str) -> Optional[str]:
    """Reverse lookup: find the callsign for a Matrix user ID."""
    for cs, mid in _callsign_map.items():
        if mid == matrix_id:
            return cs
    return None


# ---------------------------------------------------------------------------
# Compression for HF (extreme — every byte counts)
# ---------------------------------------------------------------------------

def compress_message(priority: str, body: str) -> str:
    """Compress a message for HF transmission.

    Format: <priority_code><compressed_body>
    Total must be <= JS8_MAX_CHARS characters.

    Compression strategy:
    - Single-char priority prefix
    - Strip whitespace, lowercase
    - Abbreviation table for common words
    - Truncate to fit
    """
    ptype = PRIORITY_TYPES.get(priority.upper(), PRIORITY_TYPES["PEACE"])
    prefix = ptype["code"]

    abbreviations = {
        "emergency": "EMG",
        "evacuation": "EVAC",
        "medical": "MED",
        "supply": "SUP",
        "water": "H2O",
        "food": "FD",
        "shelter": "SHLT",
        "guardian": "GRD",
        "location": "LOC",
        "confirmed": "CFM",
        "negative": "NEG",
        "affirmative": "AFM",
        "assistance": "ASST",
        "sovereignty": "SOV",
        "ierahkwa": "IER",
        "community": "COMM",
        "network": "NET",
        "operational": "OPR",
        "frequency": "FREQ",
        "received": "RCVD",
        "message": "MSG",
        "position": "POS",
        "request": "REQ",
        "status": "STS",
    }

    compressed = body.lower().strip()
    for word, abbr in abbreviations.items():
        compressed = compressed.replace(word, abbr)

    # Remove redundant spaces
    compressed = " ".join(compressed.split())

    max_body = JS8_MAX_CHARS - len(prefix)
    if len(compressed) > max_body:
        compressed = compressed[:max_body]

    return f"{prefix}{compressed}"


def decompress_message(encoded: str) -> tuple[str, str]:
    """Decompress an HF message back into priority and body.

    Returns (priority_name, body).
    """
    if not encoded:
        return "PEACE", ""

    code = encoded[0]
    body = encoded[1:]

    priority_name = "PEACE"
    for name, info in PRIORITY_TYPES.items():
        if info["code"] == code:
            priority_name = name
            break

    return priority_name, body


# ---------------------------------------------------------------------------
# JS8Call TCP API client
# ---------------------------------------------------------------------------

class JS8CallClient:
    """Async client for the JS8Call TCP API."""

    def __init__(self, host: str = JS8CALL_HOST, port: int = JS8CALL_PORT):
        self.host = host
        self.port = port
        self._reader: Optional[asyncio.StreamReader] = None
        self._writer: Optional[asyncio.StreamWriter] = None
        self._connected = False
        self._rx_callbacks: list = []

    async def connect(self) -> bool:
        """Connect to the JS8Call TCP API."""
        try:
            self._reader, self._writer = await asyncio.open_connection(
                self.host, self.port
            )
            self._connected = True
            logger.info("Connected to JS8Call API at %s:%d", self.host, self.port)
            return True
        except (ConnectionRefusedError, OSError) as exc:
            logger.error("Failed to connect to JS8Call at %s:%d: %s", self.host, self.port, exc)
            self._connected = False
            return False

    async def disconnect(self) -> None:
        """Disconnect from JS8Call API."""
        if self._writer:
            self._writer.close()
            try:
                await self._writer.wait_closed()
            except Exception:
                pass
        self._connected = False
        logger.info("Disconnected from JS8Call API")

    @property
    def connected(self) -> bool:
        return self._connected

    async def _send_command(self, cmd_type: str, value: str = "", params: dict = None) -> None:
        """Send a command to JS8Call via TCP."""
        if not self._connected or not self._writer:
            logger.warning("Cannot send command — not connected to JS8Call")
            return

        message = {
            "type": cmd_type,
            "value": value,
            "params": params or {},
        }

        raw = json.dumps(message) + "\n"
        self._writer.write(raw.encode("utf-8"))
        await self._writer.drain()
        logger.debug("Sent to JS8Call: %s", raw.strip())

    async def send_message(self, destination: str, text: str) -> None:
        """Send a directed message to a specific callsign."""
        payload = f"{destination}: {text}"
        await self._send_command("TX.SEND_MESSAGE", value=payload)
        logger.info("TX -> %s: %s", destination, text)

    async def send_broadcast(self, text: str) -> None:
        """Send a broadcast (CQ-style) message."""
        await self._send_command("TX.SEND_MESSAGE", value=text)
        logger.info("TX broadcast: %s", text)

    async def set_frequency(self, band: str) -> None:
        """Set the dial frequency for a given band."""
        band_info = HF_BANDS.get(band)
        if not band_info:
            logger.error("Unknown band: %s (available: %s)", band, list(HF_BANDS.keys()))
            return
        await self._send_command("RIG.SET_FREQ", value="", params={"FREQ": band_info["freq_hz"]})
        logger.info("Set frequency to %s (%d Hz)", band, band_info["freq_hz"])

    async def get_station_callsign(self) -> Optional[str]:
        """Request the station callsign from JS8Call."""
        await self._send_command("STATION.GET_CALLSIGN")
        return CALLSIGN  # fallback to configured

    def on_receive(self, callback) -> None:
        """Register a callback for received messages."""
        self._rx_callbacks.append(callback)

    async def receive_loop(self) -> None:
        """Continuously read incoming messages from JS8Call."""
        if not self._reader:
            return

        buffer = b""
        while self._connected:
            try:
                chunk = await asyncio.wait_for(self._reader.read(4096), timeout=5.0)
                if not chunk:
                    logger.warning("JS8Call connection closed by remote")
                    self._connected = False
                    break

                buffer += chunk

                while b"\n" in buffer:
                    line, buffer = buffer.split(b"\n", 1)
                    line_str = line.decode("utf-8", errors="replace").strip()
                    if not line_str:
                        continue

                    try:
                        msg = json.loads(line_str)
                        await self._handle_message(msg)
                    except json.JSONDecodeError:
                        logger.debug("Non-JSON from JS8Call: %s", line_str[:100])

            except asyncio.TimeoutError:
                continue
            except Exception as exc:
                logger.error("Error in JS8Call receive loop: %s", exc)
                await asyncio.sleep(2)

    async def _handle_message(self, msg: dict) -> None:
        """Handle a parsed JSON message from JS8Call."""
        msg_type = msg.get("type", "")
        value = msg.get("value", "")
        params = msg.get("params", {})

        if msg_type == "RX.DIRECTED":
            snr = params.get("SNR", None)
            from_call = params.get("FROM", "UNKNOWN")
            text = value

            log_signal_report(from_call, snr)
            logger.info("RX <- %s (SNR: %s): %s", from_call, snr, text)

            for cb in self._rx_callbacks:
                try:
                    await cb(from_call, text, snr)
                except Exception as exc:
                    logger.error("Error in RX callback: %s", exc)

        elif msg_type == "RX.ACTIVITY":
            logger.debug("Activity: %s", value[:80])

        elif msg_type == "RIG.FREQ":
            freq = params.get("FREQ", 0)
            logger.info("Current frequency: %d Hz", freq)


# ---------------------------------------------------------------------------
# Signal reporting
# ---------------------------------------------------------------------------

def log_signal_report(callsign: str, snr: Optional[int]) -> None:
    """Log SNR for a received message."""
    entry = {
        "timestamp": datetime.now(timezone.utc).isoformat(),
        "callsign": callsign,
        "snr": snr,
    }
    _signal_log.append(entry)
    # Keep last 500 entries
    if len(_signal_log) > 500:
        _signal_log.pop(0)


def get_signal_reports() -> list[dict]:
    """Return recent signal reports."""
    return list(_signal_log)


# ---------------------------------------------------------------------------
# Matrix client helpers
# ---------------------------------------------------------------------------

async def matrix_login(client: AsyncClient) -> bool:
    """Log in to Matrix and return True on success."""
    response = await client.login(MATRIX_PASSWORD)
    if isinstance(response, LoginResponse):
        logger.info("Matrix login successful as %s", MATRIX_USER)
        return True
    logger.error("Matrix login failed: %s", response)
    return False


async def matrix_send(client: AsyncClient, room_id: str, text: str) -> None:
    """Send a text message to a Matrix room."""
    try:
        await client.room_send(
            room_id=room_id,
            message_type="m.room.message",
            content={"msgtype": "m.text", "body": text},
        )
    except Exception as exc:
        logger.error("Failed to send to Matrix: %s", exc)


# ---------------------------------------------------------------------------
# ntfy fallback
# ---------------------------------------------------------------------------

async def notify_ntfy(message: str, priority: str = "default") -> None:
    """Send a notification via ntfy as last-resort fallback."""
    ntfy_priority_map = {
        "PEACE": "low",
        "ALERT": "default",
        "EMERGENCY": "urgent",
        "HEARTBEAT": "min",
    }
    ntfy_prio = ntfy_priority_map.get(priority, "default")

    try:
        async with aiohttp.ClientSession() as session:
            await session.post(
                f"{NTFY_URL}/{NTFY_TOPIC}",
                data=message.encode("utf-8"),
                headers={
                    "Title": f"Ierahkwa HF: {priority}",
                    "Priority": ntfy_prio,
                    "Tags": "radio,ierahkwa",
                },
                timeout=aiohttp.ClientTimeout(total=10),
            )
        logger.info("ntfy notification sent: %s", message[:40])
    except Exception as exc:
        logger.error("ntfy fallback failed: %s", exc)


# ---------------------------------------------------------------------------
# Fallback chain: JS8Call -> LoRa -> Matrix -> ntfy
# ---------------------------------------------------------------------------

async def fallback_cascade(message: str, priority: str = "PEACE") -> bool:
    """Attempt delivery through the fallback chain.

    Order: JS8Call -> LoRa -> Matrix -> ntfy
    Returns True if at least one channel delivered.
    """
    delivered = False

    # 1. Matrix (primary digital channel)
    try:
        mx_client = AsyncClient(MATRIX_HOMESERVER, MATRIX_USER)
        if await matrix_login(mx_client):
            await matrix_send(mx_client, MATRIX_ROOM, f"[HF-{priority}] {message}")
            delivered = True
            logger.info("Fallback: delivered via Matrix")
        await mx_client.close()
    except Exception as exc:
        logger.warning("Fallback: Matrix failed: %s", exc)

    # 2. ntfy (always attempt as backup)
    try:
        await notify_ntfy(f"[{priority}] {message}", priority)
        delivered = True
        logger.info("Fallback: delivered via ntfy")
    except Exception as exc:
        logger.warning("Fallback: ntfy failed: %s", exc)

    # 3. LoRa (if enabled and available — would import lora_mesh_bridge)
    if LORA_FALLBACK:
        try:
            from scripts.protocols.lora_mesh_bridge import compress_for_lora
            lora_payload = compress_for_lora("hf-bridge", f"[{priority}] {message}")
            logger.info("Fallback: LoRa payload prepared (%d bytes)", len(lora_payload))
            delivered = True
        except ImportError:
            logger.debug("Fallback: LoRa bridge not available")
        except Exception as exc:
            logger.warning("Fallback: LoRa failed: %s", exc)

    if not delivered:
        logger.critical("ALL FALLBACK CHANNELS FAILED for message: %s", message[:50])

    return delivered


# ---------------------------------------------------------------------------
# Main bridge orchestrator
# ---------------------------------------------------------------------------

class HFBridge:
    """Main bridge between JS8Call HF radio and Matrix."""

    def __init__(self):
        self.js8 = JS8CallClient()
        self.matrix: Optional[AsyncClient] = None
        self._running = False
        self._last_heartbeat = 0.0

    async def start(self) -> None:
        """Start the HF bridge."""
        logger.info("=" * 60)
        logger.info("  Ierahkwa JS8Call HF Radio Bridge")
        logger.info("  Callsign: %s | Band: %s", CALLSIGN, HF_BAND)
        logger.info("  Heartbeat every %d seconds", HEARTBEAT_INTERVAL)
        logger.info("=" * 60)

        # Connect to Matrix
        self.matrix = AsyncClient(MATRIX_HOMESERVER, MATRIX_USER)
        if not await matrix_login(self.matrix):
            logger.error("Cannot start without Matrix connection")
            return

        # Connect to JS8Call
        if not await self.js8.connect():
            logger.error("Cannot connect to JS8Call at %s:%d", JS8CALL_HOST, JS8CALL_PORT)
            logger.info("Ensure JS8Call is running with TCP API enabled")
            return

        # Set band
        await self.js8.set_frequency(HF_BAND)

        # Register receive callback
        self.js8.on_receive(self._on_hf_receive)

        # Register default callsign mapping
        register_callsign(CALLSIGN, MATRIX_USER)

        self._running = True

        # Run tasks
        await asyncio.gather(
            self.js8.receive_loop(),
            self._heartbeat_loop(),
            self._matrix_listener(),
            return_exceptions=True,
        )

    async def stop(self) -> None:
        """Gracefully stop the bridge."""
        self._running = False
        await self.js8.disconnect()
        if self.matrix:
            await self.matrix.close()
        logger.info("HF Bridge stopped")

    async def _on_hf_receive(self, from_call: str, text: str, snr: Optional[int]) -> None:
        """Handle a received HF message — relay to Matrix."""
        priority, body = decompress_message(text)

        matrix_user = lookup_callsign(from_call) or from_call
        formatted = f"[HF {from_call} SNR:{snr}dB] ({priority}) {body}"

        logger.info("Relaying HF -> Matrix: %s", formatted)

        if self.matrix and MATRIX_ROOM:
            await matrix_send(self.matrix, MATRIX_ROOM, formatted)
        else:
            await fallback_cascade(formatted, priority)

    async def _heartbeat_loop(self) -> None:
        """Send heartbeat beacon at configured intervals."""
        while self._running:
            now = time.time()
            if now - self._last_heartbeat >= HEARTBEAT_INTERVAL:
                heartbeat = compress_message("HEARTBEAT", f"{CALLSIGN} OPR")
                await self.js8.send_broadcast(heartbeat)
                self._last_heartbeat = now
                logger.info("Heartbeat sent: %s", heartbeat)

                # Also report to Matrix
                if self.matrix and MATRIX_ROOM:
                    await matrix_send(
                        self.matrix,
                        MATRIX_ROOM,
                        f"[HF HEARTBEAT] {CALLSIGN} operational on {HF_BAND}",
                    )

            await asyncio.sleep(60)

    async def _matrix_listener(self) -> None:
        """Listen for Matrix messages to relay to HF.

        Only relays messages that start with !hf or are tagged as priority.
        """
        if not self.matrix or not MATRIX_ROOM:
            logger.warning("Matrix not configured — skipping Matrix listener")
            return

        sync_token = None
        while self._running:
            try:
                sync_response = await self.matrix.sync(timeout=10000, since=sync_token)
                sync_token = sync_response.next_batch

                for room_id, room_info in sync_response.rooms.join.items():
                    if room_id != MATRIX_ROOM:
                        continue

                    for event in room_info.timeline.events:
                        if not isinstance(event, RoomMessageText):
                            continue
                        if event.sender == MATRIX_USER:
                            continue

                        body = event.body.strip()

                        # Only relay messages prefixed with !hf
                        if not body.lower().startswith("!hf"):
                            continue

                        # Parse: !hf [PRIORITY] [CALLSIGN] message
                        parts = body[3:].strip().split(None, 2)
                        priority = "PEACE"
                        destination = "@ALLCALL"
                        message = body[3:].strip()

                        if len(parts) >= 1 and parts[0].upper() in PRIORITY_TYPES:
                            priority = parts[0].upper()
                            rest = parts[1:] if len(parts) > 1 else []
                        else:
                            rest = parts

                        if rest and rest[0].upper().replace("-", "").isalnum() and len(rest[0]) <= 10:
                            destination = rest[0].upper()
                            message = rest[1] if len(rest) > 1 else ""
                        elif rest:
                            message = " ".join(rest)

                        compressed = compress_message(priority, message)
                        logger.info("Matrix -> HF: [%s] %s -> %s", priority, compressed, destination)

                        if destination == "@ALLCALL":
                            await self.js8.send_broadcast(compressed)
                        else:
                            await self.js8.send_message(destination, compressed)

            except Exception as exc:
                logger.error("Matrix listener error: %s", exc)
                await asyncio.sleep(5)


# ---------------------------------------------------------------------------
# Entry point
# ---------------------------------------------------------------------------

def main():
    """Run the JS8Call HF Radio Bridge."""
    bridge = HFBridge()
    try:
        asyncio.run(bridge.start())
    except KeyboardInterrupt:
        logger.info("Interrupted — shutting down")
        asyncio.run(bridge.stop())


if __name__ == "__main__":
    main()
