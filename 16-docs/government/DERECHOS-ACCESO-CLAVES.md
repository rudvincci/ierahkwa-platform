# Derechos, Acceso, Clave/Serial/Token — Ierahkwa Sovereign Platform v01

**Qué más recibes: acceso, derechos de uso, claves de activación y validación**  
Sovereign Government of Ierahkwa Ne Kanienke • Office of the Prime Minister

---

## 1. Acceso

### 1.1 Portal de descargas

- **Dónde**: Según lo acordado con el Licenciante (portal oficial, enlace privado, entrega en soporte físico).  
- **Qué incluye**: Paquetes del software (ZIP, instaladores, imagen), documentación (`docs/`), y si aplica, parches y actualizaciones dentro del período de soporte.

### 1.2 Soporte técnico

| Tipo de licencia | Canal | Horario típico |
|------------------|-------|----------------|
| Personal | Comunidad, FAQ, documentación | — |
| Comercial | Email, portal de tickets | Laboral |
| Gubernamental | Canal prioritario, contacto dedicado | 24/7 según contrato |
| Institucional | Email, portal | Laboral |

### 1.3 Actualizaciones

- **Incluidas**: Según tipo de licencia (ver EULA).  
- **Forma**: Descarga desde el portal o notificación por correo con enlace.  
- **Versiones**: Parches de seguridad (recomendados) y versiones menores/mayores según el nivel de soporte.

### 1.4 Comunidad de usuarios

- Si existe: foro, grupo o canal oficial.  
- Uso: buenas prácticas, preguntas, avisos de incidencias (sin exponer datos sensibles ni claves).

---

## 2. Derechos (definidos en el contrato / EULA)

### 2.1 Uso

| Derecho | Descripción |
|---------|-------------|
| **Uso personal** | Uso individual, no comercial. |
| **Uso comercial** | Uso en el marco de una actividad económica, según límites del contrato (usuarios, instalaciones, etc.). |
| **Uso gubernamental** | Uso por órganos del Sovereign Government o entidades autorizadas, según acuerdo marco. |
| **Número de usuarios** | Concurrencia o puestos permitidos: 1, 5, 10, ilimitado, etc., según licencia. |
| **Instalaciones** | Una o varias instalaciones (servidores, sedes) según contrato. |

### 2.2 Restricciones (resumen EULA)

- No sublicenciar, alquilar, revender ni transferir la licencia.  
- No descompilar, desensamblar ni hacer ingeniería inversa del software propietario (salvo lo permitido por ley).  
- No eliminar avisos de propiedad intelectual ni de licencia.  
- No usar el software para fines ilegales o contrarios al EULA.

### 2.3 Documentación y formación

- **Documentación**: Incluida en `docs/` (EULA, Manual de Usuario, Instalación, Documentación Técnica, Certificado, Librerías, este documento).  
- **Formación**: Si se acuerda en el contrato (on-site, online, materiales).

---

## 3. Clave / Serial / Token de activación

### 3.1 Función

- **Activar** el software dentro de los términos de la licencia.  
- **Validar** que la instalación está autorizada (comunicación con servidor de licencias o verificación local, según implementación).  
- **Vincular** la licencia a un titular, contrato o identificador de instalación.

### 3.2 Formato (según implementación)

| Tipo | Ejemplo (ilustrativo) | Uso |
|------|------------------------|-----|
| **Clave alfanumérica** | `IERAHKWA-XXXX-XXXX-XXXX-XXXX` | Introducir en pantalla de activación o en archivo de configuración. |
| **Serial** | Número o string único por licencia | Identificador de licencia en sistemas legacy. |
| **Token (JWT o opaco)** | `eyJhbGci...` o string largo | Uso en APIs o en arranque del servicio. |
| **Archivo de licencia** | `license.json` o `license.key` | Archivo colocado en ruta indicada por la aplicación. |

Los formatos reales los define el Licenciante y se documentan en el Manual de Instalación o en el propio asistente de activación.

### 3.3 Dónde se introduce

- **Interfaz**: Pantalla de “Activar licencia” o “Configuración > Licencia” en la aplicación.  
- **Archivo**: Por ejemplo `config/license.json`, `license.key` en la raíz del proyecto o la ruta que indique la documentación.  
- **Variable de entorno**: `LICENSE_KEY`, `IERAHKWA_LICENSE`, etc., si la implementación lo soporta.

### 3.4 Entrega

- Por **correo electrónico** (canal seguro) tras la firma del contrato y, si aplica, el pago.  
- Desde el **portal de licencias** (descarga de fichero o copia de clave).  
- En **soporte físico** (documento o USB) si se acuerda.  

La **Clave/Serial/Token no debe compartirse** ni publicarse. Es responsabilidad del Licenciatario su custodia.

### 3.5 Caducidad y renovación

- **Licencia perpetua**: La clave puede no tener fecha de caducidad; las actualizaciones pueden tener un período limitado si se pacta.  
- **Suscripción**: La validez puede ser anual o por otro periodo; al renovar se puede emitir una nueva clave o prorrogar la vigencia en el servidor de licencias.  
- Si la clave caduca: el software puede pasar a modo restringido o dejar de funcionar según el diseño; la renovación se gestiona con el Licenciante.

---

## 4. Tokens IGT (blockchain) vs. Tokens de licencia

- **Tokens IGT** (IGT-PM, IGT-BDET, IGT-MARKET, etc.): Son activos de la **Ierahkwa Sovereign Blockchain** para uso en la plataforma (pagos, gobernanza, servicios). No son claves de licencia de software.  
- **Token de licencia** (o “clave de activación”): Credencial para **activar y validar** el derecho de uso del software Ierahkwa Sovereign Platform. Es un secreto que no debe confundirse con los IGT.

---

## 5. Resumen: qué más recibes

| Concepto | Qué recibes |
|----------|-------------|
| **Acceso** | Portal de descargas, soporte técnico, actualizaciones (según licencia), y si aplica, comunidad. |
| **Derechos** | Uso personal, comercial o gubernamental; número de usuarios e instalaciones según contrato. |
| **Clave/Serial/Token** | Código o fichero para activar y validar la licencia, entregado por canal seguro. |
| **Documentación** | EULA, Manual de Usuario, Instalación, Documentación Técnica, Certificado, Librerías, este documento. |

---

## 6. Contacto

- **EULA y licencias**: `docs/EULA-CONTRATO-LICENCIA.md`  
- **Certificado de licencia**: `docs/CERTIFICADO-LICENCIA.md`  
- **Consultas**: Office of the Prime Minister, a través del portal o canal indicado en su contrato.

---

```
Sovereign Government of Ierahkwa Ne Kanienke — Office of the Prime Minister
Ierahkwa Sovereign Platform v01 — Derechos, Acceso, Clave/Serial/Token
```
