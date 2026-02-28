#!/usr/bin/env python3
"""Inyecta ierahkwa-agents.js en TODAS las plataformas.
7 AI Agents: Guardian, Pattern, Anomaly, Trust, Shield, Forensic, Evolution
"""
import os

BASE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.dirname(BASE)
TAG = '<script src="../shared/ierahkwa-agents.js"></script>'
SKIP = {'shared', 'admin-dashboard', 'investor-audit-presentation', 'icons'}

injected = 0
already = 0
skipped = 0

for d in sorted(os.listdir(ROOT)):
    path = os.path.join(ROOT, d)
    if not os.path.isdir(path) or d in SKIP or d.startswith('.') or d.startswith('nexus-'):
        continue
    html = os.path.join(path, 'index.html')
    if not os.path.isfile(html):
        continue
    with open(html, 'r', encoding='utf-8') as f:
        content = f.read()
    # Already has it?
    if 'ierahkwa-agents.js' in content:
        already += 1
        continue
    # Determine the correct relative path prefix
    prefix = '../shared/'
    if '../shared/' in content:
        prefix = '../shared/'
    # Insert before </body>
    if '</body>' in content:
        content = content.replace('</body>', TAG + '\n</body>', 1)
        with open(html, 'w', encoding='utf-8') as f:
            f.write(content)
        injected += 1
    else:
        skipped += 1
        print(f"  SKIP (no </body>): {d}/index.html")

print(f"\n--- Resultado inject-agents.py ---")
print(f"  Inyectados:  {injected}")
print(f"  Ya tenian:   {already}")
print(f"  Saltados:    {skipped}")
print(f"  Total dirs:  {injected + already + skipped}")
