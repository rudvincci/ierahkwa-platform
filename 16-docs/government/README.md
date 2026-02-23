# Sovereign Government of Ierahkwa Ne Kanienke – Red y minería

**Ruddie Solution** – Proyecto de infraestructura de red y minería: Sagemcom → TP-Link AFB4 → Fortinet → Cisco 800 → Cisco SF300-24P → servidores, PCs, Mac, HP G4 y miners.

---

## Por dónde empezar

**Ruddie Solution:** [RUDDIE-SOLUTION.md](RUDDIE-SOLUTION.md) – Entrada rápida a la solución.

**Un solo enlace:** [EMPEZAR-AQUI.md](EMPEZAR-AQUI.md) – Desde ahí tienes enlaces a todo (bloques para pegar, configs, tabla de IPs, plan, verificación).

1. **Índice de documentación:** [docs/INDICE-DOCUMENTACION.md](docs/INDICE-DOCUMENTACION.md) – Lista de todos los documentos.
2. **Plan completo:** [docs/REPORTE-PLAN-COMPLETO.md](docs/REPORTE-PLAN-COMPLETO.md) – Objetivo, fases, IPs, checklist.
3. **Guía rápida:** [SOLO-HAZ-ESTO.txt](SOLO-HAZ-ESTO.txt) – Comandos mínimos para verificar y seguir.

---

## Estructura del proyecto

| Carpeta / archivo | Contenido |
|-------------------|-----------|
| **docs/** | Documentación: plan, planos, implementación, Fortinet, Cisco, troubleshooting. |
| **configs/** | Plantillas de configuración (Cisco 800, Cisco SF300). |
| **backups/** | Plantillas de inventario y credenciales; lugar para guardar configuraciones respaldadas. |
| **scripts/** | Verificación de red, implementar, upgrade Cisco 880, backup. |

---

## Comandos útiles

```bash
# Dar permisos a los scripts (una vez)
chmod +x scripts/*.sh

# Verificar conectividad a Sagemcom, Fortinet, Cisco 800, internet
./scripts/verificar-dos-dispositivos.sh

# Ver pasos de implementación y recordatorios
./scripts/implementar.sh
```

---

## Resumen en una frase

Backup de equipos usados, cablear según plan, encender en orden (Sagemcom → AFB4 → Fortinet → Cisco → switches → equipos), configurar Fortinet y Cisco con las plantillas, verificar con scripts y documentación, y luego configurar blockchain y minería.
