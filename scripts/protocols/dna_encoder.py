#!/usr/bin/env python3
"""
DNA Digital Storage Encoder -- Ierahkwa Sovereign Archive
Converts binary data to nucleotide sequences (A, C, G, T) for
long-term archival in synthetic DNA. The Ierahkwa Manifesto and
critical governance documents can be encoded into DNA sequences
that are synthesizable with current technology and will survive
for thousands of years.

Theory
------
Each nucleotide encodes 2 bits:
    A = 00, C = 01, G = 10, T = 11

1 byte = 4 nucleotides.
The entire MANIFESTO.md (~10 KB) = ~40,000 nucleotides.

A typical DNA synthesis order (Twist Bioscience, IDT) can handle
sequences of 300-3,000 bp per fragment. Longer payloads are split
into numbered fragments with overlap regions for assembly.

Reed-Solomon error correction protects against nucleotide
substitution errors during synthesis, storage, and sequencing.

Environment variables
---------------------
DNA_FRAGMENT_SIZE     Nucleotides per fragment   (default: 1000)
DNA_RS_NSYM           Reed-Solomon ECC symbols   (default: 32)
DNA_OUTPUT_DIR        Output directory            (default: data/dna_archive/)
LOG_DIR               Logging directory           (default: logs/)

Usage
-----
    python3 dna_encoder.py encode   --input message.txt --output encoded.dna
    python3 dna_encoder.py decode   --input encoded.dna --output decoded.txt
    python3 dna_encoder.py encode-file --input document.pdf --output archive.dna
    python3 dna_encoder.py decode-file --input archive.dna --output document.pdf
    python3 dna_encoder.py repository --output repo_archive/
    python3 dna_encoder.py stats    --input encoded.dna
"""

import argparse
import hashlib
import json
import logging
import os
import struct
import sys
from pathlib import Path
from typing import Optional

try:
    from reedsolo import RSCodec
except ImportError:
    sys.exit("reedsolo is required: pip install reedsolo")

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------

DNA_FRAGMENT_SIZE = int(os.environ.get("DNA_FRAGMENT_SIZE", "1000"))
DNA_RS_NSYM = int(os.environ.get("DNA_RS_NSYM", "32"))
DNA_OUTPUT_DIR = Path(os.environ.get("DNA_OUTPUT_DIR", "data/dna_archive"))
LOG_DIR = Path(os.environ.get("LOG_DIR", "logs"))

LOG_DIR.mkdir(parents=True, exist_ok=True)
DNA_OUTPUT_DIR.mkdir(parents=True, exist_ok=True)

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(name)s: %(message)s",
    handlers=[
        logging.StreamHandler(sys.stdout),
        logging.FileHandler(LOG_DIR / "dna_encoder.log", encoding="utf-8"),
    ],
)
logger = logging.getLogger("ierahkwa.dna")

# ---------------------------------------------------------------------------
# Nucleotide mapping (2 bits per nucleotide)
# ---------------------------------------------------------------------------

BITS_TO_NUC = {0b00: "A", 0b01: "C", 0b10: "G", 0b11: "T"}
NUC_TO_BITS = {"A": 0b00, "C": 0b01, "G": 0b10, "T": 0b11}

# Reed-Solomon codec
_rs_codec = RSCodec(DNA_RS_NSYM)

# ---------------------------------------------------------------------------
# Core encoding / decoding
# ---------------------------------------------------------------------------


def encode_to_dna(data: bytes) -> str:
    """Convert bytes to a DNA nucleotide sequence (ACGT).

    Each byte produces 4 nucleotides (2 bits per nucleotide).

    Args:
        data: Raw bytes to encode.

    Returns:
        String of A, C, G, T characters.
    """
    nucleotides = []
    for byte in data:
        # Extract 4 pairs of 2 bits, MSB first
        nucleotides.append(BITS_TO_NUC[(byte >> 6) & 0b11])
        nucleotides.append(BITS_TO_NUC[(byte >> 4) & 0b11])
        nucleotides.append(BITS_TO_NUC[(byte >> 2) & 0b11])
        nucleotides.append(BITS_TO_NUC[byte & 0b11])
    return "".join(nucleotides)


