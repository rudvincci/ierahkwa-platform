#!/usr/bin/env python3
"""
Ierahkwa LoRa-to-Satellite Uplink Driver
=========================================
When terrestrial internet is severed, sovereign nodes point LoRa antennas
at LEO satellites (TinyGS, EchoStar) to maintain communications for 1B+
citizens across the Americas.

Supports SX1276/78 via adafruit_rfm9x over SPI, TinyGS ground-station
registration, simplified orbital pass scheduling, duplex relay, and an
emergency SOS broadcast with GPS coordinates.

Environment variables
---------------------
SAT_FREQ_MHZ           Uplink frequency in MHz        (default: 433.0)
SAT_SPREADING_FACTOR   LoRa spreading factor 7-12     (default: 12)
SAT_TX_POWER           Transmit power in dBm          (default: 20)
SAT_CODING_RATE        Coding rate 5-8                (default: 8)
SAT_BANDWIDTH          Bandwidth in Hz                (default: 125000)
SAT_NODE_ID            Unique node identifier         (default: IER-SAT-001)
SAT_GPS_LAT            Station latitude               (default: 19.4326)
SAT_GPS_LON            Station longitude              (default: -99.1332)
SAT_GPS_ALT            Station altitude in meters     (default: 2240)
SAT_TLE_PATH           Path to TLE data file          (default: data/tle_catalog.json)
MATRIX_HOMESERVER      Matrix server URL              (default: https://matrix.ierahkwa.org)
MATRIX_USER            Bot Matrix user ID
MATRIX_PASSWORD        Bot password
MATRIX_ROOM            Default bridge room
NTFY_URL               ntfy fallback URL              (default: https://ntfy.sh)
NTFY_TOPIC             ntfy topic                     (default: ierahkwa-sat)
LOG_DIR                Logging directory              (default: logs/)
"""

import asyncio
import json
import logging
import math
import os
import struct
import sys
import time
import zlib
from datetime import datetime, timezone
from pathlib import Path
from typing import Optional

try:
    from nio import AsyncClient, LoginResponse
except ImportError:
    AsyncClient = None
    LoginResponse = None

try:
    import aiohttp
except ImportError:
    aiohttp = None

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------

SAT_FREQ_MHZ = float(os.environ.get("SAT_FREQ_MHZ", "433.0"))
SAT_SPREADING_FACTOR = int(os.environ.get("SAT_SPREADING_FACTOR", "12"))
SAT_TX_POWER = int(os.environ.get("SAT_TX_POWER", "20"))
SAT_CODING_RATE = int(os.environ.get("SAT_CODING_RATE", "8"))
SAT_BANDWIDTH = int(os.environ.get("SAT_BANDWIDTH", "125000"))
SAT_NODE_ID = os.environ.get("SAT_NODE_ID", "IER-SAT-001")
SAT_GPS_LAT = float(os.environ.get("SAT_GPS_LAT", "19.4326"))
SAT_GPS_LON = float(os.environ.get("SAT_GPS_LON", "-99.1332"))
SAT_GPS_ALT = float(os.environ.get("SAT_GPS_ALT", "2240"))
SAT_TLE_PATH = Path(os.environ.get("SAT_TLE_PATH", "data/tle_catalog.json"))
MATRIX_HOMESERVER = os.environ.get("MATRIX_HOMESERVER", "https://matrix.ierahkwa.org")
MATRIX_USER = os.environ.get("MATRIX_USER", "@sat-uplink:ierahkwa.org")
MATRIX_PASSWORD = os.environ.get("MATRIX_PASSWORD", "")
MATRIX_ROOM = os.environ.get("MATRIX_ROOM", "")
NTFY_URL = os.environ.get("NTFY_URL", "https://ntfy.sh")
NTFY_TOPIC = os.environ.get("NTFY_TOPIC", "ierahkwa-sat")
LOG_DIR = Path(os.environ.get("LOG_DIR", "logs"))

LOG_DIR.mkdir(parents=True, exist_ok=True)
SAT_TLE_PATH.parent.mkdir(parents=True, exist_ok=True)

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(name)s: %(message)s",
    handlers=[
        logging.StreamHandler(sys.stdout),
        logging.FileHandler(LOG_DIR / "satellite_uplink.log", encoding="utf-8"),
    ],
)
logger = logging.getLogger("ierahkwa.sat_uplink")

