#!/usr/bin/env python3
"""
============================================================================
STEGANOGRAPHY BRIDGE — Sovereign Camouflage Communications
Least Significant Bit (LSB) encoding for hiding encrypted messages
in ordinary image files (PNG/BMP).

Usage:
    python steganography_bridge.py hide <image> <message> <output>
    python steganography_bridge.py reveal <image>
    python steganography_bridge.py hide-file <image> <file> <output>
    python steganography_bridge.py reveal-file <image> <output_file>

No external dependencies beyond Pillow (PIL).
Ierahkwa Ne Kanienke — Sovereign Digital Nation
============================================================================
"""

import sys
import os
import hashlib
import struct
import hmac
from pathlib import Path

try:
    from PIL import Image
except ImportError:
    print("ERROR: Pillow is required. Install with: pip install Pillow")
    sys.exit(1)


# ── Constants ─────────────────────────────────────────────────

MAGIC_HEADER = b"IRHK"  # Ierahkwa magic bytes
VERSION = 1
MAX_MESSAGE_SIZE = 10 * 1024 * 1024  # 10 MB


# ── LSB Encoding Core ─────────────────────────────────────────

def _bits_from_bytes(data: bytes):
    """Yield individual bits from a byte sequence."""
    for byte in data:
        for i in range(7, -1, -1):
            yield (byte >> i) & 1


def _bytes_from_bits(bits):
    """Convert a list of bits back to bytes."""
    result = bytearray()
    for i in range(0, len(bits), 8):
        byte = 0
        for j in range(8):
            if i + j < len(bits):
                byte = (byte << 1) | bits[i + j]
            else:
                byte = byte << 1
        result.append(byte)
    return bytes(result)


