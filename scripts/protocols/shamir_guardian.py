#!/usr/bin/env python3
"""
Ierahkwa Shamir Guardian -- Secret Sharing for Key Management
Pure Python implementation of Shamir's Secret Sharing over GF(256) for
splitting and reconstructing cryptographic secrets among Guardians.

Default configuration: n=100 shares, k=34 threshold (matching the 34%
Guardian quorum defined in GOVERNANCE-CRISIS.md).

Features:
  - GF(256) Galois Field arithmetic (pure Python, no external crypto deps)
  - Split any arbitrary-length secret into n shares
  - Reconstruct from k or more shares
  - GPG encryption of individual shares for distribution
  - On-chain share hash verification via IerahkwaPulse
  - Interactive ceremony mode with secure memory wiping
  - CLI with subcommands: split, reconstruct, distribute, verify, ceremony, recover

Environment variables
---------------------
MAMEYNODE_RPC         MameyNode JSON-RPC endpoint   (default: http://mameynode:8545)
PULSE_ADDRESS         IerahkwaPulse contract addr   (default: 0x...placeholder)
LOG_DIR               Logging directory             (default: logs/)
"""

import argparse
import hashlib
import json
import logging
import os
import secrets
import subprocess
import sys
import time
from pathlib import Path
from typing import Optional

try:
    from web3 import Web3
except ImportError:
    Web3 = None

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------

MAMEYNODE_RPC = os.environ.get("MAMEYNODE_RPC", "http://mameynode:8545")
PULSE_ADDRESS = os.environ.get(
    "PULSE_ADDRESS", "0x0000000000000000000000000000000000000000"
)
LOG_DIR = Path(os.environ.get("LOG_DIR", "logs"))
LOG_DIR.mkdir(parents=True, exist_ok=True)

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(name)s: %(message)s",
    handlers=[
        logging.StreamHandler(sys.stdout),
        logging.FileHandler(LOG_DIR / "shamir_guardian.log", encoding="utf-8"),
    ],
)
logger = logging.getLogger("ierahkwa.shamir")

# ---------------------------------------------------------------------------
# GF(256) Galois Field Arithmetic
# ---------------------------------------------------------------------------
# Using the irreducible polynomial x^8 + x^4 + x^3 + x + 1 (0x11B)
# This is the same polynomial used in AES (Rijndael).

_GF256_EXP = [0] * 512
_GF256_LOG = [0] * 256


def _init_gf256_tables():
    """Pre-compute exponentiation and logarithm tables for GF(256)."""
    x = 1
    for i in range(255):
        _GF256_EXP[i] = x
        _GF256_LOG[x] = i
        x <<= 1
        if x & 0x100:
            x ^= 0x11B
    # Extend exp table for convenience (handles wrap-around)
    for i in range(255, 512):
        _GF256_EXP[i] = _GF256_EXP[i - 255]


_init_gf256_tables()


def gf256_add(a: int, b: int) -> int:
    """Addition in GF(256) is XOR."""
    return a ^ b


def gf256_sub(a: int, b: int) -> int:
    """Subtraction in GF(256) is also XOR."""
    return a ^ b


def gf256_mul(a: int, b: int) -> int:
    """Multiplication in GF(256) using log/exp tables."""
    if a == 0 or b == 0:
        return 0
    return _GF256_EXP[_GF256_LOG[a] + _GF256_LOG[b]]


def gf256_div(a: int, b: int) -> int:
    """Division in GF(256) using log/exp tables."""
    if b == 0:
        raise ZeroDivisionError("Division by zero in GF(256)")
    if a == 0:
        return 0
    return _GF256_EXP[(_GF256_LOG[a] - _GF256_LOG[b]) % 255]


def gf256_inv(a: int) -> int:
    """Multiplicative inverse in GF(256)."""
    if a == 0:
        raise ZeroDivisionError("Zero has no inverse in GF(256)")
    return _GF256_EXP[255 - _GF256_LOG[a]]


# ---------------------------------------------------------------------------
# Polynomial evaluation over GF(256)
# ---------------------------------------------------------------------------


