#!/usr/bin/env python3
"""
Ierahkwa Bio-Ledger -- IoT Sensor Integration for Environmental Governance
Connects Arduino/ESP32 environmental sensors to the Ierahkwa governance system.
Reads sensor data over serial, evaluates thresholds, triggers governance
proposals for emergency fund release, archives readings to IPFS hourly,
and pushes alerts to Matrix.

Environment variables
---------------------
BIO_SERIAL_PORT       Serial port for ESP32          (default: /dev/ttyUSB1)
BIO_SERIAL_BAUD       Baud rate                      (default: 115200)
MATRIX_HOMESERVER     Matrix server URL               (default: https://matrix.ierahkwa.org)
MATRIX_USER           Bot Matrix user ID              (e.g. @bio-oracle:ierahkwa.org)
MATRIX_PASSWORD       Password for the bot user
MATRIX_BIO_ROOM       Matrix room for bio alerts      (e.g. !bio:ierahkwa.org)
MAMEYNODE_RPC         MameyNode JSON-RPC endpoint     (default: http://mameynode:8545)
TREASURY_ADDRESS      IerahkwaTreasury contract addr  (default: 0x...placeholder)
IPFS_API_URL          IPFS Kubo API endpoint          (default: http://127.0.0.1:5101)
BIO_DB_PATH           SQLite database path            (default: data/bio_ledger.db)
FLASK_PORT            REST API port                   (default: 5555)
LOG_DIR               Logging directory               (default: logs/)
IPFS_BATCH_INTERVAL   Seconds between IPFS batches    (default: 3600)
ALERT_COOLDOWN        Seconds between repeat alerts   (default: 300)
"""

import asyncio
import json
import logging
import os
import sqlite3
import sys
import threading
import time
from dataclasses import dataclass, asdict, field
from datetime import datetime, timezone
from pathlib import Path
from typing import Optional

try:
    import serial
except ImportError:
    serial = None

try:
    from flask import Flask, jsonify, request as flask_request
except ImportError:
    sys.exit("flask is required: pip install flask")

try:
    from nio import AsyncClient, LoginResponse
except ImportError:
    sys.exit("matrix-nio is required: pip install matrix-nio")

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

BIO_SERIAL_PORT = os.environ.get("BIO_SERIAL_PORT", "/dev/ttyUSB1")
BIO_SERIAL_BAUD = int(os.environ.get("BIO_SERIAL_BAUD", "115200"))
MATRIX_HOMESERVER = os.environ.get("MATRIX_HOMESERVER", "https://matrix.ierahkwa.org")
MATRIX_USER = os.environ.get("MATRIX_USER", "@bio-oracle:ierahkwa.org")
MATRIX_PASSWORD = os.environ.get("MATRIX_PASSWORD", "")
MATRIX_BIO_ROOM = os.environ.get("MATRIX_BIO_ROOM", "")
MAMEYNODE_RPC = os.environ.get("MAMEYNODE_RPC", "http://mameynode:8545")
TREASURY_ADDRESS = os.environ.get(
    "TREASURY_ADDRESS", "0x0000000000000000000000000000000000000000"
)
IPFS_API_URL = os.environ.get("IPFS_API_URL", "http://127.0.0.1:5101")
BIO_DB_PATH = Path(os.environ.get("BIO_DB_PATH", "data/bio_ledger.db"))
FLASK_PORT = int(os.environ.get("FLASK_PORT", "5555"))
LOG_DIR = Path(os.environ.get("LOG_DIR", "logs"))
IPFS_BATCH_INTERVAL = int(os.environ.get("IPFS_BATCH_INTERVAL", "3600"))
ALERT_COOLDOWN = int(os.environ.get("ALERT_COOLDOWN", "300"))

LOG_DIR.mkdir(parents=True, exist_ok=True)
BIO_DB_PATH.parent.mkdir(parents=True, exist_ok=True)

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(name)s: %(message)s",
    handlers=[
        logging.StreamHandler(sys.stdout),
        logging.FileHandler(LOG_DIR / "bio_ledger.log", encoding="utf-8"),
    ],
)
logger = logging.getLogger("ierahkwa.bio_ledger")

