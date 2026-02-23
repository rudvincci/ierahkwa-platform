# Empezar aquí – Enlaces a todo

**Ruddie Solution** – Solución integral (red, Phantom, docs, scripts, backup). *Creado con los agentes (Cursor/IA).*

Abre este archivo y haz clic en el enlace que necesites (en Cursor/VSCode los enlaces abren el archivo).

---

## Ecosistema Soberano Unificado (Mamey + Ierahkwa + Akwesasne)

| Qué | Enlace |
|-----|--------|
| **Punto de entrada unificado** | [../Mamey-main/EMPEZAR-AQUI-SOBERANO.md](../Mamey-main/EMPEZAR-AQUI-SOBERANO.md) |
| **Plan de unificación 12 semanas** | [../Mamey-main/PLAN-UNIFICACION-3-SISTEMAS.md](../Mamey-main/PLAN-UNIFICACION-3-SISTEMAS.md) |
| **Mapeo Ierahkwa ↔ Mamey** | [../Mamey-main/MAPEO-COMPONENTES.md](../Mamey-main/MAPEO-COMPONENTES.md) |

---

## Pegar en equipos (bloques listos)

| Qué | Enlace |
|-----|--------|
| **Bloques Fortinet** (5 bloques para consola/SSH del FortiGate) | [configs/BLOQUES-PEGAR-FORTINET.txt](configs/BLOQUES-PEGAR-FORTINET.txt) |
| **Bloques Cisco 800** (4 bloques para consola del Cisco) | [configs/BLOQUES-PEGAR-CISCO800.txt](configs/BLOQUES-PEGAR-CISCO800.txt) |

---

## Configuración y tabla de IPs

| Qué | Enlace |
|-----|--------|
| **Configs + tabla** (Fortinet y Cisco, resumen) | [docs/CONFIGS-LISTOS-PEGAR.md](docs/CONFIGS-LISTOS-PEGAR.md) |
| **Tabla de IPs completa** (todos los equipos) | [docs/TABLA-IPs-COMPLETA.md](docs/TABLA-IPs-COMPLETA.md) |
| **Qué configurar y en qué orden** | [docs/CONFIGURAR-TODO.md](docs/CONFIGURAR-TODO.md) |
| **Qué instalar en los servidores** (ProLiant EC200a) | [docs/INSTALAR-EN-SERVIDORES.md](docs/INSTALAR-EN-SERVIDORES.md) |
| **Fortinet paso a paso** | [docs/CONFIGURACION-FORTINET.md](docs/CONFIGURACION-FORTINET.md) |

---

## Backup de los códigos creados (scripts, configs, docs)

| Qué | Enlace |
|-----|--------|
| **Inventario de códigos creados** (lista de todo lo creado) | [docs/INVENTARIO-CODIGOS-CREADOS.md](docs/INVENTARIO-CODIGOS-CREADOS.md) |
| **Crear ZIP con los códigos** | Ejecutar: `bash scripts/backup-codigos-creados.sh` (desde la raíz del proyecto) |

---

## Backup (plataforma de seguridad – Fortinet)

| Qué | Enlace |
|-----|--------|
| **Plan de backups (Fortinet, Cisco, SF300, etc.)** | [docs/PLAN-EQUIPOS-USADOS.md](docs/PLAN-EQUIPOS-USADOS.md) |
| **Carpeta donde guardar backups** | [backups/](backups/) |

**Fortinet (plataforma de seguridad):** Web → System → Configuration → Backup → Download. Guardar en **backups/fortinet-YYYYMMDD.conf**

---

## Plan y verificación

| Qué | Enlace |
|-----|--------|
| **Orden de encendido y verificación** | [docs/ORDEN-ENCENDIDO-Y-VERIFICACION.md](docs/ORDEN-ENCENDIDO-Y-VERIFICACION.md) |
| **Implementar todo** (cableado, encendido, config) | [docs/IMPLEMENTAR-TODO.md](docs/IMPLEMENTAR-TODO.md) |
| **Verificación sin script** (comando para pegar en Terminal) | [docs/VERIFICAR-RED-SIN-SCRIPT.txt](docs/VERIFICAR-RED-SIN-SCRIPT.txt) |
| **Diagnóstico si algo falla** | [docs/TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md) |

---

## Reporte Phantom – Somos Fantasmas

| Qué | Enlace |
|-----|--------|
| **Reporte Phantom Plataforma (Somos Fantasmas)** | [docs/REPORTE-PHANTOM-PLATAFORMA.md](docs/REPORTE-PHANTOM-PLATAFORMA.md) |
| **Competencias, valorización, servicios y cómo trabaja** | [docs/PHANTOM-COMPETENCIAS-SERVICIOS-VALOR.md](docs/PHANTOM-COMPETENCIAS-SERVICIOS-VALOR.md) |

---

## Índice y guía mínima

| Qué | Enlace |
|-----|--------|
| **Índice de toda la documentación** | [docs/INDICE-DOCUMENTACION.md](docs/INDICE-DOCUMENTACION.md) |
| **Comandos mínimos** (SOLO-HAZ-ESTO) | [SOLO-HAZ-ESTO.txt](SOLO-HAZ-ESTO.txt) |
| **Plan completo** (objetivo, fases, checklist) | [docs/REPORTE-PLAN-COMPLETO.md](docs/REPORTE-PLAN-COMPLETO.md) |

---

## Plantillas e inventario

| Qué | Enlace |
|-----|--------|
| **Credenciales** (IPs y contraseñas) | [backups/credenciales-plantilla.txt](backups/credenciales-plantilla.txt) |
| **Inventario** (ProLiant, HP G4, etc.) | [backups/inventario-servidores-plantilla.txt](backups/inventario-servidores-plantilla.txt) |

---

**Ruddie Solution:** [RUDDIE-SOLUTION.md](RUDDIE-SOLUTION.md) – Resumen y entrada rápida a la solución.

**Plan de todas las plataformas** (creado con los agentes): [docs/PLAN-TODAS-LAS-PLATAFORMAS.md](docs/PLAN-TODAS-LAS-PLATAFORMAS.md)

**Diseño azul oscuro + neón** (regresar el look lindo): [docs/DISENO-AZUL-OSCURO-NEON.md](docs/DISENO-AZUL-OSCURO-NEON.md)

**Funciones de las plataformas** (como estaban antes): [docs/FUNCIONES-PLATAFORMAS-REFERENCIA.md](docs/FUNCIONES-PLATAFORMAS-REFERENCIA.md)

**Plataformas independientes de los servidores** (lista + asignación opcional): [docs/PLATAFORMAS-INDEPENDIENTES-DE-SERVIDORES.md](docs/PLATAFORMAS-INDEPENDIENTES-DE-SERVIDORES.md) · [platforms/](platforms/)

**Plataforma unificada (Login · Front Office · Back Office · 60 plataformas)** – Abrir en Chrome: [docs/plataforma-unificada-login-front-back.html](docs/plataforma-unificada-login-front-back.html)

**Un solo enlace:** abre **EMPEZAR-AQUI.md** (este archivo) o **RUDDIE-SOLUTION.md** y desde ahí accede a todo.
