#!/usr/bin/env node
/**
 * ╔═══════════════════════════════════════════════════════════════════════════╗
 * ║         IERAHKWA FUTUREHEAD SHOP - MULTI-PURPOSE E-COMMERCE               ║
 * ║                                                                           ║
 * ║  Complete E-Commerce System with Powerful Admin Panel                     ║
 * ║  + Real-time Chat (Socket.IO / SignalR-style)                             ║
 * ║  Ierahkwa Futurehead Mamey Node • IGT-MARKET                              ║
 * ║  Sovereign Government of Ierahkwa Ne Kanienke                             ║
 * ║  Office of the Prime Minister                                             ║
 * ╚═══════════════════════════════════════════════════════════════════════════╝
 */
import Fastify from 'fastify';
import cors from '@fastify/cors';
import fastifyStatic from '@fastify/static';
import formbody from '@fastify/formbody';
import { Server as SocketIO } from 'socket.io';
import { createServer } from 'http';
import { createReadStream, existsSync, readFileSync, writeFileSync, mkdirSync } from 'fs';
import { join, dirname } from 'path';
import { fileURLToPath } from 'url';
import { v4 as uuidv4 } from 'uuid';
import db from './src/db.js';
import shopRoutes from './src/routes/shop.js';
import adminRoutes from './src/routes/admin.js';
import posRoutes from './src/routes/pos.js';
import inventoryRoutes from './src/routes/inventory.js';
import bankingRoutes from './src/routes/banking.js';
import monetaryRoutes from './src/routes/monetary.js';
import backupRoutes from './src/routes/backup.js';
import globalBankingRoutes from './src/routes/global-banking.js';
import nodeRoutes from './src/routes/node.js';
import servicesRoutes from './src/routes/services.js';

const __dirname = dirname(fileURLToPath(import.meta.url));
const publicDir = join(__dirname, 'public');
const PORT = process.env.PORT || 3100;

// ============================================
// CHAT DATABASE
// ============================================
const CHAT_DATA_DIR = join(__dirname, 'data');
const CHAT_DB_FILE = join(CHAT_DATA_DIR, 'chat-db.json');

const defaultChatDb = {
  users: [],
  rooms: [
    { id: 'general', name: 'General', description: 'Canal general de chat', isPrivate: false, createdAt: new Date().toISOString() },
    { id: 'soporte', name: 'Soporte', description: 'Canal de soporte técnico', isPrivate: false, createdAt: new Date().toISOString() },
    { id: 'ventas', name: 'Ventas', description: 'Canal de ventas', isPrivate: false, createdAt: new Date().toISOString() }
  ],
  messages: [],
  _counters: { messages: 0 }
};

let chatDb = null;

function ensureChatDir() {
  if (!existsSync(CHAT_DATA_DIR)) mkdirSync(CHAT_DATA_DIR, { recursive: true });
}

function loadChatDb() {
  if (chatDb) return chatDb;
  ensureChatDir();
  if (existsSync(CHAT_DB_FILE)) {
    try {
      chatDb = JSON.parse(readFileSync(CHAT_DB_FILE, 'utf8'));
    } catch {
      chatDb = JSON.parse(JSON.stringify(defaultChatDb));
    }
  } else {
    chatDb = JSON.parse(JSON.stringify(defaultChatDb));
    saveChatDb();
  }
  return chatDb;
}

function saveChatDb() {
  ensureChatDir();
  writeFileSync(CHAT_DB_FILE, JSON.stringify(chatDb, null, 2), 'utf8');
}

// Online users tracking
const onlineUsers = new Map();
const typingUsers = new Map();

// ============================================
// CHATHUB - SignalR-style Implementation
// ============================================
class ChatHub {
  constructor(socket) {
    this.socket = socket;
    this.userId = null;
    this.username = null;
    this.currentRoom = 'general';
  }

  async connect(userData) {
    this.userId = userData.userId || uuidv4();
    this.username = userData.username || `User_${this.userId.slice(0, 6)}`;
    this.avatar = userData.avatar || this.generateAvatar(this.username);
    
    const user = {
      id: this.userId,
      username: this.username,
      avatar: this.avatar,
      socketId: this.socket.id,
      status: 'online',
      lastSeen: new Date().toISOString(),
      connectedAt: new Date().toISOString()
    };
    
    onlineUsers.set(this.socket.id, user);
    
    const db = loadChatDb();
    const existingIndex = db.users.findIndex(u => u.id === this.userId);
    if (existingIndex >= 0) {
      db.users[existingIndex] = { ...db.users[existingIndex], ...user };
    } else {
      db.users.push(user);
    }
    saveChatDb();
    
    this.socket.join(this.currentRoom);
    this.socket.broadcast.emit('UserConnected', user);
    global.io.emit('OnlineUsersUpdated', this.getOnlineUsers());
    
    return {
      success: true,
      user,
      rooms: db.rooms,
      onlineUsers: this.getOnlineUsers()
    };
  }