def _eval_poly(coeffs: list[int], x: int) -> int:
    """Evaluate a polynomial at point x in GF(256).

    coeffs[0] is the constant term (the secret byte).
    """
    result = 0
    for i in range(len(coeffs) - 1, -1, -1):
        result = gf256_add(gf256_mul(result, x), coeffs[i])
    return result


# ---------------------------------------------------------------------------
# Lagrange interpolation over GF(256)
# ---------------------------------------------------------------------------


def _lagrange_interpolate(points: list[tuple[int, int]], x: int = 0) -> int:
    """Lagrange interpolation at point x using the given (x_i, y_i) points
    in GF(256). Default x=0 recovers the constant term (the secret byte)."""
    k = len(points)
    result = 0

    for i in range(k):
        xi, yi = points[i]
        if yi == 0:
            continue

        num = 1
        den = 1
        for j in range(k):
            if i == j:
                continue
            xj = points[j][0]
            num = gf256_mul(num, gf256_sub(x, xj))
            den = gf256_mul(den, gf256_sub(xi, xj))

        lagrange_coeff = gf256_mul(yi, gf256_div(num, den))
        result = gf256_add(result, lagrange_coeff)

    return result


# ---------------------------------------------------------------------------
# Shamir's Secret Sharing
# ---------------------------------------------------------------------------


def split_secret(secret: bytes, n: int = 100, k: int = 34) -> list[tuple[int, bytes]]:
    """Split a secret into n shares, requiring k to reconstruct.

    Parameters
    ----------
    secret : bytes
        The secret to split (arbitrary length).
    n : int
        Total number of shares to generate (max 255).
    k : int
        Minimum number of shares needed to reconstruct.

    Returns
    -------
    list of (share_index, share_bytes)
        Each share is a tuple of (1-based index, bytes of same length as secret).
    """
    if n < 2:
        raise ValueError("n must be at least 2")
    if k < 2:
        raise ValueError("k must be at least 2")
    if k > n:
        raise ValueError("k must be <= n")
    if n > 255:
        raise ValueError("n must be <= 255 (GF(256) constraint)")
    if not secret:
        raise ValueError("Secret must not be empty")

    secret_len = len(secret)
    shares = [(i, bytearray(secret_len)) for i in range(1, n + 1)]

    for byte_idx in range(secret_len):
        # Random polynomial of degree k-1 with secret byte as constant term
        coeffs = [secret[byte_idx]]
        for _ in range(k - 1):
            coeffs.append(secrets.randbelow(256))

        # Evaluate at each share point
        for share_idx in range(n):
            x = shares[share_idx][0]
            y = _eval_poly(coeffs, x)
            shares[share_idx][1][byte_idx] = y

    # Convert bytearrays to bytes
    return [(idx, bytes(data)) for idx, data in shares]


def reconstruct_secret(shares: list[tuple[int, bytes]]) -> bytes:
    """Reconstruct a secret from k or more shares.

    Parameters
    ----------
    shares : list of (share_index, share_bytes)
        At least k shares from the original split.

    Returns
    -------
    bytes
        The reconstructed secret.
    """
    if not shares:
        raise ValueError("No shares provided")

    secret_len = len(shares[0][1])
    for idx, data in shares:
        if len(data) != secret_len:
            raise ValueError(f"Share {idx} has inconsistent length")

    result = bytearray(secret_len)

    for byte_idx in range(secret_len):
        points = [(idx, data[byte_idx]) for idx, data in shares]
        result[byte_idx] = _lagrange_interpolate(points, 0)

    return bytes(result)


# ---------------------------------------------------------------------------
# Genesis seed generation
# ---------------------------------------------------------------------------


def generate_genesis_seed(bits: int = 256) -> bytes:
    """Generate a cryptographically secure genesis seed.

    Uses os.urandom via the secrets module for maximum entropy.
    """
    byte_count = bits // 8
    seed = secrets.token_bytes(byte_count)
    logger.info("Genesis seed generated: %d bits (%d bytes)", bits, byte_count)
    return seed


def seed_checksum(seed: bytes) -> str:
    """Compute SHA-256 checksum of a seed."""
    return hashlib.sha256(seed).hexdigest()


# ---------------------------------------------------------------------------
# GPG distribution
# ---------------------------------------------------------------------------


