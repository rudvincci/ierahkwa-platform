#!/usr/bin/env python3
"""Inyecta ierahkwa-api.js en TODAS las plataformas HTML.
Inserta el tag ANTES de ierahkwa-agents.js (o antes de </body> si no existe).
"""
import os
import glob

BASE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.dirname(BASE)

API_TAG = '<script src="../shared/ierahkwa-api.js"></script>'
AGENTS_TAG = '<script src="../shared/ierahkwa-agents.js"></script>'

# Buscar todos los index.html en subdirectorios directos
pattern = os.path.join(ROOT, '*', 'index.html')
html_files = sorted(glob.glob(pattern))

injected = 0
already = 0
failed = 0
failed_list = []

for html in html_files:
    dirname = os.path.basename(os.path.dirname(html))

    # Saltar el directorio shared
    if dirname == 'shared':
        continue

    try:
        with open(html, 'r', encoding='utf-8') as f:
            content = f.read()

        # Ya tiene ierahkwa-api.js?
        if 'ierahkwa-api.js' in content:
            already += 1
            continue

        # Inyectar ANTES de ierahkwa-agents.js si existe
        if AGENTS_TAG in content:
            content = content.replace(AGENTS_TAG, API_TAG + '\n' + AGENTS_TAG, 1)
            with open(html, 'w', encoding='utf-8') as f:
                f.write(content)
            injected += 1
        elif '</body>' in content:
            # Fallback: antes de </body>
            content = content.replace('</body>', API_TAG + '\n</body>', 1)
            with open(html, 'w', encoding='utf-8') as f:
                f.write(content)
            injected += 1
        else:
            failed += 1
            failed_list.append(f"  SIN </body>: {dirname}/index.html")

    except Exception as e:
        failed += 1
        failed_list.append(f"  ERROR: {dirname}/index.html -> {e}")

print()
print("=" * 50)
print("  RESULTADO: inject-api-sdk.py")
print("=" * 50)
print(f"  Inyectados:    {injected}")
print(f"  Ya lo tenian:  {already}")
print(f"  Fallidos:      {failed}")
print(f"  Total archivos: {injected + already + failed}")
print("=" * 50)

if failed_list:
    print("\nDetalles de fallos:")
    for msg in failed_list:
        print(msg)

print("\nListo.")
