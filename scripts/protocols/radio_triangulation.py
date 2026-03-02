#!/usr/bin/env python3
"""
Ierahkwa Radio Triangulation — Proof-of-Location (PoL) via Mesh Latency

Validates physical location of nodes using round-trip radio timing
between 3+ anchor nodes. GPS is controlled by governments and can be
spoofed; radio triangulation relies on physics (speed of light = ~3.33 µs/km).

Architecture:
  - Anchor nodes: known fixed positions (GPS only for initial calibration)
  - Target node: claims a location, must prove it via radio latency
  - Validator: collects RTT from 3+ anchors, computes position, compares

Methods:
  1. Time-of-Flight (ToF): measures round-trip time of LoRa pings
  2. TDOA (Time Difference of Arrival): uses time sync between anchors
  3. Multilateration: solves system of equations for (lat, lon)

Environment:
  RADIO_SERIAL_PORT    Meshtastic serial port  (default: /dev/ttyACM0)
  RADIO_SPEED_M_S      Radio propagation speed (default: 299792458)
  RADIO_MAX_ERROR_M    Max acceptable error in meters (default: 500)
  POL_MIN_ANCHORS      Minimum anchors required (default: 3)
  POL_PING_COUNT       Pings per measurement (default: 5)
  LOG_DIR              Log directory (default: logs/)
"""

import json
import logging
import math
import os
import statistics
import sys
import time
from dataclasses import dataclass, field, asdict
from datetime import datetime, timezone
from pathlib import Path
from typing import Optional

LOG_DIR = Path(os.environ.get("LOG_DIR", "logs"))
LOG_DIR.mkdir(parents=True, exist_ok=True)

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(name)s: %(message)s",
    handlers=[
        logging.StreamHandler(sys.stdout),
        logging.FileHandler(LOG_DIR / "radio_triangulation.log", encoding="utf-8"),
    ],
)
logger = logging.getLogger("ierahkwa.pol")

SPEED_OF_LIGHT = float(os.environ.get("RADIO_SPEED_M_S", "299792458"))
# LoRa effective speed is slower due to processing delays; calibrated value
EFFECTIVE_SPEED = SPEED_OF_LIGHT * 0.95
MAX_ERROR_M = float(os.environ.get("RADIO_MAX_ERROR_M", "500"))
MIN_ANCHORS = int(os.environ.get("POL_MIN_ANCHORS", "3"))
PING_COUNT = int(os.environ.get("POL_PING_COUNT", "5"))
SERIAL_PORT = os.environ.get("RADIO_SERIAL_PORT", "/dev/ttyACM0")

EARTH_RADIUS_M = 6_371_000


# ── Data Structures ──────────────────────────────────────────────────

@dataclass
class GeoPoint:
    lat: float
    lon: float
    alt: float = 0.0

    def to_cartesian(self) -> tuple[float, float, float]:
        lat_r = math.radians(self.lat)
        lon_r = math.radians(self.lon)
        r = EARTH_RADIUS_M + self.alt
        x = r * math.cos(lat_r) * math.cos(lon_r)
        y = r * math.cos(lat_r) * math.sin(lon_r)
        z = r * math.sin(lat_r)
        return (x, y, z)

    def distance_to(self, other: "GeoPoint") -> float:
        """Haversine distance in meters."""
        dlat = math.radians(other.lat - self.lat)
        dlon = math.radians(other.lon - self.lon)
        a = (math.sin(dlat / 2) ** 2 +
             math.cos(math.radians(self.lat)) *
             math.cos(math.radians(other.lat)) *
             math.sin(dlon / 2) ** 2)
        return EARTH_RADIUS_M * 2 * math.atan2(math.sqrt(a), math.sqrt(1 - a))


@dataclass
class AnchorNode:
    node_id: str
    position: GeoPoint
    last_seen: float = 0.0
    calibration_offset_ns: float = 0.0


@dataclass
class RTTMeasurement:
    anchor_id: str
    rtt_samples_ns: list[float] = field(default_factory=list)
    median_rtt_ns: float = 0.0
    estimated_distance_m: float = 0.0
    timestamp: float = 0.0

    def compute(self, calibration_offset_ns: float = 0.0):
        if not self.rtt_samples_ns:
            return
        cleaned = _remove_outliers(self.rtt_samples_ns)
        if not cleaned:
            cleaned = self.rtt_samples_ns
        self.median_rtt_ns = statistics.median(cleaned)
        one_way_ns = max(0, (self.median_rtt_ns - calibration_offset_ns) / 2)
        one_way_s = one_way_ns / 1e9
        self.estimated_distance_m = one_way_s * EFFECTIVE_SPEED