def decode_from_dna(sequence: str) -> bytes:
    """Convert a DNA nucleotide sequence back to bytes.

    Args:
        sequence: String of A, C, G, T characters.

    Returns:
        Decoded bytes.

    Raises:
        ValueError: If sequence length is not a multiple of 4 or
                    contains invalid characters.
    """
    # Strip whitespace and newlines
    sequence = sequence.strip().upper().replace("\n", "").replace(" ", "")

    if len(sequence) % 4 != 0:
        raise ValueError(
            f"Sequence length {len(sequence)} is not a multiple of 4. "
            f"Cannot decode to whole bytes."
        )

    result = bytearray()
    for i in range(0, len(sequence), 4):
        quad = sequence[i : i + 4]
        byte = 0
        for j, nuc in enumerate(quad):
            if nuc not in NUC_TO_BITS:
                raise ValueError(
                    f"Invalid nucleotide '{nuc}' at position {i + j}. "
                    f"Expected A, C, G, or T."
                )
            byte = (byte << 2) | NUC_TO_BITS[nuc]
        result.append(byte)
    return bytes(result)


# ---------------------------------------------------------------------------
# Error correction (Reed-Solomon)
# ---------------------------------------------------------------------------


def encode_with_ecc(data: bytes) -> bytes:
    """Encode data with Reed-Solomon error correction.

    Args:
        data: Raw bytes.

    Returns:
        Bytes with appended ECC symbols.
    """
    encoded = bytes(_rs_codec.encode(data))
    logger.debug(
        "ECC encoded: %d bytes -> %d bytes (+%d ECC symbols)",
        len(data),
        len(encoded),
        DNA_RS_NSYM,
    )
    return encoded


def decode_with_ecc(data: bytes) -> bytes:
    """Decode data with Reed-Solomon error correction.

    Args:
        data: Bytes with appended ECC symbols.

    Returns:
        Corrected original bytes.

    Raises:
        Exception: If too many errors to correct.
    """
    decoded = bytes(_rs_codec.decode(data)[0])
    logger.debug("ECC decoded: %d bytes -> %d bytes", len(data), len(decoded))
    return decoded


# ---------------------------------------------------------------------------
# File encoding / decoding
# ---------------------------------------------------------------------------

# File header format:
# - 4 bytes: magic number (0x444E4131 = "DNA1")
# - 4 bytes: original file size (uint32 big-endian)
# - 32 bytes: SHA-256 checksum
# - 2 bytes: filename length (uint16 big-endian)
# - N bytes: filename (UTF-8)
# - payload bytes: file content

FILE_MAGIC = b"\x44\x4e\x41\x31"  # "DNA1"
HEADER_FIXED_SIZE = 4 + 4 + 32 + 2  # 42 bytes


def encode_file(filepath: str, use_ecc: bool = True) -> str:
    """Encode an entire file with header into a DNA sequence.

    The header contains magic number, file size, SHA-256 checksum,
    and original filename for reconstruction.

    Args:
        filepath: Path to the file to encode.
        use_ecc: Whether to apply Reed-Solomon error correction.

    Returns:
        DNA nucleotide sequence string.
    """
    path = Path(filepath)
    if not path.is_file():
        raise FileNotFoundError(f"File not found: {filepath}")

    file_data = path.read_bytes()
    filename = path.name.encode("utf-8")

    checksum = hashlib.sha256(file_data).digest()

    # Build header
    header = bytearray()
    header.extend(FILE_MAGIC)
    header.extend(struct.pack(">I", len(file_data)))
    header.extend(checksum)
    header.extend(struct.pack(">H", len(filename)))
    header.extend(filename)

    payload = bytes(header) + file_data

    if use_ecc:
        payload = encode_with_ecc(payload)

    dna_sequence = encode_to_dna(payload)

    logger.info(
        "Encoded file '%s': %d bytes -> %d nucleotides (ECC: %s)",
        path.name,
        len(file_data),
        len(dna_sequence),
        "yes" if use_ecc else "no",
    )

    return dna_sequence