# ---------------------------------------------------------------------------
# IERAHKWA_SAT Frame Protocol
# ---------------------------------------------------------------------------
# Header:  4 bytes  "ISAT"
# Type:    1 byte   (0=HEARTBEAT, 1=ALERT, 2=DATA, 3=SOS)
# Payload: N bytes  (zlib compressed)
# CRC:     2 bytes  (CRC-16/CCITT)

FRAME_HEADER = b"ISAT"
MSG_HEARTBEAT = 0x00
MSG_ALERT = 0x01
MSG_DATA = 0x02
MSG_SOS = 0x03

MSG_TYPE_NAMES = {
    MSG_HEARTBEAT: "HEARTBEAT",
    MSG_ALERT: "ALERT",
    MSG_DATA: "DATA",
    MSG_SOS: "SOS",
}

# Supported frequency bands
FREQ_BANDS = {
    "433": {"min": 433.0, "max": 434.79, "region": "Global ISM"},
    "868": {"min": 868.0, "max": 868.6, "region": "EU ISM"},
    "915": {"min": 902.0, "max": 928.0, "region": "Americas ISM"},
}


def crc16_ccitt(data: bytes) -> int:
    """Compute CRC-16/CCITT over raw bytes."""
    crc = 0xFFFF
    for byte in data:
        crc ^= byte << 8
        for _ in range(8):
            if crc & 0x8000:
                crc = (crc << 1) ^ 0x1021
            else:
                crc <<= 1
            crc &= 0xFFFF
    return crc


def build_frame(msg_type: int, payload: dict) -> bytes:
    """Build an IERAHKWA_SAT frame with header, type, compressed payload, CRC."""
    raw_payload = json.dumps(payload, separators=(",", ":")).encode("utf-8")
    compressed = zlib.compress(raw_payload, level=9)
    body = FRAME_HEADER + struct.pack("B", msg_type) + compressed
    crc = crc16_ccitt(body)
    return body + struct.pack(">H", crc)


def parse_frame(data: bytes) -> Optional[dict]:
    """Parse an IERAHKWA_SAT frame. Returns dict or None on failure."""
    if len(data) < 7:
        return None
    if data[:4] != FRAME_HEADER:
        return None

    crc_received = struct.unpack(">H", data[-2:])[0]
    body = data[:-2]
    crc_computed = crc16_ccitt(body)
    if crc_received != crc_computed:
        logger.warning("CRC mismatch: received 0x%04X, computed 0x%04X", crc_received, crc_computed)
        return None

    msg_type = struct.unpack("B", data[4:5])[0]
    compressed = data[5:-2]

    try:
        raw_payload = zlib.decompress(compressed)
        payload = json.loads(raw_payload.decode("utf-8"))
    except (zlib.error, json.JSONDecodeError, UnicodeDecodeError) as exc:
        logger.warning("Frame payload decode failed: %s", exc)
        return None

    return {
        "type": msg_type,
        "type_name": MSG_TYPE_NAMES.get(msg_type, "UNKNOWN"),
        "payload": payload,
    }


# ---------------------------------------------------------------------------
# Simplified Orbital Mechanics for LEO satellite pass prediction
# ---------------------------------------------------------------------------

EARTH_RADIUS_KM = 6371.0
EARTH_MU = 398600.4418  # km^3/s^2
J2 = 1.08263e-3
TWO_PI = 2 * math.pi
DEG2RAD = math.pi / 180
RAD2DEG = 180 / math.pi


