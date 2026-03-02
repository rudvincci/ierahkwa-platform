'use strict';

const crypto = require('crypto');

// ============================================================
// Sovereign Social Network Engine — In-Memory
// Supports: posts, feed, stories, profiles, follow, groups,
//           comments, likes, notifications, live streams
// ============================================================

function uuid() { return crypto.randomUUID(); }

class SocialEngine {
  constructor() {
    this.users = new Map();          // userId → profile
    this.posts = new Map();          // postId → post
    this.comments = new Map();       // commentId → comment
    this.follows = new Map();        // userId → Set(followingIds)
    this.followers = new Map();      // userId → Set(followerIds)
    this.groups = new Map();         // groupId → group
    this.stories = new Map();        // storyId → story
    this.notifications = new Map();  // userId → [notifications]
    this.liveStreams = new Map();     // streamId → stream
  }

  // ── Profiles ─────────────────────────────────────────

  createProfile(userId, data) {
    const profile = {
      userId,
      username: data.username,
      displayName: data.displayName || data.username,
      bio: data.bio || '',
      avatar: data.avatar || null,
      language: data.language || 'es',
      verified: false,
      postsCount: 0,
      followersCount: 0,
      followingCount: 0,
      createdAt: new Date().toISOString()
    };
    this.users.set(userId, profile);
    this.follows.set(userId, new Set());
    this.followers.set(userId, new Set());
    this.notifications.set(userId, []);
    return profile;
  }

  getProfile(userId) { return this.users.get(userId) || null; }

  updateProfile(userId, data) {
    const p = this.users.get(userId);
    if (!p) return null;
    if (data.displayName) p.displayName = data.displayName;
    if (data.bio !== undefined) p.bio = data.bio;
    if (data.avatar) p.avatar = data.avatar;
    if (data.language) p.language = data.language;
    return p;
  }

  searchProfiles(query, limit = 20) {
    const q = query.toLowerCase();
    const results = [];
    for (const p of this.users.values()) {
      if (p.username.toLowerCase().includes(q) || p.displayName.toLowerCase().includes(q)) {
        results.push(p);
      }
      if (results.length >= limit) break;
    }
    return results;
  }

  // ── Follow System ────────────────────────────────────

  follow(followerId, targetId) {
    if (followerId === targetId) throw new Error('Cannot follow yourself');
    if (!this.users.has(followerId)) throw new Error('Follower not found');
    if (!this.users.has(targetId)) throw new Error('Target user not found');

    const following = this.follows.get(followerId);
    if (following.has(targetId)) throw new Error('Already following');

    following.add(targetId);
    this.followers.get(targetId).add(followerId);
    this.users.get(followerId).followingCount++;
    this.users.get(targetId).followersCount++;

    this._notify(targetId, 'follow', { userId: followerId, username: this.users.get(followerId).username });
    return { following: true };
  }

  unfollow(followerId, targetId) {
    const following = this.follows.get(followerId);
    if (!following || !following.has(targetId)) throw new Error('Not following');

    following.delete(targetId);
    this.followers.get(targetId).delete(followerId);
    this.users.get(followerId).followingCount--;
    this.users.get(targetId).followersCount--;
    return { following: false };
  }

  getFollowers(userId, limit = 50) {
    const set = this.followers.get(userId);
    if (!set) return [];
    return [...set].slice(0, limit).map(id => this.users.get(id)).filter(Boolean);
  }

  getFollowing(userId, limit = 50) {
    const set = this.follows.get(userId);
    if (!set) return [];
    return [...set].slice(0, limit).map(id => this.users.get(id)).filter(Boolean);
  }

  isFollowing(followerId, targetId) {
    const set = this.follows.get(followerId);
    return set ? set.has(targetId) : false;
  }

  // ── Posts ────────────────────────────────────────────

