# Plan de integración — Una sola plataforma con ATABEY al centro

```
═══════════════════════════════════════════════════════════════════════
    SOVEREIGN GOVERNMENT OF IERAHKWA NE KANIENKE
    ATABEY mira todo · Una sola plataforma · Todo integrado
═══════════════════════════════════════════════════════════════════════
```

## Problema actual

Hay muchas plataformas separadas, mal organizadas:

| Plataforma | Ubicación | Qué hace |
|------------|-----------|----------|
| Security Fortress | security-fortress.html | Ghost Mode, AI Guardian, cifrado |
| Quantum | quantum-platform.html | Post-quantum, Kyber, Dilithium |
| AI Platform | ai-platform.html | Chat, Code, Web/App builders |
| ATABEY | atabey-dashboard.html | Workers, tareas, vigilancia |
| Telecom | telecom-platform.html | Satélite, VoIP, móvil |
| Comando Conjunto | comando-conjunto-fortress-ai-quantum.html | Estado Fortress+AI+Quantum |
| Webcams | webcams-platform.html | Cámaras en vivo |
| Compliance | compliance-center.html | KYC, NDCA, emergencias |
| Vigilancia | plataforma-vigilancia-inteligencia.html | ATABEY, anomalías, nodos |
| Face Recognition | face-recognition-propio.html | Buscar por foto |
| Watchlist | watchlist-alerta-proteccion.html | Alertas protección |
| Safety Link | safety-link-create.html | Ubicación por link |
| Emergencias | emergencies.html | Botón emergencia |

**Resultado:** dispersión, duplicación, el usuario no sabe dónde ir.

---

## Objetivo: Una sola plataforma

**ATABEY como centro.** Todo lo que ATABEY vigila debe estar visible desde un único dashboard.

### Arquitectura propuesta

```
┌─────────────────────────────────────────────────────────────────────────┐
│  ATABEY COMMAND CENTER — Una sola plataforma                             │
│  atabey-platform.html (o index.html unificado)                           │
├─────────────────────────────────────────────────────────────────────────┤
│  [Pestañas / Secciones]                                                  │
│                                                                          │
│  1. Vista Global      — Estado conjunto (Fortress, AI, Quantum, Telecom) │
│  2. Seguridad         — Fortress, Ghost, Face, Watchlist, Safety Link    │
│  3. Quantum           — Cripto post-cuántica, algoritmos                 │
│  4. Telecom           — Satélite, VoIP, móvil, internet propio           │
│  5. Vigilancia        — Cámaras, anomalías, NDCA, emergencias            │
│  6. Cumplimiento      — KYC, NDCA, auditoría                             │
│  7. Operaciones       — Servicios, salud, nodos                          │
│                                                                          │
│  ATABEY mira todo — widgets en tiempo real en cada sección               │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## Fases de implementación

### Fase 1: Dashboard unificado (prioridad alta)

- Crear **atabey-platform.html** como página principal del centro de mando
- Una sola URL: `/platform/atabey-platform.html` (o `/platform` redirige ahí)
- Tabs: Global | Seguridad | Quantum | Telecom | Vigilancia | Cumplimiento
- Cada tab carga widgets/iframes de las plataformas existentes (sin duplicar código)

### Fase 2: ATABEY como orquestador

- ATABEY obtiene estado de: Fortress, Quantum, Telecom, Face, Watchlist, Vigilancia
- Un solo API: `GET /api/v1/atabey/status` que agrega todo
- ATABEY muestra alertas unificadas (anomalías, emergencias, watchlist, etc.)

### Fase 3: Eliminar redundancia

- Comando Conjunto → integrado como tab "Vista Global" en ATABEY
- Plataforma Vigilancia → integrada como tab "Vigilancia"
- Mantener páginas individuales (Fortress, Quantum, etc.) como "vistas detalladas" accesibles desde el tab

### Fase 4: Navegación simplificada

- Index principal con UN solo enlace: "Centro de mando ATABEY"
- Las demás plataformas se acceden desde ATABEY, no desde el index

---

## Estructura de archivos propuesta

```
platform/
  atabey-platform.html     ← NUEVA: centro de mando único (tabs)
  index.html              ← Simplificado: enlace a ATABEY + resumen
  
  # Vistas detalladas (accesibles desde ATABEY)
  security-fortress.html
  quantum-platform.html
  ai-platform.html
  telecom-platform.html
  webcams-platform.html
  face-recognition-propio.html
  watchlist-alerta-proteccion.html
  safety-link-create.html
  emergencies.html
  compliance-center.html
  plataforma-vigilancia-inteligencia.html
  operations-dashboard.html
```

---

## API unificada para ATABEY

```text
GET /api/v1/atabey/status
  → {
      fortress: { ok, status },
      quantum: { ok, status },
      telecom: { ok, status },
      ai: { workers, tasks },
      vigilance: { lastScan, level },
      face: { ready },
      watchlist: { count, lastAlert },
      emergencies: { active },
      nodes: [ ... ]
    }
```

---

## Checklist de implementación

- [x] Crear `atabey-platform.html` con tabs
- [x] Implementar `GET /api/v1/atabey/status` que agregue todo
- [x] Tab Vista Global: Comando Conjunto integrado (estado conjunto)
- [x] Tab Seguridad: Fortress, Face, Watchlist, Safety Link
- [x] Tab Quantum: Quantum platform integrado
- [x] Tab Telecom: Telecom platform integrado
- [x] Tab Vigilancia: Vigilancia + Webcams
- [x] Tab Cumplimiento: Compliance center
- [x] Simplificar index.html: CTA "Centro ATABEY" prominente
- [x] Actualizar navegación (compliance, fortress, todos-juntos)

---

## Principios

1. **ATABEY mira todo** — Un solo lugar para ver el estado de toda la plataforma
2. **Una entrada** — Usuario va a ATABEY, desde ahí navega a detalle
3. **Sin duplicar** — Reutilizar páginas existentes como sub-vistas
4. **Todo propio** — Sin terceros, Ghost Mode, Ellos no nos encuentran

---

*Siguiente paso: implementar Fase 1 (atabey-platform.html con tabs).*