class SimpleTLE:
    """Simplified Two-Line Element for LEO pass prediction."""

    def __init__(self, name: str, inclination_deg: float, raan_deg: float,
                 eccentricity: float, arg_perigee_deg: float,
                 mean_anomaly_deg: float, mean_motion_revs_day: float,
                 epoch_unix: float, norad_id: int = 0):
        self.name = name
        self.norad_id = norad_id
        self.inclination = inclination_deg * DEG2RAD
        self.raan = raan_deg * DEG2RAD
        self.eccentricity = eccentricity
        self.arg_perigee = arg_perigee_deg * DEG2RAD
        self.mean_anomaly = mean_anomaly_deg * DEG2RAD
        self.mean_motion = mean_motion_revs_day * TWO_PI / 86400.0  # rad/s
        self.epoch_unix = epoch_unix

        # Derive semi-major axis from mean motion
        n_rad_s = self.mean_motion
        if n_rad_s > 0:
            self.semi_major_axis = (EARTH_MU / (n_rad_s ** 2)) ** (1 / 3)
        else:
            self.semi_major_axis = EARTH_RADIUS_KM + 550  # default LEO

        self.altitude_km = self.semi_major_axis - EARTH_RADIUS_KM
        self.period_s = TWO_PI / n_rad_s if n_rad_s > 0 else 5400

    def satellite_position(self, t_unix: float) -> tuple:
        """Return approximate (lat, lon, alt_km) at time t_unix.
        Uses simplified Keplerian propagation without perturbations.
        """
        dt = t_unix - self.epoch_unix
        mean_anom = self.mean_anomaly + self.mean_motion * dt
        mean_anom = mean_anom % TWO_PI

        # Solve Kepler's equation (Newton's method, 5 iterations)
        E = mean_anom
        for _ in range(5):
            E = E - (E - self.eccentricity * math.sin(E) - mean_anom) / \
                (1 - self.eccentricity * math.cos(E))

        # True anomaly
        nu = 2 * math.atan2(
            math.sqrt(1 + self.eccentricity) * math.sin(E / 2),
            math.sqrt(1 - self.eccentricity) * math.cos(E / 2),
        )

        # Position in orbital plane
        r = self.semi_major_axis * (1 - self.eccentricity * math.cos(E))
        u = nu + self.arg_perigee

        # RAAN drift (J2 approximation)
        raan = self.raan + (-1.5 * J2 * (EARTH_RADIUS_KM / self.semi_major_axis) ** 2 *
                            math.cos(self.inclination) * self.mean_motion) * dt

        # Convert to ECI-like lat/lon
        x_orb = r * math.cos(u)
        y_orb = r * math.sin(u)

        x = x_orb * math.cos(raan) - y_orb * math.sin(raan) * math.cos(self.inclination)
        y = x_orb * math.sin(raan) + y_orb * math.cos(raan) * math.cos(self.inclination)
        z = y_orb * math.sin(self.inclination)

        lon = math.atan2(y, x) * RAD2DEG
        lat = math.atan2(z, math.sqrt(x ** 2 + y ** 2)) * RAD2DEG
        alt = r - EARTH_RADIUS_KM

        # Correct for Earth rotation
        earth_rotation_rate = 7.2921159e-5  # rad/s
        lon -= (earth_rotation_rate * dt) * RAD2DEG
        lon = ((lon + 180) % 360) - 180

        return lat, lon, alt


def angular_distance(lat1: float, lon1: float, lat2: float, lon2: float) -> float:
    """Compute great-circle angular distance in degrees."""
    phi1, phi2 = lat1 * DEG2RAD, lat2 * DEG2RAD
    dphi = (lat2 - lat1) * DEG2RAD
    dlam = (lon2 - lon1) * DEG2RAD
    a = math.sin(dphi / 2) ** 2 + math.cos(phi1) * math.cos(phi2) * math.sin(dlam / 2) ** 2
    return 2 * math.asin(math.sqrt(a)) * RAD2DEG


def max_elevation_angle(distance_deg: float, alt_km: float) -> float:
    """Approximate maximum elevation angle of satellite from ground station."""
    distance_km = distance_deg * DEG2RAD * EARTH_RADIUS_KM
    if distance_km <= 0:
        return 90.0
    elev = math.atan2(alt_km - EARTH_RADIUS_KM * (1 - math.cos(distance_deg * DEG2RAD)),
                      distance_km) * RAD2DEG
    return max(0, elev)


# ---------------------------------------------------------------------------
# Pass Scheduler
# ---------------------------------------------------------------------------

