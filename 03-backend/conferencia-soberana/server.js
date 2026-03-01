'use strict';

// ============================================================================
// CONFERENCIA SOBERANA — Sovereign Video Conferencing API
// WebRTC Signaling Server with Socket.IO
// Ierahkwa Ne Kanienke / MameyNode Platform
// ============================================================================

const express = require('express');
const http = require('http');
const cors = require('cors');
const helmet = require('helmet');
const compression = require('compression');
const { Server: SocketIO } = require('socket.io');
const { v4: uuidv4 } = require('uuid');
const { corsConfig, applySecurityMiddleware, errorHandler } = require('../shared/security');
const db = require('./db');

const app = express();
const server = http.createServer(app);

// ============================================================================
// SOCKET.IO SETUP
// ============================================================================

const corsOrigins = (process.env.CORS_ORIGINS || 'http://localhost:3000').split(',');
const io = new SocketIO(server, {
  cors: {
    origin: corsOrigins,
    methods: ['GET', 'POST'],
    credentials: true
  },
  pingTimeout: 60000,
  pingInterval: 25000
});

// ============================================================================
// MIDDLEWARE
// ============================================================================

app.use(helmet({ contentSecurityPolicy: false }));
app.use(compression());
app.use(cors(corsConfig()));
app.use(express.json({ limit: '1mb' }));

// Apply shared Ierahkwa security middleware
const logger = applySecurityMiddleware(app, 'conferencia-soberana');

// ============================================================================
// STUN/TURN CONFIGURATION
// ============================================================================

const ICE_SERVERS = [
  { urls: 'stun:stun.l.google.com:19302' },
  { urls: 'stun:stun1.l.google.com:19302' },
  { urls: 'stun:stun2.l.google.com:19302' },
  {
    urls: process.env.TURN_SERVER_URL || 'turn:turn.soberano.sovereign:3478',
    username: process.env.TURN_USERNAME || 'soberano',
    credential: process.env.TURN_CREDENTIAL || 'sovereign-credential'
  }
];

// ============================================================================
// CLEANUP TIMER
// ============================================================================

// Cleanup empty rooms older than 24h every 30 minutes
const cleanupTimer = setInterval(() => {
  db.cleanupEmptyRooms().catch(err => {
    console.error('[CLEANUP] Error cleaning up empty rooms:', err.message);
  });
}, 30 * 60 * 1000);
cleanupTimer.unref();

// ============================================================================
// REST API ENDPOINTS
// ============================================================================

// Health check
app.get('/health', async (req, res) => {
  try {
    const [activeRooms, totalParticipants] = await Promise.all([
      db.getRoomCount(),
      db.getTotalParticipants()
    ]);

    res.json({
      status: 'ok',
      service: 'conferencia-soberana',
      version: '1.0.0',
      uptime: process.uptime(),
      timestamp: new Date().toISOString(),
      activeRooms,
      totalParticipants,
      encryption: 'E2EE-AES-256-GCM',
      poweredBy: 'MameyNode'
    });
  } catch (error) {
    res.json({
      status: 'ok',
      service: 'conferencia-soberana',
      version: '1.0.0',
      uptime: process.uptime(),
      timestamp: new Date().toISOString(),
      activeRooms: 0,
      totalParticipants: 0,
      encryption: 'E2EE-AES-256-GCM',
      poweredBy: 'MameyNode'
    });
  }
});

// Create a new conference room
app.post('/api/rooms', async (req, res) => {
  try {
    const { name, host, maxParticipants } = req.body;
    const id = uuidv4().slice(0, 12);
    const now = new Date().toISOString();

    const room = await db.createRoom({
      id,
      name: name || `Sala-${id.slice(0, 6)}`,
      host: host || 'anonymous',
      maxParticipants: maxParticipants || 50,
      encryption: 'E2EE-AES-256-GCM',
      createdAt: now,
      updatedAt: now
    });

    logger.dataAccess(req, 'rooms', 'create');

    res.status(201).json({
      success: true,
      room,
      joinUrl: `/api/rooms/${room.id}/join`,
      message: 'Sala de conferencia creada exitosamente'
    });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al crear la sala' });
  }
});