def calculate_capacity(image_path: str) -> int:
    """Calculate how many bytes can be hidden in an image."""
    img = Image.open(image_path)
    width, height = img.size
    channels = len(img.getbands())
    total_bits = width * height * channels
    # Subtract header overhead (magic + version + length + checksum = 4+1+4+32 = 41 bytes)
    usable_bytes = (total_bits // 8) - 41
    return max(0, usable_bytes)


def hide_message(image_path: str, message: str, output_path: str) -> dict:
    """
    Hide a text message inside an image using LSB encoding.

    Args:
        image_path: Path to carrier image (PNG or BMP recommended)
        message: Text message to hide
        output_path: Where to save the stego image

    Returns:
        dict with stats about the operation
    """
    data = message.encode("utf-8")
    return _hide_data(image_path, data, output_path)


def hide_file(image_path: str, file_path: str, output_path: str) -> dict:
    """Hide an entire file inside an image."""
    with open(file_path, "rb") as f:
        data = f.read()
    return _hide_data(image_path, data, output_path)


def _hide_data(image_path: str, data: bytes, output_path: str) -> dict:
    """Core LSB hiding logic."""
    if len(data) > MAX_MESSAGE_SIZE:
        raise ValueError(f"Data too large: {len(data)} bytes (max {MAX_MESSAGE_SIZE})")

    img = Image.open(image_path).convert("RGB")
    pixels = list(img.getdata())
    width, height = img.size

    # Build payload: MAGIC(4) + VERSION(1) + LENGTH(4) + DATA + SHA256(32)
    checksum = hashlib.sha256(data).digest()
    payload = MAGIC_HEADER + struct.pack(">BI", VERSION, len(data)) + data + checksum

    total_bits_available = len(pixels) * 3  # RGB = 3 channels
    bits_needed = len(payload) * 8

    if bits_needed > total_bits_available:
        raise ValueError(
            f"Image too small: needs {bits_needed} bits, has {total_bits_available}. "
            f"Max data size for this image: {total_bits_available // 8 - 41} bytes"
        )

    # Encode payload into pixel LSBs
    bits = list(_bits_from_bytes(payload))
    bit_index = 0
    new_pixels = []

    for pixel in pixels:
        new_pixel = list(pixel)
        for channel in range(3):
            if bit_index < len(bits):
                # Replace LSB
                new_pixel[channel] = (new_pixel[channel] & 0xFE) | bits[bit_index]
                bit_index += 1
        new_pixels.append(tuple(new_pixel))

    # Create output image
    stego_img = Image.new("RGB", (width, height))
    stego_img.putdata(new_pixels)
    stego_img.save(output_path, "PNG")

    return {
        "success": True,
        "input_image": image_path,
        "output_image": output_path,
        "data_size": len(data),
        "payload_size": len(payload),
        "bits_used": bits_needed,
        "bits_available": total_bits_available,
        "utilization_pct": round((bits_needed / total_bits_available) * 100, 2),
        "checksum": checksum.hex()
    }


def reveal_message(image_path: str) -> str:
    """
    Extract a hidden text message from a stego image.

    Args:
        image_path: Path to stego image

    Returns:
        The hidden message as a string
    """
    data = _reveal_data(image_path)
    return data.decode("utf-8")


def reveal_file(image_path: str, output_path: str) -> dict:
    """Extract a hidden file from a stego image."""
    data = _reveal_data(image_path)
    with open(output_path, "wb") as f:
        f.write(data)
    return {"success": True, "output": output_path, "size": len(data)}


def _reveal_data(image_path: str) -> bytes:
    """Core LSB extraction logic."""
    img = Image.open(image_path).convert("RGB")
    pixels = list(img.getdata())

    # Extract all LSBs
    all_bits = []
    for pixel in pixels:
        for channel in range(3):
            all_bits.append(pixel[channel] & 1)

    all_bytes = _bytes_from_bits(all_bits)

    # Parse header
    if all_bytes[:4] != MAGIC_HEADER:
        raise ValueError("No hidden message found (invalid magic header)")

    version = all_bytes[4]
    if version != VERSION:
        raise ValueError(f"Unsupported version: {version}")

    data_length = struct.unpack(">I", all_bytes[5:9])[0]

    if data_length > MAX_MESSAGE_SIZE:
        raise ValueError(f"Corrupt data: reported size {data_length} exceeds maximum")

    data = all_bytes[9 : 9 + data_length]
    stored_checksum = all_bytes[9 + data_length : 9 + data_length + 32]

    # Verify integrity
    computed_checksum = hashlib.sha256(data).digest()
    if computed_checksum != stored_checksum:
        raise ValueError("Data integrity check failed (checksum mismatch)")

    return data


# ── CLI Interface ─────────────────────────────────────────────

def main():
    if len(sys.argv) < 2:
        print(__doc__)
        sys.exit(0)

    command = sys.argv[1].lower()

    if command == "hide" and len(sys.argv) == 5:
        image_path, message, output_path = sys.argv[2], sys.argv[3], sys.argv[4]
        result = hide_message(image_path, message, output_path)
        print(f"Message hidden successfully.")
        print(f"  Data size: {result['data_size']} bytes")
        print(f"  Utilization: {result['utilization_pct']}%")
        print(f"  Checksum: {result['checksum'][:16]}...")
        print(f"  Output: {result['output_image']}")

    elif command == "reveal" and len(sys.argv) == 3:
        image_path = sys.argv[2]
        message = reveal_message(image_path)
        print(f"Hidden message ({len(message)} chars):")
        print(message)

    elif command == "hide-file" and len(sys.argv) == 5:
        image_path, file_path, output_path = sys.argv[2], sys.argv[3], sys.argv[4]
        result = hide_file(image_path, file_path, output_path)
        print(f"File hidden successfully.")
        print(f"  Data size: {result['data_size']} bytes")
        print(f"  Utilization: {result['utilization_pct']}%")

    elif command == "reveal-file" and len(sys.argv) == 4:
        image_path, output_path = sys.argv[2], sys.argv[3]
        result = reveal_file(image_path, output_path)
        print(f"File extracted: {result['output']} ({result['size']} bytes)")

    elif command == "capacity" and len(sys.argv) == 3:
        image_path = sys.argv[2]
        cap = calculate_capacity(image_path)
        print(f"Image capacity: {cap} bytes ({cap / 1024:.1f} KB)")

    else:
        print(__doc__)
        sys.exit(1)


if __name__ == "__main__":
    main()
