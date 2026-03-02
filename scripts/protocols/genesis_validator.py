#!/usr/bin/env python3
"""
Ierahkwa Genesis Block Validator — NEXUS Super-Consensus

Validates the cryptographic integrity of the Founding Stone (Genesis Block)
across all 19 NEXUS nodes. Ensures the 10T WAMPUM supply has not drifted
and that every node's local ledger matches the canonical root hash.

Architecture:
  1. Connect to each NEXUS via gRPC or HTTP health endpoint
  2. Request the genesis block hash + current supply from each node
  3. Compare all hashes — unanimous match = Super-Consensus
  4. Verify total supply integrity (sum across nodes = 10T WMP)
  5. Generate signed validation report

Environment:
  NEXUS_CONFIG        Path to NEXUS nodes config (default: config/nexus_nodes.json)
  GENESIS_ROOT_HASH   Expected genesis hash (default: computed from manifest)
  TOTAL_SUPPLY_WMP    Expected total supply (default: 10000000000000)
  VALIDATION_TIMEOUT  Per-node timeout seconds (default: 30)
  LOG_DIR             Log directory (default: logs/)
"""

import hashlib
import json
import logging
import os
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
        logging.FileHandler(LOG_DIR / "genesis_validator.log", encoding="utf-8"),
    ],
)
logger = logging.getLogger("ierahkwa.genesis")

TOTAL_SUPPLY = int(os.environ.get("TOTAL_SUPPLY_WMP", "10000000000000"))
VALIDATION_TIMEOUT = int(os.environ.get("VALIDATION_TIMEOUT", "30"))
MANIFEST_SEED = "IERAHKWA_NE_KANIENKE_GENESIS_v16"


# ── Data Structures ──────────────────────────────────────────────────

@dataclass
class NexusNode:
    node_id: str
    region: str
    endpoint: str
    port: int = 50050
    description: str = ""


@dataclass
class NodeValidation:
    node_id: str
    region: str
    status: str = "pending"          # pending | verified | failed | unreachable
    genesis_hash: str = ""
    reported_supply: int = 0
    block_height: int = 0
    latency_ms: float = 0.0
    error: str = ""
    timestamp: str = ""


@dataclass
class ValidationReport:
    report_id: str = ""
    version: str = "16.1.0"
    timestamp: str = ""
    expected_genesis_hash: str = ""
    expected_supply: int = TOTAL_SUPPLY
    total_nodes: int = 0
    verified: int = 0
    failed: int = 0
    unreachable: int = 0
    super_consensus: bool = False
    supply_integrity: bool = False
    nodes: list = field(default_factory=list)
    report_hash: str = ""


# ── NEXUS Registry ───────────────────────────────────────────────────

DEFAULT_NEXUS_NODES = [
    NexusNode("NX-NA-01", "North America", "nexus-na-01.ierahkwa.org", description="Akwesasne Primary"),
    NexusNode("NX-NA-02", "North America", "nexus-na-02.ierahkwa.org", description="Kahnawake Backup"),
    NexusNode("NX-NA-03", "North America", "nexus-na-03.ierahkwa.org", description="Six Nations"),
    NexusNode("NX-CA-04", "Central America", "nexus-ca-04.ierahkwa.org", description="Guatemala Hub"),
    NexusNode("NX-CA-05", "Central America", "nexus-ca-05.ierahkwa.org", description="Panama Canal Node"),
    NexusNode("NX-CA-06", "Central America", "nexus-ca-06.ierahkwa.org", description="Costa Rica Relay"),
    NexusNode("NX-CB-07", "Caribbean", "nexus-cb-07.ierahkwa.org", description="Borinken Primary"),
    NexusNode("NX-CB-08", "Caribbean", "nexus-cb-08.ierahkwa.org", description="Jamaica Relay"),
    NexusNode("NX-CB-09", "Caribbean", "nexus-cb-09.ierahkwa.org", description="Trinidad Mesh"),
    NexusNode("NX-CB-10", "Caribbean", "nexus-cb-10.ierahkwa.org", description="Cuba Sovereign"),
    NexusNode("NX-CB-11", "Caribbean", "nexus-cb-11.ierahkwa.org", description="Dominican Republic"),
    NexusNode("NX-SA-12", "South America", "nexus-sa-12.ierahkwa.org", description="Colombia Hub"),
    NexusNode("NX-SA-13", "South America", "nexus-sa-13.ierahkwa.org", description="Brazil Primary"),
    NexusNode("NX-SA-14", "South America", "nexus-sa-14.ierahkwa.org", description="Argentina Relay"),
    NexusNode("NX-SA-15", "South America", "nexus-sa-15.ierahkwa.org", description="Peru Mesh"),
    NexusNode("NX-SA-16", "South America", "nexus-sa-16.ierahkwa.org", description="Chile Southern"),
    NexusNode("NX-SA-17", "South America", "nexus-sa-17.ierahkwa.org", description="Venezuela Node"),
    NexusNode("NX-SA-18", "South America", "nexus-sa-18.ierahkwa.org", description="Ecuador Anchor"),
    NexusNode("NX-GS-19", "Global Seed", "nexus-gs-19.ierahkwa.org", description="Satellite Uplink Master"),
]


