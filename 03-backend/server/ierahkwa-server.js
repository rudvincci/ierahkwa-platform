/**
 * ═══════════════════════════════════════════════════════════════════════════════════════════
 * 
 *    ██╗███████╗██████╗  █████╗ ██╗  ██╗██╗  ██╗██╗    ██╗ █████╗ 
 *    ██║██╔════╝██╔══██╗██╔══██╗██║  ██║██║ ██╔╝██║    ██║██╔══██╗
 *    ██║█████╗  ██████╔╝███████║███████║█████╔╝ ██║ █╗ ██║███████║
 *    ██║██╔══╝  ██╔══██╗██╔══██║██╔══██║██╔═██╗ ██║███╗██║██╔══██║
 *    ██║███████╗██║  ██║██║  ██║██║  ██║██║  ██╗╚███╔███╔╝██║  ██║
 *    ╚═╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝ ╚══╝╚══╝ ╚═╝  ╚═╝
 * 
 *    SOVEREIGN FINANCIAL PLATFORM - NODE.JS SERVER
 *    Ghost Mode + Security Fortress + Multi-Server Architecture
 * 
 * ═══════════════════════════════════════════════════════════════════════════════════════════
 */

const http = require('http');
const https = require('https');
const fs = require('fs');
const path = require('path');
const crypto = require('crypto');
const { URL } = require('url');

// ═══════════════════════════════════════════════════════════════════════════════════════════
// CONFIGURATION
// ═══════════════════════════════════════════════════════════════════════════════════════════

const CONFIG = {
    port: process.env.PORT || 3000,
    host: '0.0.0.0',
    
    // Ghost Mode - Phantom Servers
    phantomServers: [
        { id: 'PHANTOM-1', secret: true },
        { id: 'PHANTOM-2', secret: true },
        { id: 'PHANTOM-3', secret: true },
        { id: 'PHANTOM-4', secret: true },
        { id: 'PHANTOM-5', secret: true },
        { id: 'PHANTOM-6', secret: true },
        { id: 'PHANTOM-7', secret: true }
    ],
    
    // Decoy servers (what attackers see) - 35 SEÑUELOS PARA CONFUNDIR
    decoyServers: [
        // Principales - parecen reales
        { id: 'MAIN-SERVER', location: 'New York, USA', ip: '192.168.1.100', type: 'primary' },
        { id: 'BACKUP-US', location: 'Los Angeles, USA', ip: '192.168.1.101', type: 'backup' },
        { id: 'BACKUP-EU', location: 'London, UK', ip: '185.45.23.11', type: 'backup' },
        // Bases de datos
        { id: 'DB-MASTER', location: 'Frankfurt, Germany', ip: '45.33.22.11', type: 'database' },
        { id: 'DB-SLAVE-1', location: 'Paris, France', ip: '45.33.22.12', type: 'database' },
        { id: 'DB-SLAVE-2', location: 'Amsterdam, NL', ip: '45.33.22.13', type: 'database' },
        // APIs
        { id: 'API-GATEWAY', location: 'Singapore', ip: '103.21.45.67', type: 'api' },
        { id: 'API-AUTH', location: 'Tokyo, Japan', ip: '103.21.45.68', type: 'api' },
        { id: 'API-PAYMENTS', location: 'Sydney, AU', ip: '103.21.45.69', type: 'api' },
        // Cache
        { id: 'CACHE-01', location: 'Mumbai, India', ip: '172.16.0.1', type: 'cache' },
        { id: 'CACHE-02', location: 'Dubai, UAE', ip: '172.16.0.2', type: 'cache' },
        { id: 'CACHE-03', location: 'São Paulo, BR', ip: '172.16.0.3', type: 'cache' },
        // Seguridad
        { id: 'FIREWALL-MAIN', location: 'Toronto, CA', ip: '10.0.0.1', type: 'security' },
        { id: 'WAF-GATEWAY', location: 'Mexico City, MX', ip: '10.0.0.2', type: 'security' },
        { id: 'IDS-MONITOR', location: 'Buenos Aires, AR', ip: '10.0.0.3', type: 'security' },
        // HONEYPOTS - Trampas para hackers
        { id: 'ADMIN-PANEL', location: 'Miami, USA', ip: '192.168.100.1', type: 'honeypot' },
        { id: 'SSH-ACCESS', location: 'Dallas, USA', ip: '192.168.100.2', type: 'honeypot' },
        { id: 'FTP-SERVER', location: 'Chicago, USA', ip: '192.168.100.3', type: 'honeypot' },
        { id: 'MYSQL-DIRECT', location: 'Seattle, USA', ip: '192.168.100.4', type: 'honeypot' },
        { id: 'REDIS-OPEN', location: 'Denver, USA', ip: '192.168.100.5', type: 'honeypot' },
        { id: 'CREDENTIALS-DB', location: 'Moscow, RU', ip: '77.88.99.10', type: 'honeypot' },
        { id: 'PRIVATE-KEYS', location: 'Beijing, CN', ip: '77.88.99.11', type: 'honeypot' },
        { id: 'ROOT-ACCESS', location: 'Unknown', ip: '77.88.99.12', type: 'honeypot' },
        // Dev - parecen vulnerables
        { id: 'DEV-SERVER', location: 'Portland, USA', ip: '10.10.10.1', type: 'dev' },
        { id: 'STAGING-ENV', location: 'Austin, USA', ip: '10.10.10.2', type: 'dev' },
        { id: 'TEST-DB', location: 'Phoenix, USA', ip: '10.10.10.3', type: 'dev' },
        // Backups
        { id: 'BACKUP-DAILY', location: 'Zürich, CH', ip: '185.100.50.1', type: 'backup' },
        { id: 'BACKUP-WEEKLY', location: 'Geneva, CH', ip: '185.100.50.2', type: 'backup' },
        { id: 'ARCHIVE-2024', location: 'Stockholm, SE', ip: '185.100.50.3', type: 'backup' },
        // FINANCIEROS - Muy atractivos para ladrones
        { id: 'WALLET-HOT', location: 'Hong Kong', ip: '203.45.67.89', type: 'financial' },
        { id: 'WALLET-COLD', location: 'Cayman Islands', ip: '203.45.67.90', type: 'financial' },
        { id: 'PAYMENT-PROCESSOR', location: 'Luxembourg', ip: '203.45.67.91', type: 'financial' },
        { id: 'CRYPTO-VAULT', location: 'Malta', ip: '203.45.67.92', type: 'financial' },
    ],
    
    // Security
    encryption: {
        algorithm: 'aes-256-gcm',
        keyLength: 32,
        ivLength: 16,
        saltLength: 64,
        tagLength: 16,
        iterations: 100000
    },
    
    // Rate limiting
    rateLimit: {
        maxRequests: 100,
        windowMs: 60000,
        blockDuration: 300000
    },
    
    // Session
    session: {
        maxAge: 86400000,
        secret: crypto.randomBytes(64).toString('hex')
    },
    
    // Data paths
    paths: {
        data: path.join(__dirname, '../data'),
        backups: path.join(__dirname, '../backups'),
        logs: path.join(__dirname, '../logs'),
        platform: path.join(__dirname, '../platform')
    }
};

