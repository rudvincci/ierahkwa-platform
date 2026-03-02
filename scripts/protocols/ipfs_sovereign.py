#!/usr/bin/env python3
"""
Ierahkwa IPFS Sovereign Archive -- Archivo Eterno
Pinning, upload, replication, and doomsday archival service for the
Ierahkwa Sovereign Network. Ensures critical documents, contracts, and
training data are permanently preserved across IPFS and Filecoin.

Environment variables
---------------------
IPFS_API_URL            IPFS Kubo API endpoint          (default: http://127.0.0.1:5101)
MAMEYNODE_RPC           MameyNode JSON-RPC endpoint     (default: http://mameynode:8545)
VERITAS_ADDRESS         IerahkwaVeritas contract addr   (default: 0x...placeholder)
WEB3_STORAGE_TOKEN      web3.storage API token for Filecoin replication
PROJECT_ROOT            Path to Soberano-Organizado root (default: auto-detect)
LOG_DIR                 Logging directory                (default: logs/)
MANIFEST_DIR            Where to store archive manifests (default: data/ipfs_manifests)
"""

import argparse
import hashlib
import json
import logging
import os
import sys
import time
from datetime import datetime, timezone
from pathlib import Path
from typing import Optional

try:
    import requests as http_requests
except ImportError:
    sys.exit("requests is required: pip install requests")

try:
    from web3 import Web3
except ImportError:
    Web3 = None

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------

IPFS_API_URL = os.environ.get("IPFS_API_URL", "http://127.0.0.1:5101")
MAMEYNODE_RPC = os.environ.get("MAMEYNODE_RPC", "http://mameynode:8545")
VERITAS_ADDRESS = os.environ.get(
    "VERITAS_ADDRESS", "0x0000000000000000000000000000000000000000"
)
WEB3_STORAGE_TOKEN = os.environ.get("WEB3_STORAGE_TOKEN", "")

SCRIPT_DIR = Path(__file__).resolve().parent
_default_root = SCRIPT_DIR.parent.parent
PROJECT_ROOT = Path(os.environ.get("PROJECT_ROOT", str(_default_root)))

LOG_DIR = Path(os.environ.get("LOG_DIR", "logs"))
MANIFEST_DIR = Path(os.environ.get("MANIFEST_DIR", "data/ipfs_manifests"))

LOG_DIR.mkdir(parents=True, exist_ok=True)
MANIFEST_DIR.mkdir(parents=True, exist_ok=True)

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(name)s: %(message)s",
    handlers=[
        logging.StreamHandler(sys.stdout),
        logging.FileHandler(LOG_DIR / "ipfs_sovereign.log", encoding="utf-8"),
    ],
)
logger = logging.getLogger("ierahkwa.ipfs_sovereign")

# ---------------------------------------------------------------------------
# IPFS API helpers
# ---------------------------------------------------------------------------


def ipfs_is_alive() -> bool:
    """Check if the local IPFS daemon is responding."""
    try:
        resp = http_requests.post(
            f"{IPFS_API_URL}/api/v0/id", timeout=10
        )
        return resp.status_code == 200
    except Exception:
        return False


def pin_file(path: str) -> str:
    """Pin a single file to IPFS. Return the CID or empty string on failure."""
    file_path = Path(path)
    if not file_path.exists():
        logger.error("File not found: %s", path)
        return ""
    if not file_path.is_file():
        logger.error("Not a file: %s", path)
        return ""

    try:
        with open(file_path, "rb") as fh:
            resp = http_requests.post(
                f"{IPFS_API_URL}/api/v0/add",
                files={"file": (file_path.name, fh)},
                timeout=120,
            )
        resp.raise_for_status()
        result = resp.json()
        cid = result.get("Hash", "")
        size = result.get("Size", "?")
        logger.info("Pinned file: %s -> %s (%s bytes)", file_path.name, cid, size)
        return cid
    except Exception as exc:
        logger.error("Failed to pin file %s: %s", path, exc)
        return ""


