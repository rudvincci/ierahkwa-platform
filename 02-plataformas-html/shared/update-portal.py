#!/usr/bin/env python3
"""
update-portal.py - Actualiza el index.html principal del Portal Ierahkwa
con TODAS las plataformas organizadas en 18 NEXUS mega-portales.

Lee nexus-map.json para asignaciones, escanea subdirectorios,
extrae nombres/iconos de cada plataforma y regenera el index.html.
"""

import os
import re
import json
from collections import defaultdict

# -- Paths --
BASE = "/Users/ruddie/Desktop/files/Soberano-Organizado/02-plataformas-html"
INDEX_PATH = os.path.join(BASE, "index.html")
NEXUS_MAP_PATH = os.path.join(BASE, "shared", "nexus-map.json")

# -- Exclusions --
EXCLUDED = {
    "admin-dashboard", "investor-audit-presentation", "shared", "icons",
    "portal-central", "portal-soberano", "soberano-unificado",
    "soberano-ecosystem", "landing-ierahkwa", "landing-page",
    "infographic", "pitch-deck",
}

# -- NEXUS definitions --
NEXUS_ORDER = [
    "orbital", "escudo", "cerebro", "tesoro", "voces", "consejo",
    "tierra", "forja", "urbe", "raices", "salud", "academia",
    "escolar", "entretenimiento", "escritorio", "comercio", "amparo", "cosmos"
]

NEXUS_META = {
    "orbital":         {"color": "#00bcd4",  "rgba": "rgba(0,188,212,.12)",   "icon": "\U0001f6f0\ufe0f", "label": "NEXUS Orbital",         "desc": "Telecomunicaciones, satelites, GPS y redes soberanas"},
    "escudo":          {"color": "#f44336",  "rgba": "rgba(244,67,54,.12)",   "icon": "\U0001f512", "label": "NEXUS Escudo",           "desc": "Ciberdefensa, seguridad militar e inteligencia soberana"},
    "cerebro":         {"color": "#7c4dff",  "rgba": "rgba(124,77,255,.12)",  "icon": "\u269b\ufe0f", "label": "NEXUS Cerebro",          "desc": "AI, quantum computing, IoT, robotica y cloud soberana"},
    "tesoro":          {"color": "#ffd600",  "rgba": "rgba(255,214,0,.12)",   "icon": "\U0001f3e6", "label": "NEXUS Tesoro",           "desc": "BDET Bank, WAMPUM CBDC, trading, blockchain y finanzas soberanas"},
    "voces":           {"color": "#e040fb",  "rgba": "rgba(224,64,251,.12)",  "icon": "\U0001f4f1", "label": "NEXUS Voces",            "desc": "Red social, streaming, noticias y traduccion en 200+ lenguas"},
    "consejo":         {"color": "#1565c0",  "rgba": "rgba(21,101,192,.12)",  "icon": "\U0001f3db\ufe0f", "label": "NEXUS Consejo",          "desc": "Gobierno digital, justicia, democracia y servicios publicos"},
    "tierra":          {"color": "#43a047",  "rgba": "rgba(67,160,71,.12)",   "icon": "\U0001f30d", "label": "NEXUS Tierra",           "desc": "Agricultura, agua, energia, ambiente y biodiversidad"},
    "forja":           {"color": "#00e676",  "rgba": "rgba(0,230,118,.12)",   "icon": "\U0001f4bb", "label": "NEXUS Forja",            "desc": "IDE, DevOps, low-code, AI agents y desarrollo tech"},
    "urbe":            {"color": "#ff9100",  "rgba": "rgba(255,145,0,.12)",   "icon": "\U0001f3d9\ufe0f", "label": "NEXUS Urbe",             "desc": "Smart city, transporte, vivienda e infraestructura urbana"},
    "raices":          {"color": "#00FF41",  "rgba": "rgba(0,255,65,.12)",    "icon": "\U0001f3ad", "label": "NEXUS Raices",           "desc": "Cultura, patrimonio, artesania y archivo"},
    "salud":           {"color": "#FF5722",  "rgba": "rgba(255,87,34,.12)",   "icon": "\U0001f3e5", "label": "NEXUS Salud",            "desc": "Hospitales, telemedicina, farmacia, salud mental y genomica"},
    "academia":        {"color": "#9C27B0",  "rgba": "rgba(156,39,176,.12)",  "icon": "\U0001f393", "label": "NEXUS Academia",         "desc": "Universidad, biblioteca, sabiduria ancestral y laboratorios"},
    "escolar":         {"color": "#1E88E5",  "rgba": "rgba(30,136,229,.12)",  "icon": "\U0001f392", "label": "NEXUS Escolar",          "desc": "Primaria, secundaria, preescolar, curriculum y gestion escolar K-12"},
    "entretenimiento": {"color": "#E91E63",  "rgba": "rgba(233,30,99,.12)",   "icon": "\U0001f3b0", "label": "NEXUS Entretenimiento",  "desc": "Casino, apuestas deportivas, loteria, rifas, esports y gaming soberano"},
    "escritorio":      {"color": "#26C6DA",  "rgba": "rgba(38,198,218,.12)",  "icon": "\U0001f5a5\ufe0f", "label": "NEXUS Escritorio",       "desc": "Docs, ofimatica, hojas de calculo, calendario, notas y colaboracion"},
    "comercio":        {"color": "#FF6D00",  "rgba": "rgba(255,109,0,.12)",   "icon": "\U0001f3ea", "label": "NEXUS Comercio",         "desc": "Comercio, marketing, empresa, hospedaje y turismo soberano"},
    "amparo":          {"color": "#607D8B",  "rgba": "rgba(96,125,139,.12)",  "icon": "\U0001f91d", "label": "NEXUS Amparo",           "desc": "Familia, discapacidad, veteranos, emergencias y empleo"},
    "cosmos":          {"color": "#1a237e",  "rgba": "rgba(26,35,126,.12)",   "icon": "\U0001f30c", "label": "NEXUS Cosmos",           "desc": "Programa espacial, estacion terrena, aviacion y exploracion estelar"},
}

