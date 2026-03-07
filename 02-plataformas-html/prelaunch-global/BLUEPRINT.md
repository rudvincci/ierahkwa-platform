# BLUEPRINT: Arquitectura Tecnica del Lanzamiento Global
## Ierahkwa Ne Kanienke — Prelaunch Platform

**Version:** 1.0.0
**Fecha:** Marzo 2026
**Tipo:** Blueprint Tecnico

---

## 1. Arquitectura de la Plataforma Prelaunch

### 1.1 Estructura de Archivos
```
prelaunch-global/
├── index.html              ← UI principal (1765 lineas)
│   ├── <head>              ← Meta tags, OG, CSS
│   ├── <style>             ← CSS inline (400+ lineas)
│   │   ├── Variables CSS   ← --accent, --launch-*
│   │   ├── Hero            ← Countdown, particulas
│   │   ├── Readiness       ← Dashboard barras progreso
│   │   ├── Checklist       ← Sistema interactivo
│   │   ├── Timeline        ← 7 fases visuales
│   │   ├── Ecosystem       ← Contadores animados
│   │   ├── NEXUS Grid      ← 18 portales
│   │   ├── Governance      ← Consejo cards
│   │   └── Responsive      ← Breakpoints 480/768px
│   ├── <body>              ← Contenido semantico
│   │   ├── Header + Nav    ← 7 enlaces de navegacion
│   │   ├── Hero Section    ← Countdown + CTA
│   │   ├── Readiness       ← 8 categorias
│   │   ├── Checklist       ← 6 grupos, 48 items
│   │   ├── Timeline        ← 7 fases
│   │   ├── Ecosystem       ← 8 contadores
│   │   ├── NEXUS           ← 18 portales
│   │   ├── Governance      ← 6 cards
│   │   ├── CTA Banner      ← Llamada a accion
│   │   └── Footer          ← Info + badges
│   └── <script>            ← JavaScript funcional
│       ├── Countdown       ← Timer configurable
│       ├── Particles       ← 50 particulas animadas
│       ├── Progress Bars   ← Animacion scroll
│       ├── Counters        ← easeOutCubic animation
│       ├── Scroll Observer ← IntersectionObserver-like
│       ├── Checklist       ← localStorage persistence
│       ├── NEXUS Hover     ← Glow effects
│       ├── Smooth Scroll   ← Nav behavior
│       └── Timeline        ← Fase animations
├── README.md               ← Documentacion de uso
├── WHITEPAPER.md           ← Estrategia de lanzamiento
└── BLUEPRINT.md            ← Este archivo
```

### 1.2 Dependencias del Sistema de Diseno
```
../shared/
├── ierahkwa.css            ← Design system principal
├── ierahkwa.js             ← Funcionalidad compartida
├── ierahkwa-security.js    ← Capa de seguridad
├── ierahkwa-quantum.js     ← Encriptacion quantum
├── ierahkwa-protocols.js   ← Protocolos soberanos
├── ierahkwa-interconnect.js ← Comunicacion inter-plataforma
├── ierahkwa-agents.js      ← 7 AI Agents
├── sw.js                   ← Service Worker PWA
└── manifest.json           ← PWA Manifest
```

## 2. Componentes JavaScript

### 2.1 Countdown Timer
```
Entrada:  LAUNCH_DATE (configurable)
Proceso:  setInterval(1000ms) → calcula diff
Salida:   Actualiza DOM (dias, horas, minutos, segundos)
Post-launch: Cambia status a "SISTEMA ACTIVO"
```

**Configuracion:**
```javascript
var LAUNCH_DATE = new Date(2026, 5, 21, 0, 0, 0);
// Parametros: (año, mes-1, dia, hora, min, seg)
// Mes es 0-indexado: 5 = Junio
```

### 2.2 Particle System
```
Cantidad:   50 particulas
Colores:    5 (cyan, gold, green, purple, pink)
Animacion:  float-particle (8-20s duracion)
Tamano:     2-5px aleatorio
Posicion:   Distribucion aleatoria horizontal
```