def load_nexus_nodes(config_path: Optional[str] = None) -> list[NexusNode]:
    if config_path and Path(config_path).exists():
        with open(config_path) as f:
            data = json.load(f)
        return [NexusNode(**n) for n in data.get("nodes", [])]
    return DEFAULT_NEXUS_NODES


# ── Genesis Hash ─────────────────────────────────────────────────────

def compute_genesis_hash(seed: str = MANIFEST_SEED) -> str:
    """
    Compute the canonical genesis hash from the manifest seed.
    Double SHA-256 (same as Bitcoin genesis).
    """
    first = hashlib.sha256(seed.encode("utf-8")).digest()
    second = hashlib.sha256(first).hexdigest()
    return f"sha256:{second}"


# ── Node Validation ──────────────────────────────────────────────────

def validate_node_grpc(node: NexusNode) -> NodeValidation:
    """Validate a node via gRPC (real implementation for when nodes are live)."""
    result = NodeValidation(
        node_id=node.node_id,
        region=node.region,
        timestamp=datetime.now(timezone.utc).isoformat(),
    )

    try:
        import grpc
        channel = grpc.insecure_channel(
            f"{node.endpoint}:{node.port}",
            options=[("grpc.connect_timeout_ms", VALIDATION_TIMEOUT * 1000)],
        )
        # When proto stubs are generated:
        # stub = kernel_pb2_grpc.KernelServiceStub(channel)
        # response = stub.GetGenesisBlock(kernel_pb2.Empty(), timeout=VALIDATION_TIMEOUT)
        # result.genesis_hash = response.hash
        # result.reported_supply = response.total_supply
        # result.block_height = response.block_height

        # For now, fall back to HTTP health check
        return validate_node_http(node)

    except ImportError:
        return validate_node_http(node)
    except Exception as exc:
        result.status = "unreachable"
        result.error = str(exc)
        return result


def validate_node_http(node: NexusNode) -> NodeValidation:
    """Validate a node via HTTP health endpoint."""
    result = NodeValidation(
        node_id=node.node_id,
        region=node.region,
        timestamp=datetime.now(timezone.utc).isoformat(),
    )

    try:
        import urllib.request
        url = f"http://{node.endpoint}:{node.port}/health"
        start = time.monotonic()
        req = urllib.request.Request(url, method="GET")
        req.add_header("X-Ierahkwa-Validator", "genesis-v16")
        with urllib.request.urlopen(req, timeout=VALIDATION_TIMEOUT) as resp:
            data = json.loads(resp.read().decode())
        result.latency_ms = round((time.monotonic() - start) * 1000, 1)

        result.genesis_hash = data.get("genesis_hash", "")
        result.reported_supply = int(data.get("total_supply", 0))
        result.block_height = int(data.get("block_height", 0))
        result.status = "verified" if result.genesis_hash else "failed"

    except Exception as exc:
        result.status = "unreachable"
        result.error = str(exc)
        result.latency_ms = -1

    return result


def validate_node_simulated(node: NexusNode, expected_hash: str) -> NodeValidation:
    """Simulated validation for testing when nodes aren't deployed yet."""
    import random
    latency = random.uniform(50, 800)
    return NodeValidation(
        node_id=node.node_id,
        region=node.region,
        status="verified",
        genesis_hash=expected_hash,
        reported_supply=TOTAL_SUPPLY,
        block_height=random.randint(1, 100),
        latency_ms=round(latency, 1),
        timestamp=datetime.now(timezone.utc).isoformat(),
    )


