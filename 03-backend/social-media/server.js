'use strict';

const express = require('express');
const cors = require('cors');
const helmet = require('helmet');
const http = require('http');
const { WebSocketServer } = require('ws');
const { SocialEngine } = require('./lib/social-engine');

const app = express();
const server = http.createServer(app);
const wss = new WebSocketServer({ server, path: '/ws' });
const social = new SocialEngine();

// Middleware
try {
  const { corsConfig } = require('../shared/security');
  app.use(helmet());
  app.use(cors(corsConfig()));
} catch (_) {
  app.use(helmet());
  app.use(cors());
}
app.use(express.json());

// ── Helper: extract userId from header ───────────────
function getUserId(req) { return req.headers['x-user-id'] || null; }

// ── Profiles ─────────────────────────────────────────

app.post('/v1/profiles', (req, res) => {
  try {
    const profile = social.createProfile(req.body.userId || getUserId(req), req.body);
    res.status(201).json(profile);
  } catch (e) { res.status(400).json({ error: e.message }); }
});

app.get('/v1/profiles/search', (req, res) => {
  const results = social.searchProfiles(req.query.q || '', parseInt(req.query.limit) || 20);
  res.json({ profiles: results, count: results.length });
});

app.get('/v1/profiles/:userId', (req, res) => {
  const p = social.getProfile(req.params.userId);
  if (!p) return res.status(404).json({ error: 'Profile not found' });
  res.json(p);
});

app.put('/v1/profiles/:userId', (req, res) => {
  const p = social.updateProfile(req.params.userId, req.body);
  if (!p) return res.status(404).json({ error: 'Profile not found' });
  res.json(p);
});

// ── Feed ─────────────────────────────────────────────

app.get('/v1/feed', (req, res) => {
  const userId = getUserId(req);
  if (!userId) return res.status(400).json({ error: 'X-User-Id header required' });
  const posts = social.getFeed(userId, parseInt(req.query.limit) || 30);
  res.json({ posts, count: posts.length });
});

app.get('/v1/feed/explore', (req, res) => {
  const posts = social.getExploreFeed(parseInt(req.query.limit) || 30);
  res.json({ posts, count: posts.length });
});

// ── Posts ─────────────────────────────────────────────

app.post('/v1/posts', (req, res) => {
  try {
    const authorId = getUserId(req) || req.body.authorId;
    if (!authorId) return res.status(400).json({ error: 'authorId required' });
    const post = social.createPost(authorId, req.body);
    res.status(201).json(post);
  } catch (e) { res.status(400).json({ error: e.message }); }
});

app.get('/v1/posts/:id', (req, res) => {
  const post = social.getPost(req.params.id);
  if (!post) return res.status(404).json({ error: 'Post not found' });
  res.json(post);
});

app.put('/v1/posts/:id', (req, res) => {
  try {
    const userId = getUserId(req);
    const post = social.updatePost(req.params.id, userId, req.body);
    if (!post) return res.status(404).json({ error: 'Post not found' });
    res.json(post);
  } catch (e) { res.status(403).json({ error: e.message }); }
});

app.delete('/v1/posts/:id', (req, res) => {
  try {
    const userId = getUserId(req);
    const deleted = social.deletePost(req.params.id, userId);
    if (!deleted) return res.status(404).json({ error: 'Post not found' });
    res.json({ deleted: true });
  } catch (e) { res.status(403).json({ error: e.message }); }
});

app.get('/v1/posts/user/:userId', (req, res) => {
  const posts = social.getUserPosts(req.params.userId, parseInt(req.query.limit) || 30);
  res.json({ posts, count: posts.length });
});

app.post('/v1/posts/:id/tip', (req, res) => {
  try {
    const result = social.tipPost(req.params.id, getUserId(req), Number(req.body.amount) || 0);
    res.json(result);
  } catch (e) { res.status(400).json({ error: e.message }); }
});

app.post('/v1/posts/:id/share', (req, res) => {
  try {
    const userId = getUserId(req) || req.body.userId;
    const repost = social.sharePost(req.params.id, userId);
    res.status(201).json(repost);
  } catch (e) { res.status(400).json({ error: e.message }); }
});

// ── Stories ──────────────────────────────────────────

app.post('/v1/stories', (req, res) => {
  try {
    const story = social.createStory(getUserId(req) || req.body.authorId, req.body);
    res.status(201).json(story);
  } catch (e) { res.status(400).json({ error: e.message }); }
});

app.get('/v1/stories', (req, res) => {
  const userId = getUserId(req);
  if (!userId) return res.status(400).json({ error: 'X-User-Id header required' });
  res.json({ stories: social.getStories(userId) });
});

// ── Comments ─────────────────────────────────────────

app.post('/v1/comments', (req, res) => {
  try {
    const authorId = getUserId(req) || req.body.authorId;
    const comment = social.createComment(req.body.postId, authorId, req.body.content);
    res.status(201).json(comment);
  } catch (e) { res.status(400).json({ error: e.message }); }
});

