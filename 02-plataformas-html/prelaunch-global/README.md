# Ierahkwa Ne Kanienke — Lanzamiento Global Soberano

## Descripcion

Plataforma de pre-lanzamiento y lanzamiento global para la infraestructura digital soberana Ierahkwa Ne Kanienke. Esta pagina es el centro de comando para coordinar la activacion de la red soberana digital mas grande del mundo, que sirve a 72M+ personas indigenas a traves de 574 naciones tribales en 19 paises.

## Componentes Principales

### 1. Cuenta Regresiva Animada
- Temporizador en tiempo real hasta la fecha de lanzamiento (configurable)
- Fecha por defecto: 21 de Junio de 2026 (Solsticio)
- Transicion automatica al estado "SISTEMA ACTIVO" al llegar a cero
- Particulas animadas de fondo con los colores del proyecto

### 2. Panel de Preparacion del Lanzamiento
8 categorias de sistemas criticos con barras de progreso animadas:
- Blockchain MameyNode (Chain 777777)
- 422+ Plataformas Digitales
- Seguridad & Quantum
- DeFi Soberano
- Gobernanza Soberana
- Infraestructura
- Cobertura Global
- Tokens Soberanos

### 3. Pre-Flight Checklist Interactivo
48 verificaciones agrupadas en 6 categorias:
- Blockchain & Smart Contracts (13 items)
- Plataformas & Frontend (8 items)
- Infraestructura (8 items)
- Seguridad (8 items)
- Legal & Compliance (5 items)
- Documentacion (6 items)

Caracteristicas:
- Estado persistente via `localStorage`
- Contadores por grupo y totales
- Barra de progreso general
- Indicador visual de grupos completos

### 4. Fases del Lanzamiento Global
7 fases inspiradas en la filosofia Haudenosaunee de las Siete Generaciones:
1. Semilla — Testnet & Auditoria
2. Raiz — Mainnet & 50 naciones
3. Tronco — 200 naciones & Staking
4. Ramas — 400 naciones & Marketplace
5. Copa — 574 naciones, soberania total
6. Bosque — 72M usuarios
7. Generaciones — Prosperidad perpetua

### 5. Estadisticas del Ecosistema
Contadores animados con efecto easeOutCubic mostrando metricas clave del ecosistema.

### 6. 18 NEXUS Mega-Portales
Grid visual con los 18 portales tematicos y sus colores distintivos.

### 7. Consejo Digital Soberano
Estructura de gobernanza: Multisig 5-de-9, DAO, Tesoreria, SovereignID, AI Guardians, Licencia Soberana.

## Arquitectura Tecnica

```
prelaunch-global/
├── index.html          ← Plataforma principal (1765 lineas)
├── README.md           ← Este archivo
├── WHITEPAPER.md       ← Estrategia de lanzamiento global
└── BLUEPRINT.md        ← Arquitectura tecnica del lanzamiento
```

## Stack Tecnologico

- **HTML5** semántico con roles ARIA para accesibilidad
- **CSS3** moderno: Grid, Flexbox, Glassmorphism, Gradientes, Variables CSS
- **JavaScript** vanilla (ES5+ compatible) — cero dependencias externas
- **Design System**: `ierahkwa.css` + `ierahkwa.js`
- **Seguridad**: 5 scripts de seguridad soberanos
- **AI**: 7 agentes de IA via `ierahkwa-agents.js`
- **PWA**: Service Worker + Manifest

## Configuracion

### Cambiar fecha de lanzamiento
En el bloque `<script>` del `index.html`, modificar la variable:
```javascript
var LAUNCH_DATE = new Date(2026, 5, 21, 0, 0, 0); // Mes es 0-indexado
```

### Checklist persistente
Los estados del checklist se guardan en `localStorage` con la clave `ierahkwa-prelaunch-checklist`. Para reiniciar:
```javascript
localStorage.removeItem('ierahkwa-prelaunch-checklist');
```

## Compatibilidad

- Responsive: Mobile-first (480px → 768px → 1400px)
- Navegadores: Chrome 80+, Firefox 80+, Safari 14+, Edge 80+
- Accesibilidad: WCAG 2.1 AA, skip-nav, roles ARIA, tabular navigation
- Zero external dependencies — funciona 100% offline

## Licencia

Sovereign-1.0 License — Ierahkwa Ne Kanienke