class PassScheduler:
    """Schedule satellite passes and determine transmission windows."""

    def __init__(self, station_lat: float, station_lon: float, station_alt: float,
                 min_elevation: float = 10.0):
        self.station_lat = station_lat
        self.station_lon = station_lon
        self.station_alt = station_alt
        self.min_elevation = min_elevation
        self.satellites: list[SimpleTLE] = []
        self._pass_cache: list[dict] = []

    def load_tle_catalog(self, path: Path) -> int:
        """Load satellite TLE data from JSON catalog. Returns count loaded."""
        if not path.exists():
            logger.warning("TLE catalog not found at %s, using defaults", path)
            self._load_defaults()
            return len(self.satellites)

        try:
            with open(path, "r", encoding="utf-8") as fh:
                catalog = json.load(fh)
            for entry in catalog:
                tle = SimpleTLE(
                    name=entry.get("name", "UNKNOWN"),
                    inclination_deg=entry.get("inclination", 97.5),
                    raan_deg=entry.get("raan", 0),
                    eccentricity=entry.get("eccentricity", 0.001),
                    arg_perigee_deg=entry.get("arg_perigee", 0),
                    mean_anomaly_deg=entry.get("mean_anomaly", 0),
                    mean_motion_revs_day=entry.get("mean_motion", 15.5),
                    epoch_unix=entry.get("epoch_unix", time.time()),
                    norad_id=entry.get("norad_id", 0),
                )
                self.satellites.append(tle)
            logger.info("Loaded %d satellites from TLE catalog", len(self.satellites))
        except (json.JSONDecodeError, KeyError, TypeError) as exc:
            logger.error("Failed to parse TLE catalog: %s", exc)
            self._load_defaults()

        return len(self.satellites)

    def _load_defaults(self) -> None:
        """Load default LEO satellite entries for TinyGS and EchoStar."""
        defaults = [
            {"name": "NORBI", "inclination": 97.5, "mean_motion": 15.2, "raan": 45.0},
            {"name": "FOSSASAT-2E", "inclination": 97.7, "mean_motion": 15.3, "raan": 120.0},
            {"name": "TINYGS-1", "inclination": 51.6, "mean_motion": 15.5, "raan": 200.0},
            {"name": "ECHOSTAR-LEO-1", "inclination": 53.0, "mean_motion": 15.1, "raan": 300.0},
            {"name": "LACUNA-SAT", "inclination": 97.4, "mean_motion": 15.4, "raan": 80.0},
        ]
        for d in defaults:
            tle = SimpleTLE(
                name=d["name"],
                inclination_deg=d["inclination"],
                raan_deg=d.get("raan", 0),
                eccentricity=0.001,
                arg_perigee_deg=0,
                mean_anomaly_deg=0,
                mean_motion_revs_day=d["mean_motion"],
                epoch_unix=time.time(),
            )
            self.satellites.append(tle)
        logger.info("Loaded %d default satellite entries", len(self.satellites))

    def find_next_passes(self, duration_hours: float = 24.0, step_seconds: float = 30.0) -> list[dict]:
        """Find all satellite passes within the next duration_hours.
        Returns list of {satellite, start_time, end_time, max_elevation, duration}.
        """
        passes = []
        now = time.time()
        end_time = now + duration_hours * 3600

        for sat in self.satellites:
            in_pass = False
            pass_start = 0.0
            peak_elev = 0.0
            t = now

            while t < end_time:
                lat, lon, alt = sat.satellite_position(t)
                dist = angular_distance(self.station_lat, self.station_lon, lat, lon)
                elev = max_elevation_angle(dist, alt)

                if elev >= self.min_elevation:
                    if not in_pass:
                        in_pass = True
                        pass_start = t
                        peak_elev = elev
                    else:
                        peak_elev = max(peak_elev, elev)
                else:
                    if in_pass:
                        passes.append({
                            "satellite": sat.name,
                            "norad_id": sat.norad_id,
                            "start_time": pass_start,
                            "end_time": t,
                            "max_elevation": round(peak_elev, 1),
                            "duration_s": round(t - pass_start),
                            "altitude_km": round(sat.altitude_km, 1),
                        })
                        in_pass = False
                        peak_elev = 0.0

                t += step_seconds

            # Close any open pass at end of window
            if in_pass:
                passes.append({
                    "satellite": sat.name,
                    "norad_id": sat.norad_id,
                    "start_time": pass_start,
                    "end_time": end_time,
                    "max_elevation": round(peak_elev, 1),
                    "duration_s": round(end_time - pass_start),
                    "altitude_km": round(sat.altitude_km, 1),
                })

        passes.sort(key=lambda p: p["start_time"])
        self._pass_cache = passes
        logger.info("Found %d satellite passes in next %.0fh", len(passes), duration_hours)
        return passes

    def next_pass(self) -> Optional[dict]:
        """Return the soonest upcoming pass or None."""
        now = time.time()
        for p in self._pass_cache:
            if p["end_time"] > now:
                return p
        return None

    def is_pass_active(self) -> Optional[dict]:
        """Return the currently active pass, or None."""
        now = time.time()
        for p in self._pass_cache:
            if p["start_time"] <= now <= p["end_time"]:
                return p
        return None


# ---------------------------------------------------------------------------
# LoRa Radio Driver
# ---------------------------------------------------------------------------

