const router = require('express').Router();
const auth = require('../middleware/auth');
const { v4: uuid } = require('uuid');

// In-memory store (production: PostgreSQL + Redis cache)
const posts = new Map();

// POST /v1/posts — create post
router.post('/', auth, (req, res) => {
  const { content, media, type = 'text', language = 'es', visibility = 'public', monetized = false } = req.body;
  if (!content && !media) return res.status(400).json({ error: 'Content or media required' });
  
  const post = {
    id: uuid(), authorId: req.userId, content, media: media || [],
    type, // text, image, video, audio, poll, article
    language, visibility, // public, followers, private, community
    monetized, // if true, viewers pay small WMP fee (92% to creator)
    likes: 0, comments: 0, shares: 0, views: 0, tips: 0,
    hashtags: (content || '').match(/#\w+/g) || [],
    mentions: (content || '').match(/@\w+/g) || [],
    createdAt: new Date(), updatedAt: new Date(),
    earnings: 0, // WMP earned from monetization
    adContent: false, // ALWAYS false — no ads ever
  };
  
  posts.set(post.id, post);
  res.status(201).json({ post, taxOnEarnings: '0%' });
});

// GET /v1/posts/:id
router.get('/:id', (req, res) => {
  const post = posts.get(req.params.id);
  if (!post) return res.status(404).json({ error: 'Not found' });
  post.views++;
  res.json(post);
});

// PUT /v1/posts/:id
router.put('/:id', auth, (req, res) => {
  const post = posts.get(req.params.id);
  if (!post) return res.status(404).json({ error: 'Not found' });
  if (post.authorId !== req.userId) return res.status(403).json({ error: 'Not your post' });
  Object.assign(post, req.body, { updatedAt: new Date() });
  res.json(post);
});

// DELETE /v1/posts/:id
router.delete('/:id', auth, (req, res) => {
  const post = posts.get(req.params.id);
  if (!post) return res.status(404).json({ error: 'Not found' });
  if (post.authorId !== req.userId) return res.status(403).json({ error: 'Not your post' });
  posts.delete(req.params.id);
  res.json({ deleted: true });
});

// POST /v1/posts/:id/tip — tip creator in WMP
router.post('/:id/tip', auth, (req, res) => {
  const post = posts.get(req.params.id);
  if (!post) return res.status(404).json({ error: 'Not found' });
  const { amount } = req.body;
  if (amount <= 0) return res.status(400).json({ error: 'Invalid amount' });
  post.tips += amount;
  post.earnings += amount * 0.92; // 92% to creator
  res.json({ txId: 'tip_'+Date.now().toString(36), amount, creatorReceives: amount*0.92, platformFee: amount*0.08, taxPaid: 0 });
});

// POST /v1/posts/:id/share
router.post('/:id/share', auth, (req, res) => {
  const post = posts.get(req.params.id);
  if (!post) return res.status(404).json({ error: 'Not found' });
  post.shares++;
  // Create repost
  const repost = {
    id: uuid(), authorId: req.userId, type: 'repost',
    originalPost: post.id, originalAuthor: post.authorId,
    content: req.body.comment || '', likes: 0, comments: 0, shares: 0, views: 0,
    createdAt: new Date()
  };
  posts.set(repost.id, repost);
  res.json(repost);
});

// GET /v1/posts/user/:userId
router.get('/user/:userId', (req, res) => {
  const { limit = 20, offset = 0 } = req.query;
  const userPosts = [...posts.values()]
    .filter(p => p.authorId === req.params.userId && p.visibility === 'public')
    .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
    .slice(+offset, +offset + +limit);
  res.json({ posts: userPosts, total: userPosts.length });
});

module.exports = router;
module.exports._posts = posts; // for feed access