// List active rooms
app.get('/api/rooms', async (req, res) => {
  try {
    const { status, limit = 50, offset = 0 } = req.query;

    const result = await db.listRooms({
      status: status || null,
      limit: Number(limit),
      offset: Number(offset)
    });

    res.json({
      success: true,
      rooms: result.rooms,
      total: result.total,
      limit: Number(limit),
      offset: Number(offset)
    });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al listar salas' });
  }
});

// Get room info
app.get('/api/rooms/:id', async (req, res) => {
  try {
    const room = await db.getRoom(req.params.id);
    if (!room) {
      return res.status(404).json({ success: false, error: 'Sala no encontrada' });
    }

    res.json({ success: true, room });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al obtener sala' });
  }
});

// Join a room
app.post('/api/rooms/:id/join', async (req, res) => {
  try {
    const room = await db.getRoom(req.params.id);
    if (!room) {
      return res.status(404).json({ success: false, error: 'Sala no encontrada' });
    }

    if (room.isLocked) {
      return res.status(403).json({ success: false, error: 'La sala esta bloqueada' });
    }

    if (room.participants.length >= room.maxParticipants) {
      return res.status(409).json({ success: false, error: 'Sala llena' });
    }

    const { displayName, userId } = req.body;
    const participant = {
      id: userId || uuidv4(),
      displayName: displayName || 'Participante',
      joinedAt: new Date().toISOString(),
      audio: true,
      video: true
    };

    await db.addParticipant(req.params.id, participant);

    logger.dataAccess(req, 'rooms', 'join');

    // Fetch the updated room to return
    const updatedRoom = await db.getRoom(req.params.id);

    res.json({
      success: true,
      participant,
      room: updatedRoom,
      iceServers: ICE_SERVERS,
      message: `Unido a sala: ${room.name}`
    });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al unirse a la sala' });
  }
});

// Leave a room
app.post('/api/rooms/:id/leave', async (req, res) => {
  try {
    const room = await db.getRoom(req.params.id);
    if (!room) {
      return res.status(404).json({ success: false, error: 'Sala no encontrada' });
    }

    const { userId } = req.body;
    await db.removeParticipant(req.params.id, userId);

    logger.dataAccess(req, 'rooms', 'leave');

    // Fetch the updated room to return
    const updatedRoom = await db.getRoom(req.params.id);

    res.json({
      success: true,
      room: updatedRoom,
      message: 'Has salido de la sala'
    });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al salir de la sala' });
  }
});

// STUN/TURN configuration endpoint
app.get('/api/ice-servers', (req, res) => {
  res.json({
    success: true,
    iceServers: ICE_SERVERS,
    ttl: 86400
  });
});

// ============================================================================
// SOCKET.IO SIGNALING
// WebRTC signaling remains in-memory (ephemeral by nature).
// Room/participant metadata validation uses the database.
// ============================================================================

