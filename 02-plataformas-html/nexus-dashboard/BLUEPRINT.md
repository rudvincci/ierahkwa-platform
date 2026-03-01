# NEXUS Dashboard — Blueprint Tecnico

## 1. Vista General de Arquitectura

```
+------------------------------------------------------------------+
|                     NEXUS DASHBOARD v5.5.0                        |
|                   Centro de Comando Soberano                      |
+------------------------------------------------------------------+
|                                                                    |
|  +------------------+  +------------------+  +------------------+  |
|  |    HEADER        |  |   HERO SECTION   |  |   STATS ROW      |  |
|  |  Logo + Nav +    |  |  Badge + Title + |  |  422 | 18 | 7 |  |  |
|  |  Live Status     |  |  Subtitle        |  |  109 tokens      |  |
|  +------------------+  +------------------+  +------------------+  |
|                                                                    |
|  +--------------------------------------------------------------+  |
|  |                    NEXUS GRID (18 cards)                      |  |
|  |  +----------+  +----------+  +----------+  +----------+      |  |
|  |  | Orbital  |  | Escudo   |  | Cerebro  |  | Tesoro   |      |  |
|  |  | #00bcd4  |  | #f44336  |  | #7c4dff  |  | #ffd600  |      |  |
|  |  | 21 plat  |  | 21 plat  |  | 23 plat  |  | 36 plat  |      |  |
|  |  +----------+  +----------+  +----------+  +----------+      |  |
|  |  +----------+  +----------+  +----------+  +----------+      |  |
|  |  | Voces    |  | Consejo  |  | Tierra   |  | Forja    |      |  |
|  |  | #e040fb  |  | #1565c0  |  | #43a047  |  | #00e676  |      |  |
|  |  | 16 plat  |  | 27 plat  |  | 22 plat  |  | 101 plat |      |  |
|  |  +----------+  +----------+  +----------+  +----------+      |  |
|  |  +----------+  +----------+  +----------+  +----------+      |  |
|  |  | Urbe     |  | Raices   |  | Salud    |  | Academia |      |  |
|  |  | #ff9100  |  | #00FF41  |  | #FF5722  |  | #9C27B0  |      |  |
|  |  | 11 plat  |  | 16 plat  |  | 9 plat   |  | 5 plat   |      |  |
|  |  +----------+  +----------+  +----------+  +----------+      |  |
|  |  +----------+  +----------+  +----------+  +----------+      |  |
|  |  | Escolar  |  | Entret.  |  | Escrit.  |  | Comercio |      |  |
|  |  | #1E88E5  |  | #E91E63  |  | #26C6DA  |  | #FF6D00  |      |  |
|  |  | 10 plat  |  | 13 plat  |  | 17 plat  |  | 17 plat  |      |  |
|  |  +----------+  +----------+  +----------+  +----------+      |  |
|  |  +----------+  +----------+                                   |  |
|  |  | Amparo   |  | Cosmos   |                                   |  |
|  |  | #607D8B  |  | #1a237e  |                                   |  |
|  |  | 13 plat  |  | 8 plat   |                                   |  |
|  |  +----------+  +----------+                                   |  |
|  +--------------------------------------------------------------+  |
|                                                                    |
|  +--------------------------------------------------------------+  |
|  |                  MAPA DE CONEXIONES                           |  |
|  |          Orbital                                              |  |
|  |            o                                                  |  |
|  |    Cosmos / \ Escudo                                          |  |
|  |        o     o                                                |  |
|  |       /       \                                               |  |
|  |  Amparo o--[CORE]--o Cerebro                                  |  |
|  |       \       /                                               |  |
|  |        o     o                                                |  |
|  |  Comercio \ / Tesoro                                          |  |
|  |            o                                                  |  |
|  |          Voces                                                |  |
|  +--------------------------------------------------------------+  |
|                                                                    |
|  +------------------+  +------------------+  +------------------+  |
|  |  SALUD GENERAL   |  |  UPTIME          |  |  INFO SISTEMA    |  |
|  |  Score: 98.7/100 |  |  Infra: 99.99%   |  |  v5.5.0          |  |
|  |  [==========] OK |  |  NEXUS: 99.97%   |  |  19,580 archivos |  |
|  |                   |  |  AI:    99.95%   |  |  254 MB          |  |
|  |                   |  |  Chain: 100.0%   |  |  ~2M lineas      |  |
|  +------------------+  +------------------+  +------------------+  |
|                                                                    |
|  +--------------------------------------------------------------+  |
|  |                   FEED DE ACTIVIDAD                           |  |
|  |  [>] NEXUS Cosmos desplegado            hace 2h               |  |
|  |  [~] ierahkwa-agents.js v2.1            hace 4h               |  |
|  |  [!] Auditoria de seguridad OK          hace 6h               |  |
|  |  [*] 3 tokens IGT registrados           hace 8h               |  |
|  +--------------------------------------------------------------+  |
+------------------------------------------------------------------+
```

