/**
 * Ierahkwa Chat - Service Worker
 * PWA Support for offline functionality
 */

const CACHE_NAME = 'ierahkwa-chat-v1';
const STATIC_ASSETS = ['/chat/', '/chat/index.html', '/chat/app.js', '/chat/manifest.json'];

self.addEventListener('install', (event) => {
  event.waitUntil(
    caches.open(CACHE_NAME)
      .then((cache) => cache.addAll(STATIC_ASSETS))
      .then(() => self.skipWaiting())
  );
});

self.addEventListener('activate', (event) => {
  event.waitUntil(
    caches.keys()
      .then((names) => Promise.all(names.filter((n) => n !== CACHE_NAME).map((n) => caches.delete(n))))
      .then(() => self.clients.claim())
  );
});

self.addEventListener('fetch', (event) => {
  if (event.request.method !== 'GET') return;
  if (event.request.url.includes('/socket.io/')) return;
  if (event.request.url.includes('/api/')) return;

  event.respondWith(
    fetch(event.request)
      .then((response) => {
        const clone = response.clone();
        caches.open(CACHE_NAME).then((cache) => cache.put(event.request, clone));
        return response;
      })
      .catch(() => caches.match(event.request).then((r) => r || (event.request.mode === 'navigate' ? caches.match('/chat/index.html') : new Response('Offline', { status: 503 }))))
  );
});

self.addEventListener('push', (event) => {
  const data = event.data?.json() || {};
  event.waitUntil(
    self.registration.showNotification(data.title || 'Ierahkwa Chat', {
      body: data.body || 'New message received',
      icon: '/chat/icon.svg',
      vibrate: [100, 50, 100],
      data: { url: data.url || '/chat' }
    })
  );
});

self.addEventListener('notificationclick', (event) => {
  event.notification.close();
  event.waitUntil(
    clients.matchAll({ type: 'window', includeUncontrolled: true })
      .then((list) => {
        for (const c of list) if (c.url.includes('/chat')) return c.focus();
        return clients.openWindow('/chat');
      })
  );
});
