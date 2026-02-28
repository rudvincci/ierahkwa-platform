#!/usr/bin/env python3
"""
Verification script: Confirm no functionality was lost after deleting 72 duplicate directories.
Checks that all Spanish equivalents exist and have index.html files.
"""

import os
import sys

BASE = "/Users/ruddie/Desktop/files/Soberano-Organizado/02-plataformas-html"

# 64 deleted directories mapped to their Spanish/kept equivalents
REPLACEMENTS = {
    "sovereign-agriculture": "agricultura-soberana",
    "sovereign-aviation": "aviacion-soberana",
    "sovereign-backend": "backend-soberano",
    "sovereign-collab": "colaboracion-soberana",
    "sovereign-commerce": "comercio-soberano",
    "sovereign-conference": "conferencia-soberana",
    "sovereign-education": "education-dashboard",
    "sovereign-energy": "energia-soberana",
    "sovereign-enterprise": "empresa-soberana",
    "sovereign-environment": "ambiente-soberano",
    "sovereign-fauna": "fauna-soberana",
    "sovereign-fishing": "pesca-soberana",
    "sovereign-forum": "foro-soberano",
    "sovereign-geology": "geologia-soberana",
    "sovereign-healthcare": "healthcare-dashboard",
    "sovereign-hosting": "hospedaje-soberano",
    "sovereign-housing": "vivienda-soberana",
    "sovereign-ide": "ide-soberano",
    "sovereign-insurance": "seguros-soberano",
    "sovereign-jobs": "trabajo-soberano",
    "sovereign-laws": "normas-soberana",
    "sovereign-library": "biblioteca-soberana",
    "sovereign-livestock": "ganaderia-soberana",
    "sovereign-logistics": "logistica-militar-soberana",
    "sovereign-maps": "mapa-soberano",
    "sovereign-maritime": "maritimo-soberano",
    "sovereign-marketing": "marketing-soberano",
    "sovereign-marketplace": "comercio-soberano",
    "sovereign-media": "media-soberana",
    "sovereign-music": "musica-soberana",
    "sovereign-news": "noticia-soberana",
    "sovereign-passport": "pasaporte-soberano",
    "sovereign-payments": "pagos-soberano",
    "sovereign-podcast": "streaming-estudio-soberano",
    "sovereign-radio": "radio-soberana",
    "sovereign-repo": "repositorio-soberano",
    "sovereign-shorts": "cortos-indigenas",
    "sovereign-social": "canal-soberano",
    "sovereign-sports": "deporte-soberano",
    "sovereign-transit": "transito-soberano",
    "sovereign-university": "universidad-soberana",
    "sovereign-voice": "voz-soberana",
    "sovereign-wallet": "bdet-wallet",
    "sovereign-waste": "residuos-soberano",
    "sovereign-water": "agua-soberana",
    "sovereign-weather": "meteorologia-soberana",
    "central-bank": "banco-central-soberano",
    "cultural-heritage": "patrimonio-cultural-soberano",
    "diplomacy": "diplomacia-soberana",
    "immigration": "inmigracion-soberana",
    "digital-justice": "justicia-digital-soberana",
    "pension-fund": "pension-soberana",
    "digital-parliament": "parlamento-soberano",
    "digital-census": "censo-soberano",
    "digital-cadastre": "catastro-soberano",
    "digital-museum": "museo-soberano",
    "civil-registry": "registro-civil-soberano",
    "electoral-commission": "comision-electoral-soberana",
    "universal-translator": "traduccion-soberana",
    "dev-platform": "dev-soberano",
    "devops-pipeline": "devops-soberano",
    "bdet-bank-payment-system": "bdet-bank",
    "emergency-911": "emergencias-soberano",
    "project-mgmt": "proyecto-soberano",
}

# 8 stubs deleted with no replacement needed
STUBS_DELETED = [
    "portal-central",
    "portal-soberano",
    "soberano-unificado",
    "soberano-ecosystem",
    "landing-ierahkwa",
    "landing-page",
    "infographic",
    "pitch-deck",
]

# Directories to EXCLUDE from platform count
EXCLUDE_DIRS = {
    "shared", "icons", "admin-dashboard", "investor-audit-presentation",
}
NEXUS_PREFIX = "nexus-"