class LoRaSatelliteRadio:
    """SX1276/78 LoRa radio driver for satellite uplink via adafruit_rfm9x."""

    def __init__(self, freq_mhz: float = SAT_FREQ_MHZ, sf: int = SAT_SPREADING_FACTOR,
                 tx_power: int = SAT_TX_POWER, coding_rate: int = SAT_CODING_RATE,
                 bandwidth: int = SAT_BANDWIDTH):
        self.freq_mhz = freq_mhz
        self.sf = sf
        self.tx_power = tx_power
        self.coding_rate = coding_rate
        self.bandwidth = bandwidth
        self.rfm9x = None
        self._initialized = False
        self._tx_count = 0
        self._rx_count = 0
        self._last_rssi = None
        self._last_snr = None

    def initialize(self) -> bool:
        """Initialize the SX1276/78 radio over SPI."""
        try:
            import board
            import busio
            import digitalio
            import adafruit_rfm9x

            spi = busio.SPI(board.SCK, MOSI=board.MOSI, MISO=board.MISO)
            cs = digitalio.DigitalInOut(board.CE1)
            reset = digitalio.DigitalInOut(board.D25)

            self.rfm9x = adafruit_rfm9x.RFM9x(spi, cs, reset, self.freq_mhz)
            self.rfm9x.spreading_factor = self.sf
            self.rfm9x.signal_bandwidth = self.bandwidth
            self.rfm9x.coding_rate = self.coding_rate
            self.rfm9x.tx_power = self.tx_power
            self.rfm9x.enable_crc = True

            self._initialized = True
            logger.info(
                "LoRa radio initialized: %.3f MHz, SF%d, BW %d Hz, CR 4/%d, TX %d dBm",
                self.freq_mhz, self.sf, self.bandwidth, self.coding_rate, self.tx_power,
            )
            return True

        except ImportError:
            logger.warning(
                "adafruit_rfm9x not available -- running in simulation mode. "
                "Install on Raspberry Pi: pip install adafruit-circuitpython-rfm9x"
            )
            self._initialized = True  # Simulation mode
            return True
        except Exception as exc:
            logger.error("Failed to initialize LoRa radio: %s", exc)
            return False

    def set_frequency(self, freq_mhz: float) -> None:
        """Change the operating frequency."""
        self.freq_mhz = freq_mhz
        if self.rfm9x:
            self.rfm9x.frequency_mhz = freq_mhz
        logger.info("Frequency set to %.3f MHz", freq_mhz)

    def transmit(self, data: bytes) -> bool:
        """Transmit raw bytes over LoRa."""
        if not self._initialized:
            logger.error("Radio not initialized")
            return False

        if self.rfm9x:
            try:
                self.rfm9x.send(data)
                self._tx_count += 1
                logger.info("TX %d bytes (total: %d)", len(data), self._tx_count)
                return True
            except Exception as exc:
                logger.error("TX failed: %s", exc)
                return False
        else:
            # Simulation mode
            self._tx_count += 1
            logger.info("[SIM] TX %d bytes (total: %d)", len(data), self._tx_count)
            return True

    def receive(self, timeout: float = 5.0) -> Optional[bytes]:
        """Receive data from LoRa radio. Returns bytes or None on timeout."""
        if not self._initialized:
            return None

        if self.rfm9x:
            try:
                data = self.rfm9x.receive(timeout=timeout)
                if data:
                    self._rx_count += 1
                    self._last_rssi = self.rfm9x.last_rssi
                    self._last_snr = getattr(self.rfm9x, "last_snr", None)
                    logger.info(
                        "RX %d bytes (RSSI: %s dBm, SNR: %s dB)",
                        len(data), self._last_rssi, self._last_snr,
                    )
                return data
            except Exception as exc:
                logger.error("RX failed: %s", exc)
                return None
        return None

    def sleep(self) -> None:
        """Put the radio into low-power sleep mode."""
        if self.rfm9x:
            try:
                self.rfm9x.sleep()
                logger.info("Radio entering sleep mode")
            except Exception:
                pass

    def wake(self) -> None:
        """Wake the radio from sleep mode."""
        if self.rfm9x:
            try:
                self.rfm9x.listen()
                logger.info("Radio waking up -- listen mode")
            except Exception:
                pass

    @property
    def stats(self) -> dict:
        return {
            "tx_count": self._tx_count,
            "rx_count": self._rx_count,
            "last_rssi": self._last_rssi,
            "last_snr": self._last_snr,
            "frequency_mhz": self.freq_mhz,
            "spreading_factor": self.sf,
        }


# ---------------------------------------------------------------------------
# Transmission Log
# ---------------------------------------------------------------------------

TX_LOG_PATH = LOG_DIR / "satellite_tx.jsonl"


def log_transmission(entry: dict) -> None:
    """Log a satellite transmission to the JSONL log."""
    record = {
        "ts": datetime.now(timezone.utc).isoformat(),
        **entry,
    }
    try:
        with open(TX_LOG_PATH, "a", encoding="utf-8") as fh:
            fh.write(json.dumps(record, ensure_ascii=False) + "\n")
    except OSError as exc:
        logger.error("Failed to write TX log: %s", exc)