### 2.3 Progress Bars (Readiness Dashboard)
```
Trigger:   Scroll (getBoundingClientRect)
Threshold: 85% viewport
Animacion: CSS transition width 1.5s ease-out
Shimmer:   CSS animation 2s infinite
Data:      data-progress="XX" en cada .readiness-card
```

### 2.4 Counter Animation (Ecosystem Stats)
```
Trigger:   Scroll (90% viewport)
Algoritmo: easeOutCubic → 1 - (1-t)^3
Duracion:  2000ms
Formato:   toLocaleString() para separadores
Soporte:   prefix ($), suffix (+, M+)
```

### 2.5 Checklist System
```
Storage:   localStorage key = 'ierahkwa-prelaunch-checklist'
Formato:   JSON { index: true, ... }
Eventos:   click → toggleCheck()
Updates:   Contadores por grupo + total + progress bar
Visual:    .checked class → green checkmark + strikethrough
6 grupos:  blockchain(13), platforms(8), infra(8),
           security(8), legal(5), docs(6)
Total:     48 items
```

### 2.6 Scroll Animations
```
Clase:     .animate-on-scroll
Trigger:   scroll event (passive)
Threshold: 88% viewport
Effect:    opacity 0→1, translateY 30px→0
Timeline:  opacity 0→1, translateX -20px→0
```

## 3. Sistema de Estilos CSS

### 3.1 Variables CSS Custom
```css
--accent:        #00bcd4   (Cyan soberano)
--launch-gold:   #ffd600   (Oro)
--launch-green:  #00e676   (Verde)
--launch-cyan:   #00bcd4   (Cyan)
--launch-red:    #f44336   (Rojo)
--launch-purple: #7c4dff   (Purpura)
--glass-bg:      rgba(10,14,23,0.65)
--glass-border:  rgba(0,188,212,0.25)
```

### 3.2 Glassmorphism Pattern
```css
.glass-card {
  background: var(--glass-bg);         /* Semi-transparente */
  border: 1px solid var(--glass-border); /* Borde sutil */
  border-radius: 16px;                 /* Bordes redondeados */
  backdrop-filter: blur(12px);         /* Blur de fondo */
}
```

### 3.3 Breakpoints Responsive
```
480px  → Checklist 1 columna, readiness 1 col, stats 2 cols
768px  → Countdown compacto, timeline ajustado, stats 2 cols
1400px → Max-width de secciones
```

### 3.4 Animaciones Clave
| Nombre | Tipo | Duracion | Uso |
|--------|------|----------|-----|
| pulse-badge | box-shadow | 3s infinite | Hero badge |
| shimmer | translateX | 2s infinite | Progress bars |
| float-particle | translateY + scale | 8-20s | Particulas |
| blink-status | opacity | 1.5s infinite | Status dot |
| fade-up | opacity + translateY | 0.8s | Scroll reveal |

## 4. Arquitectura de Seguridad del Lanzamiento

### 4.1 Cadena de Scripts de Seguridad
```
Orden de carga (critico):
1. ierahkwa.js              ← Funcionalidad base
2. ierahkwa-security.js     ← Validacion e integridad
3. ierahkwa-quantum.js      ← Encriptacion post-quantum
4. ierahkwa-protocols.js    ← Protocolos soberanos
5. ierahkwa-interconnect.js ← Comunicacion segura
6. sw.js                    ← Service Worker (registro)
7. ierahkwa-agents.js       ← 7 AI Agents (ultimo)
```

### 4.2 Headers de Seguridad
```html
X-Content-Type-Options: nosniff
X-Frame-Options: SAMEORIGIN
Referrer-Policy: strict-origin-when-cross-origin
Permissions-Policy: camera=(), microphone=(), geolocation=(self), payment=()
```

## 5. Pre-Flight Checklist — Arquitectura de Datos