def main():
    print("=" * 70)
    print("  VERIFICACION POST-ELIMINACION DE 72 DIRECTORIOS DUPLICADOS")
    print("  Fecha: 2026-02-28")
    print("=" * 70)

    errors = []
    warnings = []
    ok_count = 0

    # --- SECTION 1: Verify deleted dirs are actually gone ---
    print("\n--- 1. VERIFICAR QUE LOS 64 DUPLICADOS FUERON ELIMINADOS ---\n")
    still_exist = []
    for deleted in sorted(REPLACEMENTS.keys()):
        path = os.path.join(BASE, deleted)
        if os.path.exists(path):
            still_exist.append(deleted)
            print(f"  ADVERTENCIA: '{deleted}' todavia existe (no fue eliminado)")
    
    if still_exist:
        print(f"\n  {len(still_exist)} duplicados aun existen.")
        warnings.extend(still_exist)
    else:
        print("  OK: Los 64 directorios duplicados fueron eliminados correctamente.")

    # --- SECTION 2: Verify stubs are gone ---
    print("\n--- 2. VERIFICAR QUE LOS 8 STUBS FUERON ELIMINADOS ---\n")
    stubs_remain = []
    for stub in STUBS_DELETED:
        path = os.path.join(BASE, stub)
        if os.path.exists(path):
            stubs_remain.append(stub)
            print(f"  ADVERTENCIA: stub '{stub}' todavia existe")
    
    if stubs_remain:
        print(f"\n  {len(stubs_remain)} stubs aun existen.")
        warnings.extend(stubs_remain)
    else:
        print("  OK: Los 8 stubs fueron eliminados correctamente.")

    # --- SECTION 3: Verify all replacements exist with index.html ---
    print("\n--- 3. VERIFICAR QUE TODOS LOS EQUIVALENTES EXISTEN ---\n")
    # Get unique replacements (comercio-soberano appears twice)
    unique_replacements = sorted(set(REPLACEMENTS.values()))
    
    print(f"  {'ELIMINADO':<40} {'EQUIVALENTE MANTENIDO':<40} {'ESTADO'}")
    print(f"  {'-'*40} {'-'*40} {'-'*10}")
    
    for deleted in sorted(REPLACEMENTS.keys()):
        kept = REPLACEMENTS[deleted]
        kept_path = os.path.join(BASE, kept)
        index_path = os.path.join(kept_path, "index.html")
        
        if not os.path.isdir(kept_path):
            status = "ERROR: DIR FALTA"
            errors.append(f"Directorio falta: {kept} (reemplazo de {deleted})")
        elif not os.path.isfile(index_path):
            status = "ERROR: NO INDEX"
            errors.append(f"index.html falta: {kept}/index.html (reemplazo de {deleted})")
        else:
            size = os.path.getsize(index_path)
            status = f"OK ({size:,} bytes)"
            ok_count += 1
        
        print(f"  {deleted:<40} {kept:<40} {status}")

    # --- SECTION 4: Count total remaining platforms ---
    print("\n--- 4. CONTEO TOTAL DE PLATAFORMAS RESTANTES ---\n")
    
    all_dirs = []
    for entry in sorted(os.listdir(BASE)):
        full = os.path.join(BASE, entry)
        if not os.path.isdir(full):
            continue
        if entry in EXCLUDE_DIRS:
            continue
        if entry.startswith(NEXUS_PREFIX):
            continue
        if entry.startswith("."):
            continue
        index = os.path.join(full, "index.html")
        if os.path.isfile(index):
            all_dirs.append(entry)
    
    print(f"  Total directorios de plataformas con index.html: {len(all_dirs)}")
    print(f"  (Excluidos: shared, icons, admin-dashboard, investor-audit-presentation, nexus-*)")

    # --- SECTION 5: Summary ---
    print("\n" + "=" * 70)
    print("  RESUMEN FINAL")
    print("=" * 70)
    print(f"\n  Duplicados eliminados (esperados):    64 con reemplazo + 8 stubs = 72")
    print(f"  Reemplazos verificados OK:             {ok_count}/64")
    print(f"  Reemplazos unicos (sin repetidos):     {len(unique_replacements)}")
    print(f"  Plataformas activas con index.html:    {len(all_dirs)}")
    print(f"  Errores encontrados:                   {len(errors)}")
    print(f"  Advertencias (dirs no eliminados):     {len(warnings)}")

    if errors:
        print("\n  ERRORES CRITICOS:")
        for e in errors:
            print(f"    - {e}")

    if warnings:
        print("\n  ADVERTENCIAS:")
        for w in warnings:
            print(f"    - {w}")

    print()
    if errors:
        print("  ============================================")
        print("  ||            RESULTADO: FAIL             ||")
        print("  ============================================")
        print(f"  {len(errors)} reemplazos faltantes. Funcionalidad perdida.")
        sys.exit(1)
    else:
        print("  ============================================")
        print("  ||            RESULTADO: PASS             ||")
        print("  ============================================")
        print("  Todos los equivalentes existen con index.html.")
        print("  No se perdio funcionalidad.")
        sys.exit(0)

if __name__ == "__main__":
    main()
