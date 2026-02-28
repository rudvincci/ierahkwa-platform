#!/usr/bin/env python3
"""Inyecta ierahkwa-interconnect.js en TODAS las plataformas."""
import os

BASE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.dirname(BASE)
TAG = '<script src="../shared/ierahkwa-interconnect.js"></script>'
SKIP = {'shared', 'admin-dashboard', 'investor-audit-presentation'}
count = already = skipped = 0

for d in sorted(os.listdir(ROOT)):
    path = os.path.join(ROOT, d)
    if not os.path.isdir(path) or d in SKIP or d.startswith('.'):
        continue
    html = os.path.join(path, 'index.html')
    if not os.path.isfile(html):
        continue
    with open(html, 'r', encoding='utf-8') as f:
        content = f.read()
    if 'ierahkwa-interconnect.js' in content:
        already += 1
        continue
    if 'ierahkwa-protocols.js' in content:
        content = content.replace(
            '<script src="../shared/ierahkwa-protocols.js"></script>',
            '<script src="../shared/ierahkwa-protocols.js"></script>\n' + TAG
        )
    elif 'ierahkwa-quantum.js' in content:
        content = content.replace(
            '<script src="../shared/ierahkwa-quantum.js"></script>',
            '<script src="../shared/ierahkwa-quantum.js"></script>\n' + TAG
        )
    elif '</body>' in content:
        content = content.replace('</body>', TAG + '\n</body>')
    else:
        skipped += 1
        continue
    with open(html, 'w', encoding='utf-8') as f:
        f.write(content)
    count += 1

print(f"✅ Inyectados: {count}")
print(f"⏭️  Ya tenían: {already}")
print(f"⚠️  Sin punto: {skipped}")
