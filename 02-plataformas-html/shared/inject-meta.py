#!/usr/bin/env python3
"""
Ierahkwa Platform - Meta Tags + SEO Injection Script
Injects: description, theme-color, canonical, manifest, icons, OG, Twitter Card
into ALL platform HTML files that don't already have them.
"""
import os, re, sys

BASE = "/Users/ruddie/Desktop/files/Soberano-Organizado/02-plataformas-html"
DOMAIN = "https://ierahkwa.nation"
SKIP = {"shared", "icons", "screenshots", "admin-dashboard", ".git"}

modified = 0
skipped = 0

def get_pretty_name(dirname):
    name = dirname
    for suffix in ["-soberana", "-soberano", "-soberan"]:
        name = name.replace(suffix, "")
    name = name.replace("-", " ").title()
    return name

def get_accent(content):
    m = re.search(r'--accent:\s*([#\w]+)', content)
    return m.group(1) if m else "#00FF41"

def get_title(content):
    m = re.search(r'<title>([^<]+)</title>', content)
    return m.group(1) if m else None

def build_meta_block(dirname, title, description, accent):
    slug = dirname
    lines = []
    lines.append(f'<meta name="description" content="{description}">')
    lines.append(f'<meta name="theme-color" content="{accent}">')
    lines.append(f'<link rel="canonical" href="{DOMAIN}/{slug}/">')
    lines.append(f'<link rel="manifest" href="../shared/manifest.json">')
    lines.append(f'<link rel="icon" href="../icons/icon-96.svg" type="image/svg+xml">')
    lines.append(f'<link rel="apple-touch-icon" href="../icons/icon-192.svg">')
    lines.append(f'<meta property="og:title" content="{title}">')
    lines.append(f'<meta property="og:description" content="{description}">')
    lines.append(f'<meta property="og:type" content="website">')
    lines.append(f'<meta property="og:url" content="{DOMAIN}/{slug}/">')
    lines.append(f'<meta property="og:image" content="{DOMAIN}/icons/icon-512.svg">')
    lines.append(f'<meta name="twitter:card" content="summary">')
    lines.append(f'<meta name="twitter:title" content="{title}">')
    lines.append(f'<meta name="twitter:description" content="{description}">')
    return "\n".join(lines)

def process_file(filepath, dirname, is_main=False):
    global modified, skipped

    with open(filepath, "r", encoding="utf-8") as f:
        content = f.read()

    # Skip small files (redirect stubs)
    if len(content) < 500:
        skipped += 1
        return

    # Check if already has meta description (already processed)
    if 'meta name="description"' in content:
        return

    title = get_title(content) or f"Ierahkwa - {dirname}"
    accent = get_accent(content)
    pretty = get_pretty_name(dirname)

    if is_main:
        description = "Portal Central de Ierahkwa Ne Kanienke - 190 plataformas soberanas, 15 dominios NEXUS, infraestructura digital para 574 naciones tribales"
        meta_block = []
        meta_block.append(f'<meta name="description" content="{description}">')
        meta_block.append(f'<meta name="theme-color" content="#00FF41">')
        meta_block.append(f'<link rel="canonical" href="{DOMAIN}/">')
        meta_block.append(f'<link rel="manifest" href="shared/manifest.json">')
        meta_block.append(f'<link rel="icon" href="icons/icon-96.svg" type="image/svg+xml">')
        meta_block.append(f'<link rel="apple-touch-icon" href="icons/icon-192.svg">')
        meta_block.append(f'<meta property="og:title" content="Ierahkwa Ne Kanienke - Portal Central">')
        meta_block.append(f'<meta property="og:description" content="{description}">')
        meta_block.append(f'<meta property="og:type" content="website">')
        meta_block.append(f'<meta property="og:url" content="{DOMAIN}/">')
        meta_block.append(f'<meta property="og:image" content="{DOMAIN}/icons/icon-512.svg">')
        meta_block.append(f'<meta name="twitter:card" content="summary_large_image">')
        meta_block.append(f'<meta name="twitter:title" content="Ierahkwa Ne Kanienke">')
        meta_block.append(f'<meta name="twitter:description" content="{description}">')
        inject = "\n".join(meta_block)
    else:
        description = f"Plataforma soberana de {pretty} para la infraestructura digital de 574 naciones tribales - Ierahkwa Ne Kanienke"
        inject = build_meta_block(dirname, title, description, accent)

    # Insert after the viewport meta tag line
    viewport_pattern = re.compile(r'(<meta\s+name="viewport"[^>]*>)')
    if viewport_pattern.search(content):
        content = viewport_pattern.sub(r'\1\n' + inject, content, count=1)
    else:
        # Fallback: insert after <meta charset>
        charset_pattern = re.compile(r'(<meta\s+charset="UTF-8"[^>]*>)', re.IGNORECASE)
        content = charset_pattern.sub(r'\1\n' + inject, content, count=1)

    with open(filepath, "w", encoding="utf-8") as f:
        f.write(content)

    modified += 1
    print(f"  [OK] {dirname}")

# Process all platform directories
for entry in sorted(os.listdir(BASE)):
    full = os.path.join(BASE, entry)
    if not os.path.isdir(full):
        continue
    if entry in SKIP:
        continue

    index_file = os.path.join(full, "index.html")
    if os.path.isfile(index_file):
        process_file(index_file, entry)

# Process main index.html
main_index = os.path.join(BASE, "index.html")
if os.path.isfile(main_index):
    process_file(main_index, "index", is_main=True)

print(f"\n=== Completado ===")
print(f"  Modificados: {modified}")
print(f"  Omitidos (stubs): {skipped}")