# ---------------------------------------------------------------------------
# Data model
# ---------------------------------------------------------------------------

SENSOR_FIELDS = [
    "soil_moisture",
    "air_quality_aqi",
    "temperature_c",
    "humidity_pct",
    "co2_ppm",
    "water_ph",
    "uv_index",
]


@dataclass
class BioReading:
    """Single environmental sensor reading from ESP32."""

    timestamp: str = ""
    soil_moisture: float = 0.0
    air_quality_aqi: int = 0
    temperature_c: float = 0.0
    humidity_pct: float = 0.0
    co2_ppm: int = 0
    water_ph: float = 7.0
    uv_index: float = 0.0
    alerts: list = field(default_factory=list)
    ipfs_cid: str = ""

    def to_dict(self) -> dict:
        return asdict(self)


# ---------------------------------------------------------------------------
# Threshold definitions
# ---------------------------------------------------------------------------

ALERT_LEVEL_WARNING = "WARNING"
ALERT_LEVEL_EMERGENCY = "EMERGENCY"

THRESHOLDS = {
    "DROUGHT_ALERT": {
        "field": "soil_moisture",
        "condition": "lt",
        "value": 15.0,
        "level": ALERT_LEVEL_WARNING,
        "message_es": "Alerta de sequia: humedad del suelo por debajo del 15%",
        "message_en": "Drought alert: soil moisture below 15%",
    },
    "AIR_EMERGENCY": {
        "field": "air_quality_aqi",
        "condition": "gt",
        "value": 150,
        "level": ALERT_LEVEL_EMERGENCY,
        "message_es": "Emergencia de calidad del aire: AQI superior a 150",
        "message_en": "Air quality emergency: AQI above 150",
    },
    "CO2_WARNING": {
        "field": "co2_ppm",
        "condition": "gt",
        "value": 1000,
        "level": ALERT_LEVEL_WARNING,
        "message_es": "Advertencia de CO2: concentracion superior a 1000 ppm",
        "message_en": "CO2 warning: concentration above 1000 ppm",
    },
    "WATER_CONTAMINATION_LOW": {
        "field": "water_ph",
        "condition": "lt",
        "value": 5.5,
        "level": ALERT_LEVEL_EMERGENCY,
        "message_es": "Contaminacion del agua: pH por debajo de 5.5",
        "message_en": "Water contamination: pH below 5.5",
    },
    "WATER_CONTAMINATION_HIGH": {
        "field": "water_ph",
        "condition": "gt",
        "value": 8.5,
        "level": ALERT_LEVEL_EMERGENCY,
        "message_es": "Contaminacion del agua: pH superior a 8.5",
        "message_en": "Water contamination: pH above 8.5",
    },
    "HEAT_EMERGENCY": {
        "field": "temperature_c",
        "condition": "gt",
        "value": 45.0,
        "level": ALERT_LEVEL_EMERGENCY,
        "message_es": "Emergencia de calor: temperatura superior a 45 C",
        "message_en": "Heat emergency: temperature above 45 C",
    },
}

# ---------------------------------------------------------------------------
# SQLite database
# ---------------------------------------------------------------------------


