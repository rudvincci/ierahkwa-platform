#!/usr/bin/env python3
"""
Ierahkwa Energy Monitor — Gestión de Autonomía Solar para Nodos Soberanos

Monitorea el nivel de batería (LiFePO4) y la carga solar para gestionar
el consumo del nodo. Apaga servicios no críticos si la energía baja del
umbral configurado y los reinicia cuando hay excedente.

Integrado con ierahkwa_sentinel.py para reportar estado energético.

Fuentes de datos soportadas:
  1. Victron SmartSolar via VE.Direct (serial USB)
  2. INA219/INA226 sensor via I2C (Raspberry Pi)
  3. /sys/class/power_supply/ (Linux battery sysfs)
  4. Fallback: psutil (laptops)

Environment variables
---------------------
ENERGY_SOURCE           Data source: victron|ina219|sysfs|psutil  (default: sysfs)
ENERGY_SERIAL_PORT      Victron VE.Direct port   (default: /dev/ttyUSB2)
ENERGY_I2C_BUS          I2C bus number           (default: 1)
ENERGY_I2C_ADDR         INA219 I2C address hex   (default: 0x40)
ENERGY_LOW_PCT          Low threshold percent    (default: 20)
ENERGY_CRITICAL_PCT     Critical threshold       (default: 10)
ENERGY_HIGH_PCT         High threshold (restart)  (default: 60)
ENERGY_CHECK_INTERVAL   Seconds between checks   (default: 60)
LOG_DIR                 Logging directory         (default: logs/)
"""

import json
import logging
import os
import subprocess
import sys
import time
from datetime import datetime, timezone
from pathlib import Path

LOG_DIR = Path(os.environ.get("LOG_DIR", "logs"))
LOG_DIR.mkdir(parents=True, exist_ok=True)

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(name)s: %(message)s",
    handlers=[
        logging.StreamHandler(sys.stdout),
        logging.FileHandler(LOG_DIR / "energy_monitor.log", encoding="utf-8"),
    ],
)
logger = logging.getLogger("ierahkwa.energy")

ENERGY_SOURCE = os.environ.get("ENERGY_SOURCE", "sysfs")
SERIAL_PORT = os.environ.get("ENERGY_SERIAL_PORT", "/dev/ttyUSB2")
I2C_BUS = int(os.environ.get("ENERGY_I2C_BUS", "1"))
I2C_ADDR = int(os.environ.get("ENERGY_I2C_ADDR", "0x40"), 16)
LOW_PCT = int(os.environ.get("ENERGY_LOW_PCT", "20"))
CRITICAL_PCT = int(os.environ.get("ENERGY_CRITICAL_PCT", "10"))
HIGH_PCT = int(os.environ.get("ENERGY_HIGH_PCT", "60"))
CHECK_INTERVAL = int(os.environ.get("ENERGY_CHECK_INTERVAL", "60"))

HEAVY_CONTAINERS = ["ollama", "bdet-bank", "grafana", "prometheus"]
CRITICAL_CONTAINERS = ["synapse", "tor-proxy", "sovereign-core", "postgres", "redis"]


def _docker_cmd(action: str, containers: list[str]) -> None:
    for c in containers:
        try:
            result = subprocess.run(
                ["docker", action, c],
                capture_output=True, text=True, timeout=15,
            )
            if result.returncode == 0:
                logger.info("docker %s %s OK", action, c)
            else:
                logger.warning("docker %s %s failed: %s", action, c, result.stderr.strip())
        except FileNotFoundError:
            logger.warning("docker not found")
            return
        except subprocess.TimeoutExpired:
            logger.warning("docker %s %s timed out", action, c)


def read_sysfs() -> dict:
    """Read battery from Linux sysfs (works on laptops and some SBCs)."""
    base = Path("/sys/class/power_supply")
    for supply in sorted(base.iterdir()) if base.exists() else []:
        cap_file = supply / "capacity"
        status_file = supply / "status"
        if cap_file.exists():
            capacity = int(cap_file.read_text().strip())
            status = status_file.read_text().strip() if status_file.exists() else "Unknown"
            return {"source": "sysfs", "battery_pct": capacity, "charging": status == "Charging",
                    "device": supply.name}
    return {"source": "sysfs", "battery_pct": -1, "charging": False, "device": "none"}


def read_psutil() -> dict:
    """Fallback using psutil for laptops."""
    try:
        import psutil
        bat = psutil.sensors_battery()
        if bat is None:
            return {"source": "psutil", "battery_pct": -1, "charging": False}
        return {"source": "psutil", "battery_pct": int(bat.percent), "charging": bat.power_plugged}
    except ImportError:
        return {"source": "psutil", "battery_pct": -1, "charging": False}


