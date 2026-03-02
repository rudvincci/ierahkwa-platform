'use strict';

const { SocialEngine } = require('../lib/social-engine');

describe('SocialEngine', () => {
  let engine;
  let user1, user2;

  beforeEach(() => {
    engine = new SocialEngine();
    user1 = engine.createProfile('u1', { username: 'maria', displayName: 'María García', bio: 'Artista' });
    user2 = engine.createProfile('u2', { username: 'carlos', displayName: 'Carlos' });
  });

  // ── Profiles ─────────────────────────────────────────

  describe('profiles', () => {
    test('createProfile returns profile with defaults', () => {
      expect(user1.userId).toBe('u1');
      expect(user1.username).toBe('maria');
      expect(user1.verified).toBe(false);
      expect(user1.language).toBe('es');
    });

    test('getProfile returns null for unknown', () => {
      expect(engine.getProfile('fake')).toBeNull();
    });

    test('updateProfile modifies fields', () => {
      const updated = engine.updateProfile('u1', { bio: 'Updated bio', displayName: 'New Name' });
      expect(updated.bio).toBe('Updated bio');
      expect(updated.displayName).toBe('New Name');
    });

    test('searchProfiles finds by username', () => {
      const results = engine.searchProfiles('maria');
      expect(results.length).toBe(1);
      expect(results[0].userId).toBe('u1');
    });
  });

  // ── Follow ───────────────────────────────────────────

  describe('follow', () => {
    test('follow updates counts', () => {
      engine.follow('u1', 'u2');
      expect(engine.getProfile('u1').followingCount).toBe(1);
      expect(engine.getProfile('u2').followersCount).toBe(1);
      expect(engine.isFollowing('u1', 'u2')).toBe(true);
    });

    test('cannot follow self', () => {
      expect(() => engine.follow('u1', 'u1')).toThrow('Cannot follow yourself');
    });

    test('cannot follow twice', () => {
      engine.follow('u1', 'u2');
      expect(() => engine.follow('u1', 'u2')).toThrow('Already following');
    });

    test('unfollow decrements counts', () => {
      engine.follow('u1', 'u2');
      engine.unfollow('u1', 'u2');
      expect(engine.getProfile('u1').followingCount).toBe(0);
      expect(engine.isFollowing('u1', 'u2')).toBe(false);
    });

    test('getFollowers returns profiles', () => {
      engine.follow('u1', 'u2');
      const followers = engine.getFollowers('u2');
      expect(followers.length).toBe(1);
      expect(followers[0].userId).toBe('u1');
    });
  });

  // ── Posts ────────────────────────────────────────────

  describe('posts', () => {
    test('createPost extracts hashtags and mentions', () => {
      const post = engine.createPost('u1', { content: 'Hello #world @carlos #test' });
      expect(post.hashtags).toEqual(['#world', '#test']);
      expect(post.mentions).toEqual(['carlos']);
    });

    test('getPost increments views', () => {
      const post = engine.createPost('u1', { content: 'Test' });
      engine.getPost(post.id);
      engine.getPost(post.id);
      expect(engine.getPost(post.id).viewsCount).toBe(3);
    });

    test('updatePost only by author', () => {
      const post = engine.createPost('u1', { content: 'Original' });
      expect(() => engine.updatePost(post.id, 'u2', { content: 'Hacked' })).toThrow('Not the author');
    });

    test('deletePost removes post', () => {
      const post = engine.createPost('u1', { content: 'Delete me' });
      engine.deletePost(post.id, 'u1');
      expect(engine.getPost(post.id)).toBeNull();
    });

    test('getUserPosts returns author posts', () => {
      engine.createPost('u1', { content: 'Post 1' });
      engine.createPost('u1', { content: 'Post 2' });
      engine.createPost('u2', { content: 'Other user' });
      const posts = engine.getUserPosts('u1');
      expect(posts.length).toBe(2);
    });
  });

  // ── Feed ─────────────────────────────────────────────

  describe('feed', () => {
    test('feed includes own and followed users posts', () => {
      engine.follow('u1', 'u2');
      engine.createPost('u2', { content: 'From u2' });
      engine.createPost('u1', { content: 'From u1' });

      const feed = engine.getFeed('u1');
      expect(feed.length).toBe(2);
    });

    test('explore feed sorted by engagement', () => {
      const p1 = engine.createPost('u1', { content: 'Popular' });
      engine.createPost('u2', { content: 'Less popular' });
      engine.likePost(p1.id, 'u2');
      engine.likePost(p1.id, 'u2');

      const explore = engine.getExploreFeed();
      expect(explore[0].id).toBe(p1.id);
    });
  });

  // ── Comments ─────────────────────────────────────────

  describe('comments', () => {
    let post;
    beforeEach(() => {
      post = engine.createPost('u1', { content: 'Test post' });
    });

    test('createComment increments post comments count', () => {
      engine.createComment(post.id, 'u2', 'Great post!');
      expect(engine.getPost(post.id).commentsCount).toBe(1);
    });

    test('getComments returns sorted by date', () => {
      engine.createComment(post.id, 'u1', 'First');
      engine.createComment(post.id, 'u2', 'Second');
      const comments = engine.getComments(post.id);
      expect(comments.length).toBe(2);
    });

    test('deleteComment decrements count', () => {
      const c = engine.createComment(post.id, 'u2', 'Delete me');
      engine.deleteComment(c.id, 'u2');
      expect(engine.getPost(post.id).commentsCount).toBe(0);
    });
  });

  // ── Likes & Shares ──────────────────────────────────

  describe('likes and shares', () => {
    test('likePost increments count', () => {
      const post = engine.createPost('u1', { content: 'Like me' });
      const result = engine.likePost(post.id, 'u2');
      expect(result.likesCount).toBe(1);
    });

    test('unlikePost decrements count', () => {
      const post = engine.createPost('u1', { content: 'Unlike me' });
      engine.likePost(post.id, 'u2');
      const result = engine.unlikePost(post.id);
      expect(result.likesCount).toBe(0);
    });

    test('sharePost creates repost', () => {
      const post = engine.createPost('u1', { content: 'Share me' });
      const repost = engine.sharePost(post.id, 'u2');
      expect(repost.repostOf).toBe(post.id);
      expect(repost.type).toBe('repost');
      expect(engine.getPost(post.id).sharesCount).toBe(1);
    });

    test('tipPost calculates 92% to creator', () => {
      const post = engine.createPost('u1', { content: 'Tip me' });
      const result = engine.tipPost(post.id, 'u2', 100);
      expect(result.creatorReceives).toBe(92);
      expect(result.platformFee).toBe(8);
      expect(result.taxPaid).toBe(0);
    });
  });

  // ── Stories ──────────────────────────────────────────

  describe('stories', () => {
    test('createStory sets 24h expiry', () => {
      const story = engine.createStory('u1', { text: 'My story', media: ['img.jpg'] });
      expect(story.id).toBeDefined();
      const expires = new Date(story.expiresAt);
      const now = new Date();
      expect(expires.getTime() - now.getTime()).toBeGreaterThan(23 * 60 * 60 * 1000);
    });

    test('getStories returns from followed users', () => {
      engine.follow('u1', 'u2');
      engine.createStory('u2', { text: 'Story' });
      const stories = engine.getStories('u1');
      expect(stories.length).toBe(1);
    });
  });

  // ── Groups ───────────────────────────────────────────

  describe('groups', () => {
    test('createGroup adds creator as member', () => {
      const group = engine.createGroup('u1', { name: 'Artistas', description: 'Art group' });
      expect(group.members).toBe(1);
    });

    test('joinGroup increments members', () => {
      const group = engine.createGroup('u1', { name: 'Test' });
      engine.joinGroup(group.id, 'u2');
      const groups = engine.listGroups();
      expect(groups.find(g => g.id === group.id).members).toBe(2);
    });

    test('leaveGroup decrements members', () => {
      const group = engine.createGroup('u1', { name: 'Test' });
      engine.joinGroup(group.id, 'u2');
      engine.leaveGroup(group.id, 'u2');
      const groups = engine.listGroups();
      expect(groups.find(g => g.id === group.id).members).toBe(1);
    });
  });

  // ── Notifications ────────────────────────────────────

  describe('notifications', () => {
    test('follow generates notification', () => {
      engine.follow('u1', 'u2');
      const notifs = engine.getNotifications('u2');
      expect(notifs.length).toBe(1);
      expect(notifs[0].type).toBe('follow');
    });

    test('like generates notification', () => {
      const post = engine.createPost('u1', { content: 'Test' });
      engine.likePost(post.id, 'u2');
      const notifs = engine.getNotifications('u1');
      expect(notifs.some(n => n.type === 'like')).toBe(true);
    });

    test('markNotificationsRead marks all read', () => {
      engine.follow('u1', 'u2');
      const count = engine.markNotificationsRead('u2');
      expect(count).toBe(1);
    });
  });

  // ── Live Streams ─────────────────────────────────────

  describe('live streams', () => {
    test('startStream and endStream lifecycle', () => {
      const stream = engine.startStream('u1', { title: 'Art Session' });
      expect(stream.status).toBe('live');
      expect(engine.getLiveStreams().length).toBe(1);

      engine.endStream(stream.id);
      expect(engine.getLiveStreams().length).toBe(0);
    });
  });

  // ── Stats & Reset ────────────────────────────────────

  describe('stats', () => {
    test('getStats returns counts', () => {
      const stats = engine.getStats();
      expect(stats.users).toBe(2);
      expect(stats.posts).toBe(0);
    });

    test('reset clears all data', () => {
      engine.createPost('u1', { content: 'test' });
      engine.reset();
      expect(engine.getStats().users).toBe(0);
    });
  });
});