def init_db(db_path: Path) -> sqlite3.Connection:
    """Create the bio_readings table and return a connection."""
    conn = sqlite3.connect(str(db_path), check_same_thread=False)
    conn.execute("PRAGMA journal_mode=WAL")
    conn.execute(
        """
        CREATE TABLE IF NOT EXISTS bio_readings (
            id              INTEGER PRIMARY KEY AUTOINCREMENT,
            timestamp       TEXT NOT NULL,
            soil_moisture   REAL,
            air_quality_aqi INTEGER,
            temperature_c   REAL,
            humidity_pct    REAL,
            co2_ppm         INTEGER,
            water_ph        REAL,
            uv_index        REAL,
            alerts          TEXT,
            ipfs_cid        TEXT
        )
        """
    )
    conn.execute(
        """
        CREATE TABLE IF NOT EXISTS bio_alerts (
            id              INTEGER PRIMARY KEY AUTOINCREMENT,
            timestamp       TEXT NOT NULL,
            alert_type      TEXT NOT NULL,
            level           TEXT NOT NULL,
            value           REAL,
            message         TEXT,
            governance_tx   TEXT
        )
        """
    )
    conn.execute(
        "CREATE INDEX IF NOT EXISTS idx_readings_ts ON bio_readings (timestamp DESC)"
    )
    conn.execute(
        "CREATE INDEX IF NOT EXISTS idx_alerts_ts ON bio_alerts (timestamp DESC)"
    )
    conn.commit()
    return conn


def store_reading(conn: sqlite3.Connection, reading: BioReading) -> int:
    """Insert a reading into the database. Return the row id."""
    cur = conn.execute(
        """
        INSERT INTO bio_readings
            (timestamp, soil_moisture, air_quality_aqi, temperature_c,
             humidity_pct, co2_ppm, water_ph, uv_index, alerts, ipfs_cid)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """,
        (
            reading.timestamp,
            reading.soil_moisture,
            reading.air_quality_aqi,
            reading.temperature_c,
            reading.humidity_pct,
            reading.co2_ppm,
            reading.water_ph,
            reading.uv_index,
            json.dumps(reading.alerts, ensure_ascii=False),
            reading.ipfs_cid,
        ),
    )
    conn.commit()
    return cur.lastrowid


def store_alert(conn: sqlite3.Connection, alert_type: str, level: str,
                value: float, message: str, governance_tx: str = "") -> None:
    """Record an alert event."""
    conn.execute(
        """
        INSERT INTO bio_alerts (timestamp, alert_type, level, value, message, governance_tx)
        VALUES (?, ?, ?, ?, ?, ?)
        """,
        (datetime.now(timezone.utc).isoformat(), alert_type, level, value,
         message, governance_tx),
    )
    conn.commit()


def get_latest_readings(conn: sqlite3.Connection, limit: int = 50) -> list[dict]:
    """Return the most recent readings as a list of dicts."""
    cur = conn.execute(
        "SELECT * FROM bio_readings ORDER BY timestamp DESC LIMIT ?", (limit,)
    )
    columns = [desc[0] for desc in cur.description]
    rows = []
    for row in cur.fetchall():
        d = dict(zip(columns, row))
        if d.get("alerts"):
            try:
                d["alerts"] = json.loads(d["alerts"])
            except json.JSONDecodeError:
                pass
        rows.append(d)
    return rows


def get_recent_alerts(conn: sqlite3.Connection, limit: int = 100) -> list[dict]:
    """Return the most recent alerts."""
    cur = conn.execute(
        "SELECT * FROM bio_alerts ORDER BY timestamp DESC LIMIT ?", (limit,)
    )
    columns = [desc[0] for desc in cur.description]
    return [dict(zip(columns, row)) for row in cur.fetchall()]


# ---------------------------------------------------------------------------
# Serial reader -- ESP32
# ---------------------------------------------------------------------------


def parse_sensor_json(raw_line: str) -> Optional[BioReading]:
    """Parse a JSON line from the ESP32 serial output into a BioReading."""
    try:
        data = json.loads(raw_line.strip())
    except json.JSONDecodeError:
        return None

    reading = BioReading(
        timestamp=datetime.now(timezone.utc).isoformat(),
        soil_moisture=float(data.get("soil_moisture", 0)),
        air_quality_aqi=int(data.get("air_quality_aqi", 0)),
        temperature_c=float(data.get("temperature_c", 0)),
        humidity_pct=float(data.get("humidity_pct", 0)),
        co2_ppm=int(data.get("co2_ppm", 0)),
        water_ph=float(data.get("water_ph", 7.0)),
        uv_index=float(data.get("uv_index", 0)),
    )
    return reading


