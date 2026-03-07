# Ierahkwa Trading — SDK Soberano

> SDK unificado de la Red Soberana que integra 20+ microservicios: banca BDET, social media, servicios, salud, educacion, gobernanza, blockchain y mas.

## Descripcion

El modulo Trading contiene el SDK oficial de la Red Soberana (`SoberanoSDK`), una clase JavaScript isomorfica (funciona en Node.js y en el navegador) que proporciona acceso unificado a los 20+ microservicios del ecosistema Ierahkwa Ne Kanienke a traves del API Gateway central.

El SDK encapsula todas las operaciones del ecosistema en metodos simples y autoexplicativos: desde operaciones bancarias del BDET Bank (billetera, pagos, exchange, trading, remesas, escrow, prestamos, seguros, staking, tesoreria, fiscal) hasta funciones sociales (posts, feed, stories, comentarios, likes, follows, grupos, chat, live streaming), reserva de servicios, telemedicina, educacion, transporte, gobernanza, identidad digital, correo soberano, traduccion, busqueda, blockchain, almacenamiento en la nube, agricultura inteligente, freelance, punto de venta y turismo.

Disenado para ser consumido tanto como modulo CommonJS en Node.js (`require`) como variable global en el navegador (`window.SoberanoSDK`), el SDK utiliza la API Fetch nativa para todas las comunicaciones HTTP con autenticacion Bearer Token.

## Stack Tecnico

- **Runtime**: Node.js 22 / Browser (isomorfico)
- **HTTP Client**: Fetch API nativo
- **Dependencias de produccion**: compression, cors, dotenv, express, helmet, ws
- **Autenticacion**: Bearer Token (JWT)
- **Puerto**: 3007 (segun Dockerfile)

## API del SDK (Metodos Disponibles)

| Categoria | Metodo | Descripcion |
|-----------|--------|-------------|
| **Auth** | `register(email, pw, nation, lang)` | Registrar usuario |
| **Auth** | `login(email, pw)` | Iniciar sesion (guarda token) |
| **Auth** | `me()` | Obtener perfil autenticado |
| **Banca** | `balance()` | Consultar saldo de billetera |
| **Banca** | `send(to, amount, currency)` | Enviar WMP/fondos |
| **Banca** | `history()` | Historial de transacciones |
| **Pagos** | `processPayment(data)` | Procesar pago |
| **Exchange** | `rates()` | Tasas de cambio |
| **Exchange** | `convert(from, to, amount)` | Convertir divisas |
| **Trading** | `placeOrder(pair, side, type, price, amount)` | Crear orden |
| **Trading** | `cancelOrder(id)` | Cancelar orden |
| **Trading** | `orderBook(pair)` | Libro de ordenes |
| **Trading** | `candles(pair, interval)` | Datos OHLCV |
| **Remesas** | `sendRemittance(data)` | Enviar remesa |
| **Remesas** | `trackRemittance(id)` | Rastrear remesa |
| **Escrow** | `createEscrow(data)` | Crear escrow |
| **Escrow** | `releaseEscrow(id)` | Liberar escrow |
| **Prestamos** | `applyLoan(data)` | Solicitar prestamo |
| **Prestamos** | `loanProducts()` | Productos de prestamo |
| **Seguros** | `insureProducts()` | Productos de seguro |
| **Staking** | `stake(amount, nation)` | Stakear WMP |
| **Staking** | `unstake(id)` | Desstakear |
| **Staking** | `claimRewards()` | Reclamar recompensas |
| **Tesoreria** | `treasury()` | Estado de tesoreria |
| **Fiscal** | `fiscal()` | Asignacion fiscal |
| **Social** | `createPost(data)` | Crear publicacion |
| **Social** | `feed(type)` | Obtener feed |
| **Social** | `trending()` | Contenido trending |
| **Social** | `createStory(data)` | Crear story |
| **Social** | `comment(postId, text)` | Comentar |
| **Social** | `like(type, id)` | Dar like |
| **Social** | `follow(userId)` | Seguir usuario |
| **Social** | `profile(userId)` | Ver perfil |
| **Social** | `createGroup(data)` | Crear grupo |
| **Social** | `sendMessage(convId, text)` | Enviar mensaje |
| **Social** | `startLive(data)` | Iniciar live |
| **Servicios** | `serviceCategories()` | Categorias |
| **Servicios** | `searchProviders(query)` | Buscar proveedores |
| **Servicios** | `bookService(data)` | Reservar servicio |
| **Servicios** | `myBookings(role)` | Mis reservas |
| **Doctor** | `bookAppointment(data)` | Agendar cita medica |
| **Doctor** | `findDoctors(query)` | Buscar doctores |
| **Doctor** | `emergency()` | Emergencia medica |
| **Educacion** | `courses(query)` | Buscar cursos |
| **Educacion** | `enroll(courseId)` | Inscribirse |
| **Transporte** | `requestRide(data)` | Solicitar viaje |
| **Comida** | `orderFood(data)` | Ordenar comida |
| **Gobernanza** | `createElection(data)` | Crear eleccion |
| **Gobernanza** | `vote(electionId, candidateId)` | Votar |
| **Identidad** | `createIdentity(data)` | Crear identidad digital |
| **Identidad** | `verifyIdentity(level)` | Verificar identidad |
| **Correo** | `sendEmail(data)` | Enviar correo |
| **Correo** | `inbox()` | Bandeja de entrada |
| **Traduccion** | `translate(text, from, to)` | Traducir texto |
| **Traduccion** | `languages()` | Idiomas disponibles |
| **Busqueda** | `search(query, type)` | Busqueda global |
| **Blockchain** | `chainStatus()` | Estado de la cadena |
| **Blockchain** | `block(n)` | Obtener bloque |
| **Blockchain** | `tx(hash)` | Obtener transaccion |
| **Cloud** | `uploadFile(data)` | Subir archivo |
| **Cloud** | `myFiles()` | Mis archivos |
| **Agricultura** | `farmRecommend(query)` | Recomendaciones IA |
| **Freelance** | `postGig(data)` | Publicar trabajo |
| **Freelance** | `gigs(query)` | Buscar trabajos |
| **POS** | `processSale(data)` | Procesar venta |
| **Turismo** | `listTours(query)` | Listar tours |
| **Turismo** | `bookTour(tourId, data)` | Reservar tour |