def pin_directory(path: str) -> dict:
    """Pin an entire directory recursively to IPFS.

    Returns a dict mapping relative paths to CIDs, plus a 'root' key
    with the directory CID.
    """
    dir_path = Path(path)
    if not dir_path.is_dir():
        logger.error("Not a directory: %s", path)
        return {}

    files = []
    for file_path in sorted(dir_path.rglob("*")):
        if file_path.is_file():
            rel = file_path.relative_to(dir_path)
            files.append((str(rel), file_path))

    if not files:
        logger.warning("Directory is empty: %s", path)
        return {}

    try:
        multipart = []
        for rel_name, abs_path in files:
            multipart.append(
                ("file", (rel_name, open(abs_path, "rb")))
            )

        resp = http_requests.post(
            f"{IPFS_API_URL}/api/v0/add",
            params={"recursive": "true", "wrap-with-directory": "true"},
            files=multipart,
            timeout=300,
        )
        resp.raise_for_status()

        # IPFS returns one JSON object per line
        mapping = {}
        root_cid = ""
        for line in resp.text.strip().split("\n"):
            entry = json.loads(line)
            name = entry.get("Name", "")
            cid = entry.get("Hash", "")
            if name == "":
                root_cid = cid
            else:
                mapping[name] = cid

        # Close file handles
        for _, (_, fh) in multipart:
            fh.close()

        mapping["_root"] = root_cid
        logger.info("Pinned directory: %s -> root %s (%d files)",
                     dir_path.name, root_cid, len(files))
        return mapping

    except Exception as exc:
        logger.error("Failed to pin directory %s: %s", path, exc)
        return {}


def pin_evidence(content: str, metadata: dict) -> str:
    """Pin evidence for the Veritas protocol with metadata envelope.

    Returns the CID of the pinned evidence package.
    """
    package = {
        "type": "veritas_evidence",
        "timestamp": datetime.now(timezone.utc).isoformat(),
        "metadata": metadata,
        "content": content,
        "content_hash": hashlib.sha256(content.encode("utf-8")).hexdigest(),
    }
    payload = json.dumps(package, ensure_ascii=False, indent=2)

    try:
        resp = http_requests.post(
            f"{IPFS_API_URL}/api/v0/add",
            files={"file": ("evidence.json", payload.encode("utf-8"))},
            timeout=60,
        )
        resp.raise_for_status()
        cid = resp.json().get("Hash", "")
        logger.info("Evidence pinned: %s (metadata: %s)", cid,
                     json.dumps(metadata, ensure_ascii=False)[:120])
        return cid
    except Exception as exc:
        logger.error("Failed to pin evidence: %s", exc)
        return ""


# ---------------------------------------------------------------------------
# Doomsday archive
# ---------------------------------------------------------------------------

# Critical files relative to PROJECT_ROOT
CRITICAL_FILES = [
    "MANIFESTO.md",
    "01-documentos/legal/TOKENOMICS-WAMPUM.md",
    "01-documentos/legal/GENESIS-PROPOSAL-GP001.md",
    "01-documentos/legal/GOVERNANCE-CRISIS.md",
]

# Critical directories relative to PROJECT_ROOT
CRITICAL_DIRS = [
    "ai-training",
]

# Glob patterns for contract files
CONTRACT_GLOBS = [
    "**/*.sol",
]

# Glob patterns for whitepapers
WHITEPAPER_GLOBS = [
    "**/whitepaper*",
    "**/WHITEPAPER*",
    "docs/**/*.md",
    "01-documentos/**/*.md",
]


def _collect_files_by_glob(root: Path, patterns: list[str]) -> list[Path]:
    """Collect files matching any of the glob patterns under root."""
    found = set()
    for pattern in patterns:
        for p in root.glob(pattern):
            if p.is_file():
                found.add(p)
    return sorted(found)