### 5.1 Estructura de Estado
```javascript
// localStorage: 'ierahkwa-prelaunch-checklist'
{
  "0": true,   // Item 0 checked
  "5": true,   // Item 5 checked
  "12": true,  // Item 12 checked
  // ... solo items marcados se almacenan
}
```

### 5.2 Grupos y Contadores
```
Grupo          | Data-group  | Items | Counter selector
Blockchain     | blockchain  | 13    | [data-group="blockchain"]
Plataformas    | platforms   | 8     | [data-group="platforms"]
Infraestructura| infra       | 8     | [data-group="infra"]
Seguridad      | security    | 8     | [data-group="security"]
Legal          | legal       | 5     | [data-group="legal"]
Documentacion  | docs        | 6     | [data-group="docs"]
TOTAL          | —           | 48    | #checklist-total
```

### 5.3 Flujo de Datos
```
Click → toggleCheck(item)
  ├── Toggle .checked class
  ├── saveChecklistState() → localStorage.setItem()
  └── updateChecklistCounts()
       ├── Count per group → update .checklist-group-count
       ├── Count total → update #checklist-total
       └── Update #checklist-progress-bar width
```

## 6. Readiness Dashboard — Configuracion

### 6.1 Metricas por Sistema
```
Sistema          | Progreso | Gradiente
Blockchain       | 92%      | cyan → green
Plataformas      | 98%      | purple → pink
Seguridad        | 95%      | red → orange
DeFi             | 88%      | gold → orange
Gobernanza       | 90%      | blue → cyan
Infraestructura  | 85%      | green → lime
Cobertura Global | 96%      | pink → purple
Tokens           | 94%      | gold → green
```

### 6.2 Actualizacion de Progreso
Para actualizar el porcentaje de un sistema, modificar `data-progress` en el HTML:
```html
<div class="readiness-card" data-progress="92">
```

## 7. Timeline — Fases del Lanzamiento

### 7.1 Estructura Visual
```
Linea vertical: linear-gradient(cyan → gold → green)
Nodos:          Circulos numerados con box-shadow glow
Cards:          Glass cards con hover translateX(8px)
Tags:           Badges con border-color matching fase
```

### 7.2 Colores por Fase
```
Fase 1 Semilla:       #00bcd4 (Cyan)
Fase 2 Raiz:          #00e676 (Green)
Fase 3 Tronco:        #ffd600 (Gold)
Fase 4 Ramas:         #e040fb (Pink)
Fase 5 Copa:          #ff9100 (Orange)
Fase 6 Bosque:        #f44336 (Red)
Fase 7 Generaciones:  gradient(cyan, gold)
```

## 8. Rendimiento

### 8.1 Metricas Objetivo
- First Contentful Paint: < 1.5s
- Largest Contentful Paint: < 2.5s
- Total Blocking Time: < 200ms
- Cumulative Layout Shift: < 0.1

### 8.2 Optimizaciones Implementadas
- CSS inline (evita flash de contenido sin estilo)
- JavaScript defer-pattern (scripts al final del body)
- Scroll listeners con `{ passive: true }`
- Animaciones CSS (no JS) para shimmer, pulse, particles
- `font-variant-numeric: tabular-nums` para countdown
- `will-change` implicito via `transform` transitions
- Zero external dependencies (no CDN, no fonts externas)

## 9. Accesibilidad

### 9.1 Implementaciones WCAG 2.1 AA
- `role="document"` en body
- `role="timer"` en countdown con `aria-label`
- `role="list"` / `role="listitem"` en stats
- Skip navigation link
- `aria-hidden="true"` en iconos decorativos
- Semantic HTML5 (`section`, `nav`, `header`, `main`, `footer`)
- Contraste minimo 4.5:1 para texto
- Focus indicators via ierahkwa.css

---

**Ierahkwa Ne Kanienke — Blueprint Tecnico**
**Consejo Digital Soberano — 2026**
**Sovereign-1.0 License**
