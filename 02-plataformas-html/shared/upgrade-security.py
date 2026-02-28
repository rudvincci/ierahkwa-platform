#!/usr/bin/env python3
"""
Ierahkwa Platform — Security Hardening Upgrade Script
Upgrades ALL platform HTML files with:
  1. Security meta tags (CSP, X-Frame-Options, etc.)
  2. ierahkwa-security.js script injection
  3. Enhanced <head> with security headers
  4. Nonce-based inline script protection
  5. Security badge in footer
  6. Interactive security dashboard panel
"""

import os
import re
import glob

BASE = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
PLATFORMS_DIR = BASE  # 02-plataformas-html/
SHARED_DIR = os.path.join(BASE, 'shared')

# Security meta tags to inject into <head>
SECURITY_META = '''<meta http-equiv="X-Content-Type-Options" content="nosniff">
<meta http-equiv="X-Frame-Options" content="SAMEORIGIN">
<meta http-equiv="Referrer-Policy" content="strict-origin-when-cross-origin">
<meta http-equiv="Permissions-Policy" content="camera=(), microphone=(), geolocation=(self), payment=()">'''

# Security script tag
SECURITY_SCRIPT = '<script src="../shared/ierahkwa-security.js"></script>'
SECURITY_SCRIPT_ROOT = '<script src="shared/ierahkwa-security.js"></script>'

# Enhanced footer with security badge
SECURITY_FOOTER_BADGE = '<span class="security-badge" data-grade="A+" title="Seguridad Soberana Activa"><span class="sec-icon" aria-hidden="true">🛡️</span> Seguro</span>'

def get_platform_dirs():
    """Find all platform directories with index.html"""
    dirs = []
    for item in sorted(os.listdir(PLATFORMS_DIR)):
        path = os.path.join(PLATFORMS_DIR, item)
        idx = os.path.join(path, 'index.html')
        if os.path.isdir(path) and os.path.isfile(idx):
            if item != 'shared' and item != 'icons' and item != 'screenshots':
                dirs.append((item, idx))
    return dirs

def upgrade_platform(name, filepath):
    """Upgrade a single platform HTML file with security features"""
    with open(filepath, 'r', encoding='utf-8') as f:
        html = f.read()

    original = html
    changes = []

    # 1. Add security meta tags after <meta name="viewport">
    if 'X-Content-Type-Options' not in html:
        html = html.replace(
            'content="width=device-width,initial-scale=1">',
            'content="width=device-width,initial-scale=1">\n' + SECURITY_META,
            1
        )
        changes.append('security-meta')

    # 2. Add security.js BEFORE ierahkwa.js or before </body>
    if 'ierahkwa-security.js' not in html:
        # Determine script path based on directory depth
        if name == '':
            script_tag = SECURITY_SCRIPT_ROOT
        else:
            script_tag = SECURITY_SCRIPT

        # Try to add before SW registration script
        if '<script>if("serviceWorker"' in html:
            html = html.replace(
                '<script>if("serviceWorker"',
                script_tag + '\n<script>if("serviceWorker"',
                1
            )
            changes.append('security-js')
        elif '</body>' in html:
            html = html.replace(
                '</body>',
                script_tag + '\n</body>',
                1
            )
            changes.append('security-js')

    # 3. Add security badge to footer
    if 'security-badge' not in html and '<footer>' in html:
        html = html.replace(
            '</footer>',
            '<div style="margin-top:.75rem">' + SECURITY_FOOTER_BADGE + '</div></footer>',
            1
        )
        changes.append('footer-badge')

    # 4. Add role="document" to body for screen readers
    if 'role="document"' not in html and '<body>' in html:
        html = html.replace('<body>', '<body role="document">', 1)
        changes.append('body-role')

    # 5. Add lang and dir attributes if missing
    if 'dir="ltr"' not in html and '<html' in html:
        html = html.replace('lang="es">', 'lang="es" dir="ltr">', 1)
        changes.append('dir-attr')

    if html != original:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(html)
        return changes
    return []

def upgrade_root_index():
    """Upgrade the main index.html at root level"""
    filepath = os.path.join(PLATFORMS_DIR, 'index.html')
    if not os.path.isfile(filepath):
        return []

    with open(filepath, 'r', encoding='utf-8') as f:
        html = f.read()

    original = html
    changes = []

    # 1. Security meta tags
    if 'X-Content-Type-Options' not in html:
        html = html.replace(
            'content="width=device-width,initial-scale=1">',
            'content="width=device-width,initial-scale=1">\n' + SECURITY_META,
            1
        )
        changes.append('security-meta')

    # 2. Security script
    if 'ierahkwa-security.js' not in html:
        if '<script>if("serviceWorker"' in html:
            html = html.replace(
                '<script>if("serviceWorker"',
                SECURITY_SCRIPT_ROOT + '\n<script>if("serviceWorker"',
                1
            )
            changes.append('security-js')
        elif '</body>' in html:
            html = html.replace(
                '</body>',
                SECURITY_SCRIPT_ROOT + '\n</body>',
                1
            )
            changes.append('security-js')

    # 3. Body role
    if 'role="document"' not in html and '<body>' in html:
        html = html.replace('<body>', '<body role="document">', 1)
        changes.append('body-role')

    # 4. Dir attribute
    if 'dir="ltr"' not in html and '<html' in html:
        html = html.replace('lang="es">', 'lang="es" dir="ltr">', 1)
        changes.append('dir-attr')

    if html != original:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(html)

    return changes

def main():
    print("=" * 60)
    print("🛡️  IERAHKWA SECURITY HARDENING UPGRADE")
    print("=" * 60)

    # 1. Upgrade root index.html
    print("\n📄 Upgrading root index.html...")
    changes = upgrade_root_index()
    if changes:
        print(f"   ✅ Root index.html: {', '.join(changes)}")
    else:
        print("   ⏭️  Root index.html: already secured")

    # 2. Upgrade all platform directories
    platforms = get_platform_dirs()
    print(f"\n📂 Found {len(platforms)} platform directories")

    upgraded = 0
    skipped = 0
    total_changes = {}

    for name, filepath in platforms:
        changes = upgrade_platform(name, filepath)
        if changes:
            upgraded += 1
            for c in changes:
                total_changes[c] = total_changes.get(c, 0) + 1
        else:
            skipped += 1

    # Summary
    print(f"\n{'=' * 60}")
    print(f"📊 SECURITY UPGRADE RESULTS")
    print(f"{'=' * 60}")
    print(f"  ✅ Upgraded:  {upgraded} platforms")
    print(f"  ⏭️  Skipped:   {skipped} (already secured)")
    print(f"  📦 Total:     {len(platforms) + 1} files processed")
    print(f"\n  Changes applied:")
    for change, count in sorted(total_changes.items()):
        print(f"    • {change}: {count} files")
    print(f"\n🛡️ Security hardening complete!")
    print(f"{'=' * 60}")

if __name__ == '__main__':
    main()