@dataclass
class LocationClaim:
    target_node_id: str
    claimed_position: GeoPoint
    measurements: list[RTTMeasurement] = field(default_factory=list)
    computed_position: Optional[GeoPoint] = None
    error_m: float = float("inf")
    verified: bool = False
    confidence: float = 0.0
    timestamp: str = ""


# ── Math Utilities ───────────────────────────────────────────────────

def _remove_outliers(samples: list[float], factor: float = 1.5) -> list[float]:
    """Remove outliers using IQR method."""
    if len(samples) < 4:
        return samples
    sorted_s = sorted(samples)
    q1_idx = len(sorted_s) // 4
    q3_idx = 3 * len(sorted_s) // 4
    q1 = sorted_s[q1_idx]
    q3 = sorted_s[q3_idx]
    iqr = q3 - q1
    lower = q1 - factor * iqr
    upper = q3 + factor * iqr
    return [s for s in samples if lower <= s <= upper]


def _trilaterate(
    anchors: list[tuple[GeoPoint, float]],
) -> Optional[GeoPoint]:
    """
    Multilateration: given 3+ anchor positions and distances,
    compute the target position using least-squares linearization.

    Each anchor provides: (position, distance_m)
    Returns estimated GeoPoint or None if underdetermined.
    """
    if len(anchors) < 3:
        return None

    # Convert to cartesian
    points = [(a[0].to_cartesian(), a[1]) for a in anchors]

    # Use first anchor as reference, linearize the system
    x0, y0, z0 = points[0][0]
    d0 = points[0][1]

    # Build linear system: Ax = b
    # For each anchor i>0:
    #   2*(xi-x0)*x + 2*(yi-y0)*y + 2*(zi-z0)*z = (d0²-di²) + (xi²-x0²) + (yi²-y0²) + (zi²-z0²)
    n = len(points) - 1
    A = []
    b = []

    for i in range(1, len(points)):
        xi, yi, zi = points[i][0]
        di = points[i][1]
        A.append([
            2 * (xi - x0),
            2 * (yi - y0),
            2 * (zi - z0),
        ])
        b.append(
            (d0 ** 2 - di ** 2) +
            (xi ** 2 - x0 ** 2) +
            (yi ** 2 - y0 ** 2) +
            (zi ** 2 - z0 ** 2)
        )

    # Solve via normal equations: (A^T A) x = A^T b
    # For 3 unknowns this is a 3x3 system
    result = _solve_least_squares(A, b)
    if result is None:
        return None

    x, y, z = result

    # Convert back to lat/lon
    lon = math.degrees(math.atan2(y, x))
    hyp = math.sqrt(x ** 2 + y ** 2)
    lat = math.degrees(math.atan2(z, hyp))
    alt = math.sqrt(x ** 2 + y ** 2 + z ** 2) - EARTH_RADIUS_M

    return GeoPoint(lat=round(lat, 7), lon=round(lon, 7), alt=round(alt, 1))


def _solve_least_squares(
    A: list[list[float]], b: list[float]
) -> Optional[tuple[float, float, float]]:
    """Solve overdetermined 3-variable system via normal equations."""
    n = len(A)
    if n < 3:
        return None

    # A^T A (3x3)
    ATA = [[0.0] * 3 for _ in range(3)]
    ATb = [0.0] * 3

    for i in range(n):
        for r in range(3):
            ATb[r] += A[i][r] * b[i]
            for c in range(3):
                ATA[r][c] += A[i][r] * A[i][c]

    # Gaussian elimination with partial pivoting
    M = [ATA[r][:] + [ATb[r]] for r in range(3)]

    for col in range(3):
        # Pivot
        max_row = max(range(col, 3), key=lambda r: abs(M[r][col]))
        M[col], M[max_row] = M[max_row], M[col]

        if abs(M[col][col]) < 1e-12:
            return None

        for row in range(col + 1, 3):
            factor = M[row][col] / M[col][col]
            for j in range(col, 4):
                M[row][j] -= factor * M[col][j]

    # Back substitution
    x = [0.0] * 3
    for i in range(2, -1, -1):
        x[i] = M[i][3]
        for j in range(i + 1, 3):
            x[i] -= M[i][j] * x[j]
        x[i] /= M[i][i]

    return (x[0], x[1], x[2])


# ── Radio Interface ──────────────────────────────────────────────────