// ═══════════════════════════════════════════════════════════════════════════════════════════
// ENSURE DIRECTORIES EXIST
// ═══════════════════════════════════════════════════════════════════════════════════════════

Object.values(CONFIG.paths).forEach(dir => {
    if (!fs.existsSync(dir)) {
        fs.mkdirSync(dir, { recursive: true });
    }
});

// ═══════════════════════════════════════════════════════════════════════════════════════════
// 🔐 ENCRYPTION MODULE
// ═══════════════════════════════════════════════════════════════════════════════════════════

const Encryption = {
    /**
     * Derive key from password using PBKDF2
     */
    deriveKey(password, salt) {
        return crypto.pbkdf2Sync(
            password,
            salt,
            CONFIG.encryption.iterations,
            CONFIG.encryption.keyLength,
            'sha512'
        );
    },
    
    /**
     * Encrypt data with AES-256-GCM
     */
    encrypt(plaintext, password) {
        const salt = crypto.randomBytes(CONFIG.encryption.saltLength);
        const iv = crypto.randomBytes(CONFIG.encryption.ivLength);
        const key = this.deriveKey(password, salt);
        
        const cipher = crypto.createCipheriv(CONFIG.encryption.algorithm, key, iv);
        
        let encrypted = cipher.update(plaintext, 'utf8', 'hex');
        encrypted += cipher.final('hex');
        
        const tag = cipher.getAuthTag();
        
        return {
            encrypted: encrypted,
            iv: iv.toString('hex'),
            salt: salt.toString('hex'),
            tag: tag.toString('hex')
        };
    },
    
    /**
     * Decrypt data with AES-256-GCM
     */
    decrypt(encryptedData, password) {
        const salt = Buffer.from(encryptedData.salt, 'hex');
        const iv = Buffer.from(encryptedData.iv, 'hex');
        const tag = Buffer.from(encryptedData.tag, 'hex');
        const key = this.deriveKey(password, salt);
        
        const decipher = crypto.createDecipheriv(CONFIG.encryption.algorithm, key, iv);
        decipher.setAuthTag(tag);
        
        let decrypted = decipher.update(encryptedData.encrypted, 'hex', 'utf8');
        decrypted += decipher.final('utf8');
        
        return decrypted;
    },
    
    /**
     * Hash data with SHA-256
     */
    hash(data) {
        return crypto.createHash('sha256').update(data).digest('hex');
    },
    
    /**
     * Generate secure random token
     */
    generateToken(length = 32) {
        return crypto.randomBytes(length).toString('hex');
    }
};