## 2. Flujo de Datos

```
+------------------+     +------------------+     +------------------+
|  NEXUS Endpoint  | --> |  Dashboard Core  | --> |  UI Components   |
|  (heartbeat)     |     |  (JS Engine)     |     |  (DOM Render)    |
+------------------+     +------------------+     +------------------+
        |                        |                        |
        v                        v                        v
+------------------+     +------------------+     +------------------+
|  Status Data     |     |  State Manager   |     |  NEXUS Cards     |
|  - active/down   |     |  - NEXUS_DATA[]  |     |  - 18 cards      |
|  - platform cnt  |     |  - animations    |     |  - expandable    |
|  - uptime pct    |     |  - timers        |     |  - mini-charts   |
+------------------+     +------------------+     +------------------+
        |                        |                        |
        v                        v                        v
+------------------+     +------------------+     +------------------+
|  AI Agents       |     |  Map Engine      |     |  Health Monitor  |
|  ierahkwa-       |     |  - radial layout |     |  - score pulse   |
|  agents.js       |     |  - connections   |     |  - uptime track  |
|  (7 agents)      |     |  - click-to-card |     |  - sys info      |
+------------------+     +------------------+     +------------------+
```

## 3. Jerarquia de Componentes

```
index.html
  |
  +-- <head>
  |     +-- meta tags (viewport, description, theme-color, OG, Twitter)
  |     +-- <link> ierahkwa.css
  |     +-- <style> inline (~400 lineas de CSS custom)
  |
  +-- <body>
        +-- .skip-nav (accesibilidad)
        +-- .dash-header
        |     +-- .header-left (logo-icon + header-title)
        |     +-- nav > .header-nav (6 links)
        |     +-- .header-right (status-live + encrypted-badge)
        |
        +-- main#main-content.dashboard-main
        |     +-- section.hero
        |     |     +-- .hero-badge ("NEXUS Command Center")
        |     |     +-- h1 ("Panel de Control NEXUS")
        |     |     +-- p.hero-sub
        |     |
        |     +-- section.stats-row
        |     |     +-- .stat-card x4 (Plataformas, NEXUS, Agents, Tokens)
        |     |
        |     +-- section#nexus-grid
        |     |     +-- .section-heading
        |     |     +-- .nexus-grid#nexusGrid (18 cards via JS)
        |     |           +-- .nexus-card
        |     |                 +-- .nexus-top (icon + info + status)
        |     |                 +-- .nexus-stats-row (count, %, uptime)
        |     |                 +-- .mini-chart (12 bars)
        |     |                 +-- .nexus-expand-btn
        |     |                 +-- .nexus-subplatforms > .sub-list
        |     |
        |     +-- section.nexus-map-section
        |     |     +-- .section-heading
        |     |     +-- .nexus-map#nexusMap
        |     |           +-- .map-ring x3
        |     |           +-- .map-center ("NEXUS CORE")
        |     |           +-- .map-node x18 (via JS)
        |     |           +-- .map-connection x18 (via JS)
        |     |
        |     +-- section#health-section.health-section
        |     |     +-- .section-heading
        |     |     +-- .health-grid
        |     |           +-- .health-card (puntuacion general)
        |     |           +-- .health-card (uptime x5 servicios)
        |     |           +-- .health-card (info sistema x5 metricas)
        |     |
        |     +-- section#activity-section.activity-section
        |           +-- .section-heading
        |           +-- .activity-list
        |                 +-- .activity-item x8
        |
        +-- footer.dash-footer
        +-- <script> inline (~250 lineas JS)
        +-- <script src="../shared/ierahkwa-agents.js">
```

## 4. Integracion con ierahkwa-agents.js

