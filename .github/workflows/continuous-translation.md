---
on:
  push:
    branches: [main]
    paths:
      - '02-plataformas-html/**'
      - '03-backend/**/locales/**'
      - '03-backend/**/i18n/**'
  permissions:
    contents: read
  safe-outputs:
    create-issue:
      title-prefix: "[i18n] "
      labels: [translation, continuous-ai]
  tools:
    github:
---

# Continuous Translation — Ierahkwa Platform

Monitor and maintain translation coverage across the sovereign platform ecosystem.

## Context

The Ierahkwa Platform serves 72 million indigenous people across 37+ indigenous languages and 6 global languages. Translation accuracy and coverage is critical for accessibility and inclusion.

## Analysis Tasks

1. **Translation inventory**:
   - Scan `03-backend/mobile-app/src/i18n/` for language files
   - Scan `03-backend/ierahkwa-shop/public/locales/` for locale files
   - Identify which languages have translation files
   - Count total translatable strings per language

2. **Coverage analysis**:
   - Compare each language file against the English (en) base
   - Identify missing translation keys per language
   - Calculate coverage percentage per language
   - Flag languages below 80% coverage

3. **Drift detection**:
   - When English source text changes, identify affected translation keys
   - Flag translations that may be stale (English changed but translation didn't)
   - Check for placeholder/variable consistency ({name}, {count}, etc.)

4. **HTML platform language**:
   - Verify all HTML platforms have `lang` attribute set correctly
   - Check if platforms have language switcher or multilingual support
   - Flag platforms with hardcoded English text that should be translatable

5. **Indigenous language priorities**:
   - Prioritize Kanien'keha (Mohawk) translations
   - Check for proper Unicode rendering of indigenous characters
   - Verify diacritics and special characters are preserved

## Output

Create an issue with:
- Translation coverage matrix (language x component)
- Missing keys per language (top 20 highest priority)
- Stale translations requiring update
- Recommendations for improving coverage