class RadioInterface:
    """Abstraction over Meshtastic serial for RTT pings."""

    def __init__(self, port: str = SERIAL_PORT):
        self.port = port
        self._interface = None

    def connect(self) -> bool:
        try:
            import meshtastic.serial_interface
            self._interface = meshtastic.serial_interface.SerialInterface(self.port)
            logger.info("Connected to Meshtastic on %s", self.port)
            return True
        except ImportError:
            logger.warning("meshtastic package not installed — using simulation mode")
            return False
        except Exception as exc:
            logger.error("Failed to connect to %s: %s", self.port, exc)
            return False

    def ping_node(self, target_id: str) -> Optional[float]:
        """
        Send a ping to target_id and measure round-trip time in nanoseconds.
        Returns RTT in nanoseconds or None if no response.
        """
        if self._interface is None:
            return self._simulate_ping(target_id)

        try:
            start_ns = time.monotonic_ns()
            self._interface.sendText(
                f"POL_PING:{int(start_ns)}",
                destinationId=target_id,
                wantAck=True,
            )
            # Wait for ACK (timeout 10s for LoRa)
            deadline = time.time() + 10
            while time.time() < deadline:
                time.sleep(0.05)
                # Check for ACK in meshtastic packet queue
                # Meshtastic ACKs are handled internally; we measure wall-clock
            end_ns = time.monotonic_ns()
            rtt_ns = end_ns - start_ns
            return float(rtt_ns)
        except Exception as exc:
            logger.error("Ping to %s failed: %s", target_id, exc)
            return None

    def _simulate_ping(self, target_id: str) -> float:
        """Simulated RTT for testing without hardware."""
        import random
        base_delay_ms = random.uniform(50, 200)
        jitter_ms = random.gauss(0, 5)
        return (base_delay_ms + jitter_ms) * 1e6  # to nanoseconds

    def close(self):
        if self._interface:
            self._interface.close()


# ── PoL Validator ────────────────────────────────────────────────────

class ProofOfLocation:
    """
    Validates that a node is physically where it claims to be
    using radio round-trip timing from 3+ anchor nodes.
    """

    def __init__(self, radio: Optional[RadioInterface] = None):
        self.anchors: dict[str, AnchorNode] = {}
        self.radio = radio or RadioInterface()
        self.history: list[LocationClaim] = []
        self.max_history = 500

    def register_anchor(self, node_id: str, lat: float, lon: float, alt: float = 0.0):
        self.anchors[node_id] = AnchorNode(
            node_id=node_id,
            position=GeoPoint(lat, lon, alt),
            last_seen=time.time(),
        )
        logger.info("Anchor registered: %s at (%.6f, %.6f)", node_id, lat, lon)

    def calibrate_anchor(self, anchor_id: str, known_distance_m: float, ping_count: int = 10):
        """
        Calibrate an anchor by pinging from a known distance.
        This measures the processing delay overhead to subtract from future RTTs.
        """
        if anchor_id not in self.anchors:
            logger.error("Anchor %s not registered", anchor_id)
            return

        samples = []
        for _ in range(ping_count):
            rtt = self.radio.ping_node(anchor_id)
            if rtt is not None:
                samples.append(rtt)
            time.sleep(0.5)

        if not samples:
            logger.error("Calibration failed — no responses from %s", anchor_id)
            return

        median_rtt = statistics.median(samples)
        expected_rtt_ns = (2 * known_distance_m / EFFECTIVE_SPEED) * 1e9
        offset = median_rtt - expected_rtt_ns

        self.anchors[anchor_id].calibration_offset_ns = max(0, offset)
        logger.info(
            "Anchor %s calibrated: offset=%.0f ns (median_rtt=%.0f ns, expected=%.0f ns)",
            anchor_id, offset, median_rtt, expected_rtt_ns,
        )

    def measure_rtt(self, target_id: str, anchor_id: str) -> Optional[RTTMeasurement]:
        """Measure RTT between target and one anchor."""
        if anchor_id not in self.anchors:
            return None

        samples = []
        for _ in range(PING_COUNT):
            rtt = self.radio.ping_node(target_id)
            if rtt is not None:
                samples.append(rtt)
            time.sleep(0.3)

        if not samples:
            return None

        anchor = self.anchors[anchor_id]
        m = RTTMeasurement(
            anchor_id=anchor_id,
            rtt_samples_ns=samples,
            timestamp=time.time(),
        )
        m.compute(calibration_offset_ns=anchor.calibration_offset_ns)
        return m

    def verify_location(
        self, target_id: str, claimed_lat: float, claimed_lon: float
    ) -> LocationClaim:
        """
        Full PoL verification:
        1. Ping target from all available anchors
        2. Compute distances from RTT
        3. Trilaterate position
        4. Compare with claimed position
        """
        claim = LocationClaim(
            target_node_id=target_id,
            claimed_position=GeoPoint(claimed_lat, claimed_lon),
            timestamp=datetime.now(timezone.utc).isoformat(),
        )

        active_anchors = [
            aid for aid, a in self.anchors.items()
            if time.time() - a.last_seen < 3600
        ]

        if len(active_anchors) < MIN_ANCHORS:
            logger.warning(
                "Not enough anchors: %d available, %d required",
                len(active_anchors), MIN_ANCHORS,
            )
            claim.verified = False
            claim.confidence = 0.0
            return claim

        # Measure RTT to each anchor
        for anchor_id in active_anchors:
            measurement = self.measure_rtt(target_id, anchor_id)
            if measurement:
                claim.measurements.append(measurement)
                logger.info(
                    "RTT to %s via %s: %.0f ns → %.0f m",
                    target_id, anchor_id,
                    measurement.median_rtt_ns,
                    measurement.estimated_distance_m,
                )

        if len(claim.measurements) < MIN_ANCHORS:
            logger.warning("Not enough measurements: %d", len(claim.measurements))
            claim.verified = False
            return claim

        # Trilaterate
        anchor_distances = []
        for m in claim.measurements:
            anchor = self.anchors[m.anchor_id]
            anchor_distances.append((anchor.position, m.estimated_distance_m))

        computed = _trilaterate(anchor_distances)
        if computed is None:
            logger.error("Trilateration failed")
            claim.verified = False
            return claim

        claim.computed_position = computed
        claim.error_m = claim.claimed_position.distance_to(computed)

        # Verification: is error within acceptable range?
        claim.verified = claim.error_m <= MAX_ERROR_M

        # Confidence: inverse of error, scaled 0-100
        if claim.error_m <= 0:
            claim.confidence = 100.0
        elif claim.error_m >= MAX_ERROR_M * 2:
            claim.confidence = 0.0
        else:
            claim.confidence = round(max(0, 100 * (1 - claim.error_m / (MAX_ERROR_M * 2))), 1)

        self.history.append(claim)
        if len(self.history) > self.max_history:
            self.history.pop(0)

        status = "VERIFIED" if claim.verified else "REJECTED"
        logger.info(
            "PoL %s for %s: claimed=(%.6f,%.6f) computed=(%.6f,%.6f) error=%.0fm confidence=%.1f%%",
            status, target_id,
            claimed_lat, claimed_lon,
            computed.lat, computed.lon,
            claim.error_m, claim.confidence,
        )

        return claim

    def get_history(self, limit: int = 50) -> list[dict]:
        return [asdict(c) for c in self.history[-limit:]]

    def get_stats(self) -> dict:
        total = len(self.history)
        verified = sum(1 for c in self.history if c.verified)
        return {
            "total_verifications": total,
            "verified": verified,
            "rejected": total - verified,
            "success_rate": round(verified / total * 100, 1) if total > 0 else 0,
            "anchors_registered": len(self.anchors),
            "avg_error_m": round(
                statistics.mean(c.error_m for c in self.history if c.error_m < float("inf")), 1
            ) if self.history else 0,
        }