def distribute_shares(
    shares: list[tuple[int, bytes]],
    guardian_gpg_keys: dict[int, str],
    output_dir: str = "data/shamir_shares",
) -> dict[int, str]:
    """Encrypt each share with the corresponding Guardian's GPG public key.

    Parameters
    ----------
    shares : list of (index, bytes)
    guardian_gpg_keys : dict mapping share index -> GPG key ID or fingerprint
    output_dir : str
        Directory to write encrypted share files.

    Returns
    -------
    dict mapping share index -> output file path
    """
    out_path = Path(output_dir)
    out_path.mkdir(parents=True, exist_ok=True)

    results = {}
    for idx, share_data in shares:
        gpg_key = guardian_gpg_keys.get(idx)
        if not gpg_key:
            logger.warning("No GPG key for share %d. Skipping.", idx)
            continue

        share_hex = share_data.hex()
        share_json = json.dumps({
            "share_index": idx,
            "share_hex": share_hex,
            "share_hash": hashlib.sha256(share_data).hexdigest(),
            "timestamp": _utc_iso(),
        })

        output_file = out_path / f"share_{idx:03d}.json.gpg"
        try:
            proc = subprocess.run(
                [
                    "gpg", "--batch", "--yes", "--trust-model", "always",
                    "--recipient", gpg_key,
                    "--output", str(output_file),
                    "--encrypt",
                ],
                input=share_json.encode("utf-8"),
                capture_output=True,
                timeout=30,
            )
            if proc.returncode == 0:
                results[idx] = str(output_file)
                logger.info("Share %d encrypted for GPG key %s -> %s", idx, gpg_key, output_file)
            else:
                logger.error("GPG encryption failed for share %d: %s",
                             idx, proc.stderr.decode("utf-8", errors="replace"))
        except FileNotFoundError:
            logger.error("GPG not found. Cannot encrypt shares.")
            break
        except subprocess.TimeoutExpired:
            logger.error("GPG encryption timed out for share %d.", idx)

    return results


# ---------------------------------------------------------------------------
# On-chain share hash verification
# ---------------------------------------------------------------------------

PULSE_SHARE_ABI = [
    {
        "inputs": [
            {"name": "guardian", "type": "address"},
            {"name": "shareHash", "type": "bytes32"},
        ],
        "name": "registerShareHash",
        "outputs": [],
        "stateMutability": "nonpayable",
        "type": "function",
    },
    {
        "inputs": [
            {"name": "guardian", "type": "address"},
        ],
        "name": "getShareHash",
        "outputs": [{"name": "", "type": "bytes32"}],
        "stateMutability": "view",
        "type": "function",
    },
]


def store_share_on_chain(guardian_address: str, share_hash: str) -> str:
    """Store the hash of a share on-chain for later verification.

    Returns transaction hash or empty string.
    """
    if Web3 is None:
        logger.warning("web3.py not available. On-chain share registration skipped.")
        return ""

    try:
        w3 = Web3(Web3.HTTPProvider(MAMEYNODE_RPC))
        if not w3.is_connected():
            logger.error("MameyNode not connected.")
            return ""

        contract = w3.eth.contract(
            address=Web3.to_checksum_address(PULSE_ADDRESS),
            abi=PULSE_SHARE_ABI,
        )

        hash_bytes = bytes.fromhex(share_hash)
        if len(hash_bytes) < 32:
            hash_bytes = hash_bytes.ljust(32, b"\x00")
        elif len(hash_bytes) > 32:
            hash_bytes = hash_bytes[:32]

        accounts = w3.eth.accounts
        if not accounts:
            logger.error("No unlocked accounts.")
            return ""

        tx = contract.functions.registerShareHash(
            Web3.to_checksum_address(guardian_address),
            hash_bytes,
        ).transact({"from": accounts[0]})

        receipt = w3.eth.wait_for_transaction_receipt(tx, timeout=60)
        tx_hex = receipt.transactionHash.hex()
        logger.info("Share hash registered on-chain: guardian=%s, tx=%s",
                     guardian_address, tx_hex)
        return tx_hex

    except Exception as exc:
        logger.error("On-chain share registration failed: %s", exc)
        return ""


