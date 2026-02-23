# VozSoberana - Ierahkwa Platform

> Plataforma soberana de microblogging. Tu voz, tus datos, tu soberania.

## Que es VozSoberana

VozSoberana es el MVP de una alternativa soberana a Twitter/X, construida como parte de la red Ierahkwa.
Permite publicar voces (posts), amplificarlas (repost), apoyarlas (like), y ver tendencias.

## API Endpoints

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | /api/voces | Listar voces (paginado) |
| POST | /api/voces | Crear nueva voz |
| GET | /api/voces/:id | Obtener voz por ID |
| POST | /api/voces/:id/amplificar | Amplificar una voz |
| POST | /api/voces/:id/apoyar | Apoyar una voz |
| GET | /api/trending | Hashtags tendencia |
| GET | /api/ciudadano/:id | Perfil de ciudadano |

## Inicio Rapido

chmod +x start.sh && ./start.sh

O manualmente: npm install && node server.js

Abrir http://localhost:3002

## Stack: Node.js + Express.js + HTML5/CSS3/JS vanilla | Puerto: 3002

MIT - Powered by Ierahkwa Network
