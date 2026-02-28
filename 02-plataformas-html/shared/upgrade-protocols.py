#!/usr/bin/env python3
"""Inyecta ierahkwa-protocols.js en TODAS las plataformas Ierahkwa."""
import os, glob

BASE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.dirname(BASE)
SCRIPT_TAG = '<script src="../shared/ierahkwa-protocols.js"></script>'
SKIP = {'shared','admin-dashboard','investor-audit-presentation'}
count = 0
skipped = 0
already = 0

for d in sorted(os.listdir(ROOT)):
    path = os.path.join(ROOT, d)
    if not os.path.isdir(path) or d in SKIP or d.startswith('.'):
        continue
    html = os.path.join(path, 'index.html')
    if not os.path.isfile(html):
        continue
    with open(html, 'r', encoding='utf-8') as f:
        content = f.read()

    # Skip if already has protocols.js
    if 'ierahkwa-protocols.js' in content:
        already += 1
        continue

    # Strategy 1: Insert after quantum.js line
    if 'ierahkwa-quantum.js' in content:
        content = content.replace(
            '<script src="../shared/ierahkwa-quantum.js"></script>',
            '<script src="../shared/ierahkwa-quantum.js"></script>\n' + SCRIPT_TAG
        )
    # Strategy 2: Insert after security.js line
    elif 'ierahkwa-security.js' in content:
        content = content.replace(
            '<script src="../shared/ierahkwa-security.js"></script>',
            '<script src="../shared/ierahkwa-security.js"></script>\n' + SCRIPT_TAG
        )
    # Strategy 3: Insert after ierahkwa.js line
    elif 'ierahkwa.js' in content:
        content = content.replace(
            '<script src="../shared/ierahkwa.js"></script>',
            '<script src="../shared/ierahkwa.js"></script>\n' + SCRIPT_TAG
        )
    # Strategy 4: Insert before </body>
    elif '</body>' in content:
        content = content.replace('</body>', SCRIPT_TAG + '\n</body>')
    else:
        skipped += 1
        print(f"  SKIP: {d} (no injection point)")
        continue

    with open(html, 'w', encoding='utf-8') as f:
        f.write(content)
    count += 1

print(f"\n✅ Inyectados: {count}")
print(f"⏭️  Ya tenían: {already}")
print(f"⚠️  Sin punto: {skipped}")
print(f"📊 Total procesados: {count + already + skipped}")