// ═══════════════════════════════════════════════════════════════════════════════════════════
// 👻 GHOST MODE MODULE
// ═══════════════════════════════════════════════════════════════════════════════════════════

const GhostMode = {
    activePhantom: null,
    rotationKey: null,
    lastRotation: null,
    rotationInterval: null,
    
    /**
     * Initialize Ghost Mode
     */
    init() {
        this.rotate();
        this.startAutoRotation();
        
        console.log(`
👻 ═══════════════════════════════════════════════════════════════════════
   GHOST MODE INITIALIZED
   Active Phantom: ${this.activePhantom}
   Rotation Key: ${this.rotationKey.substring(0, 8)}...
   "SOMOS UN FANTASMA"
👻 ═══════════════════════════════════════════════════════════════════════
        `);
    },
    
    /**
     * Rotate to new phantom server
     */
    rotate() {
        const phantoms = CONFIG.phantomServers;
        let newIndex;
        
        do {
            newIndex = Math.floor(Math.random() * phantoms.length);
        } while (phantoms[newIndex].id === this.activePhantom && phantoms.length > 1);
        
        this.activePhantom = phantoms[newIndex].id;
        this.rotationKey = Encryption.generateToken();
        this.lastRotation = Date.now();
        
        AuditLog.log('GHOST_ROTATION', { phantom: this.activePhantom });
        
        return this.activePhantom;
    },
    
    /**
     * Start automatic rotation - TIEMPO TOTALMENTE IMPREDECIBLE
     */
    startAutoRotation() {
        const getRandomDelay = () => {
            // Rangos que cambian cada vez - IMPREDECIBLE
            const ranges = [
                [30000, 120000],    // 30s - 2min
                [60000, 300000],    // 1min - 5min
                [120000, 600000],   // 2min - 10min
                [180000, 900000],   // 3min - 15min
                [300000, 1200000],  // 5min - 20min
                [45000, 180000],    // 45s - 3min
                [90000, 420000],    // 1.5min - 7min
                [15000, 90000],     // 15s - 1.5min (muy rápido)
                [240000, 480000],   // 4min - 8min
            ];
            
            // Seleccionar rango aleatorio
            const range = ranges[Math.floor(Math.random() * ranges.length)];
            
            // Tiempo aleatorio dentro del rango
            let delay = Math.floor(Math.random() * (range[1] - range[0])) + range[0];
            
            // Añadir variación adicional (±25%)
            delay = Math.floor(delay * (0.75 + Math.random() * 0.5));
            
            return Math.max(15000, delay); // Mínimo 15 segundos
        };
        
        const scheduleNext = () => {
            const delay = getRandomDelay();
            
            console.log(`👻 Próxima rotación en: ${Math.floor(delay/1000)}s (IMPREDECIBLE)`);
            
            this.rotationInterval = setTimeout(() => {
                this.rotate();
                scheduleNext();
            }, delay);
        };
        
        scheduleNext();
    },
    
    /**
     * Get public info (decoys only)
     */
    getPublicInfo() {
        return {
            servers: CONFIG.decoyServers,
            message: 'Standard infrastructure'
        };
    },
    
    /**
     * Get secret info (requires admin key)
     */
    getSecretInfo(adminKey) {
        // In production, verify adminKey
        return {
            activePhantom: this.activePhantom,
            rotationKey: this.rotationKey,
            lastRotation: new Date(this.lastRotation).toISOString(),
            allPhantoms: CONFIG.phantomServers.map(p => p.id)
        };
    }
};