  disconnect() {
    const user = onlineUsers.get(this.socket.id);
    if (user) {
      user.status = 'offline';
      user.lastSeen = new Date().toISOString();
      
      const db = loadChatDb();
      const idx = db.users.findIndex(u => u.id === user.id);
      if (idx >= 0) {
        db.users[idx] = { ...db.users[idx], status: 'offline', lastSeen: user.lastSeen };
        saveChatDb();
      }
      
      onlineUsers.delete(this.socket.id);
      this.socket.broadcast.emit('UserDisconnected', user);
      global.io.emit('OnlineUsersUpdated', this.getOnlineUsers());
    }
  }

  async sendMessage(data) {
    const { content, roomId = this.currentRoom, replyTo = null } = data;
    if (!content || content.trim() === '') {
      return { success: false, error: 'Message content is required' };
    }
    
    const user = onlineUsers.get(this.socket.id);
    if (!user) return { success: false, error: 'User not connected' };
    
    const db = loadChatDb();
    db._counters.messages++;
    
    const message = {
      id: `msg_${db._counters.messages}_${Date.now()}`,
      roomId,
      userId: user.id,
      username: user.username,
      avatar: user.avatar,
      content: content.trim(),
      replyTo,
      timestamp: new Date().toISOString(),
      status: 'sent',
      reactions: []
    };
    
    db.messages.push(message);
    saveChatDb();
    
    this.stopTyping({ roomId });
    global.io.to(roomId).emit('ReceiveMessage', message);
    
    return { success: true, message };
  }

  async getMessageHistory(data) {
    const { roomId = 'general', limit = 50 } = data;
    const db = loadChatDb();
    let messages = db.messages.filter(m => m.roomId === roomId).slice(-limit);
    return { success: true, messages, roomId };
  }

  startTyping(data) {
    const { roomId = this.currentRoom } = data;
    const user = onlineUsers.get(this.socket.id);
    if (!user) return;
    
    if (!typingUsers.has(roomId)) typingUsers.set(roomId, new Set());
    typingUsers.get(roomId).add(user.id);
    
    this.socket.to(roomId).emit('UserTyping', {
      roomId, userId: user.id, username: user.username, isTyping: true
    });
  }

  stopTyping(data) {
    const { roomId = this.currentRoom } = data;
    const user = onlineUsers.get(this.socket.id);
    if (!user) return;
    
    if (typingUsers.has(roomId)) typingUsers.get(roomId).delete(user.id);
    
    this.socket.to(roomId).emit('UserTyping', {
      roomId, userId: user.id, username: user.username, isTyping: false
    });
  }

  async joinRoom(data) {
    const { roomId } = data;
    const db = loadChatDb();
    const room = db.rooms.find(r => r.id === roomId);
    if (!room) return { success: false, error: 'Room not found' };
    
    const user = onlineUsers.get(this.socket.id);
    if (!user) return { success: false, error: 'User not connected' };
    
    this.socket.leave(this.currentRoom);
    this.stopTyping({ roomId: this.currentRoom });
    this.currentRoom = roomId;
    this.socket.join(roomId);
    
    global.io.to(roomId).emit('UserJoinedRoom', {
      roomId, user: { id: user.id, username: user.username, avatar: user.avatar }
    });
    
    const history = await this.getMessageHistory({ roomId, limit: 50 });
    return { success: true, room, messages: history.messages };
  }

  async createRoom(data) {
    const { name, description = '', isPrivate = false } = data;
    const user = onlineUsers.get(this.socket.id);
    if (!user) return { success: false, error: 'User not connected' };
    
    const db = loadChatDb();
    const roomId = name.toLowerCase().replace(/\s+/g, '-').replace(/[^a-z0-9-]/g, '');
    
    if (db.rooms.find(r => r.id === roomId)) {
      return { success: false, error: 'Room already exists' };
    }
    
    const room = { id: roomId, name, description, isPrivate, createdBy: user.id, createdAt: new Date().toISOString() };
    db.rooms.push(room);
    saveChatDb();
    
    global.io.emit('RoomCreated', room);
    return { success: true, room };
  }

