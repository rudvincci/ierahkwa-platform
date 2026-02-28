#!/usr/bin/env python3
"""
Duplicate Platform Finder for Soberano-Organizado
Scans all platform directories, extracts metadata, and identifies
likely duplicates based on name similarity, translations, and descriptions.

v2 - Reduced false positives by:
  - Stripping boilerplate from descriptions before comparing
  - Requiring stronger evidence for functional duplicate grouping
  - Separating TRUE duplicates from "merely similar" platforms
"""

import os
import re
from html.parser import HTMLParser
from collections import defaultdict

BASE_DIR = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))

# English-to-Spanish concept translation (canonical English key)
TRANSLATIONS = {
    "agriculture": "agricultura", "aviation": "aviacion",
    "backend": "backend", "collab": "colaboracion",
    "collaboration": "colaboracion", "commerce": "comercio",
    "conference": "conferencia", "education": "educacion",
    "energy": "energia", "enterprise": "empresa",
    "environment": "ambiente", "fauna": "fauna",
    "fishing": "pesca", "forum": "foro",
    "geology": "geologia", "healthcare": "salud",
    "hosting": "hospedaje", "housing": "vivienda",
    "ide": "ide", "insurance": "seguros",
    "jobs": "trabajo", "laws": "normas",
    "library": "biblioteca", "livestock": "ganaderia",
    "logistics": "logistica", "maps": "mapa",
    "maritime": "maritimo", "marketing": "marketing",
    "marketplace": "mercado", "media": "media",
    "music": "musica", "news": "noticia",
    "passport": "pasaporte", "payments": "pagos",
    "podcast": "canal", "radio": "radio",
    "repo": "repositorio", "shorts": "cortos",
    "social": "social", "sports": "deporte",
    "transit": "transito", "university": "universidad",
    "voice": "voz", "wallet": "wallet",
    "waste": "residuos", "water": "agua",
    "weather": "meteorologia", "museum": "museo",
    "digital": "digital", "justice": "justicia",
    "parliament": "parlamento", "census": "censo",
    "cadastre": "catastro", "democracy": "democracia",
    "diplomacy": "diplomacia", "immigration": "inmigracion",
    "emergency": "emergencias", "audit": "auditoria",
    "fiscal": "fiscal", "pension": "pension",
    "welfare": "bienestar", "planning": "urbanismo",
    "factory": "fabrica", "contracts": "contratos",
    "computing": "computacion", "quantum": "cuantico",
    "exchange": "intercambio", "bank": "banco",
    "central": "central", "tourism": "turismo",
    "parks": "parques", "heritage": "patrimonio",
    "wisdom": "sabiduria", "translator": "traductor",
    "translation": "traduccion", "blockchain": "blockchain",
    "security": "seguridad", "cybersecurity": "ciberseguridad",
    "defense": "defensa",
}
ES_TO_EN = {v: k for k, v in TRANSLATIONS.items()}


class MetaExtractor(HTMLParser):
    def __init__(self):
        super().__init__()
        self.title = ""
        self.h1 = ""
        self.h2 = ""
        self.meta_desc = ""
        self._in_title = False
        self._in_h1 = False
        self._in_h2 = False
        self._found_h1 = False
        self._found_h2 = False

    def handle_starttag(self, tag, attrs):
        d = dict(attrs)
        if tag == "title":
            self._in_title = True
        elif tag == "h1" and not self._found_h1:
            self._in_h1 = True
        elif tag == "h2" and not self._found_h2:
            self._in_h2 = True
        elif tag == "meta" and d.get("name", "").lower() == "description":
            self.meta_desc = d.get("content", "")

    def handle_endtag(self, tag):
        if tag == "title": self._in_title = False
        elif tag == "h1": self._in_h1 = False; self._found_h1 = True
        elif tag == "h2": self._in_h2 = False; self._found_h2 = True

    def handle_data(self, data):
        if self._in_title: self.title += data.strip()
        elif self._in_h1 and not self._found_h1: self.h1 += data.strip()
        elif self._in_h2 and not self._found_h2: self.h2 += data.strip()


def extract_metadata(filepath):
    try:
        with open(filepath, "r", encoding="utf-8", errors="ignore") as f:
            content = f.read()
    except Exception:
        return None
    parser = MetaExtractor()
    try:
        parser.feed(content)
    except Exception:
        pass
    return {
        "title": parser.title.strip(),
        "h1": parser.h1.strip(),
        "h2": parser.h2.strip(),
        "meta_desc": parser.meta_desc.strip(),
        "heading": parser.h1.strip() or parser.h2.strip(),
        "filesize": len(content),
    }


