# Estaty – App Flutter Real Estate y conexión con nuestro Node

**Estaty** (CodeCanyon) es la app móvil Flutter para clientes de listados inmobiliarios. Este documento describe cómo conectarla al backend Node de Ierahkwa **sin certificado** (HTTP) o con **certificado propio** (HTTPS).

---

## Situación actual del Node

- El **Mamey Node** (`RuddieSolution/node/server.js`) arranca por defecto en **HTTP** (sin SSL).
- No hay certificado público (Let's Encrypt, etc.); todo es **propio** según principio soberano.
- Para HTTPS con certificado propio existe `node/ssl/ssl-config.js` (generación con OpenSSL local).

---

## Opción 1: Usar HTTP (sin certificado) – desarrollo / red interna

La forma más directa de que Estaty hable con nuestro Node es usar **HTTP**:

1. **Base URL en la app Flutter** (config o env del proyecto Estaty):
   - Ejemplo: `http://TU_IP:8545` o `http://localhost:8545` (emulador).
   - En dispositivo físico: usar la IP de la máquina donde corre el Node (ej. `http://192.168.1.10:8545`).

2. **CORS**: El Node ya tiene CORS configurable con `CORS_ORIGIN` en `.env`. Para desarrollo puedes dejar CORS abierto o añadir el origen de la app si lo definen.

3. **Credenciales de prueba** (si el backend las implementa tipo Estaty):
   - Username: `user`
   - Password: `12345678`

4. **Flutter**: Por defecto, las peticiones a `http://...` funcionan sin cambios. En **Android 9+** (cleartext por defecto desactivado), hay que permitir cleartext en el proyecto Estaty:
   - `android/app/src/main/res/xml/network_security_config.xml` (crear si no existe):
   ```xml
   <?xml version="1.0" encoding="utf-8"?>
   <network-security-config>
     <domain-config cleartextTrafficPermitted="true">
       <domain includeSubdomains="true">localhost</domain>
       <domain includeSubdomains="true">10.0.2.2</domain>
       <domain includeSubdomains="true">192.168.0.0</domain>
       <domain includeSubdomains="true">TU_IP</domain>
     </domain-config>
   </network-security-config>
   ```
   - En `AndroidManifest.xml`: `android:networkSecurityConfig="@xml/network_security_config"`.

Con esto, **Estaty puede usar nuestro Node por HTTP sin certificado**.

---

## Opción 2: HTTPS con certificado propio en el Node

Si quieres que el Node sirva por **HTTPS** con certificado propio (sin CA externa):

1. **Generar certificado** (una vez):
   ```bash
   cd RuddieSolution/node
   node -e "
   const { SSLManager, generateCertificadoPropio } = require('./ssl/ssl-config');
   const m = new SSLManager(process.env.NODE_ENV || 'development');
   m.ensureCertificadoPropio();
   "
   ```
   Se crean `node/ssl/certs/propio/key.pem` y `cert.pem`.

2. **Activar HTTPS en el Node** (variable de entorno):
   - En `.env`: `USE_HTTPS=true`
   - Reiniciar el servidor. Escuchará en HTTPS en el mismo `PORT` (ej. 8545).

3. **Flutter y certificado no confiable**: Los clientes HTTP de Flutter (Dio, http, etc.) rechazan por defecto certificados autofirmados. Hay que permitirlo **solo para tu backend**:

   **Dio (recomendado en Estaty):**
   ```dart
   (dio.httpClientAdapter as IOHttpClientAdapter).createHttpClient = () {
     final client = HttpClient();
     client.badCertificateCallback = (X509Certificate cert, String host, int port) {
       // Solo para tu dominio/IP; en producción considerar pinning.
       return host == 'TU_IP' || host == 'node.ierahkwa.gov';
     };
     return client;
   };
   ```

   **http package:**
   ```dart
   import 'dart:io';
   class MyHttpOverrides extends HttpOverrides {
     @override
     HttpClient createHttpClient(SecurityContext? context) {
       return super.createHttpClient(context)
         ..badCertificateCallback = (_, __, ___) => true; // Restringir en producción
     }
   }
   void main() {
     HttpOverrides.global = MyHttpOverrides();
     runApp(MyApp());
   }
   ```

Con esto, **Estaty puede usar nuestro Node por HTTPS con certificado propio**.

---

## API que Estaty suele esperar

Los productos tipo Estaty suelen consumir una API REST con:

- **Auth**: login, registro, OTP, recuperar contraseña.
- **Propiedades**: listado, filtros, detalle (fotos, planos, mapa), favoritos.
- **Proyectos**: listado y detalle.
- **Agentes**: contacto, envío de consultas.

En nuestro repo:

- No hay aún un módulo “Estaty” o “property-listings” que replique esa API.
- Hay lógica de **Real Estate** en el ecosistema (IGT-REALTY, Futurehead, `government-banking.js`, `ecosistema-futurehead.json`), pero no endpoints REST listos para Estaty.

**Próximos pasos posibles:**

1. **Adaptar Estaty** para que use endpoints existentes del Node (auth, health, etc.) y añadir bajo `/api/v1/estaty` o `/api/v1/properties` los que falten (listados, favoritos, etc.).
2. **Crear un adaptador** en el Node que exponga datos de Real Estate / IGT-REALTY en el formato que espera la app (JSON con propiedades, agentes, proyectos).

---

## Resumen

| Escenario              | Node          | Estaty (Flutter) |
|------------------------|---------------|-------------------|
| Sin certificado        | HTTP (actual) | Base URL `http://IP:8545`, cleartext permitido en Android |
| Con certificado propio | `USE_HTTPS=true` + cert en `ssl/certs/propio/` | Base URL `https://IP:8545`, `badCertificateCallback` para tu host |

**Conclusión:** Sí se puede usar Estaty con nuestro Node **sin certificado**, usando HTTP y configurando la app para cleartext y la base URL correcta. Si más adelante activas HTTPS con certificado propio, en Flutter solo hay que aceptar ese cert en el cliente HTTP (badCertificateCallback / network security).