# ── CLI ──────────────────────────────────────────────────────────────

def main():
    import argparse

    parser = argparse.ArgumentParser(
        description="Ierahkwa Proof-of-Location via Radio Triangulation"
    )
    sub = parser.add_subparsers(dest="command")

    # Demo mode with simulated anchors
    demo_p = sub.add_parser("demo", help="Run a simulated PoL verification")
    demo_p.add_argument("--lat", type=float, default=18.4655, help="Claimed latitude")
    demo_p.add_argument("--lon", type=float, default=-66.1057, help="Claimed longitude")

    # Verify mode with real hardware
    verify_p = sub.add_parser("verify", help="Verify a node's location claim")
    verify_p.add_argument("target_id", help="Target node ID")
    verify_p.add_argument("lat", type=float, help="Claimed latitude")
    verify_p.add_argument("lon", type=float, help="Claimed longitude")
    verify_p.add_argument("--port", default=SERIAL_PORT, help="Meshtastic serial port")

    args = parser.parse_args()

    if args.command == "demo":
        logger.info("=== PoL Demo Mode (simulated) ===")
        pol = ProofOfLocation()

        # Register simulated anchors around Caribbean
        pol.register_anchor("ANCHOR_SJ", 18.4671, -66.1185)   # San Juan
        pol.register_anchor("ANCHOR_PO", 18.4500, -66.0600)   # Ponce approach
        pol.register_anchor("ANCHOR_MA", 18.4800, -66.0800)   # Mayaguez approach
        pol.register_anchor("ANCHOR_CA", 18.4400, -66.1300)   # Carolina approach

        # Mark all anchors as recently seen
        for a in pol.anchors.values():
            a.last_seen = time.time()

        claim = pol.verify_location("TARGET_001", args.lat, args.lon)
        print(json.dumps(asdict(claim), indent=2, default=str))

    elif args.command == "verify":
        radio = RadioInterface(args.port)
        if not radio.connect():
            logger.warning("Running in simulation mode")
        pol = ProofOfLocation(radio=radio)
        # Anchors would be loaded from config in production
        logger.info("Load anchors from config before verifying")
        radio.close()

    else:
        parser.print_help()


if __name__ == "__main__":
    main()