# -- Default icon mapping by keyword --
KEYWORD_ICONS = {
    "bank": "\U0001f3e6", "wallet": "\U0001f4b0", "payment": "\U0001f4b3", "trading": "\U0001f4c8",
    "crypto": "\u20bf", "defi": "\U0001f517", "dex": "\U0001f504", "staking": "\U0001f4ca",
    "fintech": "\U0001f4b9", "insurance": "\U0001f6e1\ufe0f", "tax": "\U0001f9fe", "crowdfunding": "\U0001f91d",
    "erp": "\U0001f4cb", "token": "\U0001fa99", "launchpad": "\U0001f680", "layer2": "\u26a1",
    "smart-contract": "\U0001f4dc", "factura": "\U0001f9fe", "algo": "\U0001f916", "nomina": "\U0001f4b5",
    "portfolio": "\U0001f4bc", "seguros": "\U0001f6e1\ufe0f", "contabilidad": "\U0001f4d2",
    "telecom": "\U0001f4e1", "voip": "\U0001f4de", "call": "\U0001f4de", "conferencia": "\U0001f4f9",
    "mensajeria": "\U0001f4ac", "correo": "\U0001f4e7", "streaming": "\U0001f4fa", "radio": "\U0001f4fb",
    "mesh": "\U0001f578\ufe0f", "satelite": "\U0001f6f0\ufe0f", "antena": "\U0001f4e1", "dns": "\U0001f310",
    "nodo": "\U0001f517", "cdn": "\U0001f310",
    "ciber": "\U0001f6e1\ufe0f", "seguridad": "\U0001f512", "vpn": "\U0001f6e1\ufe0f", "vigilancia": "\U0001f441\ufe0f",
    "ejercito": "\u2694\ufe0f", "cripto": "\U0001f510", "drone": "\U0001f6f8", "radar": "\U0001f4e1",
    "inteligencia": "\U0001f575\ufe0f", "firewall": "\U0001f510", "antivirus": "\U0001f9a0",
    "proteccion": "\U0001f510", "identidad": "\U0001faaa", "zk": "\U0001f50f", "defensa": "\U0001f6e1\ufe0f",
    "ai": "\U0001f916", "bci": "\U0001f9e0", "robot": "\U0001f916", "automat": "\u2699\ufe0f",
    "analitic": "\U0001f4ca", "bigdata": "\U0001f4ca", "buscador": "\U0001f50d", "oracle": "\U0001f52e",
    "indexa": "\U0001f4d1", "quantum": "\u269b\ufe0f", "seti": "\U0001f4e1", "senal": "\U0001f4e1",
    "ml": "\U0001f9e0", "edge": "\U0001f916", "gemelo": "\U0001fa9e", "iot": "\U0001f4e1",
    "cloud": "\u2601\ufe0f", "datos": "\U0001f4ca", "internet": "\U0001f310",
    "agricultura": "\U0001f33e", "fertirri": "\U0001f4a7", "ganaderia": "\U0001f404", "pesca": "\U0001f41f",
    "reforestacion": "\U0001f333", "reciclaje": "\u267b\ufe0f", "meteorologia": "\U0001f324\ufe0f",
    "geologia": "\U0001faa8", "supply": "\U0001f4e6", "observa": "\U0001f30d", "agua": "\U0001f4a7",
    "energia": "\u26a1", "ambiente": "\U0001f33f", "fauna": "\U0001f98e", "maritime": "\U0001f6a2",
    "fishing": "\U0001f3a3", "livestock": "\U0001f404", "water": "\U0001f4a7", "weather": "\U0001f324\ufe0f",
    "waste": "\u267b\ufe0f", "parks": "\U0001f333",
    "dev": "\U0001f4bb", "devops": "\U0001f527", "runtime": "\u2699\ufe0f", "sdk": "\U0001f4e6",
    "gateway": "\U0001f6aa", "micro": "\U0001f52c", "orm": "\U0001f4d0", "testing": "\U0001f9ea",
    "base-dato": "\U0001f5c4\ufe0f", "dapp": "\U0001f4f1", "web-des": "\U0001f310",
    "storage": "\U0001f4be", "bridge": "\U0001f309", "backup": "\U0001f4be",
    "game-engine": "\U0001f3ae", "repositorio": "\U0001f4c2", "virtual": "\U0001f5a5\ufe0f",
    "colabora": "\U0001f465", "code": "\U0001f4bb", "component": "\U0001f9e9",
    "low-code": "\U0001f50c", "orchestr": "\U0001f3b5", "ide": "\U0001f4bb", "repo": "\U0001f4c2",
    "ciudad": "\U0001f3d9\ufe0f", "transporte": "\U0001f68c", "vivienda": "\U0001f3e0",
    "alumbrado": "\U0001f4a1", "residuo": "\u267b\ufe0f", "reserva": "\U0001f4c5",
    "helpdesk": "\U0001f4de", "soporte": "\U0001f3a7", "hosting": "\U0001f5a5\ufe0f",
    "transit": "\U0001f68c", "housing": "\U0001f3e0", "urban": "\U0001f3d9\ufe0f",
    "factory": "\U0001f3ed", "vehiculo": "\U0001f697",
    "ceremonia": "\U0001fab6", "sabiduria": "\U0001f4dc", "linguist": "\U0001f4d6",
    "biblioteca": "\U0001f4da", "artisan": "\U0001f3a8", "traduccion": "\U0001f310",
    "adn": "\U0001f9ec", "museo": "\U0001f3db\ufe0f", "patrimonio": "\U0001f3db\ufe0f", "idioma": "\U0001f5e3\ufe0f",
    "lengua": "\U0001f5e3\ufe0f", "traductor": "\U0001f310", "translator": "\U0001f310",
    "cultural": "\U0001f3ad", "ancestral": "\U0001f4dc", "wisdom": "\U0001f4dc",
    "telemedicina": "\U0001f3e5", "healthcare": "\U0001f3e5", "farmacia": "\U0001f48a",
    "hospital": "\U0001f3e5", "laboratorio": "\U0001f52c", "salud": "\U0001f3e5",
    "nutricion": "\U0001f957", "emergencia": "\U0001f6a8", "ambulancia": "\U0001f691",
    "universidad": "\U0001f393", "investigacion": "\U0001f52c", "beca": "\U0001f393",
    "education": "\U0001f4da", "escuela": "\U0001f3eb", "calificacion": "\U0001f4dd",
    "capacitacion": "\U0001f4d6", "especial": "\U0001f31f",
    "gaming": "\U0001f3ae", "esport": "\U0001f3ae", "metaverso": "\U0001f97d",
    "realidad": "\U0001f97d", "video-editor": "\U0001f3ac", "modelado": "\U0001f9ca",
    "cine": "\U0001f3ac", "casino": "\U0001f3b0", "apuesta": "\U0001f3b2", "loteria": "\U0001f39f\ufe0f",
    "social": "\U0001f465", "podcast": "\U0001f399\ufe0f", "musica": "\U0001f3b5", "music": "\U0001f3b5",
    "publicacion": "\U0001f4f0", "cms": "\U0001f4dd", "email-market": "\U0001f4e7",
    "noticia": "\U0001f4f0", "news": "\U0001f4f0", "nft": "\U0001f5bc\ufe0f", "media": "\U0001f4fa",
    "forum": "\U0001f4ac", "voice": "\U0001f5e3\ufe0f", "shorts": "\U0001f3ac",
    "gobierno": "\U0001f3db\ufe0f", "democracia": "\U0001f5f3\ufe0f", "electoral": "\U0001f5f3\ufe0f",
    "justicia": "\u2696\ufe0f", "registro": "\U0001f4cb", "catastro": "\U0001f5fa\ufe0f",
    "diplomacia": "\U0001f310", "licencia": "\U0001f4dc", "notificacion": "\U0001f514",
    "impuesto": "\U0001f4b0", "transparencia": "\U0001f50d", "inmigracion": "\U0001f6c2",
    "auditoria": "\U0001f4cb", "dao": "\U0001f3db\ufe0f", "parliament": "\U0001f3db\ufe0f",
    "correccional": "\U0001f3db\ufe0f", "laws": "\u2696\ufe0f", "passport": "\U0001f6c2",
    "civil": "\U0001f4cb", "census": "\U0001f4ca", "immigration": "\U0001f6c2",
    "docs": "\U0001f4dd", "notas": "\U0001f4dd", "hojas": "\U0001f4ca", "presentacion": "\U0001f4ca",
    "formulario": "\U0001f4cb", "plantilla": "\U0001f4c4", "firma": "\u270d\ufe0f",
    "gestion-tiempo": "\u23f0", "proyecto": "\U0001f4cb", "project": "\U0001f4cb",
    "contrasena": "\U0001f511", "calendario": "\U0001f4c5", "ui": "\U0001f3a8",
    "comercio": "\U0001f6d2", "marketplace": "\U0001f6d2", "marketing": "\U0001f4e2",
    "crm": "\U0001f465", "logistic": "\U0001f4e6", "inventario": "\U0001f4e6",
    "pos": "\U0001f4b3", "mercado": "\U0001f4ca", "enterprise": "\U0001f3e2",
    "wampum": "\U0001fa99", "cbdc": "\U0001fa99",
    "discapacidad": "\u267f", "alimentacion": "\U0001f372", "pension": "\U0001f4b0",
    "refugio": "\U0001f3e0", "asistencia": "\U0001f91d", "accesibilidad": "\u267f",
    "bombero": "\U0001f692", "trabajo": "\U0001f477", "desempleo": "\U0001f4bc",
    "censo": "\U0001f4ca", "intercambio": "\U0001f91d", "veterano": "\U0001f396\ufe0f",
    "renta": "\U0001f4b0", "welfare": "\U0001f91d", "jobs": "\U0001f477", "sports": "\U0001f3c0",
    "programa-espacial": "\U0001f680", "estacion-terrena": "\U0001f4e1",
    "aviacion": "\u2708\ufe0f", "aviation": "\u2708\ufe0f", "deep-space": "\U0001f30c",
    "interestelar": "\U0001f30c", "cosmos": "\U0001f30c", "espacial": "\U0001f680",
    "audio": "\U0001f3b5", "wiki": "\U0001f4d6", "turismo": "\U0001f3dd\ufe0f", "tourism": "\U0001f3dd\ufe0f",
    "acuicultura": "\U0001f41f", "maps": "\U0001f5fa\ufe0f", "voz": "\U0001f5e3\ufe0f",
    "busqueda": "\U0001f50d", "canal": "\U0001f4fa", "agente": "\U0001f916",
    "backend": "\u2699\ufe0f", "blockchain": "\u26d3\ufe0f", "explorer": "\U0001f50d",
    "artesania": "\U0001f3a8", "audio-editor": "\U0001f3b5", "archivo": "\U0001f4c1",
    "cortos": "\U0001f3ac", "credito": "\U0001f4b3", "central-bank": "\U0001f3e6",
    "commerce": "\U0001f6d2", "digital": "\U0001f4bb",
}