def strip_boilerplate(desc):
    """Remove the shared boilerplate from descriptions to compare only unique content."""
    # Remove common boilerplate prefixes/suffixes
    desc = re.sub(r'^.*?—\s*', '', desc)  # Remove "Platform Name — "
    desc = re.sub(r'plataforma soberana de grado empresarial para las \d+ naciones del ecosistema Ierahkwa Ne Kanienke\.?\s*', '', desc, flags=re.I)
    desc = re.sub(r'Plataforma soberana de \w+ para la infraestructura digital de \d+ naciones tribales\s*-?\s*Ierahkwa Ne Kanienke\.?', '', desc, flags=re.I)
    return desc.strip()


def normalize_name(dirname):
    """Extract canonical concept words from a directory name."""
    name = dirname.lower()
    name = re.sub(r'^sovereign-', '', name)
    name = re.sub(r'^nexus-', '', name)
    name = re.sub(r'-soberan[oa]s?$', '', name)
    name = re.sub(r'-soberano$', '', name)
    name = re.sub(r'-digital$', '', name)
    words = set(name.split('-'))
    canonical = set()
    for w in words:
        if w in ES_TO_EN:
            canonical.add(ES_TO_EN[w])
        elif w in TRANSLATIONS:
            canonical.add(w)
        else:
            canonical.add(w)
    return canonical


def jaccard(s1, s2):
    if not s1 or not s2:
        return 0.0
    return len(s1 & s2) / len(s1 | s2)


def text_words(text):
    """Extract meaningful words from text, removing stopwords."""
    if not text:
        return set()
    words = set(re.findall(r'\w+', text.lower()))
    stopwords = {'de', 'la', 'el', 'los', 'las', 'del', 'y', 'en', 'para',
                 'con', 'un', 'una', 'the', 'a', 'an', 'of', 'for', 'and',
                 'in', 'to', 'with', 'soberana', 'soberano', 'sovereign',
                 'ierahkwa', 'ne', 'kanienke', 'platform', 'plataforma',
                 'digital', 'nation', 'nacion', 'sistema', 'system',
                 'grado', 'empresarial', 'naciones', 'ecosistema',
                 'infraestructura', 'tribales', 'mameynode'}
    return words - stopwords


def is_redirect_stub(filepath):
    try:
        with open(filepath, "r", encoding="utf-8", errors="ignore") as f:
            content = f.read()
        if len(content) < 500 and ("window.location" in content or "meta http-equiv" in content.lower()):
            return True
    except Exception:
        pass
    return False