  createPost(authorId, data) {
    if (!this.users.has(authorId)) throw new Error('Author not found');

    const id = uuid();
    const content = data.content || '';
    const hashtags = (content.match(/#\w+/g) || []).map(h => h.toLowerCase());
    const mentions = (content.match(/@\w+/g) || []).map(m => m.substring(1));

    const post = {
      id,
      authorId,
      content,
      media: data.media || [],
      type: data.type || 'text',
      language: data.language || 'es',
      visibility: data.visibility || 'public',
      hashtags,
      mentions,
      likesCount: 0,
      commentsCount: 0,
      sharesCount: 0,
      viewsCount: 0,
      tips: 0,
      repostOf: data.repostOf || null,
      createdAt: new Date().toISOString()
    };

    this.posts.set(id, post);
    this.users.get(authorId).postsCount++;

    // Notify mentioned users
    for (const mention of mentions) {
      for (const [uid, profile] of this.users) {
        if (profile.username === mention) {
          this._notify(uid, 'mention', { postId: id, userId: authorId });
        }
      }
    }

    return post;
  }

  getPost(id) {
    const post = this.posts.get(id);
    if (!post) return null;
    post.viewsCount++;
    return post;
  }

  updatePost(id, authorId, data) {
    const post = this.posts.get(id);
    if (!post) return null;
    if (post.authorId !== authorId) throw new Error('Not the author');
    if (data.content !== undefined) {
      post.content = data.content;
      post.hashtags = (post.content.match(/#\w+/g) || []).map(h => h.toLowerCase());
      post.mentions = (post.content.match(/@\w+/g) || []).map(m => m.substring(1));
    }
    if (data.visibility) post.visibility = data.visibility;
    if (data.media) post.media = data.media;
    return post;
  }

  deletePost(id, authorId) {
    const post = this.posts.get(id);
    if (!post) return false;
    if (post.authorId !== authorId) throw new Error('Not the author');
    this.posts.delete(id);
    this.users.get(authorId).postsCount--;
    return true;
  }

  // ── Feed ─────────────────────────────────────────────

  getFeed(userId, limit = 30) {
    const following = this.follows.get(userId) || new Set();
    const feedAuthors = new Set([userId, ...following]);

    const posts = [];
    for (const post of this.posts.values()) {
      if (feedAuthors.has(post.authorId) && post.visibility === 'public') {
        posts.push(post);
      }
    }

    posts.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
    return posts.slice(0, limit);
  }

  getExploreFeed(limit = 30) {
    const posts = [...this.posts.values()]
      .filter(p => p.visibility === 'public')
      .sort((a, b) => (b.likesCount + b.sharesCount) - (a.likesCount + a.sharesCount));
    return posts.slice(0, limit);
  }

  getUserPosts(userId, limit = 30) {
    return [...this.posts.values()]
      .filter(p => p.authorId === userId && p.visibility === 'public')
      .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
      .slice(0, limit);
  }

  // ── Likes ────────────────────────────────────────────

  likePost(postId, userId) {
    const post = this.posts.get(postId);
    if (!post) throw new Error('Post not found');
    post.likesCount++;
    this._notify(post.authorId, 'like', { postId, userId });
    return { liked: true, likesCount: post.likesCount };
  }

  unlikePost(postId) {
    const post = this.posts.get(postId);
    if (!post) throw new Error('Post not found');
    post.likesCount = Math.max(0, post.likesCount - 1);
    return { liked: false, likesCount: post.likesCount };
  }

  // ── Comments ─────────────────────────────────────────

  createComment(postId, authorId, content) {
    const post = this.posts.get(postId);
    if (!post) throw new Error('Post not found');
    if (!this.users.has(authorId)) throw new Error('Author not found');

    const id = uuid();
    const comment = {
      id,
      postId,
      authorId,
      content,
      likesCount: 0,
      createdAt: new Date().toISOString()
    };
    this.comments.set(id, comment);
    post.commentsCount++;
    this._notify(post.authorId, 'comment', { postId, commentId: id, userId: authorId });
    return comment;
  }

  getComments(postId, limit = 50) {
    return [...this.comments.values()]
      .filter(c => c.postId === postId)
      .sort((a, b) => new Date(a.createdAt) - new Date(b.createdAt))
      .slice(0, limit);
  }

  deleteComment(commentId, authorId) {
    const comment = this.comments.get(commentId);
    if (!comment) return false;
    if (comment.authorId !== authorId) throw new Error('Not the author');
    const post = this.posts.get(comment.postId);
    if (post) post.commentsCount--;
    this.comments.delete(commentId);
    return true;
  }

  // ── Share / Repost ───────────────────────────────────

  sharePost(postId, sharerId) {
    const original = this.posts.get(postId);
    if (!original) throw new Error('Post not found');

    original.sharesCount++;
    const repost = this.createPost(sharerId, {
      content: original.content,
      type: 'repost',
      repostOf: postId
    });

    this._notify(original.authorId, 'share', { postId, userId: sharerId });
    return repost;
  }

  tipPost(postId, tipperId, amount) {
    const post = this.posts.get(postId);
    if (!post) throw new Error('Post not found');
    if (amount <= 0) throw new Error('Amount must be positive');

    const fee = amount * 0.08;
    const creatorReceives = amount - fee;
    post.tips += amount;

    this._notify(post.authorId, 'tip', { postId, userId: tipperId, amount });

    return {
      txId: uuid(),
      amount,
      creatorReceives: +creatorReceives.toFixed(2),
      platformFee: +fee.toFixed(2),
      taxPaid: 0
    };
  }

  // ── Stories ──────────────────────────────────────────

  createStory(authorId, data) {
    if (!this.users.has(authorId)) throw new Error('Author not found');
    const id = uuid();
    const story = {
      id,
      authorId,
      media: data.media || [],
      text: data.text || '',
      expiresAt: new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString(), // 24h
      viewsCount: 0,
      createdAt: new Date().toISOString()
    };
    this.stories.set(id, story);
    return story;
  }

  getStories(userId) {
    const following = this.follows.get(userId) || new Set();
    const authors = new Set([userId, ...following]);
    const now = new Date();
    return [...this.stories.values()]
      .filter(s => authors.has(s.authorId) && new Date(s.expiresAt) > now)
      .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
  }

  // ── Groups ───────────────────────────────────────────

  createGroup(creatorId, data) {
    if (!this.users.has(creatorId)) throw new Error('Creator not found');
    const id = uuid();
    const group = {
      id,
      name: data.name,
      description: data.description || '',
      creatorId,
      members: new Set([creatorId]),
      privacy: data.privacy || 'public',
      createdAt: new Date().toISOString()
    };
    this.groups.set(id, group);
    return { ...group, members: group.members.size };
  }

  joinGroup(groupId, userId) {
    const group = this.groups.get(groupId);
    if (!group) throw new Error('Group not found');
    if (!this.users.has(userId)) throw new Error('User not found');
    group.members.add(userId);
    return { joined: true, members: group.members.size };
  }

  leaveGroup(groupId, userId) {
    const group = this.groups.get(groupId);
    if (!group) throw new Error('Group not found');
    group.members.delete(userId);
    return { left: true, members: group.members.size };
  }

  listGroups(limit = 20) {
    return [...this.groups.values()]
      .map(g => ({ ...g, members: g.members.size }))
      .slice(0, limit);
  }

  // ── Notifications ────────────────────────────────────

  getNotifications(userId, limit = 50) {
    const notifs = this.notifications.get(userId) || [];
    return notifs.slice(-limit).reverse();
  }

  markNotificationsRead(userId) {
    const notifs = this.notifications.get(userId);
    if (!notifs) return 0;
    let count = 0;
    for (const n of notifs) { if (!n.read) { n.read = true; count++; } }
    return count;
  }

  _notify(userId, type, data) {
    const notifs = this.notifications.get(userId);
    if (!notifs) return;
    notifs.push({ id: uuid(), type, data, read: false, createdAt: new Date().toISOString() });
    if (notifs.length > 200) notifs.splice(0, notifs.length - 200); // Cap at 200
  }

  // ── Live Streams ─────────────────────────────────────

  startStream(hostId, data) {
    if (!this.users.has(hostId)) throw new Error('Host not found');
    const id = uuid();
    const stream = {
      id,
      hostId,
      title: data.title || 'Live',
      viewers: 0,
      status: 'live',
      startedAt: new Date().toISOString()
    };
    this.liveStreams.set(id, stream);
    return stream;
  }

  endStream(streamId) {
    const stream = this.liveStreams.get(streamId);
    if (!stream) throw new Error('Stream not found');
    stream.status = 'ended';
    stream.endedAt = new Date().toISOString();
    return stream;
  }

  getLiveStreams() {
    return [...this.liveStreams.values()].filter(s => s.status === 'live');
  }

  // ── Stats ────────────────────────────────────────────

  getStats() {
    return {
      users: this.users.size,
      posts: this.posts.size,
      comments: this.comments.size,
      groups: this.groups.size,
      stories: this.stories.size,
      liveStreams: [...this.liveStreams.values()].filter(s => s.status === 'live').length
    };
  }

  reset() {
    this.users.clear();
    this.posts.clear();
    this.comments.clear();
    this.follows.clear();
    this.followers.clear();
    this.groups.clear();
    this.stories.clear();
    this.notifications.clear();
    this.liveStreams.clear();
  }
}

module.exports = { SocialEngine };