# ---------------------------------------------------------------------------
# Threshold evaluation
# ---------------------------------------------------------------------------


def evaluate_thresholds(reading: BioReading) -> list[dict]:
    """Check reading against all thresholds. Return list of triggered alerts."""
    triggered = []
    for alert_name, cfg in THRESHOLDS.items():
        field_val = getattr(reading, cfg["field"], None)
        if field_val is None:
            continue
        if cfg["condition"] == "lt" and field_val < cfg["value"]:
            triggered.append({
                "alert": alert_name,
                "level": cfg["level"],
                "field": cfg["field"],
                "value": field_val,
                "threshold": cfg["value"],
                "message_es": cfg["message_es"],
                "message_en": cfg["message_en"],
            })
        elif cfg["condition"] == "gt" and field_val > cfg["value"]:
            triggered.append({
                "alert": alert_name,
                "level": cfg["level"],
                "field": cfg["field"],
                "value": field_val,
                "threshold": cfg["value"],
                "message_es": cfg["message_es"],
                "message_en": cfg["message_en"],
            })
    return triggered


# ---------------------------------------------------------------------------
# Governance trigger -- IerahkwaTreasury proposal
# ---------------------------------------------------------------------------

# Minimal ABI for IerahkwaTreasury.propose(string description, uint256 amount)
TREASURY_ABI_FRAGMENT = [
    {
        "inputs": [
            {"name": "description", "type": "string"},
            {"name": "amount", "type": "uint256"},
        ],
        "name": "propose",
        "outputs": [{"name": "", "type": "uint256"}],
        "stateMutability": "nonpayable",
        "type": "function",
    }
]


def submit_emergency_proposal(alert_info: dict) -> str:
    """Submit an environmental emergency proposal to IerahkwaTreasury.

    Returns the transaction hash or empty string on failure.
    """
    if Web3 is None:
        logger.warning("web3.py not available. Governance proposal skipped.")
        return ""

    try:
        w3 = Web3(Web3.HTTPProvider(MAMEYNODE_RPC))
        if not w3.is_connected():
            logger.error("Cannot connect to MameyNode at %s", MAMEYNODE_RPC)
            return ""

        contract = w3.eth.contract(
            address=Web3.to_checksum_address(TREASURY_ADDRESS),
            abi=TREASURY_ABI_FRAGMENT,
        )

        description = (
            f"[BIO-EMERGENCY] {alert_info['alert']}: "
            f"{alert_info['message_en']} "
            f"(value={alert_info['value']}, threshold={alert_info['threshold']})"
        )
        # Emergency fund release amount: 1000 MATTR tokens (18 decimals)
        amount = w3.to_wei(1000, "ether")

        accounts = w3.eth.accounts
        if not accounts:
            logger.error("No unlocked accounts on MameyNode for proposal submission.")
            return ""

        tx_hash = contract.functions.propose(description, amount).transact(
            {"from": accounts[0]}
        )
        receipt = w3.eth.wait_for_transaction_receipt(tx_hash, timeout=60)
        tx_hex = receipt.transactionHash.hex()
        logger.info("Emergency proposal submitted: tx=%s", tx_hex)
        return tx_hex

    except Exception as exc:
        logger.error("Failed to submit governance proposal: %s", exc)
        return ""


# ---------------------------------------------------------------------------
# IPFS archival
# ---------------------------------------------------------------------------


def pin_readings_to_ipfs(readings: list[dict]) -> str:
    """Batch-pin sensor readings to IPFS. Return CID or empty string."""
    if not readings:
        return ""
    payload = json.dumps({
        "type": "bio_ledger_batch",
        "count": len(readings),
        "timestamp": datetime.now(timezone.utc).isoformat(),
        "readings": readings,
    }, ensure_ascii=False)

    try:
        resp = http_requests.post(
            f"{IPFS_API_URL}/api/v0/add",
            files={"file": ("bio_batch.json", payload.encode("utf-8"))},
            timeout=30,
        )
        resp.raise_for_status()
        result = resp.json()
        cid = result.get("Hash", "")
        logger.info("Pinned %d readings to IPFS: %s", len(readings), cid)
        return cid
    except Exception as exc:
        logger.error("IPFS pin failed: %s", exc)
        return ""


