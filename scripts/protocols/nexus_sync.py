#!/usr/bin/env python3
"""
NEXUS Sovereign Wealth Fund Synchronizer

Keeps the Sovereign Wealth Fund state consistent across all 19 NEXUS nodes.
Handles multi-layer sync: blockchain state, treasury balance, cross-node
replication, and Mesh/Satellite fallback when internet drops.

Architecture:
  ┌─────────────┐  gRPC/HTTP   ┌──────────────┐
  │  NEXUS-NA-01│◄────────────►│  NEXUS-CB-07 │
  │  (Primary)  │              │  (Caribbean)  │
  └──────┬──────┘              └───────┬───────┘
         │  LoRa/HF                    │
         ▼                             ▼
  ┌──────────────┐             ┌──────────────┐
  │  NEXUS-GS-19 │ ─satellite─►│  NEXUS-SA-12 │
  │  (Sat Uplink)│             │  (Colombia)  │
  └──────────────┘             └──────────────┘

Transport Layers:
  1. gRPC (primary, low latency)
  2. HTTP/REST (fallback, broader compatibility)
  3. LoRa Mesh (offline, ~500bps, critical data only)
  4. Satellite/TinyGS (emergency, ~1200bps)

Environment:
  NEXUS_CONFIG        Path to NEXUS nodes config JSON
  SYNC_INTERVAL       Seconds between sync cycles (default: 60)
  LORA_DEVICE         Serial port for LoRa modem (default: /dev/ttyUSB0)
  SATELLITE_ENABLED   Enable satellite uplink (default: false)
"""

import hashlib
import json
import logging
import os
import struct
import sys
import time
import threading
from dataclasses import dataclass, field, asdict
from datetime import datetime, timezone
from pathlib import Path
from typing import Optional
from enum import IntEnum

LOG_DIR = Path(os.environ.get("LOG_DIR", "logs"))
LOG_DIR.mkdir(parents=True, exist_ok=True)

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(name)s: %(message)s",
    handlers=[
        logging.StreamHandler(sys.stdout),
        logging.FileHandler(LOG_DIR / "nexus_sync.log", encoding="utf-8"),
    ],
)
logger = logging.getLogger("ierahkwa.sync")

SYNC_INTERVAL = int(os.environ.get("SYNC_INTERVAL", "60"))
LORA_DEVICE = os.environ.get("LORA_DEVICE", "/dev/ttyUSB0")
SATELLITE_ENABLED = os.environ.get("SATELLITE_ENABLED", "false").lower() == "true"
TOTAL_SUPPLY = 10_000_000_000_000  # 10T WMP


class SyncMsgType(IntEnum):
    STATE_REQUEST = 0x01
    STATE_RESPONSE = 0x02
    TREASURY_UPDATE = 0x03
    TREASURY_ACK = 0x04
    BLOCK_ANNOUNCE = 0x05
    BLOCK_ACK = 0x06
    HEARTBEAT = 0x07
    ALERT = 0x0F


# ── Compact Binary Protocol (for LoRa/Satellite) ────────────────────

@dataclass
class SyncMessage:
    """Wire format: [1B type][4B sender_id][8B timestamp][8B supply][32B hash][nB payload]"""
    msg_type: SyncMsgType
    sender_id: int
    timestamp: int
    supply: int
    state_hash: bytes
    payload: bytes = b""

    def serialize(self) -> bytes:
        header = struct.pack(
            "!BIqq",
            self.msg_type,
            self.sender_id,
            self.timestamp,
            self.supply,
        )
        return header + self.state_hash[:32].ljust(32, b"\x00") + self.payload

    @classmethod
    def deserialize(cls, data: bytes) -> "SyncMessage":
        if len(data) < 53:
            raise ValueError(f"Message too short: {len(data)} bytes (need >=53)")
        msg_type, sender_id, ts, supply = struct.unpack("!BIqq", data[:21])
        state_hash = data[21:53]
        payload = data[53:]
        return cls(
            msg_type=SyncMsgType(msg_type),
            sender_id=sender_id,
            timestamp=ts,
            supply=supply,
            state_hash=state_hash,
            payload=payload,
        )