def main():
    print("=" * 80)
    print("  SOBERANO DUPLICATE PLATFORM FINDER  v2")
    print("  Scanning:", BASE_DIR)
    print("=" * 80)

    # ── Scan ─────────────────────────────────────────────────────────────
    platforms = {}
    skipped = []
    redirects = []

    skip_dirs = {"shared", "icons", "screenshots", "admin-dashboard",
                 "investor-audit-presentation", "infographic",
                 "pitch-deck", "landing-page", "landing-ierahkwa",
                 "soberano-ecosystem", "soberano-unificado",
                 "portal-central", "portal-soberano"}

    for entry in sorted(os.listdir(BASE_DIR)):
        full = os.path.join(BASE_DIR, entry)
        idx = os.path.join(full, "index.html")
        if not os.path.isdir(full): continue
        if entry in skip_dirs: continue
        if entry.startswith("nexus-"): continue
        if not os.path.exists(idx):
            skipped.append(entry); continue
        if is_redirect_stub(idx):
            redirects.append(entry); continue
        meta = extract_metadata(idx)
        if meta:
            # Pre-compute stripped description
            meta["desc_clean"] = strip_boilerplate(meta["meta_desc"])
            platforms[entry] = meta

    print(f"\n  Platforms scanned: {len(platforms)}")
    print(f"  Skipped (no index.html): {len(skipped)}")
    print(f"  Redirect stubs: {len(redirects)}")
    if redirects:
        for r in redirects: print(f"    - {r}")

    # ── Find duplicates ──────────────────────────────────────────────────
    parent = {k: k for k in platforms}
    reason_map = {}  # (d1,d2) -> (reasons, category)

    def find(x):
        while parent[x] != x:
            parent[x] = parent[parent[x]]
            x = parent[x]
        return x

    def union(a, b):
        ra, rb = find(a), find(b)
        if ra != rb: parent[ra] = rb

    dirs = sorted(platforms.keys())

    for i in range(len(dirs)):
        for j in range(i + 1, len(dirs)):
            d1, d2 = dirs[i], dirs[j]
            m1, m2 = platforms[d1], platforms[d2]
            
            ns = jaccard(normalize_name(d1), normalize_name(d2))
            
            # Get title unique words (strip boilerplate-like parts)
            t1_words = text_words(m1["title"])
            t2_words = text_words(m2["title"])
            ts = jaccard(t1_words, t2_words) if t1_words and t2_words else 0
            
            h1_words = text_words(m1["heading"])
            h2_words = text_words(m2["heading"])
            hs = jaccard(h1_words, h2_words) if h1_words and h2_words else 0

            # Use CLEANED descriptions (boilerplate removed)
            dc1 = text_words(m1["desc_clean"])
            dc2 = text_words(m2["desc_clean"])
            ds = jaccard(dc1, dc2) if dc1 and dc2 else 0

            reasons = []
            category = None

            # TIER 1: EN/ES translation pair (name match >= 100%)
            if ns >= 1.0:
                reasons.append(f"name match=100% (EN/ES pair)")
                category = "TRANSLATION"
                union(d1, d2)

            # TIER 2: Strong name match + any content confirmation
            elif ns >= 0.5 and (ts >= 0.3 or hs >= 0.3 or ds >= 0.3):
                reasons.append(f"name similarity={ns:.0%}")
                if ts >= 0.3: reasons.append(f"title overlap={ts:.0%}")
                if hs >= 0.3: reasons.append(f"heading overlap={hs:.0%}")
                if ds >= 0.3: reasons.append(f"desc overlap={ds:.0%}")
                category = "STRONG_NAME"
                union(d1, d2)

            # TIER 3: Exact same concept names (like contratos-inteligentes vs smart-contracts)
            elif ns >= 0.5 and len(normalize_name(d1)) >= 2 and len(normalize_name(d2)) >= 2:
                overlap = normalize_name(d1) & normalize_name(d2)
                if len(overlap) >= 2:
                    reasons.append(f"shared concepts: {overlap}")
                    category = "CONCEPT_MATCH"
                    union(d1, d2)

            # TIER 4: Very strong content match regardless of name
            elif ts >= 0.6 and hs >= 0.5:
                reasons.append(f"title overlap={ts:.0%}, heading overlap={hs:.0%}")
                category = "CONTENT_MATCH"
                union(d1, d2)

            if reasons:
                reason_map[(d1, d2)] = (reasons, category)

    # ── Build groups ─────────────────────────────────────────────────────
    groups = defaultdict(set)
    for d in dirs:
        groups[find(d)].add(d)
    dup_groups = {k: sorted(v) for k, v in groups.items() if len(v) >= 2}

    # ── Classify each group ──────────────────────────────────────────────
    # Determine dominant category per group
    def group_category(members):
        has_en = any(d.startswith("sovereign-") for d in members)
        has_es = any(not d.startswith("sovereign-") and not d.startswith("digital-")
                     and not d.startswith("central-") and not d.startswith("public-")
                     for d in members)
        # Check if it's purely an EN/ES pair
        all_pairs = True
        for d in members:
            cn = normalize_name(d)
            has_match = False
            for d2 in members:
                if d2 != d and normalize_name(d2) == cn:
                    has_match = True
                    break
            if not has_match and len(members) == 2:
                all_pairs = False
        if has_en and has_es:
            return "EN_ES"
        return "FUNCTIONAL"

    # ── Print report ─────────────────────────────────────────────────────
    # Separate into EN/ES pairs and functional duplicates
    en_es_groups = []
    func_groups = []
    
    for root, members in sorted(dup_groups.items(), key=lambda x: -len(x[1])):
        cat = group_category(members)
        if cat == "EN_ES":
            en_es_groups.append(members)
        else:
            func_groups.append(members)

    # ════════════════════════════════════════════════════════════════════
    # SECTION A: English/Spanish Translation Pairs
    # ════════════════════════════════════════════════════════════════════
    print(f"\n\n{'=' * 80}")
    print("  SECTION A: ENGLISH/SPANISH TRANSLATION PAIRS")
    print("  These directories contain the SAME platform in two languages.")
    print("  Recommendation: Keep one, redirect the other.")
    print(f"{'=' * 80}")

    en_es_count = 0
    for members in sorted(en_es_groups, key=lambda x: x[0]):
        en_dirs = sorted([d for d in members if d.startswith("sovereign-")])
        es_dirs = sorted([d for d in members if not d.startswith("sovereign-")])
        
        # Filter: only show groups where there's a clear 1:1 or 1:N EN-ES mapping
        if not en_dirs or not es_dirs:
            # Might be a functional dup misclassified
            func_groups.append(members)
            continue

        en_es_count += 1
        print(f"\n  {en_es_count}. ", end="")
        
        # Show compact pair format
        if len(en_dirs) == 1 and len(es_dirs) == 1:
            m_en = platforms[en_dirs[0]]
            m_es = platforms[es_dirs[0]]
            print(f"{en_dirs[0]}  <-->  {es_dirs[0]}")
            print(f"     EN title: {m_en['title'][:80]}")
            print(f"     ES title: {m_es['title'][:80]}")
        else:
            print(f"Multi-platform group:")
            for d in en_dirs:
                m = platforms[d]
                print(f"     EN: {d}")
                print(f"         Title: {m['title'][:80]}")
            for d in es_dirs:
                m = platforms[d]
                print(f"     ES: {d}")
                print(f"         Title: {m['title'][:80]}")

    print(f"\n  Total EN/ES pairs: {en_es_count}")

    # ════════════════════════════════════════════════════════════════════
    # SECTION B: Functional Duplicates  
    # ════════════════════════════════════════════════════════════════════
    print(f"\n\n{'=' * 80}")
    print("  SECTION B: FUNCTIONAL DUPLICATES")
    print("  Platforms that appear to serve the same or very similar purpose.")
    print("  Review each pair to decide if they should be merged.")
    print(f"{'=' * 80}")

    func_count = 0
    for members in sorted(func_groups, key=lambda x: x[0]):
        func_count += 1
        print(f"\n  {func_count}. ", end="")
        
        if len(members) == 2:
            d1, d2 = members
            m1, m2 = platforms[d1], platforms[d2]
            key = (d1, d2) if (d1, d2) in reason_map else (d2, d1)
            reasons, cat = reason_map.get(key, (["similar names"], "UNKNOWN"))
            
            print(f"{d1}  <-->  {d2}")
            print(f"     [{d1}] {m1['title'][:80]}")
            if m1['desc_clean']:
                print(f"       Desc: {m1['desc_clean'][:100]}")
            print(f"     [{d2}] {m2['title'][:80]}")
            if m2['desc_clean']:
                print(f"       Desc: {m2['desc_clean'][:100]}")
            print(f"     Match: {'; '.join(reasons)}")
        else:
            print(f"Group of {len(members)}:")
            for d in members:
                m = platforms[d]
                print(f"     [{d}]")
                print(f"       Title: {m['title'][:80]}")
                if m['desc_clean']:
                    print(f"       Desc: {m['desc_clean'][:100]}")
            # Show inter-group matches
            for ii in range(len(members)):
                for jj in range(ii+1, len(members)):
                    key = (members[ii], members[jj])
                    if key in reason_map:
                        reasons, cat = reason_map[key]
                        print(f"     {members[ii]} <-> {members[jj]}: {'; '.join(reasons)}")

    print(f"\n  Total functional duplicate groups: {func_count}")

    # ════════════════════════════════════════════════════════════════════
    # FINAL SUMMARY
    # ════════════════════════════════════════════════════════════════════
    total_involved = sum(len(g) for g in en_es_groups) + sum(len(g) for g in func_groups)
    total_removable = sum(len(g)-1 for g in en_es_groups) + sum(len(g)-1 for g in func_groups)

    print(f"\n\n{'=' * 80}")
    print(f"  FINAL SUMMARY")
    print(f"{'=' * 80}")
    print(f"  Total platforms scanned:          {len(platforms)}")
    print(f"  EN/ES translation pairs:          {en_es_count} groups")
    print(f"  Functional duplicate groups:      {func_count} groups")
    print(f"  Total platforms in dup groups:     {total_involved}")
    print(f"  Potential removals:               {total_removable}")
    print(f"  Unique platforms after cleanup:    {len(platforms) - total_removable}")
    print(f"{'=' * 80}")
    print()


if __name__ == "__main__":
    main()