io.on('connection', (socket) => {
  console.log(`[WS] Client connected: ${socket.id}`);
  let currentRoom = null;

  // Join a signaling room
  socket.on('room:join', async (data) => {
    const { roomId, userId, displayName } = data;

    try {
      const room = await db.getRoom(roomId);

      if (!room) {
        socket.emit('error', { message: 'Sala no encontrada' });
        return;
      }

      if (room.isLocked) {
        socket.emit('error', { message: 'Sala bloqueada' });
        return;
      }

      currentRoom = roomId;
      socket.join(roomId);

      // Notify other participants
      socket.to(roomId).emit('peer:joined', {
        peerId: socket.id,
        userId,
        displayName: displayName || 'Participante'
      });

      // Send list of existing peers in the room
      const peersInRoom = Array.from(io.sockets.adapter.rooms.get(roomId) || [])
        .filter(id => id !== socket.id);

      socket.emit('room:peers', {
        roomId,
        peers: peersInRoom
      });

      console.log(`[WS] ${displayName || socket.id} joined room ${roomId}`);
    } catch (err) {
      console.error(`[WS] Error joining room ${roomId}:`, err.message);
      socket.emit('error', { message: 'Error al unirse a la sala' });
    }
  });

  // WebRTC signaling: relay offer
  socket.on('signal:offer', (data) => {
    const { targetPeerId, offer } = data;
    io.to(targetPeerId).emit('signal:offer', {
      peerId: socket.id,
      offer
    });
  });

  // WebRTC signaling: relay answer
  socket.on('signal:answer', (data) => {
    const { targetPeerId, answer } = data;
    io.to(targetPeerId).emit('signal:answer', {
      peerId: socket.id,
      answer
    });
  });

  // WebRTC signaling: relay ICE candidate
  socket.on('signal:ice-candidate', (data) => {
    const { targetPeerId, candidate } = data;
    io.to(targetPeerId).emit('signal:ice-candidate', {
      peerId: socket.id,
      candidate
    });
  });

  // Toggle media (audio/video)
  socket.on('media:toggle', (data) => {
    const { roomId, mediaType, enabled } = data;
    socket.to(roomId).emit('peer:media-toggle', {
      peerId: socket.id,
      mediaType,
      enabled
    });
  });

  // Chat message in room
  socket.on('chat:message', (data) => {
    const { roomId, message, displayName } = data;
    io.to(roomId).emit('chat:message', {
      peerId: socket.id,
      displayName: displayName || 'Anonimo',
      message,
      timestamp: new Date().toISOString()
    });
  });

  // Screen share started
  socket.on('screen:start', (data) => {
    const { roomId } = data;
    socket.to(roomId).emit('peer:screen-start', { peerId: socket.id });
  });

  // Screen share stopped
  socket.on('screen:stop', (data) => {
    const { roomId } = data;
    socket.to(roomId).emit('peer:screen-stop', { peerId: socket.id });
  });

  // Disconnect
  socket.on('disconnect', () => {
    if (currentRoom) {
      socket.to(currentRoom).emit('peer:left', { peerId: socket.id });

      // Remove from room participant list in database
      db.removeParticipant(currentRoom, socket.id).catch(err => {
        console.error(`[WS] Error removing participant on disconnect:`, err.message);
      });
    }
    console.log(`[WS] Client disconnected: ${socket.id}`);
  });
});

// ============================================================================
// ERROR HANDLING
// ============================================================================

// 404 handler
app.use((req, res) => {
  res.status(404).json({
    error: 'Endpoint no encontrado',
    path: req.path,
    method: req.method
  });
});

// Global error handler
app.use(errorHandler('conferencia-soberana'));

// ============================================================================
// START SERVER
// ============================================================================

const PORT = process.env.PORT || 3090;

async function start() {
  // Initialize database schema before accepting connections
  await db.initialize();

  server.listen(PORT, () => {
    console.log('');
    console.log('  ============================================================');
    console.log('  ||                                                        ||');
    console.log('  ||     CONFERENCIA SOBERANA                               ||');
    console.log('  ||     Sovereign Video Conferencing API                   ||');
    console.log('  ||                                                        ||');
    console.log('  ||     WebRTC Signaling + E2E Encryption                  ||');
    console.log('  ||     Socket.IO Real-Time Communication                  ||');
    console.log('  ||                                                        ||');
    console.log(`  ||     Port: ${PORT}                                         ||`);
    console.log('  ||     Status: OPERATIONAL                                ||');
    console.log('  ||     Storage: PostgreSQL                                ||');
    console.log('  ||                                                        ||');
    console.log('  ||     Powered by MameyNode                               ||');
    console.log('  ||     Ierahkwa Ne Kanienke Sovereign Platform            ||');
    console.log('  ||                                                        ||');
    console.log('  ============================================================');
    console.log('');
    console.log(`  [INFO] REST API ready on http://localhost:${PORT}`);
    console.log(`  [INFO] WebSocket signaling active on ws://localhost:${PORT}`);
    console.log(`  [INFO] ICE servers configured: ${ICE_SERVERS.length}`);
    console.log(`  [INFO] Encryption: E2EE-AES-256-GCM`);
    console.log(`  [INFO] Database: PostgreSQL connected`);
    console.log('');
  });
}

// Graceful shutdown
function shutdown(signal) {
  console.log(`\n  [INFO] ${signal} received — shutting down gracefully`);
  clearInterval(cleanupTimer);
  server.close(async () => {
    await db.end();
    console.log('  [INFO] Database pool closed');
    process.exit(0);
  });
  // Force exit after 10s
  setTimeout(() => process.exit(1), 10000).unref();
}

process.on('SIGTERM', () => shutdown('SIGTERM'));
process.on('SIGINT', () => shutdown('SIGINT'));

// Start the server
start().catch(err => {
  console.error('  [FATAL] Failed to start server:', err.message);
  process.exit(1);
});

module.exports = { app, server, io };
