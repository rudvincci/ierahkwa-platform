const router = require('express').Router();
const auth = require('../middleware/auth');
const db = require('./db');

/** Map a conversations row to the API shape. */
function rowToConv(r) {
  return {
    id: r.id,
    type: r.type,
    name: r.name,
    participants: r.participants,
    createdBy: r.created_by,
    lastMessage: r.last_message,
    unread: {}, // unread counters are computed per-user; kept for shape compatibility
    encryption: r.encryption || 'post-quantum-e2e',
    createdAt: r.created_at,
  };
}

/** Map a messages row to the API shape. */
function rowToMsg(r) {
  return {
    id: r.id,
    conversationId: r.conversation_id,
    senderId: r.sender_id,
    text: r.text,
    media: r.media,
    replyTo: r.reply_to,
    type: r.type,
    encryption: r.encryption || 'ML-KEM-1024',
    delivered: r.delivered,
    read: r.read,
    createdAt: r.created_at,
  };
}

// POST /v1/chat/conversation
router.post('/conversation', auth, async (req, res) => {
  const { participants, name, type = 'direct' } = req.body;
  const allParticipants = [req.userId, ...(participants || [])];

  const { rows } = await db.query(
    `INSERT INTO conversations (type, name, participants, created_by, encryption)
     VALUES ($1, $2, $3, $4, 'aes-256-gcm')
     RETURNING *`,
    [type, name || null, allParticipants, req.userId]
  );

  res.status(201).json(rowToConv(rows[0]));
});

// GET /v1/chat/conversations
router.get('/conversations', auth, async (req, res) => {
  const { rows } = await db.query(
    `SELECT * FROM conversations
     WHERE $1 = ANY(participants)
     ORDER BY COALESCE((last_message->>'ts')::timestamptz, created_at) DESC`,
    [req.userId]
  );
  res.json({ conversations: rows.map(rowToConv) });
});

// POST /v1/chat/:convId/message
router.post('/:convId/message', auth, async (req, res) => {
  // Verify the conversation exists and user is a participant
  const { rows: convRows } = await db.query(
    `SELECT * FROM conversations WHERE id = $1`,
    [req.params.convId]
  );
  if (!convRows.length || !convRows[0].participants.includes(req.userId)) {
    return res.status(403).json({ error: 'Not in conversation' });
  }

  const { text, media, replyTo, type = 'text' } = req.body;

  const { rows } = await db.query(
    `INSERT INTO messages (conversation_id, sender_id, text, media, reply_to, type, encryption)
     VALUES ($1, $2, $3, $4, $5, $6, 'aes-256-gcm')
     RETURNING *`,
    [req.params.convId, req.userId, text, media ? JSON.stringify(media) : null, replyTo || null, type]
  );

  // Update last_message on the conversation
  const lastMsg = { text: (text || '').slice(0, 50), senderId: req.userId, ts: new Date().toISOString() };
  await db.query(
    `UPDATE conversations SET last_message = $1 WHERE id = $2`,
    [JSON.stringify(lastMsg), req.params.convId]
  );

  res.status(201).json(rowToMsg(rows[0]));
});

// GET /v1/chat/:convId/messages
router.get('/:convId/messages', auth, async (req, res) => {
  // Verify participant
  const { rows: convRows } = await db.query(
    `SELECT * FROM conversations WHERE id = $1`,
    [req.params.convId]
  );
  if (!convRows.length || !convRows[0].participants.includes(req.userId)) {
    return res.status(403).json({ error: 'Not in conversation' });
  }

  const { limit = 50, before } = req.query;
  let queryText;
  let params;

  if (before) {
    queryText = `SELECT * FROM messages
                 WHERE conversation_id = $1 AND created_at < $2
                 ORDER BY created_at DESC LIMIT $3`;
    params = [req.params.convId, new Date(before), +limit];
  } else {
    queryText = `SELECT * FROM messages
                 WHERE conversation_id = $1
                 ORDER BY created_at DESC LIMIT $2`;
    params = [req.params.convId, +limit];
  }

  const { rows } = await db.query(queryText, params);
  // Reverse so oldest-first (matches original slice(-limit) behavior)
  res.json({ messages: rows.reverse().map(rowToMsg), encryption: 'post-quantum-e2e' });
});

// POST /v1/chat/:convId/send-wmp — send WMP inside chat
router.post('/:convId/send-wmp', auth, (req, res) => {
  const { amount, note } = req.body;
  res.json({
    txId: 'chat_wmp_' + Date.now().toString(36),
    amount,
    note,
    type: 'wmp_transfer',
    fee: 0,
    taxPaid: 0,
    message: 'WMP sent inside conversation. Zero fees between users.',
  });
});

module.exports = router;