def decode_file(dna_sequence: str, output_path: str, use_ecc: bool = True) -> dict:
    """Reconstruct a file from its DNA sequence.

    Args:
        dna_sequence: DNA nucleotide sequence string.
        output_path: Where to write the decoded file.
        use_ecc: Whether Reed-Solomon was applied during encoding.

    Returns:
        Dict with filename, size, checksum, and verification status.
    """
    raw_bytes = decode_from_dna(dna_sequence)

    if use_ecc:
        raw_bytes = decode_with_ecc(raw_bytes)

    # Parse header
    if len(raw_bytes) < HEADER_FIXED_SIZE:
        raise ValueError("Data too short to contain a valid file header")

    magic = raw_bytes[:4]
    if magic != FILE_MAGIC:
        raise ValueError(
            f"Invalid magic number: {magic.hex()} (expected {FILE_MAGIC.hex()})"
        )

    file_size = struct.unpack(">I", raw_bytes[4:8])[0]
    stored_checksum = raw_bytes[8:40]
    filename_len = struct.unpack(">H", raw_bytes[40:42])[0]

    if len(raw_bytes) < HEADER_FIXED_SIZE + filename_len:
        raise ValueError("Data too short to contain filename")

    filename = raw_bytes[42 : 42 + filename_len].decode("utf-8")
    payload_start = 42 + filename_len
    file_data = raw_bytes[payload_start : payload_start + file_size]

    # Verify checksum
    computed_checksum = hashlib.sha256(file_data).digest()
    checksum_ok = computed_checksum == stored_checksum

    # Write output
    out = Path(output_path)
    out.parent.mkdir(parents=True, exist_ok=True)
    out.write_bytes(file_data)

    result = {
        "filename": filename,
        "size": len(file_data),
        "expected_size": file_size,
        "checksum_sha256": computed_checksum.hex(),
        "checksum_verified": checksum_ok,
    }

    if checksum_ok:
        logger.info("Decoded file '%s': %d bytes, checksum OK", filename, len(file_data))
    else:
        logger.warning(
            "Decoded file '%s': CHECKSUM MISMATCH (stored: %s, computed: %s)",
            filename,
            stored_checksum.hex(),
            computed_checksum.hex(),
        )

    return result


# ---------------------------------------------------------------------------
# Repository encoding
# ---------------------------------------------------------------------------

# Critical files to encode for long-term preservation
CRITICAL_FILES = [
    "01-documentos/legal/GOVERNANCE-CONSTITUTION.md",
    "01-documentos/legal/BILL-OF-DIGITAL-RIGHTS.md",
    "01-documentos/legal/DATA-SOVEREIGNTY-ACT.md",
    "01-documentos/legal/TOKENOMICS-WAMPUM.md",
    "01-documentos/legal/GENESIS-PROPOSAL-GP001.md",
    "01-documentos/inversores/WHITEPAPER-IERAHKWA.md",
    "01-documentos/DEVELOPER-BOUNTY-PROGRAM.md",
]


def encode_repository(base_dir: str = ".", output_dir: str = None) -> dict:
    """Encode all critical Ierahkwa repository files into DNA sequences.

    Each file is encoded separately with its own header and ECC.
    A manifest JSON file is also generated.

    Args:
        base_dir: Root directory of the repository.
        output_dir: Directory to write DNA files (default: DNA_OUTPUT_DIR).

    Returns:
        Dict with encoding results for each file.
    """
    out_dir = Path(output_dir) if output_dir else DNA_OUTPUT_DIR
    out_dir.mkdir(parents=True, exist_ok=True)

    base = Path(base_dir)
    results = {}
    manifest = {
        "version": "1.0",
        "encoder": "ierahkwa-dna-encoder",
        "encoding": "2bit-ACGT",
        "ecc": f"Reed-Solomon (nsym={DNA_RS_NSYM})",
        "files": [],
    }

    for rel_path in CRITICAL_FILES:
        filepath = base / rel_path
        if not filepath.is_file():
            logger.warning("Critical file not found, skipping: %s", filepath)
            results[rel_path] = {"status": "not_found"}
            continue

        try:
            dna_seq = encode_file(str(filepath), use_ecc=True)
            stats = compute_statistics(dna_seq)

            # Write DNA sequence to file
            dna_filename = filepath.stem + ".dna"
            dna_path = out_dir / dna_filename

            # Write in FASTA-like format
            with open(dna_path, "w") as f:
                f.write(f">IERAHKWA|{filepath.name}|{stats['length_nt']}nt\n")
                # Write in lines of 80 characters
                for i in range(0, len(dna_seq), 80):
                    f.write(dna_seq[i : i + 80] + "\n")

            results[rel_path] = {
                "status": "encoded",
                "output": str(dna_path),
                "nucleotides": stats["length_nt"],
                "gc_content": stats["gc_content_pct"],
                "max_homopolymer": stats["max_homopolymer"],
            }

            manifest["files"].append(
                {
                    "source": rel_path,
                    "dna_file": dna_filename,
                    "nucleotides": stats["length_nt"],
                    "bytes": stats["length_bytes"],
                    "checksum_sha256": hashlib.sha256(
                        filepath.read_bytes()
                    ).hexdigest(),
                }
            )

            logger.info(
                "Repository encode: %s -> %s (%d nt)",
                rel_path,
                dna_filename,
                stats["length_nt"],
            )

        except Exception as exc:
            logger.error("Failed to encode %s: %s", rel_path, exc)
            results[rel_path] = {"status": "error", "error": str(exc)}

    # Write manifest
    manifest_path = out_dir / "manifest.json"
    with open(manifest_path, "w") as f:
        json.dump(manifest, f, indent=2)
    logger.info("Repository manifest written to %s", manifest_path)

    return results