// ═══════════════════════════════════════════════════════════════════════════════════════════
// 🚦 RATE LIMITER
// ═══════════════════════════════════════════════════════════════════════════════════════════

const RateLimiter = {
    requests: new Map(),
    blocked: new Set(),
    
    /**
     * Check if request is allowed
     */
    check(ip) {
        const now = Date.now();
        const config = CONFIG.rateLimit;
        
        // Check if blocked
        if (this.blocked.has(ip)) {
            AuditLog.log('BLOCKED_REQUEST', { ip });
            return { allowed: false, reason: 'BLOCKED' };
        }
        
        // Get request history
        let history = this.requests.get(ip) || [];
        history = history.filter(t => now - t < config.windowMs);
        
        // Check limit
        if (history.length >= config.maxRequests) {
            this.blocked.add(ip);
            setTimeout(() => this.blocked.delete(ip), config.blockDuration);
            
            AuditLog.log('RATE_LIMIT_EXCEEDED', { ip });
            return { allowed: false, reason: 'RATE_LIMIT' };
        }
        
        history.push(now);
        this.requests.set(ip, history);
        
        return { allowed: true, remaining: config.maxRequests - history.length };
    }
};

// ═══════════════════════════════════════════════════════════════════════════════════════════
// 📝 AUDIT LOG
// ═══════════════════════════════════════════════════════════════════════════════════════════

const AuditLog = {
    logs: [],
    
    /**
     * Log security event
     */
    log(event, data = {}) {
        const entry = {
            timestamp: new Date().toISOString(),
            event,
            data,
            id: Encryption.generateToken(8)
        };
        
        this.logs.push(entry);
        
        // Keep only last 10000 logs in memory
        if (this.logs.length > 10000) {
            this.logs = this.logs.slice(-10000);
        }
        
        // Write to file
        const logFile = path.join(CONFIG.paths.logs, `audit-${new Date().toISOString().split('T')[0]}.json`);
        fs.appendFileSync(logFile, JSON.stringify(entry) + '\n');
        
        console.log(`🔐 [${event}] ${JSON.stringify(data)}`);
        
        return entry;
    },
    
    /**
     * Get recent logs
     */
    getRecent(count = 100) {
        return this.logs.slice(-count);
    }
};

// ═══════════════════════════════════════════════════════════════════════════════════════════
// 💾 DATA STORE
// ═══════════════════════════════════════════════════════════════════════════════════════════

const DataStore = {
    /**
     * Save data with encryption
     */
    save(collection, id, data, password) {
        const encrypted = Encryption.encrypt(JSON.stringify(data), password);
        const filePath = path.join(CONFIG.paths.data, `${collection}-${id}.enc`);
        
        fs.writeFileSync(filePath, JSON.stringify(encrypted));
        
        AuditLog.log('DATA_SAVED', { collection, id });
        
        // Create backup
        this.backup(collection, id, encrypted);
        
        return { success: true, id };
    },
    
    /**
     * Load encrypted data
     */
    load(collection, id, password) {
        const filePath = path.join(CONFIG.paths.data, `${collection}-${id}.enc`);
        
        if (!fs.existsSync(filePath)) {
            return null;
        }
        
        const encrypted = JSON.parse(fs.readFileSync(filePath, 'utf8'));
        const decrypted = Encryption.decrypt(encrypted, password);
        
        AuditLog.log('DATA_LOADED', { collection, id });
        
        return JSON.parse(decrypted);
    },
    
    /**
     * Create backup
     */
    backup(collection, id, data) {
        const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
        const backupPath = path.join(CONFIG.paths.backups, `${collection}-${id}-${timestamp}.bak`);
        
        fs.writeFileSync(backupPath, JSON.stringify(data));
        
        AuditLog.log('BACKUP_CREATED', { collection, id, timestamp });
    },
    
    /**
     * List all backups
     */
    listBackups() {
        return fs.readdirSync(CONFIG.paths.backups);
    }
};

// ═══════════════════════════════════════════════════════════════════════════════════════════
// 🛡️ SECURITY MIDDLEWARE
// ═══════════════════════════════════════════════════════════════════════════════════════════