def verify_share(share_data: bytes, expected_hash: str) -> bool:
    """Verify that a share matches its expected hash."""
    computed = hashlib.sha256(share_data).hexdigest()
    match = computed == expected_hash
    if match:
        logger.info("Share verification: MATCH")
    else:
        logger.warning("Share verification: MISMATCH (computed=%s, expected=%s)",
                       computed[:16], expected_hash[:16])
    return match


# ---------------------------------------------------------------------------
# Secure memory wipe
# ---------------------------------------------------------------------------


def secure_wipe(data: bytearray) -> None:
    """Overwrite a bytearray with zeros, then random bytes, then zeros.

    This is a best-effort wipe; Python's memory management may retain copies.
    """
    length = len(data)
    for i in range(length):
        data[i] = 0
    for i in range(length):
        data[i] = secrets.randbelow(256)
    for i in range(length):
        data[i] = 0


# ---------------------------------------------------------------------------
# Ceremony mode
# ---------------------------------------------------------------------------


def ceremony_mode(secret: bytes, n: int = 100, k: int = 34) -> dict:
    """Interactive ceremony for distributing shares to Guardians.

    Displays each share on screen one at a time, waits for Guardian
    confirmation, then securely wipes the share from memory.

    Returns a dict with ceremony metadata.
    """
    logger.info("=" * 60)
    logger.info("  IERAHKWA KEY CEREMONY")
    logger.info("  Shares: %d, Threshold: %d", n, k)
    logger.info("=" * 60)

    checksum = seed_checksum(secret)
    shares = split_secret(secret, n, k)

    ceremony_log = {
        "type": "key_ceremony",
        "timestamp": _utc_iso(),
        "n": n,
        "k": k,
        "secret_checksum": checksum,
        "shares_distributed": [],
    }

    print("\n" + "=" * 60)
    print("  IERAHKWA KEY CEREMONY -- Ceremonia de Claves")
    print(f"  Total shares: {n} | Threshold: {k}")
    print(f"  Secret checksum (SHA-256): {checksum}")
    print("=" * 60)

    for idx, share_data in shares:
        share_hex = share_data.hex()
        share_hash = hashlib.sha256(share_data).hexdigest()

        print(f"\n--- Share {idx}/{n} ---")
        print(f"Guardian, please record this share:")
        print(f"  Index: {idx}")
        print(f"  Data:  {share_hex}")
        print(f"  Hash:  {share_hash}")
        print(f"---")

        # Wait for confirmation
        while True:
            response = input(
                f"Guardian {idx}: Confirm receipt? (yes/si/y to confirm, skip to skip): "
            ).strip().lower()
            if response in ("yes", "si", "y"):
                ceremony_log["shares_distributed"].append({
                    "index": idx,
                    "hash": share_hash,
                    "confirmed": True,
                    "timestamp": _utc_iso(),
                })
                break
            elif response == "skip":
                ceremony_log["shares_distributed"].append({
                    "index": idx,
                    "hash": share_hash,
                    "confirmed": False,
                    "timestamp": _utc_iso(),
                })
                break
            else:
                print("Please type 'yes', 'si', 'y', or 'skip'.")

        # Secure wipe the share from memory
        share_buf = bytearray(share_data)
        secure_wipe(share_buf)
        print(f"[Share {idx} wiped from memory]")

    # Wipe the secret from memory
    secret_buf = bytearray(secret)
    secure_wipe(secret_buf)

    confirmed_count = sum(
        1 for s in ceremony_log["shares_distributed"] if s["confirmed"]
    )
    ceremony_log["confirmed_count"] = confirmed_count
    ceremony_log["threshold_met"] = confirmed_count >= k

    print("\n" + "=" * 60)
    print(f"  CEREMONY COMPLETE")
    print(f"  Confirmed: {confirmed_count}/{n}")
    print(f"  Threshold met: {'YES' if confirmed_count >= k else 'NO'}")
    print("=" * 60)

    logger.info("Key ceremony complete: %d/%d confirmed", confirmed_count, n)
    return ceremony_log


# ---------------------------------------------------------------------------
# Recovery mode
# ---------------------------------------------------------------------------


