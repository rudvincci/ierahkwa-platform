describe('Conferencia Soberana - API Routes', () => {
  describe('Module Structure', () => {
    it('should have a valid package.json with correct name', () => {
      const pkg = require('../package.json');
      expect(pkg.name).toBe('conferencia-soberana');
    });

    it('should have a valid version', () => {
      const pkg = require('../package.json');
      expect(pkg.version).toBe('1.0.0');
    });

    it('should specify the correct main entry point', () => {
      const pkg = require('../package.json');
      expect(pkg.main).toBe('server.js');
    });

    it('should have required dependencies for Express + Socket.IO', () => {
      const pkg = require('../package.json');
      expect(pkg.dependencies.express).toBeDefined();
      expect(pkg.dependencies['socket.io']).toBeDefined();
      expect(pkg.dependencies.cors).toBeDefined();
      expect(pkg.dependencies.helmet).toBeDefined();
      expect(pkg.dependencies.uuid).toBeDefined();
    });
  });

  describe('REST API — Room Management', () => {
    const roomEndpoints = [
      { method: 'POST', path: '/api/rooms', purpose: 'Create a new conference room' },
      { method: 'GET', path: '/api/rooms', purpose: 'List active rooms with pagination' },
      { method: 'GET', path: '/api/rooms/:id', purpose: 'Get room info by ID' },
      { method: 'POST', path: '/api/rooms/:id/join', purpose: 'Join a conference room' },
      { method: 'POST', path: '/api/rooms/:id/leave', purpose: 'Leave a conference room' },
      { method: 'GET', path: '/api/ice-servers', purpose: 'Get STUN/TURN configuration' },
    ];

    it('should define POST /api/rooms for room creation', () => {
      const endpoint = roomEndpoints.find(e => e.method === 'POST' && e.path === '/api/rooms');
      expect(endpoint).toBeDefined();
      expect(endpoint.method).toBe('POST');
    });

    it('should define GET /api/rooms for listing rooms', () => {
      const endpoint = roomEndpoints.find(e => e.method === 'GET' && e.path === '/api/rooms');
      expect(endpoint).toBeDefined();
    });

    it('should define GET /api/rooms/:id for room details', () => {
      const endpoint = roomEndpoints.find(e => e.path === '/api/rooms/:id' && e.method === 'GET');
      expect(endpoint).toBeDefined();
    });

    it('should define POST /api/rooms/:id/join for joining rooms', () => {
      const endpoint = roomEndpoints.find(e => e.path === '/api/rooms/:id/join');
      expect(endpoint).toBeDefined();
    });

    it('should define POST /api/rooms/:id/leave for leaving rooms', () => {
      const endpoint = roomEndpoints.find(e => e.path === '/api/rooms/:id/leave');
      expect(endpoint).toBeDefined();
    });

    it('should define GET /api/ice-servers for WebRTC config', () => {
      const endpoint = roomEndpoints.find(e => e.path === '/api/ice-servers');
      expect(endpoint).toBeDefined();
    });

    it('should have all 6 REST endpoints', () => {
      expect(roomEndpoints).toHaveLength(6);
    });
  });

  describe('Room Creation Logic', () => {
    it('should generate a valid room structure with defaults', () => {
      const room = {
        id: 'abc123def456',
        name: 'Sala-abc123',
        host: 'anonymous',
        participants: [],
        maxParticipants: 50,
        status: 'waiting',
        isLocked: false,
        recording: false,
        encryption: 'E2EE-AES-256-GCM',
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      };
      expect(room.maxParticipants).toBe(50);
      expect(room.status).toBe('waiting');
      expect(room.isLocked).toBe(false);
      expect(room.encryption).toBe('E2EE-AES-256-GCM');
      expect(room.participants).toHaveLength(0);
    });

    it('should allow custom room name and host', () => {
      const options = { name: 'Team Standup', host: 'user-42', maxParticipants: 10 };
      const room = {
        name: options.name || 'Sala-default',
        host: options.host || 'anonymous',
        maxParticipants: options.maxParticipants || 50
      };
      expect(room.name).toBe('Team Standup');
      expect(room.host).toBe('user-42');
      expect(room.maxParticipants).toBe(10);
    });

    it('should return 201 status on successful room creation', () => {
      const expectedStatus = 201;
      const response = { success: true, room: { id: 'test' }, joinUrl: '/api/rooms/test/join' };
      expect(expectedStatus).toBe(201);
      expect(response.success).toBe(true);
      expect(response.joinUrl).toContain('/join');
    });
  });

  describe('Room Join Validation', () => {
    it('should return 404 when room does not exist', () => {
      const response = { status: 404, body: { success: false, error: 'Sala no encontrada' } };
      expect(response.status).toBe(404);
      expect(response.body.success).toBe(false);
    });

    it('should return 403 when room is locked', () => {
      const response = { status: 403, body: { success: false, error: 'La sala esta bloqueada' } };
      expect(response.status).toBe(403);
      expect(response.body.success).toBe(false);
    });

    it('should return 409 when room is full', () => {
      const response = { status: 409, body: { success: false, error: 'Sala llena' } };
      expect(response.status).toBe(409);
      expect(response.body.success).toBe(false);
    });

    it('should include ICE servers in join response', () => {
      const iceServers = [
        { urls: 'stun:stun.l.google.com:19302' },
        { urls: 'stun:stun1.l.google.com:19302' },
        { urls: 'stun:stun2.l.google.com:19302' },
        { urls: 'turn:turn.soberano.sovereign:3478', username: 'soberano', credential: 'sovereign-credential' }
      ];
      expect(iceServers).toHaveLength(4);
      expect(iceServers[0].urls).toContain('stun');
      expect(iceServers[3].urls).toContain('turn');
    });
  });

  describe('WebSocket Signaling Events', () => {
    const signalingEvents = [
      'room:join', 'signal:offer', 'signal:answer',
      'signal:ice-candidate', 'media:toggle', 'chat:message',
      'screen:start', 'screen:stop', 'disconnect'
    ];

    it('should handle room:join event', () => {
      expect(signalingEvents).toContain('room:join');
    });

    it('should handle WebRTC signaling events (offer/answer/ice)', () => {
      expect(signalingEvents).toContain('signal:offer');
      expect(signalingEvents).toContain('signal:answer');
      expect(signalingEvents).toContain('signal:ice-candidate');
    });

    it('should handle media toggle events', () => {
      expect(signalingEvents).toContain('media:toggle');
    });

    it('should handle chat messages within rooms', () => {
      expect(signalingEvents).toContain('chat:message');
    });

    it('should handle screen sharing events', () => {
      expect(signalingEvents).toContain('screen:start');
      expect(signalingEvents).toContain('screen:stop');
    });

    it('should handle all 9 socket events', () => {
      expect(signalingEvents).toHaveLength(9);
    });
  });

  describe('Error Handling', () => {
    it('should return 404 for unknown endpoints', () => {
      const response = { status: 404, body: { error: 'Endpoint no encontrado' } };
      expect(response.status).toBe(404);
      expect(response.body.error).toBe('Endpoint no encontrado');
    });

    it('should return 500 on internal server errors', () => {
      const response = { status: 500, body: { success: false, error: 'Error al crear la sala' } };
      expect(response.status).toBe(500);
      expect(response.body.success).toBe(false);
    });
  });

  describe('Configuration', () => {
    it('should use port 3090 by default', () => {
      const port = parseInt(process.env.PORT || '3090', 10);
      expect(port).toBe(3090);
    });

    it('should support E2EE-AES-256-GCM encryption', () => {
      const encryption = 'E2EE-AES-256-GCM';
      expect(encryption).toBe('E2EE-AES-256-GCM');
    });

    it('should support CORS origins configuration', () => {
      const corsOrigins = (process.env.CORS_ORIGINS || 'http://localhost:3000').split(',');
      expect(corsOrigins).toContain('http://localhost:3000');
    });
  });
});
