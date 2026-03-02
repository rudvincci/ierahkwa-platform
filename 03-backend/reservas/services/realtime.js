'use strict';

module.exports = function (wss) {
  wss.on('connection', (ws) => {
    ws._rooms = new Set();

    ws.on('message', (raw) => {
      try {
        const msg = JSON.parse(raw);

        switch (msg.type) {
          case 'subscribe':
            // Subscribe to booking updates or provider location channel
            if (msg.channel) ws._rooms.add(msg.channel);
            ws.send(JSON.stringify({ type: 'subscribed', channel: msg.channel }));
            break;

          case 'unsubscribe':
            ws._rooms.delete(msg.channel);
            ws.send(JSON.stringify({ type: 'unsubscribed', channel: msg.channel }));
            break;

          case 'location_update':
            // Broadcast provider location to subscribers
            broadcast(wss, `provider:${msg.providerId}`, {
              type: 'location_update',
              providerId: msg.providerId,
              lat: msg.lat,
              lng: msg.lng,
              timestamp: new Date().toISOString()
            }, ws);
            break;

          case 'booking_update':
            // Broadcast booking status change
            broadcast(wss, `booking:${msg.bookingId}`, {
              type: 'booking_update',
              bookingId: msg.bookingId,
              status: msg.status,
              timestamp: new Date().toISOString()
            }, ws);
            break;

          default:
            ws.send(JSON.stringify({ type: 'error', message: 'Unknown message type' }));
        }
      } catch (_) {
        ws.send(JSON.stringify({ type: 'error', message: 'Invalid JSON' }));
      }
    });
  });
};

function broadcast(wss, channel, data, exclude) {
  const msg = JSON.stringify(data);
  wss.clients.forEach(client => {
    if (client !== exclude && client.readyState === 1 && client._rooms && client._rooms.has(channel)) {
      client.send(msg);
    }
  });
}