# ---------------------------------------------------------------------------
# Satellite Uplink Orchestrator
# ---------------------------------------------------------------------------

class SatelliteUplink:
    """Main orchestrator: LoRa radio + pass scheduler + protocol."""

    def __init__(self):
        self.radio = LoRaSatelliteRadio()
        self.scheduler = PassScheduler(SAT_GPS_LAT, SAT_GPS_LON, SAT_GPS_ALT)
        self.matrix_client: Optional[AsyncClient] = None
        self._running = False
        self._tx_queue: list[bytes] = []
        self._rx_buffer: list[dict] = []

    async def start(self) -> None:
        """Initialize and start the satellite uplink system."""
        logger.info("=" * 60)
        logger.info("  Ierahkwa Satellite Uplink -- %s", SAT_NODE_ID)
        logger.info("  Station: %.4f, %.4f, %.0fm", SAT_GPS_LAT, SAT_GPS_LON, SAT_GPS_ALT)
        logger.info("  Frequency: %.3f MHz | SF%d", SAT_FREQ_MHZ, SAT_SPREADING_FACTOR)
        logger.info("=" * 60)

        if not self.radio.initialize():
            logger.error("Radio initialization failed -- aborting")
            return

        self.scheduler.load_tle_catalog(SAT_TLE_PATH)
        self.scheduler.find_next_passes(duration_hours=24)

        # Initialize Matrix client if available
        if AsyncClient and MATRIX_PASSWORD:
            self.matrix_client = AsyncClient(MATRIX_HOMESERVER, MATRIX_USER)
            try:
                resp = await self.matrix_client.login(MATRIX_PASSWORD)
                if isinstance(resp, LoginResponse):
                    logger.info("Matrix login successful")
                else:
                    logger.warning("Matrix login failed: %s", resp)
                    self.matrix_client = None
            except Exception as exc:
                logger.warning("Matrix unavailable: %s", exc)
                self.matrix_client = None

        self._running = True

        await asyncio.gather(
            self._pass_monitor_loop(),
            self._tx_queue_processor(),
            self._rx_monitor_loop(),
            self._heartbeat_loop(),
            return_exceptions=True,
        )

    async def stop(self) -> None:
        """Gracefully shut down."""
        self._running = False
        self.radio.sleep()
        if self.matrix_client:
            await self.matrix_client.close()
        logger.info("Satellite uplink stopped")

    # -- Transmission --

    def queue_message(self, msg_type: int, payload: dict, priority: int = 0) -> None:
        """Queue a message for satellite transmission."""
        payload["node"] = SAT_NODE_ID
        payload["ts"] = int(time.time())
        frame = build_frame(msg_type, payload)
        self._tx_queue.append(frame)
        logger.info(
            "Queued %s frame (%d bytes, queue size: %d)",
            MSG_TYPE_NAMES.get(msg_type, "?"), len(frame), len(self._tx_queue),
        )

    def send_sos(self, distress_message: str = "MAYDAY") -> None:
        """Send emergency SOS with GPS coordinates."""
        payload = {
            "msg": distress_message,
            "lat": SAT_GPS_LAT,
            "lon": SAT_GPS_LON,
            "alt": SAT_GPS_ALT,
            "sos": True,
        }
        # SOS goes to front of queue
        frame = build_frame(MSG_SOS, payload)
        self._tx_queue.insert(0, frame)
        logger.critical("SOS QUEUED: %s at (%.4f, %.4f)", distress_message, SAT_GPS_LAT, SAT_GPS_LON)

        log_transmission({
            "type": "SOS",
            "message": distress_message,
            "lat": SAT_GPS_LAT,
            "lon": SAT_GPS_LON,
            "frequency_mhz": self.radio.freq_mhz,
        })

    # -- Loops --

    async def _pass_monitor_loop(self) -> None:
        """Re-compute passes every 2 hours."""
        while self._running:
            self.scheduler.find_next_passes(duration_hours=24)
            nxt = self.scheduler.next_pass()
            if nxt:
                wait = max(0, nxt["start_time"] - time.time())
                logger.info(
                    "Next pass: %s in %.0f min (max elev: %.1f deg)",
                    nxt["satellite"], wait / 60, nxt["max_elevation"],
                )
            await asyncio.sleep(7200)

    async def _tx_queue_processor(self) -> None:
        """Process TX queue during active satellite passes."""
        while self._running:
            active_pass = self.scheduler.is_pass_active()
            if active_pass and self._tx_queue:
                logger.info(
                    "Pass active: %s -- transmitting %d queued frames",
                    active_pass["satellite"], len(self._tx_queue),
                )
                self.radio.wake()

                while self._tx_queue and self.scheduler.is_pass_active():
                    frame = self._tx_queue.pop(0)
                    success = self.radio.transmit(frame)
                    log_transmission({
                        "type": "TX",
                        "satellite": active_pass["satellite"],
                        "bytes": len(frame),
                        "success": success,
                        "frequency_mhz": self.radio.freq_mhz,
                        "snr": self.radio._last_snr,
                    })
                    await asyncio.sleep(2)  # Inter-frame delay

                if not self.scheduler.is_pass_active():
                    logger.info("Pass ended -- returning to sleep")
                    self.radio.sleep()
            else:
                if not active_pass:
                    self.radio.sleep()
                await asyncio.sleep(10)

    async def _rx_monitor_loop(self) -> None:
        """Monitor for downlink packets during passes."""
        while self._running:
            active_pass = self.scheduler.is_pass_active()
            if active_pass:
                self.radio.wake()
                data = self.radio.receive(timeout=3.0)
                if data:
                    parsed = parse_frame(data)
                    if parsed:
                        logger.info(
                            "Downlink received: %s from %s",
                            parsed["type_name"], active_pass["satellite"],
                        )
                        self._rx_buffer.append(parsed)
                        await self._relay_to_mesh(parsed)
                        await self._relay_to_matrix(parsed, active_pass["satellite"])
                    else:
                        logger.debug("Received unrecognized downlink packet (%d bytes)", len(data))
            else:
                await asyncio.sleep(5)

    async def _heartbeat_loop(self) -> None:
        """Send periodic heartbeat frames."""
        while self._running:
            self.queue_message(MSG_HEARTBEAT, {
                "id": SAT_NODE_ID,
                "lat": SAT_GPS_LAT,
                "lon": SAT_GPS_LON,
                "up": int(time.time()),
                "q": len(self._tx_queue),
                "stats": self.radio.stats,
            })
            await asyncio.sleep(3600)  # Heartbeat every hour

    # -- Relay functions --

    async def _relay_to_mesh(self, parsed: dict) -> None:
        """Forward received satellite data to the LoRa mesh network."""
        try:
            relay_payload = json.dumps({
                "src": "SAT",
                "type": parsed["type_name"],
                "data": parsed["payload"],
            }, separators=(",", ":")).encode("utf-8")

            # Attempt to import and use the mesh bridge
            try:
                from scripts.protocols.lora_mesh_bridge import send_to_lora
                send_to_lora(relay_payload[:230])
                logger.info("Relayed satellite data to LoRa mesh (%d bytes)", len(relay_payload[:230]))
            except ImportError:
                logger.debug("LoRa mesh bridge not available for relay")
        except Exception as exc:
            logger.error("Mesh relay failed: %s", exc)

    async def _relay_to_matrix(self, parsed: dict, satellite: str) -> None:
        """Forward received satellite data to Matrix."""
        if not self.matrix_client or not MATRIX_ROOM:
            return
        try:
            msg = (
                f"[SAT RX] {satellite} | {parsed['type_name']}\n"
                f"{json.dumps(parsed['payload'], indent=2)}"
            )
            await self.matrix_client.room_send(
                MATRIX_ROOM,
                "m.room.message",
                {"msgtype": "m.text", "body": msg},
            )
        except Exception as exc:
            logger.error("Matrix relay failed: %s", exc)