app.get('/v1/comments/:postId', (req, res) => {
  const comments = social.getComments(req.params.postId, parseInt(req.query.limit) || 50);
  res.json({ comments, count: comments.length });
});

app.delete('/v1/comments/:id', (req, res) => {
  try {
    social.deleteComment(req.params.id, getUserId(req));
    res.json({ deleted: true });
  } catch (e) { res.status(403).json({ error: e.message }); }
});

// ── Likes ────────────────────────────────────────────

app.post('/v1/likes/:postId', (req, res) => {
  try {
    const result = social.likePost(req.params.postId, getUserId(req));
    res.json(result);
  } catch (e) { res.status(400).json({ error: e.message }); }
});

app.delete('/v1/likes/:postId', (req, res) => {
  try {
    const result = social.unlikePost(req.params.postId);
    res.json(result);
  } catch (e) { res.status(400).json({ error: e.message }); }
});

// ── Follow ───────────────────────────────────────────

app.post('/v1/follow/:targetId', (req, res) => {
  try {
    const result = social.follow(getUserId(req), req.params.targetId);
    res.json(result);
  } catch (e) { res.status(400).json({ error: e.message }); }
});

app.delete('/v1/follow/:targetId', (req, res) => {
  try {
    const result = social.unfollow(getUserId(req), req.params.targetId);
    res.json(result);
  } catch (e) { res.status(400).json({ error: e.message }); }
});

app.get('/v1/follow/:userId/followers', (req, res) => {
  res.json({ followers: social.getFollowers(req.params.userId) });
});

app.get('/v1/follow/:userId/following', (req, res) => {
  res.json({ following: social.getFollowing(req.params.userId) });
});

// ── Groups ───────────────────────────────────────────

app.post('/v1/groups', (req, res) => {
  try {
    const group = social.createGroup(getUserId(req) || req.body.creatorId, req.body);
    res.status(201).json(group);
  } catch (e) { res.status(400).json({ error: e.message }); }
});

app.get('/v1/groups', (req, res) => {
  res.json({ groups: social.listGroups(parseInt(req.query.limit) || 20) });
});

app.post('/v1/groups/:id/join', (req, res) => {
  try {
    const result = social.joinGroup(req.params.id, getUserId(req));
    res.json(result);
  } catch (e) { res.status(400).json({ error: e.message }); }
});

app.post('/v1/groups/:id/leave', (req, res) => {
  try {
    const result = social.leaveGroup(req.params.id, getUserId(req));
    res.json(result);
  } catch (e) { res.status(400).json({ error: e.message }); }
});

// ── Notifications ────────────────────────────────────

app.get('/v1/notifications', (req, res) => {
  const userId = getUserId(req);
  if (!userId) return res.status(400).json({ error: 'X-User-Id header required' });
  res.json({ notifications: social.getNotifications(userId) });
});

app.post('/v1/notifications/read', (req, res) => {
  const count = social.markNotificationsRead(getUserId(req));
  res.json({ marked: count });
});

// ── Live ─────────────────────────────────────────────

app.post('/v1/live', (req, res) => {
  try {
    const stream = social.startStream(getUserId(req) || req.body.hostId, req.body);
    res.status(201).json(stream);
  } catch (e) { res.status(400).json({ error: e.message }); }
});

app.get('/v1/live', (req, res) => {
  res.json({ streams: social.getLiveStreams() });
});

app.post('/v1/live/:id/end', (req, res) => {
  try {
    const stream = social.endStream(req.params.id);
    res.json(stream);
  } catch (e) { res.status(400).json({ error: e.message }); }
});

// ── Health ───────────────────────────────────────────

app.get('/health', (req, res) => {
  const stats = social.getStats();
  res.json({
    service: 'Soberano Social',
    version: '1.0.0',
    status: 'operational',
    features: ['feed', 'posts', 'stories', 'comments', 'likes', 'follow', 'groups', 'chat', 'live', 'notifications'],
    ...stats
  });
});

// ── WebSocket — real-time social events ──────────────

wss.on('connection', (ws) => {
  ws._userId = null;
  ws.on('message', (raw) => {
    try {
      const msg = JSON.parse(raw);
      if (msg.type === 'auth') ws._userId = msg.userId;
      if (msg.type === 'subscribe') ws._channels = msg.channels || [];
    } catch (_) { /* ignore */ }
  });
});

function broadcast(channel, data) {
  const msg = JSON.stringify({ channel, data, timestamp: new Date().toISOString() });
  wss.clients.forEach(c => {
    if (c.readyState === 1) c.send(msg);
  });
}

// ── Error Handler ────────────────────────────────────

app.use((err, req, res, _next) => {
  console.error(err.stack || err.message);
  res.status(500).json({ error: 'Internal server error' });
});

// ── Start ────────────────────────────────────────────

const PORT = process.env.SOCIAL_PORT || 4001;
server.listen(PORT, () => {
  console.log(`Soberano Social on :${PORT} — 10 features — 0% taxes on tips`);
  console.log(`WebSocket: ws://localhost:${PORT}/ws`);
});

module.exports = { app, server, social };