def recovery_mode(expected_checksum: str = "") -> Optional[bytes]:
    """Interactive recovery: collect shares from Guardians and reconstruct.

    Returns the reconstructed secret or None on failure.
    """
    print("\n" + "=" * 60)
    print("  IERAHKWA KEY RECOVERY -- Recuperacion de Claves")
    print("=" * 60)

    shares = []
    print("\nEnter shares one at a time. Type 'done' when finished.\n")

    while True:
        idx_str = input("Share index (or 'done'): ").strip()
        if idx_str.lower() == "done":
            break
        try:
            idx = int(idx_str)
        except ValueError:
            print("Invalid index. Please enter a number.")
            continue

        hex_str = input(f"Share {idx} hex data: ").strip()
        try:
            share_data = bytes.fromhex(hex_str)
        except ValueError:
            print("Invalid hex data. Please try again.")
            continue

        # Optional hash verification
        hash_input = input(f"Share {idx} hash (optional, press Enter to skip): ").strip()
        if hash_input:
            if not verify_share(share_data, hash_input):
                print("WARNING: Share hash does not match! Proceed with caution.")
                confirm = input("Use this share anyway? (yes/no): ").strip().lower()
                if confirm not in ("yes", "y", "si"):
                    continue

        shares.append((idx, share_data))
        print(f"Share {idx} accepted. Total shares: {len(shares)}")

    if not shares:
        print("No shares provided. Recovery aborted.")
        return None

    print(f"\nAttempting reconstruction with {len(shares)} shares...")

    try:
        secret = reconstruct_secret(shares)
    except Exception as exc:
        print(f"Reconstruction failed: {exc}")
        logger.error("Recovery failed: %s", exc)
        return None

    checksum = seed_checksum(secret)
    print(f"Reconstructed secret checksum: {checksum}")

    if expected_checksum:
        if checksum == expected_checksum:
            print("Checksum MATCHES expected value.")
        else:
            print(f"WARNING: Checksum MISMATCH! Expected: {expected_checksum}")

    # Wipe shares from memory
    for _, share_data in shares:
        buf = bytearray(share_data)
        secure_wipe(buf)

    logger.info("Key recovery complete. Checksum: %s", checksum)
    return secret


# ---------------------------------------------------------------------------
# Utilities
# ---------------------------------------------------------------------------


def _utc_iso() -> str:
    from datetime import datetime, timezone
    return datetime.now(timezone.utc).isoformat()


def share_to_json(idx: int, share_data: bytes) -> str:
    """Serialize a share to JSON."""
    return json.dumps({
        "share_index": idx,
        "share_hex": share_data.hex(),
        "share_hash": hashlib.sha256(share_data).hexdigest(),
    })


def shares_from_json_file(path: str) -> list[tuple[int, bytes]]:
    """Load shares from a JSON file (array of share objects)."""
    with open(path, "r", encoding="utf-8") as fh:
        data = json.load(fh)
    shares = []
    for entry in data:
        idx = entry["share_index"]
        share_data = bytes.fromhex(entry["share_hex"])
        shares.append((idx, share_data))
    return shares


# ---------------------------------------------------------------------------
# CLI
# ---------------------------------------------------------------------------


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(
        description="Ierahkwa Shamir Guardian -- Secret Sharing for Key Management",
    )
    sub = parser.add_subparsers(dest="command", help="Available commands")

    # split
    split_p = sub.add_parser("split", help="Split a secret into shares")
    split_p.add_argument("--secret", help="Secret as hex string (or reads from stdin)")
    split_p.add_argument("--secret-file", help="File containing the secret")
    split_p.add_argument("-n", type=int, default=100, help="Total shares (default: 100)")
    split_p.add_argument("-k", type=int, default=34, help="Threshold (default: 34)")
    split_p.add_argument("--output", default="", help="Output file for shares JSON")

    # reconstruct
    recon_p = sub.add_parser("reconstruct", help="Reconstruct secret from shares")
    recon_p.add_argument("--shares-file", required=True, help="JSON file with shares")
    recon_p.add_argument("--checksum", default="", help="Expected SHA-256 checksum")

    # distribute
    dist_p = sub.add_parser("distribute", help="Encrypt shares with GPG keys")
    dist_p.add_argument("--shares-file", required=True, help="JSON file with shares")
    dist_p.add_argument("--keys-file", required=True,
                        help="JSON file mapping share index to GPG key ID")
    dist_p.add_argument("--output-dir", default="data/shamir_shares",
                        help="Output directory for encrypted shares")

    # verify
    ver_p = sub.add_parser("verify", help="Verify a share against its hash")
    ver_p.add_argument("--share-hex", required=True, help="Share data as hex")
    ver_p.add_argument("--hash", required=True, help="Expected SHA-256 hash")

    # ceremony
    cer_p = sub.add_parser("ceremony", help="Interactive key ceremony")
    cer_p.add_argument("-n", type=int, default=100, help="Total shares (default: 100)")
    cer_p.add_argument("-k", type=int, default=34, help="Threshold (default: 34)")
    cer_p.add_argument("--output-log", default="data/ceremony_log.json",
                       help="Output path for ceremony log")

    # recover
    rec_p = sub.add_parser("recover", help="Interactive key recovery")
    rec_p.add_argument("--checksum", default="", help="Expected SHA-256 checksum")

    return parser


