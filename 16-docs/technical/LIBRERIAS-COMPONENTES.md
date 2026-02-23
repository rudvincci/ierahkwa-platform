# Librerías y Componentes — Ierahkwa Sovereign Platform v01

**Software propietario vs. Open Source. Qué se recibe: ejecutables, código, bibliotecas.**  
Sovereign Government of Ierahkwa Ne Kanienke • Office of the Prime Minister

---

## 1. Resumen: qué se recibe según el tipo de software

### 1.1 Software propietario (cerrado)

| Qué recibe | Descripción |
|------------|-------------|
| **Ejecutables** | Binarios (.exe, .dll en Windows; binarios en Linux/macOS) o paquetes Node (archivos en `node_modules` compilados o empaquetados). |
| **Librerías compiladas** | .dll, .so, .dylib o equivalentes necesarias para ejecutar. No se entrega código fuente de estos componentes propietarios. |
| **Sin código fuente** | El código fuente del núcleo propietario de Ierahkwa Sovereign Platform no se entrega; solo el derecho de uso según EULA. |

### 1.2 Software de código abierto (Open Source)

| Qué recibe | Descripción |
|------------|-------------|
| **Código fuente** | Acceso al código para estudiar, modificar y, según la licencia (MIT, Apache, GPL, etc.), redistribuir. |
| **Ejemplos** | En el ecosistema: Node.js, WordPress, Linux, muchas dependencias npm (Express, bcryptjs, etc.) son open source. |
| **Librerías** | Incluidas en `node_modules/` con sus respectivas licencias (ver `package.json` y `node_modules/<paquete>/LICENSE`). |

### 1.3 Modelo híbrido (Ierahkwa Sovereign Platform)

La **Plataforma** combina:

- **Componentes propietarios** del Sovereign Government of Ierahkwa Ne Kanienke: lógica de negocio específica, integraciones propias, diseños, flujos gubernamentales. Se entregan como **ejecutables, paquetes Node empaquetados o código ofrecido bajo EULA** (uso restringido, sin redistribución de código propietario).
- **Librerías y dependencias Open Source**: Node.js, Express, bibliotecas npm, etc. Se reciben como en cualquier proyecto Node (código en `node_modules/` bajo sus propias licencias).
- **Librerías compiladas** de terceros (p. ej. `sql.js`, binarios nativos): se entregan como parte de la distribución para que el software funcione; el código fuente corresponde a cada proyecto upstream según su licencia.

---

## 2. Librerías (bibliotecas) en la Plataforma

### 2.1 Definición

Son **colecciones de código preescrito** que los desarrolladores usan. Si el software que se adquiere depende de ellas:

- **Open Source**: se reciben en `node_modules/` (Node) o como referencias NuGet (.NET) con posibilidad de inspeccionar el código.  
- **Compiladas/propietarias**: se reciben como binarios o artefactos necesarios para la ejecución; el código no se facilita.

### 2.2 Dependencias Node (ejemplos)

Revisar en cada proyecto el `package.json`. Ejemplos típicos:

| Paquete | Uso | Licencia típica |
|---------|-----|------------------|
| express | Servidor HTTP | MIT |
| bcryptjs | Hash de contraseñas | MIT |
| jsonwebtoken | JWT | MIT |
| multer | Subida de archivos | MIT |
| socket.io | WebSockets / tiempo real | MIT |
| sql.js | SQLite en navegador/Node | MIT |
| pg | Cliente PostgreSQL | MIT |
| dotenv | Variables de entorno | BSD-2-Clause |
| ejs | Plantillas | Apache-2.0 |

La lista completa y actual se obtiene con:

```bash
npm ls --all
```

y en cada paquete: `node_modules/<nombre>/package.json` y `node_modules/<nombre>/LICENSE`.

### 2.3 Dependencias .NET (SmartSchool, InventoryManager, TradeX)

- **NuGet**: paquetes restaurados según cada `.csproj`.  
- **Licencias**: en la documentación de cada paquete en nuget.org.  
- **Entrega**: se reciben como parte del build (DLL en `bin/` o publicadas); el código fuente de terceros depende de cada proyecto upstream.

---

## 3. Software propietario (Ierahkwa) — qué está cerrado

- **Código propio** de la Plataforma que no se publica bajo licencia open source: lógica de integración Shop–POS–Inventario–Node, flujos de negocio gubernamentales, diseños de APIs internas, configuraciones específicas de BDET/IISB/ISB.  
- **Configuraciones y secretos**: `.env`, claves, certificados: no se comparten; se documentan en Manual de Instalación y en Derechos-Acceso-Claves.

---

## 4. Software Open Source — qué se puede hacer

Con las dependencias bajo MIT, Apache, BSD, etc.:

- **Estudiar** el código en `node_modules/` o en los repositorios upstream.  
- **Modificar** para uso interno si la licencia lo permite (en muchos casos sí).  
- **Compartir** según los términos de cada licencia (p. ej. MIT permite redistribución con aviso de copyright).  

Las licencias **GPL/LGPL** pueden imponer condiciones adicionales (copyleft); comprobar cada dependencia antes de redistribuir código derivado.

---

## 5. Resumen para el comprador/usuario

| Elemento | ¿Se recibe? | Formato |
|----------|-------------|---------|
| Ejecutables / app empaquetada | Sí | Archivos en la distribución (Node, .NET, estáticos) |
| Código propio Ierahkwa (propietario) | No (solo uso bajo EULA) | — |
| Código de dependencias open source | Sí (en node_modules o equivalente) | Código fuente según cada paquete |
| Librerías compiladas de terceros | Sí (necesarias para correr) | .dll, .node, wasm, etc. |
| Documentación (EULA, manuales, técnico) | Sí | `docs/` y archivos en la distribución |
| Clave/Serial/Token de activación | Según contrato | Por canal seguro |

---

## 6. Referencias

- **EULA**: `docs/EULA-CONTRATO-LICENCIA.md`  
- **Derechos y claves**: `docs/DERECHOS-ACCESO-CLAVES.md`  
- **Técnico**: `docs/DOCUMENTACION-TECNICA.md`  
- Sobre open source: [IBM – ¿Qué es el software de código abierto?](https://www.ibm.com/topics/open-source), [ESIC – Software de código abierto](https://www.esic.edu/), [ProcessMaker – Lista de verificación comprador de software](https://www.processmaker.com/).

---

```
Sovereign Government of Ierahkwa Ne Kanienke — Office of the Prime Minister
Ierahkwa Sovereign Platform v01 — Librerías y Componentes
```
