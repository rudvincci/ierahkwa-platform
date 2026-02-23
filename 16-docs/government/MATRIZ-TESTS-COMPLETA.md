# MATRIZ DE TESTS COMPLETA
**Generado:** 2026-02-05T14:24:33.491Z

## Totales
- Servicios: 54 × 6 tests = 324
- Plataformas: 144 × 10 = 1440
- Rutas: 115 × 4 = 460
- Departamentos: 41 × 5 = 205
- **Total tests (una pasada):** 2429

## Por servicio (ejemplo primeros 5)
- **Ierahkwa Futurehead Mamey Node** :8545
  - health: GET 8545/health o /api/health, status 200
  - latency: Medir latencia ms
  - cors: CORS si aplica
  - rate_limit: Rate limit si aplica
  - root: GET 8545/ o /api
  - smoke: 1 endpoint principal
- **Banking Bridge API** :3001
  - health: GET 3001/health o /api/health, status 200
  - latency: Medir latencia ms
  - cors: CORS si aplica
  - rate_limit: Rate limit si aplica
  - root: GET 3001/ o /api
  - smoke: 1 endpoint principal
- **Banking API .NET** :3000
  - health: GET 3000/health o /api/health, status 200
  - latency: Medir latencia ms
  - cors: CORS si aplica
  - rate_limit: Rate limit si aplica
  - root: GET 3000/ o /api
  - smoke: 1 endpoint principal
- **Platform Frontend** :8080
  - health: GET 8080/health o /api/health, status 200
  - latency: Medir latencia ms
  - cors: CORS si aplica
  - rate_limit: Rate limit si aplica
  - root: GET 8080/ o /api
  - smoke: 1 endpoint principal
- **BDET Bank Server** :4001
  - health: GET 4001/health o /api/health, status 200
  - latency: Medir latencia ms
  - cors: CORS si aplica
  - rate_limit: Rate limit si aplica
  - root: GET 4001/ o /api
  - smoke: 1 endpoint principal

## Por plataforma (ejemplo primeros 5)

- **abrir-todas-plataformas.html**
  - carga: GET /platform/abrir-todas-plataformas.html.html 200
  - tiempo: Tiempo carga
  - no_5xx: Sin 5xx
  - contenido: Body con <html o título
  - rutas_alias: Rutas alternativas mismo contenido
  - enlace_index: Enlace desde index
  - js_css: JS/CSS sin 404
  - a11y: title y estructura
  - auth: Session/redirect login si aplica
  - smoke_ui: 1 interacción crítica
- **admin.html**
  - carga: GET /platform/admin.html.html 200
  - tiempo: Tiempo carga
  - no_5xx: Sin 5xx
  - contenido: Body con <html o título
  - rutas_alias: Rutas alternativas mismo contenido
  - enlace_index: Enlace desde index
  - js_css: JS/CSS sin 404
  - a11y: title y estructura
  - auth: Session/redirect login si aplica
  - smoke_ui: 1 interacción crítica
- **ai-hub-dashboard.html**
  - carga: GET /platform/ai-hub-dashboard.html.html 200
  - tiempo: Tiempo carga
  - no_5xx: Sin 5xx
  - contenido: Body con <html o título
  - rutas_alias: Rutas alternativas mismo contenido
  - enlace_index: Enlace desde index
  - js_css: JS/CSS sin 404
  - a11y: title y estructura
  - auth: Session/redirect login si aplica
  - smoke_ui: 1 interacción crítica
- **ai-platform.html**
  - carga: GET /platform/ai-platform.html.html 200
  - tiempo: Tiempo carga
  - no_5xx: Sin 5xx
  - contenido: Body con <html o título
  - rutas_alias: Rutas alternativas mismo contenido
  - enlace_index: Enlace desde index
  - js_css: JS/CSS sin 404
  - a11y: title y estructura
  - auth: Session/redirect login si aplica
  - smoke_ui: 1 interacción crítica
- **americas-communication-platform.html**
  - carga: GET /platform/americas-communication-platform.html.html 200
  - tiempo: Tiempo carga
  - no_5xx: Sin 5xx
  - contenido: Body con <html o título
  - rutas_alias: Rutas alternativas mismo contenido
  - enlace_index: Enlace desde index
  - js_css: JS/CSS sin 404
  - a11y: title y estructura
  - auth: Session/redirect login si aplica
  - smoke_ui: 1 interacción crítica

*Ejecutar: `node scripts/generar-matriz-tests-completa.js`*