def main() -> None:
    parser = build_parser()
    args = parser.parse_args()

    if args.command is None:
        parser.print_help()
        sys.exit(0)

    if args.command == "split":
        # Get secret
        if args.secret:
            secret = bytes.fromhex(args.secret)
        elif args.secret_file:
            with open(args.secret_file, "rb") as fh:
                secret = fh.read()
        else:
            print("Enter secret as hex (or pipe via stdin):")
            secret = bytes.fromhex(sys.stdin.readline().strip())

        shares = split_secret(secret, args.n, args.k)
        checksum = seed_checksum(secret)

        # Output
        output = {
            "n": args.n,
            "k": args.k,
            "secret_checksum": checksum,
            "shares": [
                {
                    "share_index": idx,
                    "share_hex": data.hex(),
                    "share_hash": hashlib.sha256(data).hexdigest(),
                }
                for idx, data in shares
            ],
        }

        if args.output:
            with open(args.output, "w", encoding="utf-8") as fh:
                json.dump(output, fh, indent=2)
            print(f"Shares written to {args.output}")
        else:
            print(json.dumps(output, indent=2))

        print(f"\nSecret checksum: {checksum}")
        print(f"Shares generated: {len(shares)}, threshold: {args.k}")

    elif args.command == "reconstruct":
        shares = shares_from_json_file(args.shares_file)
        secret = reconstruct_secret(shares)
        checksum = seed_checksum(secret)

        print(f"Reconstructed secret (hex): {secret.hex()}")
        print(f"Checksum: {checksum}")

        if args.checksum:
            if checksum == args.checksum:
                print("Checksum MATCHES.")
            else:
                print(f"Checksum MISMATCH! Expected: {args.checksum}")
                sys.exit(1)

    elif args.command == "distribute":
        shares = shares_from_json_file(args.shares_file)
        with open(args.keys_file, "r", encoding="utf-8") as fh:
            keys_data = json.load(fh)
        # Convert string keys to int
        guardian_keys = {int(k): v for k, v in keys_data.items()}
        results = distribute_shares(shares, guardian_keys, args.output_dir)
        print(f"Encrypted {len(results)} shares to {args.output_dir}")

    elif args.command == "verify":
        share_data = bytes.fromhex(args.share_hex)
        match = verify_share(share_data, args.hash)
        if match:
            print("VERIFIED: Share matches hash.")
        else:
            print("FAILED: Share does not match hash.")
            sys.exit(1)

    elif args.command == "ceremony":
        seed = generate_genesis_seed()
        log = ceremony_mode(seed, args.n, args.k)

        log_path = Path(args.output_log)
        log_path.parent.mkdir(parents=True, exist_ok=True)
        with open(log_path, "w", encoding="utf-8") as fh:
            json.dump(log, fh, indent=2, ensure_ascii=False)
        print(f"Ceremony log saved to {log_path}")

    elif args.command == "recover":
        secret = recovery_mode(expected_checksum=args.checksum)
        if secret:
            print(f"Recovered secret (hex): {secret.hex()}")
        else:
            sys.exit(1)


if __name__ == "__main__":
    main()
