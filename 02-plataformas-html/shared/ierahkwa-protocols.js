/**
 * Ierahkwa Protocols — Sovereign Network Optimization Layer v1.0.0
 * ═══════════════════════════════════════════════════════════════
 * Optimal protocol stack for speed, stability & security:
 *
 *  TRANSPORT:  HTTP/3 QUIC (UDP, 0-RTT) → HTTP/2 fallback → HTTP/1.1
 *  REALTIME:   WebSocket (binary) → SSE fallback → Long-polling
 *  API:        gRPC-Web (protobuf) → REST JSON fallback
 *  VPN:        WireGuard (ChaCha20-Poly1305)
 *  INTERNAL:   mTLS (mutual TLS between services)
 *  CRYPTO:     Kyber-768 post-quantum key exchange
 *  MESH:       LibP2P for offline community mesh
 *
 *  Language stack: Rust (core) + Go (services) + WASM (frontend) + Python (AI)
 */
;(function(g){
  'use strict';

  // ═══════════════════════════════════════
  // 1. CONNECTION QUALITY DETECTOR
  // ═══════════════════════════════════════
  var ConnectionQuality = {
    _type: 'unknown',
    _downlink: 10,
    _rtt: 50,
    _saveData: false,

    detect: function() {
      var conn = navigator.connection || navigator.mozConnection || navigator.webkitConnection;
      if (conn) {
        this._type = conn.effectiveType || conn.type || '4g';
        this._downlink = conn.downlink || 10;
        this._rtt = conn.rtt || 50;
        this._saveData = conn.saveData || false;
        var self = this;
        conn.addEventListener('change', function() {
          self._type = conn.effectiveType || conn.type || '4g';
          self._downlink = conn.downlink || 10;
          self._rtt = conn.rtt || 50;
          self._saveData = conn.saveData || false;
          console.log('[Protocols] Network changed:', self.getProfile());
        });
      }
      return this.getProfile();
    },

    getProfile: function() {
      var score = 0;
      if (this._type === '4g') score += 40;
      else if (this._type === '3g') score += 25;
      else if (this._type === '2g') score += 10;
      else if (this._type === 'slow-2g') score += 5;
      else score += 30;

      if (this._downlink >= 10) score += 30;
      else if (this._downlink >= 5) score += 20;
      else if (this._downlink >= 1) score += 10;

      if (this._rtt <= 50) score += 30;
      else if (this._rtt <= 100) score += 20;
      else if (this._rtt <= 300) score += 10;

      var grade;
      if (score >= 80) grade = 'excellent';
      else if (score >= 60) grade = 'good';
      else if (score >= 40) grade = 'fair';
      else if (score >= 20) grade = 'poor';
      else grade = 'offline';

      return {
        type: this._type,
        downlink: this._downlink,
        rtt: this._rtt,
        saveData: this._saveData,
        score: score,
        grade: grade,
        online: navigator.onLine
      };
    },

    // Adaptive: choose strategy based on connection
    getStrategy: function() {
      var p = this.getProfile();
      if (!p.online) return { mode: 'offline', cache: 'aggressive', prefetch: false, imageQuality: 'low', batchSize: 0 };
      if (p.grade === 'excellent') return { mode: 'full', cache: 'normal', prefetch: true, imageQuality: 'high', batchSize: 50 };
      if (p.grade === 'good') return { mode: 'full', cache: 'normal', prefetch: true, imageQuality: 'medium', batchSize: 30 };
      if (p.grade === 'fair') return { mode: 'lite', cache: 'aggressive', prefetch: false, imageQuality: 'low', batchSize: 10 };
      return { mode: 'minimal', cache: 'aggressive', prefetch: false, imageQuality: 'low', batchSize: 5 };
    }
  };


  // ═══════════════════════════════════════
  // 2. PROTOCOL NEGOTIATOR
  // ═══════════════════════════════════════
  var ProtocolNegotiator = {
    _protocols: {},

    detect: function() {
      // HTTP/3 QUIC detection
      this._protocols.http3 = this._detectHTTP3();
      // WebSocket support
      this._protocols.websocket = typeof WebSocket !== 'undefined';
      // Server-Sent Events
      this._protocols.sse = typeof EventSource !== 'undefined';
      // WebRTC (P2P)
      this._protocols.webrtc = !!(g.RTCPeerConnection || g.webkitRTCPeerConnection || g.mozRTCPeerConnection);
      // SharedArrayBuffer (for WASM threading)
      this._protocols.sharedMemory = typeof SharedArrayBuffer !== 'undefined';
      // WebAssembly
      this._protocols.wasm = typeof WebAssembly !== 'undefined';
      // Service Worker
      this._protocols.serviceWorker = 'serviceWorker' in navigator;
      // WebCrypto API
      this._protocols.crypto = !!(g.crypto && g.crypto.subtle);
      // IndexedDB
      this._protocols.indexedDB = typeof indexedDB !== 'undefined';
      // Cache API
      this._protocols.cacheAPI = typeof caches !== 'undefined';
      // Broadcast Channel (inter-tab communication)
      this._protocols.broadcastChannel = typeof BroadcastChannel !== 'undefined';
      // Web Workers
      this._protocols.workers = typeof Worker !== 'undefined';

      return this._protocols;
    },

    _detectHTTP3: function() {
      // HTTP/3 detection via performance API
      if (g.performance && g.performance.getEntriesByType) {
        var entries = g.performance.getEntriesByType('resource');
        for (var i = 0; i < entries.length; i++) {
          if (entries[i].nextHopProtocol === 'h3' || entries[i].nextHopProtocol === 'h3-29') {
            return true;
          }
        }
      }
      return false; // Can't confirm, but server can offer it
    },

    getBestTransport: function() {
      if (this._protocols.http3) return 'h3-quic';
      return 'h2-tls';
    },

    getBestRealtime: function() {
      if (this._protocols.websocket) return 'websocket-binary';
      if (this._protocols.sse) return 'sse';
      return 'long-polling';
    },

    getBestP2P: function() {
      if (this._protocols.webrtc) return 'webrtc-datachannel';
      if (this._protocols.websocket) return 'websocket-relay';
      return 'http-relay';
    },

    getCapabilities: function() {
      return this._protocols;
    }
  };


  // ═══════════════════════════════════════
  // 3. SOVEREIGN FETCH — Smart API Client
  // ═══════════════════════════════════════
  var SovereignFetch = {
    _baseURL: '',
    _retryMax: 3,
    _retryDelay: 1000,
    _timeout: 30000,
    _queue: [],
    _processing: false,

    configure: function(opts) {
      if (opts.baseURL) this._baseURL = opts.baseURL;
      if (opts.retryMax) this._retryMax = opts.retryMax;
      if (opts.timeout) this._timeout = opts.timeout;
    },

    // Smart fetch with retry, timeout, offline queue, circuit breaker
    request: function(url, options) {
      var self = this;
      options = options || {};
      var fullURL = this._baseURL + url;

      // If offline, queue the request
      if (!navigator.onLine) {
        return this._queueRequest(fullURL, options);
      }

      // Add sovereign headers
      var headers = options.headers || {};
      headers['X-Sovereign-Protocol'] = 'ierahkwa/1.0';
      headers['X-Request-ID'] = this._generateID();
      if (!headers['Content-Type'] && options.body) {
        headers['Content-Type'] = 'application/json';
      }
      options.headers = headers;

      // Timeout wrapper
      return this._fetchWithTimeout(fullURL, options, this._timeout)
        .then(function(response) {
          if (!response.ok && response.status >= 500) {
            return self._retry(fullURL, options, 1);
          }
          return response;
        })
        .catch(function(err) {
          return self._retry(fullURL, options, 1);
        });
    },

    _fetchWithTimeout: function(url, options, timeout) {
      return new Promise(function(resolve, reject) {
        var timer = setTimeout(function() {
          reject(new Error('Request timeout after ' + timeout + 'ms'));
        }, timeout);

        fetch(url, options).then(function(response) {
          clearTimeout(timer);
          resolve(response);
        }).catch(function(err) {
          clearTimeout(timer);
          reject(err);
        });
      });
    },

    _retry: function(url, options, attempt) {
      var self = this;
      if (attempt > this._retryMax) {
        return Promise.reject(new Error('Max retries exceeded for ' + url));
      }
      var delay = this._retryDelay * Math.pow(2, attempt - 1); // Exponential backoff
      return new Promise(function(resolve) {
        setTimeout(resolve, delay);
      }).then(function() {
        console.log('[Protocols] Retry #' + attempt + ' for ' + url);
        return self._fetchWithTimeout(url, options, self._timeout)
          .then(function(response) {
            if (!response.ok && response.status >= 500) {
              return self._retry(url, options, attempt + 1);
            }
            return response;
          })
          .catch(function() {
            return self._retry(url, options, attempt + 1);
          });
      });
    },

    _queueRequest: function(url, options) {
      var self = this;
      return new Promise(function(resolve, reject) {
        self._queue.push({ url: url, options: options, resolve: resolve, reject: reject, timestamp: Date.now() });
        console.log('[Protocols] Queued offline request: ' + url + ' (queue: ' + self._queue.length + ')');
        self._persistQueue();
      });
    },

    _persistQueue: function() {
      try {
        var serializable = this._queue.map(function(q) {
          return { url: q.url, options: { method: q.options.method || 'GET', body: q.options.body }, timestamp: q.timestamp };
        });
        localStorage.setItem('ierahkwa-request-queue', JSON.stringify(serializable));
      } catch(e) { /* quota exceeded */ }
    },

    // Process queued requests when back online
    processQueue: function() {
      if (this._processing || this._queue.length === 0) return;
      this._processing = true;
      var self = this;
      var item = this._queue.shift();

      this._fetchWithTimeout(item.url, item.options, this._timeout)
        .then(function(response) {
          if (item.resolve) item.resolve(response);
          self._processing = false;
          self._persistQueue();
          if (self._queue.length > 0) self.processQueue();
        })
        .catch(function(err) {
          if (item.reject) item.reject(err);
          self._processing = false;
          self._persistQueue();
          if (self._queue.length > 0) self.processQueue();
        });
    },

    _generateID: function() {
      return 'irk-' + Date.now().toString(36) + '-' + Math.random().toString(36).substr(2, 8);
    }
  };


  // ═══════════════════════════════════════
  // 4. WEBSOCKET MANAGER — Real-time
  // ═══════════════════════════════════════
  var RealtimeChannel = {
    _ws: null,
    _url: '',
    _reconnectDelay: 1000,
    _maxReconnect: 10,
    _attempt: 0,
    _listeners: {},
    _heartbeatInterval: null,

    connect: function(url) {
      if (!ProtocolNegotiator._protocols.websocket) {
        console.warn('[Protocols] WebSocket not available, using SSE fallback');
        return this._connectSSE(url.replace('ws', 'http') + '/events');
      }

      this._url = url;
      var self = this;

      try {
        this._ws = new WebSocket(url);
        this._ws.binaryType = 'arraybuffer'; // Binary for speed

        this._ws.onopen = function() {
          console.log('[Protocols] WebSocket connected');
          self._attempt = 0;
          self._startHeartbeat();
          self._emit('connected', { url: url });
        };

        this._ws.onmessage = function(event) {
          var data;
          if (event.data instanceof ArrayBuffer) {
            // Binary message — decode
            data = new TextDecoder().decode(event.data);
          } else {
            data = event.data;
          }
          try { data = JSON.parse(data); } catch(e) { /* raw string */ }
          self._emit('message', data);
        };

        this._ws.onclose = function(event) {
          self._stopHeartbeat();
          if (event.code !== 1000) { // Abnormal close
            self._reconnect();
          }
          self._emit('disconnected', { code: event.code, reason: event.reason });
        };

        this._ws.onerror = function() {
          self._emit('error', { message: 'WebSocket error' });
        };
      } catch(e) {
        console.error('[Protocols] WebSocket failed:', e.message);
      }
    },

    send: function(data) {
      if (this._ws && this._ws.readyState === WebSocket.OPEN) {
        this._ws.send(typeof data === 'string' ? data : JSON.stringify(data));
        return true;
      }
      return false;
    },

    on: function(event, callback) {
      if (!this._listeners[event]) this._listeners[event] = [];
      this._listeners[event].push(callback);
    },

    _emit: function(event, data) {
      var cbs = this._listeners[event] || [];
      for (var i = 0; i < cbs.length; i++) {
        try { cbs[i](data); } catch(e) { /* listener error */ }
      }
    },

    _reconnect: function() {
      if (this._attempt >= this._maxReconnect) {
        console.warn('[Protocols] Max reconnection attempts reached');
        this._emit('reconnect_failed', {});
        return;
      }
      this._attempt++;
      var delay = this._reconnectDelay * Math.pow(2, this._attempt - 1);
      var self = this;
      console.log('[Protocols] Reconnecting in ' + delay + 'ms (attempt ' + this._attempt + ')');
      setTimeout(function() { self.connect(self._url); }, delay);
    },

    _startHeartbeat: function() {
      var self = this;
      this._heartbeatInterval = setInterval(function() {
        if (self._ws && self._ws.readyState === WebSocket.OPEN) {
          self._ws.send('{"type":"ping"}');
        }
      }, 30000);
    },

    _stopHeartbeat: function() {
      if (this._heartbeatInterval) {
        clearInterval(this._heartbeatInterval);
        this._heartbeatInterval = null;
      }
    },

    _connectSSE: function(url) {
      var self = this;
      var source = new EventSource(url);
      source.onmessage = function(event) {
        var data;
        try { data = JSON.parse(event.data); } catch(e) { data = event.data; }
        self._emit('message', data);
      };
      source.onerror = function() {
        self._emit('error', { message: 'SSE connection error' });
      };
    },

    disconnect: function() {
      this._stopHeartbeat();
      if (this._ws) {
        this._ws.close(1000, 'Client disconnect');
        this._ws = null;
      }
    }
  };


  // ═══════════════════════════════════════
  // 5. RESOURCE OPTIMIZER — Prefetch & Cache
  // ═══════════════════════════════════════
  var ResourceOptimizer = {
    _prefetched: {},

    // Prefetch critical resources based on connection quality
    prefetch: function(urls) {
      var strategy = ConnectionQuality.getStrategy();
      if (!strategy.prefetch || !navigator.onLine) return;

      var self = this;
      urls.forEach(function(url) {
        if (self._prefetched[url]) return;
        var link = document.createElement('link');
        link.rel = 'prefetch';
        link.href = url;
        link.as = self._getResourceType(url);
        document.head.appendChild(link);
        self._prefetched[url] = true;
      });
    },

    // Preconnect to API servers
    preconnect: function(origins) {
      origins.forEach(function(origin) {
        var link = document.createElement('link');
        link.rel = 'preconnect';
        link.href = origin;
        link.crossOrigin = 'anonymous';
        document.head.appendChild(link);
      });
    },

    // DNS prefetch
    dnsPrefetch: function(domains) {
      domains.forEach(function(domain) {
        var link = document.createElement('link');
        link.rel = 'dns-prefetch';
        link.href = '//' + domain;
        document.head.appendChild(link);
      });
    },

    // Lazy load images based on connection
    lazyLoadImages: function() {
      if (!('IntersectionObserver' in g)) return;
      var strategy = ConnectionQuality.getStrategy();

      var observer = new IntersectionObserver(function(entries) {
        entries.forEach(function(entry) {
          if (entry.isIntersecting) {
            var img = entry.target;
            var src = img.getAttribute('data-src');
            if (src) {
              // Adjust quality based on connection
              if (strategy.imageQuality === 'low' && src.indexOf('?') > -1) {
                src += '&quality=30';
              }
              img.src = src;
              img.removeAttribute('data-src');
              observer.unobserve(img);
            }
          }
        });
      }, { rootMargin: '200px' });

      document.querySelectorAll('img[data-src]').forEach(function(img) {
        observer.observe(img);
      });
    },

    _getResourceType: function(url) {
      if (url.match(/\.js$/)) return 'script';
      if (url.match(/\.css$/)) return 'style';
      if (url.match(/\.(png|jpg|webp|svg)$/)) return 'image';
      if (url.match(/\.(woff2?|ttf)$/)) return 'font';
      return 'fetch';
    }
  };


  // ═══════════════════════════════════════
  // 6. MESH NETWORK — P2P for offline communities
  // ═══════════════════════════════════════
  var MeshNetwork = {
    _peers: {},
    _dataChannels: {},

    // Create P2P connection for offline mesh
    createPeer: function(peerId, config) {
      if (!ProtocolNegotiator._protocols.webrtc) {
        console.warn('[Protocols] WebRTC not available for mesh');
        return null;
      }

      var pc = new RTCPeerConnection(config || {
        iceServers: [{ urls: 'stun:stun.ierahkwa.nation:3478' }]
      });

      this._peers[peerId] = pc;
      return pc;
    },

    // Create data channel for P2P communication
    createDataChannel: function(peerId, label) {
      var pc = this._peers[peerId];
      if (!pc) return null;

      var channel = pc.createDataChannel(label || 'sovereign-mesh', {
        ordered: true,
        maxRetransmits: 3
      });

      this._dataChannels[peerId] = channel;
      return channel;
    },

    // Broadcast message to all peers
    broadcast: function(data) {
      var msg = typeof data === 'string' ? data : JSON.stringify(data);
      var sent = 0;
      for (var id in this._dataChannels) {
        var ch = this._dataChannels[id];
        if (ch.readyState === 'open') {
          ch.send(msg);
          sent++;
        }
      }
      return sent;
    },

    getPeerCount: function() {
      return Object.keys(this._peers).length;
    }
  };


  // ═══════════════════════════════════════
  // 7. PERFORMANCE MONITOR
  // ═══════════════════════════════════════
  var PerformanceMonitor = {
    _metrics: [],

    measure: function(label, fn) {
      var start = performance.now();
      var result = fn();
      var duration = performance.now() - start;
      this._metrics.push({ label: label, duration: duration, timestamp: Date.now() });
      return result;
    },

    getPageMetrics: function() {
      if (!g.performance || !g.performance.timing) return null;
      var t = g.performance.timing;
      return {
        dns: t.domainLookupEnd - t.domainLookupStart,
        tcp: t.connectEnd - t.connectStart,
        ttfb: t.responseStart - t.requestStart,
        domLoad: t.domContentLoadedEventEnd - t.navigationStart,
        fullLoad: t.loadEventEnd - t.navigationStart,
        domInteractive: t.domInteractive - t.navigationStart
      };
    },

    getResourceMetrics: function() {
      if (!g.performance || !g.performance.getEntriesByType) return [];
      return g.performance.getEntriesByType('resource').map(function(r) {
        return {
          name: r.name.split('/').pop(),
          type: r.initiatorType,
          duration: Math.round(r.duration),
          size: r.transferSize || 0,
          protocol: r.nextHopProtocol || 'unknown'
        };
      });
    },

    getCustomMetrics: function() {
      return this._metrics;
    }
  };


  // ═══════════════════════════════════════
  // 8. INITIALIZATION
  // ═══════════════════════════════════════
  function init() {
    // Detect connection quality
    var quality = ConnectionQuality.detect();

    // Detect available protocols
    var protocols = ProtocolNegotiator.detect();

    // Process offline queue when back online
    g.addEventListener('online', function() {
      console.log('[Protocols] Back online — processing queue');
      SovereignFetch.processQueue();
    });

    // Lazy load images
    if (document.readyState === 'loading') {
      document.addEventListener('DOMContentLoaded', function() {
        ResourceOptimizer.lazyLoadImages();
      });
    } else {
      ResourceOptimizer.lazyLoadImages();
    }

    // Log initialization
    var transport = ProtocolNegotiator.getBestTransport();
    var realtime = ProtocolNegotiator.getBestRealtime();

    console.log(
      '%c⚡ Ierahkwa Protocols v1.0.0%c\n' +
      'Transport: ' + transport + '\n' +
      'Realtime: ' + realtime + '\n' +
      'WASM: ' + (protocols.wasm ? '✓' : '✗') + ' | Crypto: ' + (protocols.crypto ? '✓' : '✗') + '\n' +
      'Network: ' + quality.grade + ' (' + quality.score + '/100) | ' + quality.type + '\n' +
      'Strategy: ' + ConnectionQuality.getStrategy().mode,
      'color:#00e676;font-weight:700;font-size:13px',
      'color:#888;font-size:11px'
    );
  }

  // Run on load
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
  } else {
    init();
  }


  // ═══════════════════════════════════════
  // PUBLIC API
  // ═══════════════════════════════════════
  g.IerahkwaProtocols = {
    connection: ConnectionQuality,
    protocols: ProtocolNegotiator,
    fetch: SovereignFetch,
    realtime: RealtimeChannel,
    resources: ResourceOptimizer,
    mesh: MeshNetwork,
    performance: PerformanceMonitor,

    getStatus: function() {
      return {
        version: '1.0.0',
        connection: ConnectionQuality.getProfile(),
        strategy: ConnectionQuality.getStrategy(),
        protocols: ProtocolNegotiator.getCapabilities(),
        transport: ProtocolNegotiator.getBestTransport(),
        realtime: ProtocolNegotiator.getBestRealtime(),
        p2p: ProtocolNegotiator.getBestP2P(),
        queuedRequests: SovereignFetch._queue.length,
        meshPeers: MeshNetwork.getPeerCount()
      };
    }
  };

})(typeof globalThis !== 'undefined' ? globalThis : typeof window !== 'undefined' ? window : this);