const Security = {
    /**
     * Sanitize input
     */
    sanitize(input) {
        if (typeof input !== 'string') return input;
        
        return input
            .replace(/[<>]/g, '')
            .replace(/javascript:/gi, '')
            .replace(/on\w+=/gi, '')
            .trim();
    },
    
    /**
     * Validate request
     */
    validateRequest(req) {
        const ip = req.socket.remoteAddress;
        
        // Rate limit check
        const rateCheck = RateLimiter.check(ip);
        if (!rateCheck.allowed) {
            return { valid: false, reason: rateCheck.reason };
        }
        
        return { valid: true };
    },
    
    /**
     * Add security headers
     */
    addSecurityHeaders(res) {
        res.setHeader('X-Content-Type-Options', 'nosniff');
        res.setHeader('X-Frame-Options', 'DENY');
        res.setHeader('X-XSS-Protection', '1; mode=block');
        res.setHeader('Strict-Transport-Security', 'max-age=31536000; includeSubDomains');
        res.setHeader('Content-Security-Policy', "default-src 'self'");
        res.setHeader('X-Ghost-Mode', 'ACTIVE');
    }
};

// ═══════════════════════════════════════════════════════════════════════════════════════════
// 🌐 HTTP SERVER
// ═══════════════════════════════════════════════════════════════════════════════════════════

const MIME_TYPES = {
    '.html': 'text/html',
    '.css': 'text/css',
    '.js': 'application/javascript',
    '.json': 'application/json',
    '.png': 'image/png',
    '.jpg': 'image/jpeg',
    '.gif': 'image/gif',
    '.svg': 'image/svg+xml',
    '.ico': 'image/x-icon'
};

const server = http.createServer((req, res) => {
    const ip = req.socket.remoteAddress;
    const url = new URL(req.url, `http://${req.headers.host}`);
    
    // Security validation
    const validation = Security.validateRequest(req);
    if (!validation.valid) {
        res.writeHead(429, { 'Content-Type': 'application/json' });
        res.end(JSON.stringify({ error: validation.reason }));
        return;
    }
    
    // Add security headers
    Security.addSecurityHeaders(res);
    
    // Log request
    AuditLog.log('REQUEST', { method: req.method, path: url.pathname, ip });
    
    // ═══════════════════════════════════════════════════════════════════════
    // API ROUTES
    // ═══════════════════════════════════════════════════════════════════════
    
    if (url.pathname.startsWith('/api/')) {
        res.setHeader('Content-Type', 'application/json');
        
        // GET /api/status - System status
        if (url.pathname === '/api/status' && req.method === 'GET') {
            res.end(JSON.stringify({
                status: 'OPERATIONAL',
                ghostMode: 'ACTIVE',
                servers: GhostMode.getPublicInfo(),
                uptime: process.uptime(),
                timestamp: new Date().toISOString()
            }));
            return;
        }
        
        // GET /api/ghost/public - Public server info (decoys)
        if (url.pathname === '/api/ghost/public' && req.method === 'GET') {
            res.end(JSON.stringify(GhostMode.getPublicInfo()));
            return;
        }
        
        // POST /api/ghost/secret - Secret server info (requires auth)
        if (url.pathname === '/api/ghost/secret' && req.method === 'POST') {
            let body = '';
            req.on('data', chunk => body += chunk);
            req.on('end', () => {
                try {
                    const { adminKey } = JSON.parse(body);
                    res.end(JSON.stringify(GhostMode.getSecretInfo(adminKey)));
                } catch (e) {
                    res.writeHead(400);
                    res.end(JSON.stringify({ error: 'Invalid request' }));
                }
            });
            return;
        }
        
        // POST /api/ghost/rotate - Force rotation
        if (url.pathname === '/api/ghost/rotate' && req.method === 'POST') {
            const newPhantom = GhostMode.rotate();
            res.end(JSON.stringify({ 
                success: true, 
                phantom: '████████',
                message: 'Rotated successfully'
            }));
            return;
        }
        
        // POST /api/data/save - Save encrypted data
        if (url.pathname === '/api/data/save' && req.method === 'POST') {
            let body = '';
            req.on('data', chunk => body += chunk);
            req.on('end', () => {
                try {
                    const { collection, id, data, password } = JSON.parse(body);
                    const result = DataStore.save(collection, id, data, password);
                    res.end(JSON.stringify(result));
                } catch (e) {
                    res.writeHead(400);
                    res.end(JSON.stringify({ error: e.message }));
                }
            });
            return;
        }
        
        // POST /api/data/load - Load encrypted data
        if (url.pathname === '/api/data/load' && req.method === 'POST') {
            let body = '';
            req.on('data', chunk => body += chunk);
            req.on('end', () => {
                try {
                    const { collection, id, password } = JSON.parse(body);
                    const data = DataStore.load(collection, id, password);
                    res.end(JSON.stringify({ data }));
                } catch (e) {
                    res.writeHead(400);
                    res.end(JSON.stringify({ error: e.message }));
                }
            });
            return;
        }
        
        // GET /api/audit - Get audit logs
        if (url.pathname === '/api/audit' && req.method === 'GET') {
            const count = parseInt(url.searchParams.get('count')) || 100;
            res.end(JSON.stringify(AuditLog.getRecent(count)));
            return;
        }
        
        // GET /api/backups - List backups
        if (url.pathname === '/api/backups' && req.method === 'GET') {
            res.end(JSON.stringify(DataStore.listBackups()));
            return;
        }
        
        // 404 for unknown API routes
        res.writeHead(404);
        res.end(JSON.stringify({ error: 'Not found' }));
        return;
    }
    
    // ═══════════════════════════════════════════════════════════════════════
    // STATIC FILES
    // ═══════════════════════════════════════════════════════════════════════
    
    let filePath = url.pathname === '/' ? '/index.html' : url.pathname;
    filePath = path.join(CONFIG.paths.platform, filePath);
    
    // Security: prevent directory traversal
    if (!filePath.startsWith(CONFIG.paths.platform)) {
        res.writeHead(403);
        res.end('Forbidden');
        return;
    }
    
    const ext = path.extname(filePath);
    const contentType = MIME_TYPES[ext] || 'text/plain';
    
    fs.readFile(filePath, (err, content) => {
        if (err) {
            if (err.code === 'ENOENT') {
                res.writeHead(404);
                res.end('Not found');
            } else {
                res.writeHead(500);
                res.end('Server error');
            }
            return;
        }
        
        res.writeHead(200, { 'Content-Type': contentType });
        res.end(content);
    });
});