def read_victron() -> dict:
    """Read Victron SmartSolar via VE.Direct serial protocol."""
    try:
        import serial
    except ImportError:
        logger.warning("pyserial not installed; cannot read Victron")
        return {"source": "victron", "battery_pct": -1, "charging": False}
    try:
        ser = serial.Serial(SERIAL_PORT, 19200, timeout=5)
        data = {}
        buf = b""
        deadline = time.time() + 10
        while time.time() < deadline:
            buf += ser.read(ser.in_waiting or 1)
            while b"\r\n" in buf:
                line, buf = buf.split(b"\r\n", 1)
                line = line.decode("ascii", errors="ignore").strip()
                if "\t" in line:
                    key, val = line.split("\t", 1)
                    data[key] = val
            if "SOC" in data:
                break
        ser.close()
        soc = int(data.get("SOC", "-10")) // 10  # VE.Direct SOC is in 0.1%
        charging = data.get("CS", "0") != "0"
        voltage = int(data.get("V", "0")) / 1000.0
        current = int(data.get("I", "0")) / 1000.0
        return {"source": "victron", "battery_pct": soc, "charging": charging,
                "voltage_v": voltage, "current_a": current, "raw": data}
    except Exception as exc:
        logger.error("Victron read failed: %s", exc)
        return {"source": "victron", "battery_pct": -1, "charging": False}


def read_ina219() -> dict:
    """Read INA219/INA226 current/voltage sensor via I2C."""
    try:
        from smbus2 import SMBus
    except ImportError:
        logger.warning("smbus2 not installed; cannot read INA219")
        return {"source": "ina219", "battery_pct": -1, "charging": False}
    try:
        bus = SMBus(I2C_BUS)
        raw_v = bus.read_word_data(I2C_ADDR, 0x02)
        raw_v = ((raw_v & 0xFF) << 8) | ((raw_v >> 8) & 0xFF)
        voltage = (raw_v >> 3) * 0.004
        bus.close()
        # Estimate SOC from voltage (LiFePO4 12V: 10V=0%, 13.6V=100%)
        soc = max(0, min(100, int((voltage - 10.0) / 3.6 * 100)))
        return {"source": "ina219", "battery_pct": soc, "charging": voltage > 13.2,
                "voltage_v": voltage}
    except Exception as exc:
        logger.error("INA219 read failed: %s", exc)
        return {"source": "ina219", "battery_pct": -1, "charging": False}


READERS = {
    "sysfs": read_sysfs,
    "psutil": read_psutil,
    "victron": read_victron,
    "ina219": read_ina219,
}

_heavy_stopped = False


def manage_energy(reading: dict) -> str:
    """Apply energy policy. Returns mode: normal|saving|critical|unknown."""
    global _heavy_stopped
    pct = reading.get("battery_pct", -1)
    if pct < 0:
        return "unknown"
    if pct <= CRITICAL_PCT:
        logger.warning("CRITICAL (%d%%) — stopping all non-essential containers", pct)
        _docker_cmd("stop", HEAVY_CONTAINERS)
        _heavy_stopped = True
        return "critical"
    if pct <= LOW_PCT:
        logger.info("LOW (%d%%) — stopping heavy containers", pct)
        _docker_cmd("stop", HEAVY_CONTAINERS)
        _heavy_stopped = True
        return "saving"
    if pct >= HIGH_PCT and _heavy_stopped:
        logger.info("HIGH (%d%%) — restarting heavy containers", pct)
        _docker_cmd("start", HEAVY_CONTAINERS)
        _heavy_stopped = False
    return "normal"


def log_reading(reading: dict, mode: str) -> None:
    record = {
        "timestamp": datetime.now(timezone.utc).isoformat(),
        "mode": mode,
        **reading,
    }
    log_path = LOG_DIR / "energy_readings.jsonl"
    with open(log_path, "a", encoding="utf-8") as f:
        f.write(json.dumps(record) + "\n")


def main():
    reader = READERS.get(ENERGY_SOURCE, read_sysfs)
    logger.info("Energy monitor started — source=%s low=%d%% critical=%d%% high=%d%%",
                ENERGY_SOURCE, LOW_PCT, CRITICAL_PCT, HIGH_PCT)
    while True:
        reading = reader()
        mode = manage_energy(reading)
        pct = reading.get("battery_pct", -1)
        charging = reading.get("charging", False)
        logger.info("Battery: %d%% | Charging: %s | Mode: %s", pct, charging, mode)
        log_reading(reading, mode)
        time.sleep(CHECK_INTERVAL)


if __name__ == "__main__":
    main()
