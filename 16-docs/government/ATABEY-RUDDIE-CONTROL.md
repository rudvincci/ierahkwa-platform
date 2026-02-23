# 🌺 ATABEY + RUDDIE — Control AI y Permisos

**Gobierno Soberano de Ierahkwa Ne Kanienke**

---

## Controlado con AI siempre

**ATABEY** es la Maestra de todos los sistemas AI. Siempre en control:

- Vigila todos los workers (AI Banker, Trader, Guardian, etc.)
- Envía agentes automáticamente (tareas a workers)
- Ciclos automáticos: recolección de datos, análisis de mercado, seguridad, optimización

### Configuración

```env
ATABEY_ALWAYS_CONTROL=true   # Por defecto. false para desactivar auto-envío.
```

---

## Pipe permiso a Ruddie

**Ruddie** tiene siempre permiso total. Todas las rutas protegidas permiten acceso cuando:

1. **Por usuario:** `id` o `email` está en `RUDDIE_USER_IDS`
2. **Por header:** `X-Ruddie-Bypass` = `RUDDIE_BYPASS_SECRET` (para llamadas internas)

### Configuración

```env
RUDDIE_USER_IDS=ruddie,admin,Ruddie
RUDDIE_BYPASS_HEADER=X-Ruddie-Bypass
RUDDIE_BYPASS_SECRET=tu-secreto-interno-opcional
```

Cuando el usuario es Ruddie:
- `role` → `admin`
- `permissions` → `['all', 'ruddie-bypass']`
- `isRuddie` → `true`

---

## Flujo

```
ATABEY (siempre)          → Vigila todos los AI
                         → Envía agentes/tareas cada ciclo
                         → 1min datos, 2min mercado, 5min seguridad, 10min optimización

RUDDIE (siempre permiso)  → Bypass de auth cuando id/email en RUDDIE_USER_IDS
                         → O header X-Ruddie-Bypass = RUDDIE_BYPASS_SECRET
                         → Acceso total a KMS, Quantum, SWIFT, ML, etc.
```

---

*Documento: Febrero 2026 — IERAHKWA FUTUREHEAD*
