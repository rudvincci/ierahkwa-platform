#!/usr/bin/env python3
"""
remove-duplicates.py
====================
Elimina directorios duplicados (versiones en inglés de plataformas que ya
existen en español) y stubs de redirección del ecosistema Ierahkwa.

Fecha: 2026-02-28
"""

import os
import shutil
import sys

BASE = "/Users/ruddie/Desktop/files/Soberano-Organizado/02-plataformas-html"

# ── Directorios a eliminar ──────────────────────────────────────────────────
# Formato: (directorio_a_borrar, motivo)

DUPLICATES_TO_DELETE = [
    # --- sovereign-* duplicados en inglés (1-46) ---
    ("sovereign-agriculture",    "duplicado de agricultura-soberana"),
    ("sovereign-aviation",       "duplicado de aviacion-soberana"),
    ("sovereign-backend",        "duplicado de backend-soberano"),
    ("sovereign-collab",         "duplicado de colaboracion-soberana"),
    ("sovereign-commerce",       "duplicado de comercio-soberano"),
    ("sovereign-conference",     "duplicado de conferencia-soberana"),
    ("sovereign-education",      "duplicado de education-dashboard"),
    ("sovereign-energy",         "duplicado de energia-soberana"),
    ("sovereign-enterprise",     "duplicado de empresa-soberana"),
    ("sovereign-environment",    "duplicado de ambiente-soberano"),
    ("sovereign-fauna",          "duplicado de fauna-soberana"),
    ("sovereign-fishing",        "duplicado de pesca-soberana"),
    ("sovereign-forum",          "duplicado de foro-soberano"),
    ("sovereign-geology",        "duplicado de geologia-soberana"),
    ("sovereign-healthcare",     "duplicado de healthcare-dashboard"),
    ("sovereign-hosting",        "duplicado de hospedaje-soberano"),
    ("sovereign-housing",        "duplicado de vivienda-soberana"),
    ("sovereign-ide",            "duplicado de ide-soberano"),
    ("sovereign-insurance",      "duplicado de seguros-soberano"),
    ("sovereign-jobs",           "duplicado de trabajo-soberano"),
    ("sovereign-laws",           "duplicado de normas-soberana"),
    ("sovereign-library",        "duplicado de biblioteca-soberana"),
    ("sovereign-livestock",      "duplicado de ganaderia-soberana"),
    ("sovereign-logistics",      "duplicado de logistica-militar-soberana"),
    ("sovereign-maps",           "duplicado de mapa-soberano"),
    ("sovereign-maritime",       "duplicado de maritimo-soberano"),
    ("sovereign-marketing",      "duplicado de marketing-soberano"),
    ("sovereign-marketplace",    "duplicado de comercio-soberano"),
    ("sovereign-media",          "duplicado de media-soberana"),
    ("sovereign-music",          "duplicado de musica-soberana"),
    ("sovereign-news",           "duplicado de noticia-soberana"),
    ("sovereign-passport",       "duplicado de pasaporte-soberano"),
    ("sovereign-payments",       "duplicado de pagos-soberano"),
    ("sovereign-podcast",        "duplicado de streaming-estudio-soberano"),
    ("sovereign-radio",          "duplicado de radio-soberana"),
    ("sovereign-repo",           "duplicado de repositorio-soberano"),
    ("sovereign-shorts",         "duplicado de cortos-indigenas"),
    ("sovereign-social",         "duplicado de canal-soberano"),
    ("sovereign-sports",         "duplicado de deporte-soberano"),
    ("sovereign-transit",        "duplicado de transito-soberano"),
    ("sovereign-university",     "duplicado de universidad-soberana"),
    ("sovereign-voice",          "duplicado de voz-soberana"),
    ("sovereign-wallet",         "duplicado de bdet-wallet"),
    ("sovereign-waste",          "duplicado de residuos-soberano"),
    ("sovereign-water",          "duplicado de agua-soberana"),
    ("sovereign-weather",        "duplicado de meteorologia-soberana"),

    # --- Duplicados funcionales (47-64) ---
    ("central-bank",             "duplicado de banco-central-soberano"),
    ("cultural-heritage",        "duplicado de patrimonio-cultural-soberano"),
    ("diplomacy",                "duplicado de diplomacia-soberana"),
    ("immigration",              "duplicado de inmigracion-soberana"),
    ("digital-justice",          "duplicado de justicia-digital-soberana"),
    ("pension-fund",             "duplicado de pension-soberana"),
    ("digital-parliament",       "duplicado de parlamento-soberano"),
    ("digital-census",           "duplicado de censo-soberano"),
    ("digital-cadastre",         "duplicado de catastro-soberano"),
    ("digital-museum",           "duplicado de museo-soberano"),
    ("civil-registry",           "duplicado de registro-civil-soberano"),
    ("electoral-commission",     "duplicado de comision-electoral-soberana"),
    ("universal-translator",     "duplicado de traduccion-soberana + traductor-soberano"),
    ("dev-platform",             "duplicado de dev-soberano"),
    ("devops-pipeline",          "duplicado de devops-soberano"),
    ("bdet-bank-payment-system", "duplicado de bdet-bank + pagos-soberano"),
    ("emergency-911",            "duplicado de emergencias-soberano"),
    ("project-mgmt",             "duplicado de proyecto-soberano"),

    # --- Meta / redirect stubs (65-72) ---
    ("portal-central",           "stub de redirección"),
    ("portal-soberano",          "stub de redirección"),
    ("soberano-unificado",       "stub de redirección"),
    ("soberano-ecosystem",       "stub de redirección"),
    ("landing-ierahkwa",         "redirección"),
    ("landing-page",             "redirección"),
    ("infographic",              "página informativa, no es plataforma"),
    ("pitch-deck",               "cubierto por investor-audit-presentation"),
]


def main():
    print("=" * 70)
    print("  IERAHKWA — Eliminación de directorios duplicados")
    print(f"  Base: {BASE}")
    print(f"  Total a procesar: {len(DUPLICATES_TO_DELETE)}")
    print("=" * 70)
    print()

    deleted = 0
    skipped = 0
    not_found = 0

    for dirname, reason in DUPLICATES_TO_DELETE:
        full_path = os.path.join(BASE, dirname)

        if not os.path.exists(full_path):
            print(f"  [NO EXISTE]  {dirname}")
            not_found += 1
            continue

        if not os.path.isdir(full_path):
            print(f"  [OMITIDO]    {dirname}  (no es directorio)")
            skipped += 1
            continue

        try:
            shutil.rmtree(full_path)
            print(f"  [ELIMINADO]  {dirname}  <- {reason}")
            deleted += 1
        except Exception as e:
            print(f"  [ERROR]      {dirname}  -> {e}")
            skipped += 1

    # ── Resumen ──────────────────────────────────────────────────────────────
    print()
    print("-" * 70)
    print(f"  Eliminados:   {deleted}")
    print(f"  No existían:  {not_found}")
    print(f"  Omitidos:     {skipped}")
    print("-" * 70)

    # ── Contar directorios restantes ─────────────────────────────────────────
    remaining = sorted([
        d for d in os.listdir(BASE)
        if os.path.isdir(os.path.join(BASE, d))
        and not d.startswith(".")
        and d != "shared"
    ])

    print()
    print(f"  Directorios de plataforma restantes: {len(remaining)}")
    print()
    for i, d in enumerate(remaining, 1):
        print(f"    {i:3d}. {d}")

    print()
    print("=" * 70)
    print("  Limpieza completada.")
    print("=" * 70)


if __name__ == "__main__":
    main()
