#!/usr/bin/env python3
"""
Ierahkwa Auto-Materialización Universal v11.0.0-PHANTOM

Verifica la arquitectura existente y crea SOLO los archivos que faltan.
NO duplica código: respeta la estructura 01-17, 08-dotnet, scripts, etc.
Ejecutar desde la raíz del repo: python3 scripts/materialize_ierahkwa.py
"""

import os
import stat
from pathlib import Path

# Estructura esperada (paths relativos al repo). Solo listamos lo que podria faltar.
EXPECTED_CORE = {
    "scripts/protocols/mediator.py": None,
    "scripts/protocols/ierahkwa_sentinel.py": None,
    "scripts/protocols/shamir_guardian.py": None,
    "scripts/protocols/codec2_voice_bridge.py": None,
    "scripts/protocols/peace_oracle.py": None,
    "scripts/protocols/lora_mesh_bridge.py": None,
    "scripts/protocols/satellite_uplink.py": None,
    "08-dotnet/microservices/DeFiSoberano/contracts/IerahkwaInheritance.sol": None,
    "08-dotnet/microservices/DeFiSoberano/contracts/IerahkwaPhantom.sol": None,
    "08-dotnet/microservices/DeFiSoberano/contracts/IerahkwaReputation.sol": None,
    "08-dotnet/microservices/DeFiSoberano/contracts/IerahkwaPulse.sol": None,
    "04-infraestructura/nginx/ghost_bridge.conf": None,
    "03-backend/conciencia-captcha/conciencia_captcha.js": None,
    "01-documentos/legal/GOVERNANCE-CRISIS.md": None,
    "16-docs/government/INDEX-DOCUMENTACION.md": None,
}

STUBS = {
    "survival_sync.sh": "#!/bin/bash\n# Ierahkwa v11.0 - Survival Sync\n# Sincroniza nodos Mesh/LoRa en modo offline\necho 'Survival sync...'\n",
    "setup_hns.sh": "#!/bin/bash\n# Handshake DNS setup - ver sovereign-dns/\necho 'HNS setup...'\n",
    "raspberry-pi-setup.sh": "#!/bin/bash\n# Hardware node setup - ver hardware-node/\necho 'Raspberry Pi setup...'\n",
}

def main():
    root = Path(".").resolve()
    if not (root / "08-dotnet").exists() and not (root / "scripts").exists():
        # Quizás estamos en scripts/
        root = root.parent
    os.chdir(root)

    created = []
    exists = []
    missing_dirs = []

    for path_str in EXPECTED_CORE:
        p = Path(path_str)
        if p.exists():
            exists.append(path_str)
        else:
            # Crear directorio padre y archivo stub si no existe
            p.parent.mkdir(parents=True, exist_ok=True)
            stub = STUBS.get(p.name, f"# Ierahkwa v11.0.0-PHANTOM - {p.name}\n# Sovereign Code - placeholder\n")
            if p.suffix == ".sh":
                stub = "#!/bin/bash\n" + stub
            p.write_text(stub, encoding="utf-8")
            if p.suffix in (".sh",):
                p.chmod(p.stat().st_mode | stat.S_IXUSR | stat.S_IXGRP | stat.S_IXOTH)
            created.append(path_str)

    # Crear stubs opcionales solo si faltan
    stub_locs = {
        "scripts/protocols/survival_sync.sh": STUBS["survival_sync.sh"],
        "sovereign-dns/setup_hns.sh": STUBS["setup_hns.sh"],
        "hardware-node/raspberry-pi-setup.sh": STUBS["raspberry-pi-setup.sh"],
    }
    for loc_str, content in stub_locs.items():
        loc = Path(loc_str)
        if not loc.exists():
            loc.parent.mkdir(parents=True, exist_ok=True)
            loc.write_text(content, encoding="utf-8")
            loc.chmod(loc.stat().st_mode | stat.S_IXUSR | stat.S_IXGRP | stat.S_IXOTH)
            created.append(loc_str)

    print("💎 Materializando Ierahkwa Ne Kanienke v11.0.0-PHANTOM...")
    print(f"   Raíz: {root}")
    print(f"   Existentes: {len(exists)}")
    if created:
        print(f"   Creados: {len(created)}")
        for c in created:
            print(f"      + {c}")
    else:
        print("   ✅ Estructura completa — nada que crear.")
    print("✅ Materia verificada.")

if __name__ == "__main__":
    main()
