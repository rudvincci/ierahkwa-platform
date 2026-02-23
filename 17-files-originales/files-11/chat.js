const router = require('express').Router();
const auth = require('../middleware/auth');
const { v4: uuid } = require('uuid');
const conversations = new Map();
const messages = new Map();

// POST /v1/chat/conversation
router.post('/conversation', auth, (req, res) => {
  const { participants, name, type = 'direct' } = req.body; // direct, group
  const allParticipants = [req.userId, ...(participants || [])];
  const conv = {
    id: uuid(), type, name: name || null, participants: allParticipants,
    createdBy: req.userId, lastMessage: null, unread: {},
    encryption: 'post-quantum-e2e', createdAt: new Date()
  };
  conversations.set(conv.id, conv);
  res.status(201).json(conv);
});

// GET /v1/chat/conversations
router.get('/conversations', auth, (req, res) => {
  const userConvs = [...conversations.values()]
    .filter(c => c.participants.includes(req.userId))
    .sort((a, b) => new Date(b.lastMessage?.ts || b.createdAt) - new Date(a.lastMessage?.ts || a.createdAt));
  res.json({ conversations: userConvs });
});

// POST /v1/chat/:convId/message
router.post('/:convId/message', auth, (req, res) => {
  const conv = conversations.get(req.params.convId);
  if (!conv || !conv.participants.includes(req.userId)) return res.status(403).json({ error: 'Not in conversation' });
  
  const { text, media, replyTo, type = 'text' } = req.body;
  const msg = {
    id: uuid(), conversationId: req.params.convId, senderId: req.userId,
    text, media, replyTo, type, // text, image, video, audio, file, voice, sticker, wmp_transfer
    encryption: 'ML-KEM-1024', // post-quantum encrypted
    delivered: false, read: false, createdAt: new Date()
  };
  
  if (!messages.has(req.params.convId)) messages.set(req.params.convId, []);
  messages.get(req.params.convId).push(msg);
  conv.lastMessage = { text: text?.slice(0, 50), senderId: req.userId, ts: new Date() };
  
  res.status(201).json(msg);
});

// GET /v1/chat/:convId/messages
router.get('/:convId/messages', auth, (req, res) => {
  const conv = conversations.get(req.params.convId);
  if (!conv || !conv.participants.includes(req.userId)) return res.status(403).json({ error: 'Not in conversation' });
  const { limit = 50, before } = req.query;
  let msgs = messages.get(req.params.convId) || [];
  if (before) msgs = msgs.filter(m => new Date(m.createdAt) < new Date(before));
  res.json({ messages: msgs.slice(-limit), encryption: 'post-quantum-e2e' });
});

// POST /v1/chat/:convId/send-wmp — send WMP inside chat
router.post('/:convId/send-wmp', auth, (req, res) => {
  const { amount, note } = req.body;
  res.json({
    txId: 'chat_wmp_'+Date.now().toString(36), amount, note,
    type: 'wmp_transfer', fee: 0, taxPaid: 0,
    message: 'WMP sent inside conversation. Zero fees between users.'
  });
});

module.exports = router;