def doomsday_archive() -> dict:
    """Archive all critical files and directories to IPFS.

    Returns a manifest dict mapping logical names to CIDs.
    """
    logger.info("=" * 60)
    logger.info("  DOOMSDAY ARCHIVE -- Archivo Eterno")
    logger.info("  Project root: %s", PROJECT_ROOT)
    logger.info("=" * 60)

    manifest = {
        "type": "doomsday_archive",
        "timestamp": datetime.now(timezone.utc).isoformat(),
        "project_root": str(PROJECT_ROOT),
        "files": {},
        "directories": {},
        "contracts": {},
        "whitepapers": {},
        "errors": [],
    }

    # 1. Critical individual files
    for rel_path in CRITICAL_FILES:
        abs_path = PROJECT_ROOT / rel_path
        if abs_path.exists():
            cid = pin_file(str(abs_path))
            if cid:
                manifest["files"][rel_path] = cid
            else:
                manifest["errors"].append(f"Pin failed: {rel_path}")
        else:
            logger.warning("Critical file not found: %s", abs_path)
            manifest["errors"].append(f"Not found: {rel_path}")

    # 2. Critical directories
    for rel_dir in CRITICAL_DIRS:
        abs_dir = PROJECT_ROOT / rel_dir
        if abs_dir.is_dir():
            result = pin_directory(str(abs_dir))
            if result:
                manifest["directories"][rel_dir] = result
            else:
                manifest["errors"].append(f"Dir pin failed: {rel_dir}")
        else:
            logger.warning("Critical directory not found: %s", abs_dir)
            manifest["errors"].append(f"Dir not found: {rel_dir}")

    # 3. All Solidity contracts
    contracts = _collect_files_by_glob(PROJECT_ROOT, CONTRACT_GLOBS)
    for contract_path in contracts:
        rel = str(contract_path.relative_to(PROJECT_ROOT))
        cid = pin_file(str(contract_path))
        if cid:
            manifest["contracts"][rel] = cid

    # 4. Whitepapers and documentation
    whitepapers = _collect_files_by_glob(PROJECT_ROOT, WHITEPAPER_GLOBS)
    for wp_path in whitepapers:
        rel = str(wp_path.relative_to(PROJECT_ROOT))
        # Skip if already covered by critical files
        if rel in manifest["files"]:
            continue
        cid = pin_file(str(wp_path))
        if cid:
            manifest["whitepapers"][rel] = cid

    # Save manifest
    manifest_hash = hashlib.sha256(
        json.dumps(manifest, sort_keys=True).encode()
    ).hexdigest()
    manifest["manifest_hash"] = manifest_hash

    total_pinned = (
        len(manifest["files"])
        + len(manifest["directories"])
        + len(manifest["contracts"])
        + len(manifest["whitepapers"])
    )
    manifest["total_pinned"] = total_pinned

    ts = datetime.now(timezone.utc).strftime("%Y%m%d_%H%M%S")
    manifest_path = MANIFEST_DIR / f"doomsday_{ts}.json"
    with open(manifest_path, "w", encoding="utf-8") as fh:
        json.dump(manifest, fh, ensure_ascii=False, indent=2)

    logger.info(
        "Doomsday archive complete: %d items pinned, manifest: %s",
        total_pinned,
        manifest_path,
    )
    return manifest


# ---------------------------------------------------------------------------
# Filecoin replication via web3.storage
# ---------------------------------------------------------------------------

WEB3_STORAGE_API = "https://api.web3.storage"


def replicate_to_filecoin(cid: str) -> dict:
    """Submit a CID for Filecoin storage deal via web3.storage.

    Returns the API response dict or an error dict.
    """
    if not WEB3_STORAGE_TOKEN:
        logger.warning("WEB3_STORAGE_TOKEN not set. Filecoin replication skipped.")
        return {"error": "No web3.storage token configured"}

    if not cid:
        return {"error": "Empty CID"}

    try:
        # First, fetch the content from IPFS
        resp = http_requests.post(
            f"{IPFS_API_URL}/api/v0/cat",
            params={"arg": cid},
            timeout=120,
        )
        resp.raise_for_status()
        content = resp.content

        # Upload to web3.storage
        upload_resp = http_requests.post(
            f"{WEB3_STORAGE_API}/upload",
            headers={
                "Authorization": f"Bearer {WEB3_STORAGE_TOKEN}",
                "X-Name": f"ierahkwa-archive-{cid[:16]}",
            },
            files={"file": (f"{cid}.dat", content)},
            timeout=180,
        )
        upload_resp.raise_for_status()
        result = upload_resp.json()
        logger.info("Replicated to Filecoin via web3.storage: %s", result.get("cid", cid))
        return result

    except Exception as exc:
        logger.error("Filecoin replication failed for %s: %s", cid, exc)
        return {"error": str(exc)}


# ---------------------------------------------------------------------------
# Verification
# ---------------------------------------------------------------------------


def verify_pin(cid: str) -> dict:
    """Check if a CID is still pinned and accessible on the local node.

    Returns a dict with 'pinned' (bool) and 'accessible' (bool).
    """
    result = {"cid": cid, "pinned": False, "accessible": False, "size": 0}

    if not cid:
        return result

    # Check pin status
    try:
        resp = http_requests.post(
            f"{IPFS_API_URL}/api/v0/pin/ls",
            params={"arg": cid, "type": "all"},
            timeout=30,
        )
        if resp.status_code == 200:
            data = resp.json()
            keys = data.get("Keys", {})
            if cid in keys:
                result["pinned"] = True
    except Exception as exc:
        logger.debug("Pin check error for %s: %s", cid, exc)

    # Check accessibility via stat
    try:
        resp = http_requests.post(
            f"{IPFS_API_URL}/api/v0/object/stat",
            params={"arg": cid},
            timeout=30,
        )
        if resp.status_code == 200:
            stat = resp.json()
            result["accessible"] = True
            result["size"] = stat.get("CumulativeSize", 0)
    except Exception as exc:
        logger.debug("Stat check error for %s: %s", cid, exc)

    return result