## Variables de Entorno

| Variable | Descripcion | Ejemplo |
|----------|-------------|---------|
| PORT | Puerto del servicio | 3007 |

## Instalacion

```bash
npm install
npm start
```

### Uso en Node.js

```javascript
const SoberanoSDK = require('@ierahkwa/trading');

const sdk = new SoberanoSDK({
  baseUrl: 'https://api.soberano.bo',
  token: 'tu-jwt-token'
});

// Login
await sdk.login('usuario@soberano.bo', 'password');

// Consultar saldo
const balance = await sdk.balance();

// Crear orden de trading
await sdk.placeOrder('WMP/USD', 'buy', 'limit', 0.115, 1000);
```

### Uso en Browser

```html
<script src="index.js"></script>
<script>
  const sdk = new SoberanoSDK({ baseUrl: 'https://api.soberano.bo' });
  sdk.login('usuario@soberano.bo', 'password').then(console.log);
</script>
```

## Docker

```bash
docker build -t trading .
docker run -p 3007:3007 trading
```

## Arquitectura

```
[SoberanoSDK]
     |
     |--> req(path, opts) -- Metodo HTTP generico con auth Bearer
     |         |
     |    [Fetch API] --> https://api.soberano.bo/v1/...
     |
     |--> Auth (register, login, me)
     |--> BDET Bank (11 motores financieros)
     |--> Social Media (14 rutas)
     |--> Servicios (4 rutas)
     |--> Doctor (3 rutas)
     |--> Educacion (2 rutas)
     |--> Transporte + Comida
     |--> Gobernanza (voto)
     |--> Identidad (DID)
     |--> Correo + Busqueda
     |--> Blockchain (MameyNode)
     |--> Cloud + Farm + Freelance + POS + Turismo
```

## Parte de

**Ierahkwa Ne Kanienke** -- Plataforma Soberana Digital