# ---------------------------------------------------------------------------
# Synthesis output formats
# ---------------------------------------------------------------------------


def synthesis_format(dna_sequence: str, name: str = "IERAHKWA",
                     provider: str = "twist") -> str:
    """Format a DNA sequence for ordering from a synthesis provider.

    Supported providers:
        - twist: Twist Bioscience (fragments up to 3,000 bp)
        - idt: Integrated DNA Technologies (gBlocks up to 3,000 bp)

    Long sequences are split into fragments with 20-nt overlaps
    for Gibson Assembly reconstruction.

    Args:
        dna_sequence: Full DNA sequence string.
        name: Sequence name / identifier.
        provider: Synthesis provider format.

    Returns:
        Formatted string ready for order submission.
    """
    overlap = 20  # nucleotides overlap between fragments
    frag_size = DNA_FRAGMENT_SIZE
    effective_size = frag_size - overlap

    fragments = []
    idx = 0
    frag_num = 1

    while idx < len(dna_sequence):
        end = min(idx + frag_size, len(dna_sequence))
        fragment = dna_sequence[idx:end]
        fragments.append((frag_num, fragment))
        idx += effective_size
        frag_num += 1

    output_lines = []

    if provider == "twist":
        output_lines.append(f"# Twist Bioscience Order — {name}")
        output_lines.append(f"# Total: {len(dna_sequence)} nt in {len(fragments)} fragments")
        output_lines.append(f"# Overlap: {overlap} nt (Gibson Assembly)")
        output_lines.append("")
        output_lines.append("Name,Sequence")
        for num, seq in fragments:
            frag_name = f"{name}_F{num:04d}"
            output_lines.append(f"{frag_name},{seq}")

    elif provider == "idt":
        output_lines.append(f"; IDT gBlocks Order — {name}")
        output_lines.append(f"; Total: {len(dna_sequence)} nt in {len(fragments)} fragments")
        output_lines.append(f"; Overlap: {overlap} nt (Gibson Assembly)")
        output_lines.append("")
        for num, seq in fragments:
            frag_name = f"{name}_F{num:04d}"
            output_lines.append(f">{frag_name}")
            for i in range(0, len(seq), 80):
                output_lines.append(seq[i : i + 80])
            output_lines.append("")

    else:
        raise ValueError(f"Unknown provider: {provider}. Use 'twist' or 'idt'.")

    logger.info(
        "Formatted for %s: %d fragments of ~%d nt each",
        provider,
        len(fragments),
        frag_size,
    )

    return "\n".join(output_lines)


# ---------------------------------------------------------------------------
# Statistics
# ---------------------------------------------------------------------------