# ---------------------------------------------------------------------------
# Manifest -> on-chain via IerahkwaVeritas
# ---------------------------------------------------------------------------

VERITAS_ABI_FRAGMENT = [
    {
        "inputs": [
            {"name": "evidenceHash", "type": "bytes32"},
            {"name": "ipfsCid", "type": "string"},
        ],
        "name": "submitEvidence",
        "outputs": [],
        "stateMutability": "nonpayable",
        "type": "function",
    }
]


def manifest_to_chain(manifest: dict) -> str:
    """Store the manifest hash on the IerahkwaVeritas contract.

    Returns the transaction hash or empty string.
    """
    if Web3 is None:
        logger.warning("web3.py not available. On-chain manifest storage skipped.")
        return ""

    manifest_hash = manifest.get("manifest_hash", "")
    if not manifest_hash:
        logger.error("Manifest has no hash. Cannot store on-chain.")
        return ""

    try:
        w3 = Web3(Web3.HTTPProvider(MAMEYNODE_RPC))
        if not w3.is_connected():
            logger.error("Cannot connect to MameyNode at %s", MAMEYNODE_RPC)
            return ""

        contract = w3.eth.contract(
            address=Web3.to_checksum_address(VERITAS_ADDRESS),
            abi=VERITAS_ABI_FRAGMENT,
        )

        hash_bytes = bytes.fromhex(manifest_hash)
        # Pad to 32 bytes if needed
        if len(hash_bytes) < 32:
            hash_bytes = hash_bytes.ljust(32, b"\x00")
        elif len(hash_bytes) > 32:
            hash_bytes = hash_bytes[:32]

        # Determine manifest CID (pin the manifest itself)
        manifest_json = json.dumps(manifest, ensure_ascii=False)
        manifest_cid = ""
        try:
            resp = http_requests.post(
                f"{IPFS_API_URL}/api/v0/add",
                files={"file": ("manifest.json", manifest_json.encode("utf-8"))},
                timeout=30,
            )
            if resp.status_code == 200:
                manifest_cid = resp.json().get("Hash", "")
        except Exception:
            pass

        accounts = w3.eth.accounts
        if not accounts:
            logger.error("No unlocked accounts on MameyNode.")
            return ""

        tx_hash = contract.functions.submitEvidence(
            hash_bytes, manifest_cid or manifest_hash
        ).transact({"from": accounts[0]})

        receipt = w3.eth.wait_for_transaction_receipt(tx_hash, timeout=60)
        tx_hex = receipt.transactionHash.hex()
        logger.info("Manifest stored on-chain: tx=%s, hash=%s", tx_hex, manifest_hash[:16])
        return tx_hex

    except Exception as exc:
        logger.error("Failed to store manifest on-chain: %s", exc)
        return ""


# ---------------------------------------------------------------------------
# Scheduled backup (cron-compatible)
# ---------------------------------------------------------------------------


def scheduled_backup() -> dict:
    """Run a full doomsday archive + Filecoin replication.

    Designed to be called weekly via cron or systemd timer.
    Returns the manifest.
    """
    logger.info("Scheduled backup starting...")

    if not ipfs_is_alive():
        logger.error("IPFS daemon is not responding at %s. Aborting backup.", IPFS_API_URL)
        return {"error": "IPFS not available"}

    manifest = doomsday_archive()

    # Store manifest on-chain
    tx = manifest_to_chain(manifest)
    if tx:
        manifest["on_chain_tx"] = tx

    # Replicate critical items to Filecoin
    if WEB3_STORAGE_TOKEN:
        replicated = []
        # Replicate the most important files
        for name, cid in list(manifest.get("files", {}).items())[:10]:
            result = replicate_to_filecoin(cid)
            if "error" not in result:
                replicated.append({"name": name, "cid": cid})
        # Replicate all contracts
        for name, cid in manifest.get("contracts", {}).items():
            result = replicate_to_filecoin(cid)
            if "error" not in result:
                replicated.append({"name": name, "cid": cid})
        manifest["filecoin_replicated"] = replicated

    logger.info("Scheduled backup complete.")
    return manifest


# ---------------------------------------------------------------------------
# IPFS node status
# ---------------------------------------------------------------------------