# ── Node State ───────────────────────────────────────────────────────

@dataclass
class WealthFundState:
    total_supply: int = TOTAL_SUPPLY
    treasury_balance: int = 0
    circulating: int = 0
    staked: int = 0
    locked_inheritance: int = 0
    education_fund: int = 0
    healthcare_fund: int = 0
    infrastructure_fund: int = 0
    last_block_height: int = 0
    last_block_hash: str = ""
    last_updated: str = ""

    def compute_hash(self) -> str:
        data = json.dumps(asdict(self), sort_keys=True).encode()
        return hashlib.sha256(data).hexdigest()


@dataclass
class NexusPeer:
    node_id: str
    numeric_id: int
    endpoint: str
    port: int = 50050
    region: str = ""
    transport: str = "grpc"     # grpc | http | lora | satellite
    last_seen: float = 0.0
    state_hash: str = ""
    block_height: int = 0
    latency_ms: float = -1
    synced: bool = False


# ── Transport Adapters ───────────────────────────────────────────────

class GrpcTransport:
    def send_state(self, peer: NexusPeer, msg: SyncMessage) -> Optional[SyncMessage]:
        try:
            import grpc
            channel = grpc.insecure_channel(
                f"{peer.endpoint}:{peer.port}",
                options=[("grpc.connect_timeout_ms", 10000)],
            )
            # When proto stubs available:
            # stub = nexus_pb2_grpc.NexusSyncStub(channel)
            # resp = stub.SyncState(nexus_pb2.SyncRequest(...))
            logger.debug("gRPC send to %s — falling back to HTTP", peer.node_id)
            return None
        except Exception:
            return None

    def request_state(self, peer: NexusPeer) -> Optional[WealthFundState]:
        try:
            import urllib.request
            url = f"http://{peer.endpoint}:{peer.port}/api/v1/state"
            start = time.monotonic()
            with urllib.request.urlopen(url, timeout=15) as resp:
                data = json.loads(resp.read().decode())
            peer.latency_ms = round((time.monotonic() - start) * 1000, 1)
            return WealthFundState(**data)
        except Exception as exc:
            logger.debug("HTTP state request to %s failed: %s", peer.node_id, exc)
            return None


class LoRaTransport:
    def __init__(self, device: str = LORA_DEVICE, baud: int = 115200):
        self.device = device
        self.baud = baud
        self._serial = None

    def _connect(self):
        if self._serial is not None:
            return
        try:
            import serial
            self._serial = serial.Serial(self.device, self.baud, timeout=5)
            logger.info("LoRa connected on %s", self.device)
        except ImportError:
            logger.warning("pyserial not installed — LoRa transport disabled")
        except Exception as exc:
            logger.warning("LoRa connect failed: %s", exc)

    def send(self, msg: SyncMessage) -> bool:
        self._connect()
        if self._serial is None:
            return False
        try:
            frame = msg.serialize()
            # Meshtastic text channel: base64 the binary
            import base64
            encoded = base64.b64encode(frame)
            self._serial.write(b"!hex " + encoded + b"\n")
            self._serial.flush()
            logger.info("LoRa sent %d bytes (type=%s)", len(frame), msg.msg_type.name)
            return True
        except Exception as exc:
            logger.error("LoRa send failed: %s", exc)
            return False

    def receive(self, timeout: float = 5.0) -> Optional[SyncMessage]:
        self._connect()
        if self._serial is None:
            return None
        try:
            self._serial.timeout = timeout
            line = self._serial.readline()
            if not line:
                return None
            import base64
            if line.startswith(b"!hex "):
                raw = base64.b64decode(line[5:].strip())
                return SyncMessage.deserialize(raw)
            return None
        except Exception as exc:
            logger.debug("LoRa receive error: %s", exc)
            return None