# ---------------------------------------------------------------------------
# Matrix notifications
# ---------------------------------------------------------------------------

_matrix_client: Optional[AsyncClient] = None
_matrix_loop: Optional[asyncio.AbstractEventLoop] = None


async def _send_matrix_alert(room_id: str, message: str) -> bool:
    """Send a message to the bio-oracle Matrix room."""
    if _matrix_client is None:
        return False
    try:
        await _matrix_client.room_send(
            room_id,
            "m.room.message",
            {"msgtype": "m.text", "body": message},
        )
        return True
    except Exception as exc:
        logger.error("Matrix alert failed: %s", exc)
        return False


def send_matrix_alert_sync(message: str) -> bool:
    """Thread-safe wrapper to send a Matrix alert from the serial reader thread."""
    if _matrix_loop is None or _matrix_client is None or not MATRIX_BIO_ROOM:
        return False
    future = asyncio.run_coroutine_threadsafe(
        _send_matrix_alert(MATRIX_BIO_ROOM, message), _matrix_loop
    )
    try:
        return future.result(timeout=15)
    except Exception:
        return False


# ---------------------------------------------------------------------------
# BioLedger core class
# ---------------------------------------------------------------------------


class BioLedger:
    """Main bio-ledger orchestrator. Manages serial reading, threshold
    evaluation, governance triggers, IPFS archival, and the REST API."""

    def __init__(self):
        self.conn = init_db(BIO_DB_PATH)
        self._running = False
        self._serial_port: Optional[serial.Serial] = None
        self._last_alert_times: dict[str, float] = {}
        self._pending_ipfs_batch: list[dict] = []
        self._last_ipfs_pin = time.time()
        self._flask_app = self._create_flask_app()

    # -- Alert cooldown ------------------------------------------------

    def _alert_on_cooldown(self, alert_name: str) -> bool:
        last = self._last_alert_times.get(alert_name, 0)
        return (time.time() - last) < ALERT_COOLDOWN

    def _record_alert_time(self, alert_name: str) -> None:
        self._last_alert_times[alert_name] = time.time()

    # -- Serial reader -------------------------------------------------

    def _open_serial(self) -> bool:
        if serial is None:
            logger.error("pyserial not installed. pip install pyserial")
            return False
        try:
            self._serial_port = serial.Serial(
                port=BIO_SERIAL_PORT,
                baudrate=BIO_SERIAL_BAUD,
                timeout=2,
            )
            logger.info(
                "Serial port opened: %s @ %d baud",
                BIO_SERIAL_PORT,
                BIO_SERIAL_BAUD,
            )
            return True
        except serial.SerialException as exc:
            logger.error("Cannot open serial port %s: %s", BIO_SERIAL_PORT, exc)
            return False

    def _serial_reader_loop(self) -> None:
        """Continuously read lines from the ESP32 serial port."""
        logger.info("Serial reader thread started.")
        reconnect_delay = 5

        while self._running:
            if self._serial_port is None or not self._serial_port.is_open:
                if not self._open_serial():
                    time.sleep(reconnect_delay)
                    continue

            try:
                raw = self._serial_port.readline()
                if not raw:
                    continue
                line = raw.decode("utf-8", errors="replace").strip()
                if not line:
                    continue

                reading = parse_sensor_json(line)
                if reading is None:
                    logger.debug("Non-JSON serial line: %s", line[:120])
                    continue

                self._process_reading(reading)

            except serial.SerialException as exc:
                logger.error("Serial read error: %s", exc)
                self._serial_port = None
                time.sleep(reconnect_delay)
            except Exception as exc:
                logger.exception("Unexpected error in serial reader: %s", exc)
                time.sleep(1)

    def _process_reading(self, reading: BioReading) -> None:
        """Evaluate thresholds, store, queue for IPFS, and alert."""
        alerts = evaluate_thresholds(reading)
        reading.alerts = [a["alert"] for a in alerts]

        # Store in SQLite
        store_reading(self.conn, reading)

        # Queue for IPFS batch
        self._pending_ipfs_batch.append(reading.to_dict())

        # Check IPFS batch timer
        if (time.time() - self._last_ipfs_pin) >= IPFS_BATCH_INTERVAL:
            self._flush_ipfs_batch()

        # Process alerts
        for alert_info in alerts:
            if self._alert_on_cooldown(alert_info["alert"]):
                continue
            self._record_alert_time(alert_info["alert"])

            level = alert_info["level"]
            alert_name = alert_info["alert"]

            store_alert(
                self.conn,
                alert_name,
                level,
                alert_info["value"],
                alert_info["message_en"],
            )

            # Matrix notification
            matrix_msg = (
                f"[BIO-{level}] {alert_info['message_es']}\n"
                f"{alert_info['message_en']}\n"
                f"Valor / Value: {alert_info['value']} "
                f"(umbral / threshold: {alert_info['threshold']})"
            )
            send_matrix_alert_sync(matrix_msg)

            # Emergency governance trigger
            if level == ALERT_LEVEL_EMERGENCY:
                logger.warning(
                    "EMERGENCY triggered: %s -- submitting governance proposal",
                    alert_name,
                )
                tx_hash = submit_emergency_proposal(alert_info)
                if tx_hash:
                    store_alert(
                        self.conn, alert_name, level,
                        alert_info["value"],
                        f"Governance proposal submitted: {tx_hash}",
                        governance_tx=tx_hash,
                    )
                    send_matrix_alert_sync(
                        f"[GOVERNANCE] Propuesta de emergencia enviada: tx={tx_hash}"
                    )

    def _flush_ipfs_batch(self) -> None:
        """Pin accumulated readings to IPFS and reset the batch."""
        if not self._pending_ipfs_batch:
            return
        batch = self._pending_ipfs_batch.copy()
        self._pending_ipfs_batch.clear()
        self._last_ipfs_pin = time.time()

        cid = pin_readings_to_ipfs(batch)
        if cid:
            logger.info("IPFS batch pinned: %d readings, CID=%s", len(batch), cid)

    # -- Flask REST API ------------------------------------------------

    def _create_flask_app(self) -> Flask:
        app = Flask("bio_ledger_api")
        app.config["JSONIFY_PRETTYPRINT_REGULAR"] = True

        @app.route("/api/bio/latest", methods=["GET"])
        def api_latest():
            limit = flask_request.args.get("limit", 10, type=int)
            limit = min(limit, 500)
            readings = get_latest_readings(self.conn, limit)
            return jsonify({"status": "ok", "count": len(readings), "readings": readings})

        @app.route("/api/bio/history", methods=["GET"])
        def api_history():
            limit = flask_request.args.get("limit", 100, type=int)
            limit = min(limit, 5000)
            readings = get_latest_readings(self.conn, limit)
            return jsonify({"status": "ok", "count": len(readings), "readings": readings})

        @app.route("/api/bio/alerts", methods=["GET"])
        def api_alerts():
            limit = flask_request.args.get("limit", 50, type=int)
            alerts = get_recent_alerts(self.conn, min(limit, 1000))
            return jsonify({"status": "ok", "count": len(alerts), "alerts": alerts})

        @app.route("/api/bio/thresholds", methods=["GET"])
        def api_thresholds():
            return jsonify({"status": "ok", "thresholds": THRESHOLDS})

        @app.route("/api/bio/status", methods=["GET"])
        def api_status():
            serial_ok = (
                self._serial_port is not None and self._serial_port.is_open
            )
            readings = get_latest_readings(self.conn, 1)
            last_reading = readings[0] if readings else None
            return jsonify({
                "status": "ok",
                "serial_connected": serial_ok,
                "serial_port": BIO_SERIAL_PORT,
                "last_reading": last_reading,
                "pending_ipfs_batch": len(self._pending_ipfs_batch),
            })

        @app.route("/api/bio/dashboard", methods=["GET"])
        def api_dashboard():
            """Visualization data for ConscienceDashboard integration."""
            readings = get_latest_readings(self.conn, 100)
            alerts = get_recent_alerts(self.conn, 20)
            latest = readings[0] if readings else {}

            # Compute simple stats
            def avg_field(name):
                vals = [r.get(name) for r in readings if r.get(name) is not None]
                return round(sum(vals) / len(vals), 2) if vals else 0

            return jsonify({
                "status": "ok",
                "current": latest,
                "averages": {
                    "soil_moisture": avg_field("soil_moisture"),
                    "air_quality_aqi": avg_field("air_quality_aqi"),
                    "temperature_c": avg_field("temperature_c"),
                    "humidity_pct": avg_field("humidity_pct"),
                    "co2_ppm": avg_field("co2_ppm"),
                    "water_ph": avg_field("water_ph"),
                    "uv_index": avg_field("uv_index"),
                },
                "recent_alerts": alerts,
                "sample_count": len(readings),
            })

        return app

    def _run_flask(self) -> None:
        """Run the Flask API in a separate thread."""
        logger.info("Starting Bio-Ledger REST API on port %d", FLASK_PORT)
        self._flask_app.run(
            host="0.0.0.0",
            port=FLASK_PORT,
            debug=False,
            use_reloader=False,
        )

    # -- Matrix setup --------------------------------------------------

    async def _init_matrix(self) -> bool:
        global _matrix_client, _matrix_loop
        if not MATRIX_PASSWORD:
            logger.warning("MATRIX_PASSWORD not set. Matrix alerts disabled.")
            return False

        _matrix_loop = asyncio.get_event_loop()
        client = AsyncClient(MATRIX_HOMESERVER, MATRIX_USER)
        login_resp = await client.login(MATRIX_PASSWORD)
        if not isinstance(login_resp, LoginResponse):
            logger.error("Matrix login failed: %s", login_resp)
            return False
        _matrix_client = client
        logger.info("Matrix logged in as %s", MATRIX_USER)
        return True

    # -- Main entry point ----------------------------------------------

    def start(self) -> None:
        """Start all BioLedger subsystems."""
        logger.info("=" * 60)
        logger.info("  Ierahkwa Bio-Ledger starting")
        logger.info("  Serial: %s @ %d baud", BIO_SERIAL_PORT, BIO_SERIAL_BAUD)
        logger.info("  Database: %s", BIO_DB_PATH)
        logger.info("  REST API: http://0.0.0.0:%d", FLASK_PORT)
        logger.info("=" * 60)

        self._running = True

        # Flask API in background thread
        flask_thread = threading.Thread(target=self._run_flask, daemon=True)
        flask_thread.start()

        # Serial reader in background thread
        serial_thread = threading.Thread(target=self._serial_reader_loop, daemon=True)
        serial_thread.start()

        # Matrix + async event loop in main thread
        loop = asyncio.new_event_loop()
        asyncio.set_event_loop(loop)

        try:
            loop.run_until_complete(self._init_matrix())
            # Keep alive and handle IPFS flush periodically
            loop.run_until_complete(self._keepalive_loop())
        except KeyboardInterrupt:
            logger.info("Shutting down on keyboard interrupt.")
        finally:
            self._running = False
            if self._serial_port and self._serial_port.is_open:
                self._serial_port.close()
            if _matrix_client:
                loop.run_until_complete(_matrix_client.close())
            loop.close()
            self.conn.close()
            logger.info("Bio-Ledger stopped.")

    async def _keepalive_loop(self) -> None:
        """Periodic maintenance: flush IPFS batches."""
        while self._running:
            await asyncio.sleep(60)
            if (time.time() - self._last_ipfs_pin) >= IPFS_BATCH_INTERVAL:
                self._flush_ipfs_batch()


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------


def main() -> None:
    ledger = BioLedger()
    ledger.start()


if __name__ == "__main__":
    main()