def get_ipfs_status() -> dict:
    """Return IPFS node status including pin count and repo size."""
    status = {
        "alive": False,
        "peer_id": "",
        "pin_count": 0,
        "repo_size": 0,
        "repo_size_human": "",
    }

    try:
        # Node identity
        resp = http_requests.post(f"{IPFS_API_URL}/api/v0/id", timeout=10)
        if resp.status_code == 200:
            data = resp.json()
            status["alive"] = True
            status["peer_id"] = data.get("ID", "")

        # Pin count
        resp = http_requests.post(
            f"{IPFS_API_URL}/api/v0/pin/ls",
            params={"type": "recursive"},
            timeout=30,
        )
        if resp.status_code == 200:
            keys = resp.json().get("Keys", {})
            status["pin_count"] = len(keys)

        # Repo stats
        resp = http_requests.post(f"{IPFS_API_URL}/api/v0/repo/stat", timeout=15)
        if resp.status_code == 200:
            stat = resp.json()
            repo_size = stat.get("RepoSize", 0)
            status["repo_size"] = repo_size
            # Human-readable size
            for unit in ("B", "KB", "MB", "GB", "TB"):
                if repo_size < 1024:
                    status["repo_size_human"] = f"{repo_size:.1f} {unit}"
                    break
                repo_size /= 1024

    except Exception as exc:
        logger.error("IPFS status check failed: %s", exc)

    return status


# ---------------------------------------------------------------------------
# CLI
# ---------------------------------------------------------------------------


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(
        description="Ierahkwa IPFS Sovereign Archive -- Archivo Eterno",
    )
    sub = parser.add_subparsers(dest="command", help="Available commands")

    # pin
    pin_p = sub.add_parser("pin", help="Pin a file or directory to IPFS")
    pin_p.add_argument("path", help="File or directory path to pin")

    # evidence
    ev_p = sub.add_parser("evidence", help="Pin evidence with metadata for Veritas")
    ev_p.add_argument("content_file", help="Path to evidence content file")
    ev_p.add_argument("--title", default="", help="Evidence title")
    ev_p.add_argument("--submitter", default="", help="Submitter identifier")
    ev_p.add_argument("--case-id", default="", help="Case identifier")

    # archive
    sub.add_parser("archive", help="Run doomsday archive of all critical files")

    # verify
    ver_p = sub.add_parser("verify", help="Verify a CID is pinned and accessible")
    ver_p.add_argument("cid", help="CID to verify")

    # replicate
    rep_p = sub.add_parser("replicate", help="Replicate a CID to Filecoin")
    rep_p.add_argument("cid", help="CID to replicate")

    # status
    sub.add_parser("status", help="Show IPFS node status")

    # backup
    sub.add_parser("backup", help="Run scheduled backup (archive + replicate)")

    return parser


def main() -> None:
    parser = build_parser()
    args = parser.parse_args()

    if args.command is None:
        parser.print_help()
        sys.exit(0)

    if args.command == "pin":
        target = Path(args.path)
        if target.is_dir():
            result = pin_directory(args.path)
            if result:
                print(json.dumps(result, indent=2))
            else:
                sys.exit(1)
        elif target.is_file():
            cid = pin_file(args.path)
            if cid:
                print(f"CID: {cid}")
            else:
                sys.exit(1)
        else:
            logger.error("Path not found: %s", args.path)
            sys.exit(1)

    elif args.command == "evidence":
        content_path = Path(args.content_file)
        if not content_path.is_file():
            logger.error("Content file not found: %s", args.content_file)
            sys.exit(1)
        content = content_path.read_text(encoding="utf-8")
        metadata = {
            "title": args.title or content_path.stem,
            "submitter": args.submitter,
            "case_id": args.case_id,
            "source_file": str(content_path),
        }
        cid = pin_evidence(content, metadata)
        if cid:
            print(f"Evidence CID: {cid}")
        else:
            sys.exit(1)

    elif args.command == "archive":
        manifest = doomsday_archive()
        print(json.dumps(manifest, indent=2, ensure_ascii=False))
        tx = manifest_to_chain(manifest)
        if tx:
            print(f"On-chain TX: {tx}")

    elif args.command == "verify":
        result = verify_pin(args.cid)
        print(json.dumps(result, indent=2))
        if not result["pinned"]:
            sys.exit(1)

    elif args.command == "replicate":
        result = replicate_to_filecoin(args.cid)
        print(json.dumps(result, indent=2))
        if "error" in result:
            sys.exit(1)

    elif args.command == "status":
        status = get_ipfs_status()
        print(json.dumps(status, indent=2))
        if not status["alive"]:
            sys.exit(1)

    elif args.command == "backup":
        manifest = scheduled_backup()
        print(json.dumps(manifest, indent=2, ensure_ascii=False))


if __name__ == "__main__":
    main()