class SatelliteTransport:
    """TinyGS / Iridium SBD transport for extreme fallback."""

    def send(self, msg: SyncMessage) -> bool:
        frame = msg.serialize()
        if len(frame) > 340:
            logger.warning("Satellite payload too large (%d bytes), truncating", len(frame))
            frame = frame[:340]

        try:
            import serial
            sat = serial.Serial("/dev/ttyACM0", 19200, timeout=10)
            # Iridium SBD: AT+SBDWB=<length>
            sat.write(f"AT+SBDWB={len(frame)}\r".encode())
            time.sleep(0.5)
            checksum = sum(frame) & 0xFFFF
            sat.write(frame + struct.pack(">H", checksum))
            time.sleep(1)
            # Initiate transmission
            sat.write(b"AT+SBDIX\r")
            resp = sat.read(200).decode(errors="ignore")
            if "+SBDIX: 0" in resp or "+SBDIX: 1" in resp or "+SBDIX: 2" in resp:
                logger.info("Satellite uplink successful (%d bytes)", len(frame))
                return True
            else:
                logger.warning("Satellite response: %s", resp.strip())
                return False
        except ImportError:
            logger.warning("pyserial not installed — satellite disabled")
            return False
        except Exception as exc:
            logger.error("Satellite send failed: %s", exc)
            return False


# ── Synchronizer Engine ──────────────────────────────────────────────

class NexusSynchronizer:
    def __init__(self, local_id: str = "NX-NA-01", numeric_id: int = 1):
        self.local_id = local_id
        self.numeric_id = numeric_id
        self.state = WealthFundState(
            total_supply=TOTAL_SUPPLY,
            treasury_balance=TOTAL_SUPPLY,
            last_updated=datetime.now(timezone.utc).isoformat(),
        )
        self.peers: dict[str, NexusPeer] = {}
        self.grpc = GrpcTransport()
        self.lora = LoRaTransport()
        self.satellite = SatelliteTransport() if SATELLITE_ENABLED else None
        self._running = False
        self._lock = threading.Lock()
        self._divergences: list[dict] = []

    def add_peer(self, peer: NexusPeer):
        self.peers[peer.node_id] = peer

    def load_peers_from_registry(self):
        from scripts.protocols.genesis_validator import DEFAULT_NEXUS_NODES
        for n in DEFAULT_NEXUS_NODES:
            if n.node_id == self.local_id:
                continue
            self.add_peer(NexusPeer(
                node_id=n.node_id,
                numeric_id=int(n.node_id.split("-")[-1]),
                endpoint=n.endpoint,
                port=n.port,
                region=n.region,
            ))

    def _build_heartbeat(self) -> SyncMessage:
        return SyncMessage(
            msg_type=SyncMsgType.HEARTBEAT,
            sender_id=self.numeric_id,
            timestamp=int(time.time()),
            supply=self.state.total_supply,
            state_hash=bytes.fromhex(self.state.compute_hash()[:64]),
        )

    def sync_peer(self, peer: NexusPeer) -> bool:
        """Attempt to sync with a single peer using cascading transports."""
        # Try gRPC / HTTP first
        remote_state = self.grpc.request_state(peer)
        if remote_state:
            return self._reconcile(peer, remote_state)

        # Fallback: LoRa heartbeat
        hb = self._build_heartbeat()
        if self.lora.send(hb):
            peer.transport = "lora"
            peer.last_seen = time.time()
            logger.info("  [%s] LoRa heartbeat sent", peer.node_id)
            return True

        # Emergency: Satellite
        if self.satellite:
            if self.satellite.send(hb):
                peer.transport = "satellite"
                peer.last_seen = time.time()
                logger.info("  [%s] Satellite uplink sent", peer.node_id)
                return True

        peer.synced = False
        return False

    def _reconcile(self, peer: NexusPeer, remote: WealthFundState) -> bool:
        with self._lock:
            local_hash = self.state.compute_hash()
            remote_hash = remote.compute_hash()

            peer.state_hash = remote_hash
            peer.block_height = remote.last_block_height
            peer.last_seen = time.time()

            if local_hash == remote_hash:
                peer.synced = True
                return True

            # Supply mismatch — critical alert
            if remote.total_supply != self.state.total_supply:
                self._divergences.append({
                    "peer": peer.node_id,
                    "type": "supply_mismatch",
                    "local": self.state.total_supply,
                    "remote": remote.total_supply,
                    "timestamp": datetime.now(timezone.utc).isoformat(),
                })
                logger.error("SUPPLY MISMATCH with %s: local=%d remote=%d",
                             peer.node_id, self.state.total_supply, remote.total_supply)
                peer.synced = False
                return False

            # Block height: accept higher chain
            if remote.last_block_height > self.state.last_block_height:
                logger.info("Accepting higher chain from %s (height %d > %d)",
                            peer.node_id, remote.last_block_height, self.state.last_block_height)
                self.state.last_block_height = remote.last_block_height
                self.state.last_block_hash = remote.last_block_hash
                self.state.last_updated = datetime.now(timezone.utc).isoformat()

            peer.synced = True
            return True

    def run_sync_cycle(self):
        logger.info("=== SYNC CYCLE %s ===", datetime.now(timezone.utc).strftime("%H:%M:%S"))
        logger.info("Local state hash: %s", self.state.compute_hash()[:16])

        synced_count = 0
        for peer in self.peers.values():
            ok = self.sync_peer(peer)
            if ok:
                synced_count += 1
            logger.info("  [%s] %s via %s (latency=%.0fms)",
                        peer.node_id,
                        "SYNCED" if peer.synced else "FAILED",
                        peer.transport,
                        peer.latency_ms)

        logger.info("Sync: %d/%d peers | Divergences: %d",
                     synced_count, len(self.peers), len(self._divergences))

        return synced_count

    def start(self, interval: int = SYNC_INTERVAL):
        self._running = True
        logger.info("Synchronizer starting (node=%s, interval=%ds, peers=%d)",
                     self.local_id, interval, len(self.peers))

        while self._running:
            try:
                self.run_sync_cycle()
            except Exception as exc:
                logger.error("Sync cycle error: %s", exc)
            time.sleep(interval)

    def stop(self):
        self._running = False

    def get_status(self) -> dict:
        return {
            "node_id": self.local_id,
            "state_hash": self.state.compute_hash()[:16],
            "block_height": self.state.last_block_height,
            "total_supply": self.state.total_supply,
            "peers_total": len(self.peers),
            "peers_synced": sum(1 for p in self.peers.values() if p.synced),
            "divergences": len(self._divergences),
            "satellite_enabled": self.satellite is not None,
            "timestamp": datetime.now(timezone.utc).isoformat(),
        }