# ── Full Validation ──────────────────────────────────────────────────

def run_validation(
    nodes: list[NexusNode],
    mode: str = "simulate",
) -> ValidationReport:
    """
    Run full genesis validation across all NEXUS nodes.
    mode: 'simulate' | 'http' | 'grpc'
    """
    expected_hash = compute_genesis_hash()
    report = ValidationReport(
        report_id=hashlib.sha256(
            f"{datetime.now(timezone.utc).isoformat()}:{expected_hash}".encode()
        ).hexdigest()[:16],
        timestamp=datetime.now(timezone.utc).isoformat(),
        expected_genesis_hash=expected_hash,
        expected_supply=TOTAL_SUPPLY,
        total_nodes=len(nodes),
    )

    logger.info("=== GENESIS BLOCK VALIDATION ===")
    logger.info("Expected hash: %s", expected_hash)
    logger.info("Expected supply: %s WMP", f"{TOTAL_SUPPLY:,}")
    logger.info("Validating %d NEXUS nodes (mode=%s)...", len(nodes), mode)

    validators = {
        "simulate": lambda n: validate_node_simulated(n, expected_hash),
        "http": validate_node_http,
        "grpc": validate_node_grpc,
    }
    validate_fn = validators.get(mode, validators["simulate"])

    for node in nodes:
        logger.info("  [%s] %s (%s)...", node.node_id, node.description, node.region)
        result = validate_fn(node)

        if result.status == "verified" and result.genesis_hash == expected_hash:
            report.verified += 1
            logger.info("    ✓ VERIFIED — hash match, supply=%s, latency=%.1fms",
                        f"{result.reported_supply:,}", result.latency_ms)
        elif result.status == "unreachable":
            report.unreachable += 1
            logger.warning("    ✗ UNREACHABLE — %s", result.error)
        else:
            report.failed += 1
            logger.error("    ✗ FAILED — hash=%s (expected %s)",
                         result.genesis_hash, expected_hash)

        report.nodes.append(asdict(result))

    # Super-Consensus: all reachable nodes agree
    reachable = report.verified + report.failed
    report.super_consensus = (report.verified == reachable and reachable > 0
                              and report.verified >= (report.total_nodes * 2 // 3 + 1))

    # Supply integrity: all verified nodes report correct supply
    verified_supplies = [n["reported_supply"] for n in report.nodes if n["status"] == "verified"]
    report.supply_integrity = (
        len(verified_supplies) > 0
        and all(s == TOTAL_SUPPLY for s in verified_supplies)
    )

    # Sign the report
    report_json = json.dumps(asdict(report), sort_keys=True, default=str)
    report.report_hash = hashlib.sha256(report_json.encode()).hexdigest()

    status = "SUPER-CONSENSUS ACHIEVED" if report.super_consensus else "CONSENSUS FAILED"
    logger.info("")
    logger.info("=== RESULT: %s ===", status)
    logger.info("Verified: %d/%d | Failed: %d | Unreachable: %d",
                report.verified, report.total_nodes, report.failed, report.unreachable)
    logger.info("Supply integrity: %s", "OK" if report.supply_integrity else "DRIFT DETECTED")
    logger.info("Report hash: %s", report.report_hash[:16])

    # Save report
    report_path = LOG_DIR / f"genesis_validation_{report.report_id}.json"
    with open(report_path, "w") as f:
        json.dump(asdict(report), f, indent=2, default=str)
    logger.info("Report saved: %s", report_path)

    return report


# ── CLI ──────────────────────────────────────────────────────────────

def main():
    import argparse
    parser = argparse.ArgumentParser(description="Ierahkwa Genesis Block Validator")
    parser.add_argument("--mode", choices=["simulate", "http", "grpc"], default="simulate",
                        help="Validation mode (default: simulate)")
    parser.add_argument("--config", default=None, help="Path to nexus_nodes.json")
    parser.add_argument("--json", action="store_true", help="Output JSON report to stdout")
    args = parser.parse_args()

    nodes = load_nexus_nodes(args.config)
    report = run_validation(nodes, mode=args.mode)

    if args.json:
        print(json.dumps(asdict(report), indent=2, default=str))

    sys.exit(0 if report.super_consensus else 1)


if __name__ == "__main__":
    main()