# ---------------------------------------------------------------------------
# Fallback chain integration
# ---------------------------------------------------------------------------

async def fallback_transmit(message: str, priority: str = "ALERT") -> bool:
    """Attempt delivery through the full fallback chain:
    Satellite -> JS8Call HF -> LoRa Mesh -> Matrix -> ntfy
    """
    delivered = False

    # 1. Satellite uplink (queue for next pass)
    uplink = SatelliteUplink()
    msg_type = MSG_SOS if priority == "SOS" else MSG_ALERT if priority == "ALERT" else MSG_DATA
    uplink.queue_message(msg_type, {"msg": message, "priority": priority})
    logger.info("Fallback: queued for satellite uplink")

    # 2. JS8Call HF
    try:
        from scripts.protocols.js8call_bridge import fallback_cascade
        result = await fallback_cascade(message, priority)
        if result:
            delivered = True
            logger.info("Fallback: delivered via JS8Call cascade")
    except ImportError:
        logger.debug("Fallback: JS8Call bridge not available")

    # 3. LoRa mesh
    try:
        from scripts.protocols.lora_mesh_bridge import compress_for_lora, send_to_lora
        payload = compress_for_lora("sat-uplink", f"[{priority}] {message}")
        send_to_lora(payload)
        delivered = True
        logger.info("Fallback: delivered via LoRa mesh")
    except ImportError:
        logger.debug("Fallback: LoRa mesh bridge not available")

    # 4. Matrix
    if AsyncClient and MATRIX_PASSWORD:
        try:
            client = AsyncClient(MATRIX_HOMESERVER, MATRIX_USER)
            resp = await client.login(MATRIX_PASSWORD)
            if isinstance(resp, LoginResponse) and MATRIX_ROOM:
                await client.room_send(
                    MATRIX_ROOM,
                    "m.room.message",
                    {"msgtype": "m.text", "body": f"[SAT-{priority}] {message}"},
                )
                delivered = True
                logger.info("Fallback: delivered via Matrix")
            await client.close()
        except Exception as exc:
            logger.warning("Fallback: Matrix failed: %s", exc)

    # 5. ntfy (last resort)
    if aiohttp:
        try:
            async with aiohttp.ClientSession() as session:
                await session.post(
                    f"{NTFY_URL}/{NTFY_TOPIC}",
                    data=f"[{priority}] {message}".encode("utf-8"),
                    headers={
                        "Title": f"Ierahkwa SAT: {priority}",
                        "Priority": "urgent" if priority in ("SOS", "ALERT") else "default",
                        "Tags": "satellite,ierahkwa",
                    },
                    timeout=aiohttp.ClientTimeout(total=10),
                )
                delivered = True
                logger.info("Fallback: delivered via ntfy")
        except Exception as exc:
            logger.warning("Fallback: ntfy failed: %s", exc)

    if not delivered:
        logger.critical("ALL FALLBACK CHANNELS FAILED: %s", message[:80])

    return delivered