  updateStatus(data) {
    const { status } = data;
    const user = onlineUsers.get(this.socket.id);
    if (!user) return;
    
    user.status = status;
    user.lastSeen = new Date().toISOString();
    onlineUsers.set(this.socket.id, user);
    
    global.io.emit('UserStatusChanged', {
      userId: user.id, username: user.username, status, lastSeen: user.lastSeen
    });
  }

  getOnlineUsers() {
    return Array.from(onlineUsers.values()).map(u => ({
      id: u.id, username: u.username, avatar: u.avatar, status: u.status, lastSeen: u.lastSeen
    }));
  }

  generateAvatar(username) {
    const colors = ['#FF6B6B', '#4ECDC4', '#45B7D1', '#96CEB4', '#FFEAA7', '#DDA0DD', '#98D8C8', '#F7DC6F'];
    const color = colors[username.charCodeAt(0) % colors.length];
    const initials = username.slice(0, 2).toUpperCase();
    return { color, initials };
  }
}

async function main() {
  // Initialize databases
  db.load();
  loadChatDb();
  
  const fastify = Fastify({ 
    logger: process.env.NODE_ENV === 'development',
    trustProxy: true
  });

  // Create HTTP server for Socket.IO
  const httpServer = createServer(fastify.server);

  // Initialize Socket.IO
  global.io = new SocketIO(httpServer, {
    cors: { origin: '*', methods: ['GET', 'POST'] },
    pingTimeout: 60000,
    pingInterval: 25000
  });

  // Socket.IO Connection Handler
  global.io.on('connection', (socket) => {
    console.log(`🔌 Chat client connected: ${socket.id}`);
    const hub = new ChatHub(socket);
    
    socket.on('Connect', async (data, cb) => { const r = await hub.connect(data); if (cb) cb(r); });
    socket.on('SendMessage', async (data, cb) => { const r = await hub.sendMessage(data); if (cb) cb(r); });
    socket.on('GetMessageHistory', async (data, cb) => { const r = await hub.getMessageHistory(data); if (cb) cb(r); });
    socket.on('StartTyping', (data) => hub.startTyping(data));
    socket.on('StopTyping', (data) => hub.stopTyping(data));
    socket.on('JoinRoom', async (data, cb) => { const r = await hub.joinRoom(data); if (cb) cb(r); });
    socket.on('CreateRoom', async (data, cb) => { const r = await hub.createRoom(data); if (cb) cb(r); });
    socket.on('UpdateStatus', (data) => hub.updateStatus(data));
    socket.on('disconnect', () => { console.log(`🔌 Chat client disconnected: ${socket.id}`); hub.disconnect(); });
  });

  // Plugins
  await fastify.register(cors, { origin: true, credentials: true });
  await fastify.register(formbody);

  // API Routes
  await fastify.register(shopRoutes);
  await fastify.register(adminRoutes, { prefix: '' });
  await fastify.register(posRoutes);
  await fastify.register(inventoryRoutes);
  await fastify.register(bankingRoutes);
  await fastify.register(monetaryRoutes);
  await fastify.register(backupRoutes);
  await fastify.register(globalBankingRoutes);
  await fastify.register(nodeRoutes);
  await fastify.register(servicesRoutes);

  // Admin Login
  fastify.post('/api/admin/login', async (req, reply) => {
    const { email, password } = req.body || {};
    if (!email || !password) return reply.code(400).send({ error: 'Email and password required' });
    
    const data = db.get();
    const u = (data.admin_users || []).find(x => x.email === email && x.is_active);
    
    if (!u) return reply.code(401).send({ error: 'Invalid credentials' });
    
    // Default admin
    if (email === 'admin@ierahkwa.gov' && password === 'admin123') {
      const role = (data.roles || []).find(r => r.id === u.role_id);
      const token = Buffer.from(`${email}:${password}`).toString('base64');
      
      // Update last login
      u.last_login = db.now();
      db.save();
      
      return { 
        ok: true, 
        token, 
        user: { 
          id: u.id, 
          email: u.email, 
          name: u.name,
          role: role?.name || 'Admin',
          permissions: role?.permissions || ['all']
        } 
      };
    }
    
    const bcrypt = (await import('bcryptjs')).default;
    if (!bcrypt.compareSync(password, u.password_hash)) {
      return reply.code(401).send({ error: 'Invalid credentials' });
    }
    
    const role = (data.roles || []).find(r => r.id === u.role_id);
    const token = Buffer.from(`${email}:${password}`).toString('base64');
    
    u.last_login = db.now();
    db.save();
    
    return { 
      ok: true, 
      token, 
      user: { 
        id: u.id, 
        email: u.email, 
        name: u.name,
        role: role?.name || '',
        permissions: role?.permissions || []
      } 
    };
  });

  // Health check
  fastify.get('/api/health', async () => ({
    status: 'ok',
    platform: 'Ierahkwa Futurehead Shop',
    version: '2.0.0',
    node: 'Ierahkwa Futurehead Mamey Node',
    blockchain: 'Ierahkwa Sovereign Blockchain',
    token: 'IGT-MARKET',
    chat: 'Socket.IO ChatHub Active',
    onlineUsers: onlineUsers.size,
    timestamp: db.now()
  }));

  // Chat API Routes
  fastify.get('/api/chat/rooms', async () => {
    const db = loadChatDb();
    return { success: true, rooms: db.rooms };
  });

  fastify.get('/api/chat/users', async () => {
    const db = loadChatDb();
    return { success: true, users: db.users };
  });

  fastify.get('/api/chat/messages/:roomId', async (req) => {
    const { roomId } = req.params;
    const { limit = 50 } = req.query;
    const db = loadChatDb();
    const messages = db.messages.filter(m => m.roomId === roomId).slice(-parseInt(limit));
    return { success: true, messages };
  });

  fastify.get('/api/chat/online', async () => {
    return { success: true, users: Array.from(onlineUsers.values()) };
  });

  // Serve static files
  const sendFile = (file) => (_, reply) => {
    const path = join(publicDir, file);
    if (existsSync(path)) {
      reply.type('text/html').send(createReadStream(path));
    } else {
      reply.code(404).send({ error: 'Not found' });
    }
  };

  fastify.get('/', sendFile('index.html'));
  fastify.get('/admin', sendFile('admin/index.html'));
  fastify.get('/admin/', sendFile('admin/index.html'));
  fastify.get('/admin/*', sendFile('admin/index.html'));
  fastify.get('/chat', sendFile('chat/index.html'));
  fastify.get('/chat/', sendFile('chat/index.html'));
  fastify.get('/pos', sendFile('pos/index.html'));
  fastify.get('/pos/', sendFile('pos/index.html'));
  fastify.get('/inventory', sendFile('inventory/index.html'));
  fastify.get('/inventory/', sendFile('inventory/index.html'));
  fastify.get('/inventory/*', sendFile('inventory/index.html'));
  fastify.get('/trading', sendFile('trading/index.html'));
  fastify.get('/trading/', sendFile('trading/index.html'));
  fastify.get('/trading/*', sendFile('trading/index.html'));
  fastify.get('/banking', sendFile('trading/index.html'));
  fastify.get('/banking/', sendFile('trading/index.html'));
  fastify.get('/exchange', sendFile('trading/index.html'));
  fastify.get('/exchange/', sendFile('trading/index.html'));
  fastify.get('/monetary', sendFile('monetary/index.html'));
  fastify.get('/monetary/', sendFile('monetary/index.html'));
  fastify.get('/monetary/*', sendFile('monetary/index.html'));
  fastify.get('/receive-funds', sendFile('monetary/index.html'));
  fastify.get('/humanitarian', sendFile('monetary/index.html'));
  fastify.get('/backup', sendFile('backup/index.html'));
  fastify.get('/backup/', sendFile('backup/index.html'));
  fastify.get('/backups', sendFile('backup/index.html'));
  fastify.get('/global-banking', sendFile('global-banking/index.html'));
  fastify.get('/global-banking/', sendFile('global-banking/index.html'));
  fastify.get('/clearinghouse', sendFile('global-banking/index.html'));
  fastify.get('/settlement', sendFile('global-banking/index.html'));
  fastify.get('/banks', sendFile('global-banking/index.html'));
  fastify.get('/node', sendFile('node/index.html'));
  fastify.get('/node/', sendFile('node/index.html'));
  fastify.get('/blockchain', sendFile('node/index.html'));
  fastify.get('/portal', sendFile('portal/index.html'));
  fastify.get('/portal/', sendFile('portal/index.html'));
  fastify.get('/platform', sendFile('portal/index.html'));
  fastify.get('/services', sendFile('portal/index.html'));
  
  // Serve tokens directory
  fastify.register(import('@fastify/static'), {
    root: join(__dirname, '..', 'tokens'),
    prefix: '/tokens/',
    decorateReply: false
  });
  fastify.get('/invoice.html', sendFile('invoice.html'));

  await fastify.register(fastifyStatic, { root: publicDir, prefix: '/' });

  // Start server with HTTP server for Socket.IO
  await fastify.ready();
  httpServer.listen(PORT, '0.0.0.0');
  
  console.log(`
╔═══════════════════════════════════════════════════════════════════════════════╗
║                                                                               ║
║   ██╗███████╗██████╗  █████╗ ██╗  ██╗██╗    ██╗██╗  ██╗ █████╗               ║
║   ██║██╔════╝██╔══██╗██╔══██╗██║  ██║██║    ██║██║ ██╔╝██╔══██╗              ║
║   ██║█████╗  ██████╔╝███████║███████║██║ █╗ ██║█████╔╝ ███████║              ║
║   ██║██╔══╝  ██╔══██╗██╔══██║██╔══██║██║███╗██║██╔═██╗ ██╔══██║              ║
║   ██║███████╗██║  ██║██║  ██║██║  ██║╚███╔███╔╝██║  ██╗██║  ██║              ║
║   ╚═╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝ ╚══╝╚══╝ ╚═╝  ╚═╝╚═╝  ╚═╝              ║
║                                                                               ║
║    FUTUREHEAD PLATFORM - E-COMMERCE + POS + INVENTORY + CHAT v3.0             ║
║                                                                               ║
╠═══════════════════════════════════════════════════════════════════════════════╣
║                                                                               ║
║   STATUS: ✓ LIVE AND OPERATIONAL                                              ║
║                                                                               ║
║   Node:       Ierahkwa Futurehead Mamey Node                                  ║
║   Blockchain: Ierahkwa Sovereign Blockchain (ISB)                             ║
║   Token:      IGT-MARKET                                                      ║
║   Bank:       Ierahkwa Futurehead BDET Bank                                   ║
║   ChatHub:    Socket.IO (SignalR-style) ACTIVE                                ║
║                                                                               ║
╠═══════════════════════════════════════════════════════════════════════════════╣
║                                                                               ║
║   ENDPOINTS:                                                                  ║
║                                                                               ║
║   → Shop:       http://localhost:${PORT}                                         ║
║   → Admin:      http://localhost:${PORT}/admin                                   ║
║   → POS:        http://localhost:${PORT}/pos                                     ║
║   → Inventory:  http://localhost:${PORT}/inventory                               ║
║   → Chat:       http://localhost:${PORT}/chat                                    ║
║   → API:        http://localhost:${PORT}/api                                     ║
║                                                                               ║
║   LOGIN:      admin@ierahkwa.gov / admin123                                   ║
║                                                                               ║
╠═══════════════════════════════════════════════════════════════════════════════╣
║                                                                               ║
║   E-COMMERCE FEATURES:                                                        ║
║   ✓ Products with variants, attributes, SKU, barcode generation               ║
║   ✓ Unlimited categories and subcategories                                    ║
║   ✓ Suppliers, brands, units management                                       ║
║   ✓ Inventory tracking with stock alerts                                      ║
║   ✓ Orders with full lifecycle management                                     ║
║   ✓ Customers with groups and loyalty                                         ║
║   ✓ Multi-language (EN, ES, FR, MOH)                                          ║
║                                                                               ║
║   CHAT FEATURES (SignalR-style):                                              ║
║   ✓ Real-time messaging with Socket.IO                                        ║
║   ✓ Typing indicators                                                         ║
║   ✓ Live online status (Online, Away, Busy)                                   ║
║   ✓ Multiple chat rooms/channels                                              ║
║   ✓ Message history persistence                                               ║
║   ✓ PWA support (iOS, Android, Windows)                                       ║
║   ✓ MVVM Architecture                                                         ║
║                                                                               ║
╠═══════════════════════════════════════════════════════════════════════════════╣
║   Sovereign Government of Ierahkwa Ne Kanienke • Office of the Prime Minister ║
╚═══════════════════════════════════════════════════════════════════════════════╝
  `);
}

main().catch((e) => { console.error(e); process.exit(1); });
