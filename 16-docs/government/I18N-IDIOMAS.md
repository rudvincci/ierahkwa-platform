# i18n — Idiomas soportados

La plataforma expone traducciones vía API. Uso: `GET /api/v1/i18n/:lang` y `GET /api/v1/i18n/languages`.

---

## Idiomas disponibles

| Código | Nombre | Uso |
|--------|--------|-----|
| `en` | English | Por defecto si el idioma no existe |
| `es` | Español | |
| `moh` | Kanien'kéha (Mohawk) | |
| `tai` | Taíno | |

---

## Cómo añadir un idioma

1. En **Node:** `RuddieSolution/node/server.js` → objeto `i18nState.translations`.
2. Añadir una clave nueva, por ejemplo `fr` (français), con el mismo conjunto de keys que `en`: `welcome`, `dashboard`, `tokens`, `trading`, `wallet`, `bridge`, `voting`, `rewards`, `settings`, `connect_wallet`, `total_balance`, `recent_transactions`, `create_token`, `swap`, `stake`, `governance`, `analytics`.
3. Añadir el idioma a `i18nState.supportedLanguages`.
4. En `GET /api/v1/i18n/languages` (mismo archivo) añadir `{ code: 'fr', name: 'Français', flag: '🇫🇷' }` (o el idioma que sea).

---

## Cómo ampliar strings

- Añadir nuevas keys en cada objeto de `i18nState.translations.*` (en, es, moh, tai, etc.).
- En el front, consumir `GET /api/v1/i18n/:lang` y usar `translations.nueva_key`.

---

**Referencia en código:** `RuddieSolution/node/server.js` (buscar `i18nState`).