# ---------------------------------------------------------------------------
# TinyGS Ground Station Registration
# ---------------------------------------------------------------------------

class TinyGSStation:
    """Register this node as a TinyGS ground station."""

    def __init__(self, station_name: str = SAT_NODE_ID,
                 lat: float = SAT_GPS_LAT, lon: float = SAT_GPS_LON,
                 alt: float = SAT_GPS_ALT):
        self.station_name = station_name
        self.lat = lat
        self.lon = lon
        self.alt = alt

    def registration_payload(self) -> dict:
        """Build the TinyGS-compatible registration payload."""
        return {
            "station": self.station_name,
            "latitude": self.lat,
            "longitude": self.lon,
            "altitude": self.alt,
            "antenna": "LoRa 433MHz Yagi",
            "network": "Ierahkwa Sovereign Network",
            "firmware": "ierahkwa-sat-uplink/1.0",
            "registered_at": datetime.now(timezone.utc).isoformat(),
        }

    async def register(self) -> bool:
        """Attempt to register with TinyGS API."""
        if not aiohttp:
            logger.warning("aiohttp not available -- cannot register with TinyGS")
            return False

        payload = self.registration_payload()
        logger.info("TinyGS registration payload: %s", json.dumps(payload, indent=2))

        # TinyGS registration would normally go through their API
        # For sovereign operation we log locally and relay via Matrix
        log_transmission({
            "type": "TINYGS_REGISTER",
            "payload": payload,
        })

        if AsyncClient and MATRIX_PASSWORD:
            try:
                client = AsyncClient(MATRIX_HOMESERVER, MATRIX_USER)
                resp = await client.login(MATRIX_PASSWORD)
                if isinstance(resp, LoginResponse) and MATRIX_ROOM:
                    await client.room_send(
                        MATRIX_ROOM,
                        "m.room.message",
                        {
                            "msgtype": "m.text",
                            "body": f"[TinyGS] Estacion registrada: {self.station_name} "
                                    f"({self.lat}, {self.lon})",
                        },
                    )
                await client.close()
                return True
            except Exception as exc:
                logger.error("TinyGS Matrix notification failed: %s", exc)

        return False


# ---------------------------------------------------------------------------
# Entry point
# ---------------------------------------------------------------------------

async def main() -> None:
    uplink = SatelliteUplink()
    try:
        await uplink.start()
    except KeyboardInterrupt:
        logger.info("Interrupted -- shutting down")
    finally:
        await uplink.stop()


if __name__ == "__main__":
    asyncio.run(main())