def compute_statistics(dna_sequence: str) -> dict:
    """Compute statistics for a DNA sequence.

    Args:
        dna_sequence: String of A, C, G, T characters.

    Returns:
        Dict with length, GC content, homopolymer runs, etc.
    """
    seq = dna_sequence.strip().upper().replace("\n", "").replace(" ", "")
    length = len(seq)

    if length == 0:
        return {
            "length_nt": 0,
            "length_bytes": 0,
            "gc_content_pct": 0.0,
            "max_homopolymer": 0,
            "composition": {},
        }

    # Nucleotide composition
    composition = {nuc: seq.count(nuc) for nuc in "ACGT"}

    # GC content (ideal is ~50% for synthesis)
    gc_count = composition.get("G", 0) + composition.get("C", 0)
    gc_content = (gc_count / length) * 100

    # Maximum homopolymer run (long runs cause synthesis errors)
    max_run = 1
    current_run = 1
    for i in range(1, length):
        if seq[i] == seq[i - 1]:
            current_run += 1
            max_run = max(max_run, current_run)
        else:
            current_run = 1

    # Byte equivalent
    byte_length = length // 4

    stats = {
        "length_nt": length,
        "length_bytes": byte_length,
        "gc_content_pct": round(gc_content, 2),
        "max_homopolymer": max_run,
        "composition": composition,
        "composition_pct": {
            nuc: round((count / length) * 100, 2)
            for nuc, count in composition.items()
        },
        "synthesis_warnings": [],
    }

    # Warnings for synthesis
    if gc_content < 25 or gc_content > 75:
        stats["synthesis_warnings"].append(
            f"GC content {gc_content:.1f}% is outside ideal range (25-75%)"
        )
    if max_run > 6:
        stats["synthesis_warnings"].append(
            f"Homopolymer run of {max_run} detected (max recommended: 6)"
        )

    return stats


def print_statistics(stats: dict) -> None:
    """Print formatted statistics to stdout."""
    print("\n=== DNA Sequence Statistics ===")
    print(f"  Length:           {stats['length_nt']:,} nucleotides")
    print(f"  Bytes encoded:    {stats['length_bytes']:,} bytes")
    print(f"  GC content:       {stats['gc_content_pct']:.2f}%")
    print(f"  Max homopolymer:  {stats['max_homopolymer']}")
    print(f"  Composition:")
    for nuc in "ACGT":
        count = stats["composition"].get(nuc, 0)
        pct = stats["composition_pct"].get(nuc, 0)
        print(f"    {nuc}: {count:>10,} ({pct:.2f}%)")

    if stats.get("synthesis_warnings"):
        print(f"\n  Synthesis Warnings:")
        for warning in stats["synthesis_warnings"]:
            print(f"    ! {warning}")
    else:
        print(f"\n  Synthesis: No warnings (sequence looks good for ordering)")
    print()


# ---------------------------------------------------------------------------
# CLI
# ---------------------------------------------------------------------------


def build_parser() -> argparse.ArgumentParser:
    """Build the argument parser."""
    parser = argparse.ArgumentParser(
        prog="dna_encoder",
        description=(
            "Ierahkwa DNA Digital Storage Encoder -- "
            "Convert binary data to nucleotide sequences (A, C, G, T) "
            "for long-term archival in synthetic DNA."
        ),
    )

    subparsers = parser.add_subparsers(dest="command", help="Command to execute")

    # encode
    p_enc = subparsers.add_parser("encode", help="Encode raw bytes to DNA sequence")
    p_enc.add_argument("--input", "-i", required=True, help="Input file (raw data)")
    p_enc.add_argument("--output", "-o", required=True, help="Output file (.dna)")
    p_enc.add_argument("--no-ecc", action="store_true", help="Disable Reed-Solomon ECC")

    # decode
    p_dec = subparsers.add_parser("decode", help="Decode DNA sequence to raw bytes")
    p_dec.add_argument("--input", "-i", required=True, help="Input file (.dna)")
    p_dec.add_argument("--output", "-o", required=True, help="Output file (raw data)")
    p_dec.add_argument("--no-ecc", action="store_true", help="Disable Reed-Solomon ECC")

    # encode-file
    p_ef = subparsers.add_parser("encode-file", help="Encode a file with header metadata")
    p_ef.add_argument("--input", "-i", required=True, help="Input file to encode")
    p_ef.add_argument("--output", "-o", required=True, help="Output DNA file (.dna)")
    p_ef.add_argument("--no-ecc", action="store_true", help="Disable Reed-Solomon ECC")
    p_ef.add_argument(
        "--format",
        choices=["raw", "twist", "idt"],
        default="raw",
        help="Output format (default: raw)",
    )

    # decode-file
    p_df = subparsers.add_parser("decode-file", help="Reconstruct a file from DNA sequence")
    p_df.add_argument("--input", "-i", required=True, help="Input DNA file (.dna)")
    p_df.add_argument("--output", "-o", required=True, help="Output reconstructed file")
    p_df.add_argument("--no-ecc", action="store_true", help="Disable Reed-Solomon ECC")

    # repository
    p_repo = subparsers.add_parser(
        "repository", help="Encode all critical repository files"
    )
    p_repo.add_argument(
        "--base-dir", "-b", default=".", help="Repository root directory"
    )
    p_repo.add_argument(
        "--output", "-o", default=None, help="Output directory for DNA files"
    )

    # stats
    p_stats = subparsers.add_parser("stats", help="Show statistics for a DNA file")
    p_stats.add_argument("--input", "-i", required=True, help="Input DNA file (.dna)")

    return parser


