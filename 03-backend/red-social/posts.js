const router = require('express').Router();
const auth = require('../middleware/auth');
const db = require('./db');

/** Map a database row (snake_case) to the API shape (camelCase). */
function rowToPost(r) {
  return {
    id: r.id,
    authorId: r.author_id,
    content: r.content,
    media: r.media,
    type: r.type,
    language: r.language,
    visibility: r.visibility,
    monetized: r.monetized,
    likes: r.likes,
    comments: r.comments,
    shares: r.shares,
    views: r.views,
    tips: Number(r.tips),
    hashtags: r.hashtags,
    mentions: r.mentions,
    earnings: Number(r.earnings),
    adContent: r.ad_content,
    repostOf: r.repost_of || undefined,
    // reposts created via /share carry extra columns kept in content/type
    originalPost: r.repost_of || undefined,
    originalAuthor: undefined, // filled only for reposts via join if needed
    createdAt: r.created_at,
    updatedAt: r.updated_at,
  };
}

// POST /v1/posts — create post
router.post('/', auth, async (req, res) => {
  const { content, media, type = 'text', language = 'es', visibility = 'public', monetized = false } = req.body;
  if (!content && !media) return res.status(400).json({ error: 'Content or media required' });

  const hashtags = (content || '').match(/#\w+/g) || [];
  const mentions = (content || '').match(/@\w+/g) || [];

  const { rows } = await db.query(
    `INSERT INTO posts (author_id, content, media, type, language, visibility, monetized, hashtags, mentions, ad_content)
     VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, false)
     RETURNING *`,
    [req.userId, content, JSON.stringify(media || []), type, language, visibility, monetized, hashtags, mentions]
  );

  res.status(201).json({ post: rowToPost(rows[0]), taxOnEarnings: '0%' });
});

// GET /v1/posts/:id
router.get('/:id', async (req, res) => {
  const { rows } = await db.query(
    `UPDATE posts SET views = views + 1 WHERE id = $1 RETURNING *`,
    [req.params.id]
  );
  if (!rows.length) return res.status(404).json({ error: 'Not found' });
  res.json(rowToPost(rows[0]));
});

// PUT /v1/posts/:id
router.put('/:id', auth, async (req, res) => {
  const { rows: existing } = await db.query(`SELECT * FROM posts WHERE id = $1`, [req.params.id]);
  if (!existing.length) return res.status(404).json({ error: 'Not found' });
  if (existing[0].author_id !== req.userId) return res.status(403).json({ error: 'Not your post' });

  // Build dynamic SET clause from allowed body fields
  const allowed = ['content', 'media', 'type', 'language', 'visibility', 'monetized'];
  const sets = [];
  const vals = [];
  let idx = 1;
  for (const key of allowed) {
    if (req.body[key] !== undefined) {
      const col = key === 'media' ? 'media' : key;
      sets.push(`${col} = $${idx}`);
      vals.push(key === 'media' ? JSON.stringify(req.body[key]) : req.body[key]);
      idx++;
    }
  }
  sets.push(`updated_at = NOW()`);

  const { rows } = await db.query(
    `UPDATE posts SET ${sets.join(', ')} WHERE id = $${idx} RETURNING *`,
    [...vals, req.params.id]
  );
  res.json(rowToPost(rows[0]));
});

// DELETE /v1/posts/:id
router.delete('/:id', auth, async (req, res) => {
  const { rows } = await db.query(`SELECT author_id FROM posts WHERE id = $1`, [req.params.id]);
  if (!rows.length) return res.status(404).json({ error: 'Not found' });
  if (rows[0].author_id !== req.userId) return res.status(403).json({ error: 'Not your post' });
  await db.query(`DELETE FROM posts WHERE id = $1`, [req.params.id]);
  res.json({ deleted: true });
});

// POST /v1/posts/:id/tip — tip creator in WMP
router.post('/:id/tip', auth, async (req, res) => {
  const { amount } = req.body;
  if (amount <= 0) return res.status(400).json({ error: 'Invalid amount' });

  const { rows } = await db.query(
    `UPDATE posts SET tips = tips + $1, earnings = earnings + $2, updated_at = NOW()
     WHERE id = $3 RETURNING *`,
    [amount, amount * 0.92, req.params.id]
  );
  if (!rows.length) return res.status(404).json({ error: 'Not found' });

  res.json({
    txId: 'tip_' + Date.now().toString(36),
    amount,
    creatorReceives: amount * 0.92,
    platformFee: amount * 0.08,
    taxPaid: 0,
  });
});

// POST /v1/posts/:id/share
router.post('/:id/share', auth, async (req, res) => {
  // Increment shares on the original post
  const { rows: original } = await db.query(
    `UPDATE posts SET shares = shares + 1, updated_at = NOW() WHERE id = $1 RETURNING *`,
    [req.params.id]
  );
  if (!original.length) return res.status(404).json({ error: 'Not found' });

  // Create the repost
  const { rows } = await db.query(
    `INSERT INTO posts (author_id, type, content, repost_of, likes, comments, shares, views)
     VALUES ($1, 'repost', $2, $3, 0, 0, 0, 0)
     RETURNING *`,
    [req.userId, req.body.comment || '', req.params.id]
  );

  const repost = rows[0];
  res.json({
    id: repost.id,
    authorId: repost.author_id,
    type: 'repost',
    originalPost: repost.repost_of,
    originalAuthor: original[0].author_id,
    content: repost.content,
    likes: 0,
    comments: 0,
    shares: 0,
    views: 0,
    createdAt: repost.created_at,
  });
});

// GET /v1/posts/user/:userId
router.get('/user/:userId', async (req, res) => {
  const { limit = 20, offset = 0 } = req.query;
  const { rows } = await db.query(
    `SELECT * FROM posts
     WHERE author_id = $1 AND visibility = 'public'
     ORDER BY created_at DESC
     LIMIT $2 OFFSET $3`,
    [req.params.userId, +limit, +offset]
  );
  const mapped = rows.map(rowToPost);
  res.json({ posts: mapped, total: mapped.length });
});

module.exports = router;