def dir_to_title(dirname):
    """Convert directory name to human-readable title case."""
    name = dirname.replace("-", " ").title()
    return name


def clean_title(raw_title):
    """Clean extracted <title> to get a display name."""
    t = raw_title.strip()
    # Remove common prefixes/suffixes
    patterns = [
        r'^Ierahkwa\s*[^\w\s]\s*',
        r'\s*[^\w\s]\s*Plataforma Soberana$',
        r'\s*[^\w\s]\s*Soberan[oia]a?\s+Digital$',
        r'\s*[^\w\s]\s*Ierahkwa$',
        r'\s*[^\w\s]\s*Ierahkwa Ne Kanienke$',
        r'\s*\|\s*Ierahkwa.*$',
    ]
    for pat in patterns:
        t = re.sub(pat, '', t, flags=re.IGNORECASE)
    # If the result has a separator, take the first part
    for sep in [' \u2014 ', ' \u2013 ', ' - ']:
        if sep in t:
            t = t.split(sep)[0]
    t = t.strip()
    if not t or len(t) < 2:
        return None
    return t


def get_icon_for_dir(dirname):
    """Get an icon based on directory name keywords."""
    lower = dirname.lower()
    # Try specific multi-word matches first
    for keyword, icon in sorted(KEYWORD_ICONS.items(), key=lambda x: -len(x[0])):
        if keyword in lower:
            return icon
    return "\U0001f539"  # Default icon (small blue diamond)