def read_dna_file(filepath: str) -> str:
    """Read a DNA file, stripping FASTA headers and whitespace."""
    lines = Path(filepath).read_text().splitlines()
    sequence_lines = [
        line.strip() for line in lines if line.strip() and not line.startswith(">")
    ]
    return "".join(sequence_lines)


def main():
    """Entry point for the DNA encoder CLI."""
    parser = build_parser()
    args = parser.parse_args()

    if not args.command:
        parser.print_help()
        sys.exit(1)

    if args.command == "encode":
        data = Path(args.input).read_bytes()
        use_ecc = not args.no_ecc
        if use_ecc:
            data = encode_with_ecc(data)
        dna_seq = encode_to_dna(data)
        Path(args.output).write_text(dna_seq + "\n")
        stats = compute_statistics(dna_seq)
        logger.info("Encoded %d bytes -> %d nucleotides", len(data), stats["length_nt"])
        print_statistics(stats)

    elif args.command == "decode":
        dna_seq = read_dna_file(args.input)
        raw = decode_from_dna(dna_seq)
        use_ecc = not args.no_ecc
        if use_ecc:
            raw = decode_with_ecc(raw)
        Path(args.output).write_bytes(raw)
        logger.info("Decoded %d nucleotides -> %d bytes", len(dna_seq), len(raw))

    elif args.command == "encode-file":
        dna_seq = encode_file(args.input, use_ecc=not args.no_ecc)

        if args.format in ("twist", "idt"):
            name = Path(args.input).stem.upper().replace(" ", "_")[:20]
            formatted = synthesis_format(dna_seq, name=name, provider=args.format)
            Path(args.output).write_text(formatted + "\n")
        else:
            # Write FASTA-like format
            with open(args.output, "w") as f:
                f.write(f">IERAHKWA|{Path(args.input).name}\n")
                for i in range(0, len(dna_seq), 80):
                    f.write(dna_seq[i : i + 80] + "\n")

        stats = compute_statistics(dna_seq)
        print_statistics(stats)

    elif args.command == "decode-file":
        dna_seq = read_dna_file(args.input)
        result = decode_file(dna_seq, args.output, use_ecc=not args.no_ecc)
        print(f"\nDecoded file: {result['filename']}")
        print(f"  Size: {result['size']:,} bytes")
        print(f"  SHA-256: {result['checksum_sha256']}")
        print(f"  Checksum verified: {result['checksum_verified']}")

    elif args.command == "repository":
        results = encode_repository(args.base_dir, args.output)
        print("\n=== Repository Encoding Results ===")
        for path, info in results.items():
            status = info.get("status", "unknown")
            if status == "encoded":
                print(
                    f"  [OK]   {path} -> {info['nucleotides']:,} nt "
                    f"(GC: {info['gc_content']:.1f}%)"
                )
            elif status == "not_found":
                print(f"  [SKIP] {path} (file not found)")
            else:
                print(f"  [ERR]  {path} ({info.get('error', 'unknown error')})")

    elif args.command == "stats":
        dna_seq = read_dna_file(args.input)
        stats = compute_statistics(dna_seq)
        print_statistics(stats)

    else:
        parser.print_help()
        sys.exit(1)


if __name__ == "__main__":
    main()