```
+--------------------------------------------------------------+
|                    ierahkwa-agents.js                          |
+--------------------------------------------------------------+
|                                                                |
|  Agent 1: Guardian ----- Anti-fraude en el dashboard           |
|     - Monitorea clics sospechosos                              |
|     - Detecta automatizacion no autorizada                     |
|                                                                |
|  Agent 2: Pattern ------ Aprende uso del operador              |
|     - Almacena patrones en IndexedDB                           |
|     - Predice siguientes acciones                              |
|                                                                |
|  Agent 3: Anomaly ------ Detecta datos anomalos                |
|     - Valida que los heartbeats sean coherentes                |
|     - Alerta si un NEXUS reporta datos inusuales              |
|                                                                |
|  Agent 4: Trust -------- Score de confianza (0-100)            |
|     - Evalua sesion del operador                               |
|     - Ajusta permisos dinamicamente                            |
|                                                                |
|  Agent 5: Shield ------- Proteccion de transacciones           |
|     - Protege cookies y storage del dashboard                  |
|     - Previene manipulacion de datos de estado                 |
|                                                                |
|  Agent 6: Forensic ----- Logging forense                       |
|     - Registra toda interaccion del operador                   |
|     - Trazabilidad completa de acciones                        |
|                                                                |
|  Agent 7: Evolution ---- Auto-mejora                           |
|     - Evoluciona reglas entre generaciones                     |
|     - Adapta deteccion a nuevos patrones de ataque             |
|                                                                |
+--------------------------------------------------------------+
```

## 5. Modelo de Datos NEXUS

```
NEXUS_DATA[18] = {
  id:             string    // Identificador unico (ej: "orbital")
  name:           string    // Nombre para mostrar (ej: "Orbital")
  icon:           string    // Emoji unicode
  color:          string    // Color hex primario
  rgb:            string    // Color RGB para rgba()
  cat:            string    // Categoria vertical
  count:          int       // Numero de sub-plataformas
  subplatforms:   string[]  // Lista de nombres de plataformas
}
```

## 6. Funciones JavaScript

```
init()                  --> Punto de entrada, ejecuta todas las funciones
renderNexusCards()      --> Genera 18 tarjetas en #nexusGrid
  generateBars()        --> Crea 12 barras aleatorias para mini-chart
renderMapNodes()        --> Posiciona 18 nodos radiales en #nexusMap
animateCounters()       --> Anima contadores de stats con easing cubico
startHealthTimer()      --> Incrementa el contador de ultima actualizacion
pulseScore()            --> Varia la puntuacion general cada 5 segundos
resize handler          --> Recalcula posiciones del mapa al redimensionar
```

## 7. Layout Responsivo

```
Desktop (>1024px):  grid 3-4 columnas, mapa completo con 3 anillos
Tablet  (768-1024): grid 2-3 columnas, mapa reducido
Mobile  (<768px):   grid 1 columna, stats 2 columnas, nav oculta
Small   (<480px):   stats 1 columna, health 1 columna
```

## 8. Paleta de Colores

```
Background:    #0a0a0f (base oscura)
Accent:        #00FF41 (verde neon soberano)
Card BG:       rgba(13,26,45,.85)
Card Border:   rgba(0,255,65,.2)
Text Primary:  #ffffff
Text Muted:    #888888
NEXUS Colors:  18 colores unicos (ver NEXUS_DATA)
```

## 9. Accesibilidad (GAAD)

```
[x] Skip navigation link (.skip-nav)
[x] aria-label en todas las secciones
[x] role="list", role="listitem" en grid NEXUS
[x] role="feed", role="article" en actividad
[x] aria-expanded en botones de expansion
[x] aria-controls enlazando boton con contenido
[x] tabindex="0" en tarjetas NEXUS
[x] Keyboard support (Enter, Space)
[x] prefers-reduced-motion media query
[x] Contraste WCAG AA minimo
[x] Semantic HTML (header, main, nav, section, footer)
```

## 10. Metricas de Rendimiento

```
Tamano HTML:           ~55 KB
CSS inline:            ~8 KB
JS inline:             ~6 KB
Total sin gzip:        ~55 KB
Total gzipped:         ~14 KB (estimado)
First Paint:           < 100ms
DOM Interactive:       < 150ms
Fully Loaded:          < 200ms
Memory Usage:          < 15 MB
CPU Idle:              < 2%
Lighthouse Score:      95+ (estimado)
```

---

**Documento**: NEXUS Dashboard Blueprint v5.5.0
**Autor**: Ierahkwa Ne Kanienke - Equipo de Arquitectura
**Fecha**: 2026-03-01