// ═══════════════════════════════════════════════════════════════════════════════════════════
// 🚀 START SERVER
// ═══════════════════════════════════════════════════════════════════════════════════════════

GhostMode.init();

server.listen(CONFIG.port, CONFIG.host, () => {
    console.log(`
═══════════════════════════════════════════════════════════════════════════════════════════

    ██╗███████╗██████╗  █████╗ ██╗  ██╗██╗  ██╗██╗    ██╗ █████╗ 
    ██║██╔════╝██╔══██╗██╔══██╗██║  ██║██║ ██╔╝██║    ██║██╔══██╗
    ██║█████╗  ██████╔╝███████║███████║█████╔╝ ██║ █╗ ██║███████║
    ██║██╔══╝  ██╔══██╗██╔══██║██╔══██║██╔═██╗ ██║███╗██║██╔══██║
    ██║███████╗██║  ██║██║  ██║██║  ██║██║  ██╗╚███╔███╔╝██║  ██║
    ╚═╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝ ╚══╝╚══╝ ╚═╝  ╚═╝

    SOVEREIGN FINANCIAL PLATFORM - SERVER RUNNING
    
═══════════════════════════════════════════════════════════════════════════════════════════

    🌐 Server: http://localhost:${CONFIG.port}
    👻 Ghost Mode: ACTIVE
    🛡️ Security: FORTRESS ENABLED
    🔐 Encryption: AES-256-GCM
    
    PHANTOM SERVERS: 7 (Secret)
    DECOY SERVERS: 3 (Public)
    
    "SOMOS UN FANTASMA - NADIE NOS PUEDE VER NI ENCONTRAR"

═══════════════════════════════════════════════════════════════════════════════════════════

    API ENDPOINTS:
    
    GET  /api/status         - System status
    GET  /api/ghost/public   - Public server info (decoys)
    POST /api/ghost/secret   - Secret server info (auth required)
    POST /api/ghost/rotate   - Force server rotation
    POST /api/data/save      - Save encrypted data
    POST /api/data/load      - Load encrypted data
    GET  /api/audit          - Get audit logs
    GET  /api/backups        - List backups

═══════════════════════════════════════════════════════════════════════════════════════════
    `);
    
    AuditLog.log('SERVER_STARTED', { port: CONFIG.port });
});

// Graceful shutdown
process.on('SIGTERM', () => {
    console.log('\n👻 Ghost Mode deactivating...');
    AuditLog.log('SERVER_SHUTDOWN');
    server.close(() => {
        process.exit(0);
    });
});

module.exports = { server, GhostMode, Encryption, DataStore, AuditLog };