def extract_platform_info(dirpath, dirname):
    """Extract title and icon from a platform's index.html."""
    index_path = os.path.join(dirpath, "index.html")
    display_name = None
    icon = None

    try:
        with open(index_path, "r", encoding="utf-8", errors="ignore") as f:
            content = f.read(8000)  # Only read first 8KB for speed

        # Extract title
        title_match = re.search(r'<title>([^<]+)</title>', content)
        if title_match:
            display_name = clean_title(title_match.group(1))

        # Extract icon from aria-hidden span
        icon_match = re.search(r'aria-hidden="true">([^<]+)</span>', content)
        if icon_match:
            candidate = icon_match.group(1).strip()
            # Only accept if it looks like an emoji (not regular text)
            if len(candidate) <= 4:
                icon = candidate

    except Exception:
        pass

    # Fallback: directory name as title
    if not display_name:
        display_name = dir_to_title(dirname)

    # Fallback: keyword-based icon
    if not icon:
        icon = get_icon_for_dir(dirname)

    return display_name, icon


def main():
    print("=" * 60)
    print("  Ierahkwa Portal Updater - v5.2.0")
    print("=" * 60)

    # -- 1. Load nexus-map.json --
    with open(NEXUS_MAP_PATH, "r", encoding="utf-8") as f:
        nexus_map = json.load(f)

    # Build reverse map: platform_dir -> nexus_name
    dir_to_nexus = {}
    for nexus_name, info in nexus_map["nexus"].items():
        for platform in info.get("platforms", []):
            dir_to_nexus[platform] = nexus_name

    print(f"\n[1] nexus-map.json cargado: {len(dir_to_nexus)} asignaciones en {len(nexus_map['nexus'])} NEXUS")

    # -- 2. Read current index.html for existing assignments --
    with open(INDEX_PATH, "r", encoding="utf-8") as f:
        html = f.read()

    # Extract existing data-nexus from current cards
    existing_cards = re.findall(
        r'<a href="([^"]+)/" class="p-card" data-name="[^"]*" data-nexus="(\w+)">'
        r'<h4><span aria-hidden="true">([^<]+)</span> ([^<]+)</h4>',
        html
    )
    existing_nexus = {}
    existing_icons = {}
    existing_display = {}
    for href, nexus, icon, display in existing_cards:
        existing_nexus[href] = nexus
        existing_icons[href] = icon
        existing_display[href] = display

    print(f"[2] index.html existente: {len(existing_cards)} plataformas con data-nexus")

    # -- 3. Scan directories --
    all_platforms = {}  # dirname -> {nexus, icon, display, search_terms}

    for entry in sorted(os.listdir(BASE)):
        full_path = os.path.join(BASE, entry)
        if not os.path.isdir(full_path):
            continue
        if entry in EXCLUDED:
            continue
        if entry.startswith("nexus-"):
            continue
        if not os.path.exists(os.path.join(full_path, "index.html")):
            continue

        # Extract info
        display_name, icon = extract_platform_info(full_path, entry)

        # Use existing icon if we have one and didn't find a better one from the platform's own file
        if entry in existing_icons and icon == "\U0001f539":
            icon = existing_icons[entry]

        # Use existing display name if extraction gives a generic result
        if entry in existing_display:
            existing_disp = existing_display[entry]
            # If the extracted name equals the dir-based fallback, prefer existing
            if display_name == dir_to_title(entry) and existing_disp != dir_to_title(entry):
                display_name = existing_disp

        # Determine NEXUS assignment
        # Priority: 1. nexus-map.json, 2. existing index.html, 3. default "forja"
        nexus = dir_to_nexus.get(entry)
        if not nexus:
            nexus = existing_nexus.get(entry)
        if not nexus:
            nexus = "forja"

        # Build search terms
        search_terms = entry.replace("-", " ")

        all_platforms[entry] = {
            "nexus": nexus,
            "icon": icon,
            "display": display_name,
            "search_terms": search_terms,
        }

    print(f"[3] Directorios escaneados: {len(all_platforms)} plataformas elegibles")

    # -- 4. Group by NEXUS --
    by_nexus = defaultdict(list)
    for dirname, info in all_platforms.items():
        by_nexus[info["nexus"]].append((dirname, info))

    # Sort platforms within each NEXUS alphabetically
    for nexus in by_nexus:
        by_nexus[nexus].sort(key=lambda x: x[0])

    total_count = len(all_platforms)
    nexus_count = len([n for n in NEXUS_ORDER if len(by_nexus.get(n, [])) > 0])

    print(f"\n[4] Distribucion por NEXUS ({nexus_count} NEXUS, {total_count} plataformas):")
    for nexus in NEXUS_ORDER:
        platforms = by_nexus.get(nexus, [])
        if platforms:
            print(f"    {nexus:20s}: {len(platforms):3d} plataformas")

    # -- 5. Build new HTML sections --

    # 5a. Build filter buttons
    filter_buttons = ['<button class="fbtn active" data-filter="all" style="background:var(--gold);color:#000;border-color:var(--gold)">Todas</button>']
    for nexus in NEXUS_ORDER:
        if by_nexus.get(nexus):
            meta = NEXUS_META[nexus]
            filter_buttons.append(
                f'<button class="fbtn" data-filter="{nexus}" style="--c:{meta["color"]}">{nexus.capitalize()}</button>'
            )
    filters_html = (
        '<div class="filters" role="group" aria-label="Filtrar por NEXUS">\n'
        + "\n".join(filter_buttons)
        + "\n</div>"
    )

    # 5b. Build NEXUS cards grid
    nexus_cards = []
    for nexus in NEXUS_ORDER:
        platforms = by_nexus.get(nexus, [])
        if not platforms:
            continue
        meta = NEXUS_META[nexus]
        count = len(platforms)
        nexus_cards.append(
            f'<a href="nexus-{nexus}/" class="nx-card" style="border-left-color:{meta["color"]}">'
            f'<h3 style="color:{meta["color"]}"><span aria-hidden="true">{meta["icon"]}</span> Nexus {nexus.capitalize()}</h3>'
            f'<p>{meta["desc"]}</p>'
            f'<span class="nx-count">{count} plataformas</span></a>'
        )
    nexus_grid_html = '<div class="nexus-grid">\n' + "\n".join(nexus_cards) + "\n</div>"

    # 5c. Build platform cards
    platform_cards = []
    for nexus in NEXUS_ORDER:
        platforms = by_nexus.get(nexus, [])
        if not platforms:
            continue
        meta = NEXUS_META[nexus]
        platform_cards.append(f'<!-- {nexus.upper()} ({len(platforms)}) -->')
        for dirname, info in platforms:
            card = (
                f'<a href="{dirname}/" class="p-card" data-name="{info["search_terms"]}" '
                f'data-nexus="{nexus}">'
                f'<h4><span aria-hidden="true">{info["icon"]}</span> {info["display"]}</h4>'
                f'<span class="p-nx" style="background:{meta["rgba"]};color:{meta["color"]}">'
                f'{nexus.upper()}</span></a>'
            )
            platform_cards.append(card)

    platforms_grid_html = (
        '<div class="platforms-grid" aria-label="Grid de plataformas">\n'
        + "\n".join(platform_cards)
        + "\n</div>"
    )

    # 5d. SVG node for Cosmos
    cosmos_svg_node = (
        '<a href="nexus-cosmos/"><circle cx="430" cy="330" r="28" fill="#09090d" stroke="#1a237e" stroke-width="2.5"/>'
        '<text x="430" y="325" text-anchor="middle" fill="#1a237e" font-size="16" aria-hidden="true">\U0001f30c</text>'
        '<text x="430" y="340" text-anchor="middle" fill="#1a237e" font-size="7" font-weight="700">COSMOS</text></a>'
    )

    cosmos_svg_lines = (
        '<!-- Cosmos connections -->\n'
        '<line x1="430" y1="330" x2="427" y2="250" stroke="#1a237e"/>\n'
        '<line x1="430" y1="330" x2="390" y2="392" stroke="#1a237e"/>\n'
        '<line x1="430" y1="330" x2="250" y2="250" stroke="#1a237e"/>'
    )

    # -- 6. Apply all modifications to HTML --

    # 6.1 Replace "233 plataformas" -> actual count (all occurrences)
    html = re.sub(r'\b233\b(\s+plataformas)', f'{total_count}\\1', html)
    html = re.sub(r'data-count="233"', f'data-count="{total_count}"', html)
    html = re.sub(r'>233<', f'>{total_count}<', html)

    # 6.2 Replace "17" NEXUS -> "18"
    html = re.sub(r'\b17\b(\s+(?:dominios\s+)?NEXUS)', f'18\\1', html)
    html = re.sub(r'\b17\b(\s+[Mm]ega-[Pp]ortales)', f'18\\1', html)
    html = re.sub(r'data-count="17"', f'data-count="18"', html)
    html = re.sub(r'>17<', f'>18<', html)
    html = re.sub(r'Diagrama de 17 NEXUS', 'Diagrama de 18 NEXUS', html)

    # 6.3 Replace version
    html = html.replace("v3.9.0", "v5.2.0")

    # 6.4 Replace NEXUS section h2 and grid
    old_nexus_section = re.search(
        r'<section class="nexus-section" aria-label="Mega-Portales NEXUS">.*?</section>',
        html, re.DOTALL
    )
    if old_nexus_section:
        new_nexus_section = (
            f'<section class="nexus-section" aria-label="Mega-Portales NEXUS">\n'
            f'<h2>18 Mega-Portales NEXUS</h2>\n'
            f'<p class="sub">Cada NEXUS consolida decenas de plataformas en un dominio</p>\n'
            f'{nexus_grid_html}\n'
            f'</section>'
        )
        html = html[:old_nexus_section.start()] + new_nexus_section + html[old_nexus_section.end():]

    # 6.5 Replace filter section
    old_filter_section = re.search(
        r'<section class="filter-section"[^>]*>.*?</section>',
        html, re.DOTALL
    )
    if old_filter_section:
        new_filter_section = (
            f'<section class="filter-section" aria-label="Todas las plataformas">\n'
            f'<h2>Todas las Plataformas</h2>\n'
            f'{filters_html}\n'
            f'<div class="results-count" aria-live="polite" data-results-count>{total_count} plataformas</div>\n'
            f'</section>'
        )
        html = html[:old_filter_section.start()] + new_filter_section + html[old_filter_section.end():]

    # 6.6 Replace platforms-grid
    old_grid = re.search(
        r'<div class="platforms-grid"[^>]*>.*?</div>\s*(?=</main>)',
        html, re.DOTALL
    )
    if old_grid:
        html = html[:old_grid.start()] + platforms_grid_html + "\n" + html[old_grid.end():]

    # 6.7 Add Cosmos node and connections to SVG
    # Add connection lines before the closing </g>
    closing_g = html.find('</g>', html.find('<svg viewBox'))
    if closing_g != -1:
        html = html[:closing_g] + cosmos_svg_lines + "\n" + html[closing_g:]

    # Add Cosmos node before </svg>
    closing_svg = html.find('</svg>')
    if closing_svg != -1:
        html = html[:closing_svg] + cosmos_svg_node + "\n" + html[closing_svg:]

    # 6.8 Add missing scripts before SW registration
    sw_script = '<script>if("serviceWorker"in navigator)'
    if 'ierahkwa-protocols.js' not in html:
        html = html.replace(
            sw_script,
            '<script src="shared/ierahkwa-protocols.js"></script>\n'
            '<script src="shared/ierahkwa-interconnect.js"></script>\n'
            + sw_script
        )

    # -- 7. Write updated HTML --
    with open(INDEX_PATH, "w", encoding="utf-8") as f:
        f.write(html)

    print(f"\n[5] index.html actualizado exitosamente")
    print(f"    Archivo: {INDEX_PATH}")
    print(f"    Tamano: {len(html):,} bytes")

    # -- 8. Summary --
    print("\n" + "=" * 60)
    print("  RESUMEN FINAL")
    print("=" * 60)
    print(f"\n  Version:     v5.2.0")
    print(f"  NEXUS:       {nexus_count}")
    print(f"  Plataformas: {total_count}")
    print(f"\n  {'NEXUS':<20s} {'Count':>5s}  {'Color':<10s}")
    print(f"  {chr(9472) * 20} {chr(9472) * 5}  {chr(9472) * 10}")
    for nexus in NEXUS_ORDER:
        platforms = by_nexus.get(nexus, [])
        if platforms:
            meta = NEXUS_META[nexus]
            print(f"  {meta['icon']} {nexus:<17s} {len(platforms):>5d}  {meta['color']}")
    print(f"  {chr(9472) * 20} {chr(9472) * 5}")
    print(f"  {'TOTAL':<20s} {total_count:>5d}")
    print(f"\n  Listo. Portal actualizado con exito.")
    print("=" * 60)


if __name__ == "__main__":
    main()