# ── CLI ──────────────────────────────────────────────────────────────

def main():
    import argparse
    parser = argparse.ArgumentParser(description="NEXUS Wealth Fund Synchronizer")
    parser.add_argument("--node-id", default="NX-NA-01", help="Local node ID")
    parser.add_argument("--interval", type=int, default=SYNC_INTERVAL, help="Sync interval (seconds)")
    parser.add_argument("--once", action="store_true", help="Run one cycle and exit")
    parser.add_argument("--status", action="store_true", help="Print status JSON and exit")
    args = parser.parse_args()

    numeric = int(args.node_id.split("-")[-1]) if "-" in args.node_id else 1
    sync = NexusSynchronizer(local_id=args.node_id, numeric_id=numeric)

    # Load peers
    try:
        sync.load_peers_from_registry()
    except Exception:
        logger.info("Loading default peer list")
        for i, region in enumerate(["NA", "CA", "CB", "SA"], start=2):
            sync.add_peer(NexusPeer(
                node_id=f"NX-{region}-{i:02d}",
                numeric_id=i,
                endpoint=f"nexus-{region.lower()}-{i:02d}.ierahkwa.org",
                region=region,
            ))

    if args.status:
        print(json.dumps(sync.get_status(), indent=2))
        return

    if args.once:
        sync.run_sync_cycle()
        print(json.dumps(sync.get_status(), indent=2))
        return

    try:
        sync.start(interval=args.interval)
    except KeyboardInterrupt:
        sync.stop()
        logger.info("Synchronizer stopped")


if __name__ == "__main__":
    main()
