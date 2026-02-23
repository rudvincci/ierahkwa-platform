using IERAHKWA.Platform.Models;

namespace IERAHKWA.Platform.Services;

public interface ISocialService
{
    // Users
    Task<SocialUser> GetUserAsync(string id);
    Task<SocialUser> GetUserByUsernameAsync(string username);
    Task<SocialUser> CreateUserAsync(string username, string email);
    Task<SocialUser> UpdateUserAsync(string id, SocialUser updates);
    
    // Posts
    Task<Post> CreatePostAsync(CreatePostRequest request);
    Task<Post?> GetPostAsync(string id);
    Task<bool> DeletePostAsync(string id);
    Task<bool> LikePostAsync(string postId, string oderId);
    Task<bool> UnlikePostAsync(string postId, string oderId);
    Task<bool> BookmarkPostAsync(string postId, string oderId);
    
    // Comments
    Task<Comment> AddCommentAsync(string postId, string oderId, string content);
    Task<List<Comment>> GetCommentsAsync(string postId);
    
    // Feed
    Task<FeedResponse> GetFeedAsync(FeedRequest request);
    Task<List<Post>> GetUserPostsAsync(string userId);
    
    // Follow
    Task<bool> FollowUserAsync(FollowRequest request);
    Task<bool> UnfollowUserAsync(FollowRequest request);
    Task<List<SocialUser>> GetFollowersAsync(string userId);
    Task<List<SocialUser>> GetFollowingAsync(string userId);
    
    // Stories
    Task<Story> CreateStoryAsync(string userId, string mediaUrl, string mediaType, string? caption);
    Task<List<Story>> GetStoriesAsync(string userId);
    Task<List<Story>> GetFollowingStoriesAsync(string userId);
    
    // Messages
    Task<Conversation> GetOrCreateConversationAsync(string user1Id, string user2Id);
    Task<List<Conversation>> GetConversationsAsync(string userId);
    Task<Message> SendMessageAsync(SendMessageRequest request);
    Task<List<Message>> GetMessagesAsync(string conversationId);
    
    // Notifications
    Task<List<Notification>> GetNotificationsAsync(string userId);
    Task<bool> MarkNotificationReadAsync(string notificationId);
    
    // Search & Trending
    Task<SearchResult> SearchAsync(SearchRequest request);
    Task<List<TrendingTopic>> GetTrendingAsync();
    
    // Live & Spaces
    Task<LiveStream> StartLiveStreamAsync(string userId, string title);
    Task<List<LiveStream>> GetLiveStreamsAsync();
    Task<Space> CreateSpaceAsync(string hostId, string title);
    Task<List<Space>> GetSpacesAsync();
}

public class SocialService : ISocialService
{
    private readonly ILogger<SocialService> _logger;
    private readonly Random _random = new();
    
    // In-memory storage
    private readonly Dictionary<string, SocialUser> _users = new();
    private readonly Dictionary<string, Post> _posts = new();
    private readonly Dictionary<string, Comment> _comments = new();
    private readonly Dictionary<string, Story> _stories = new();
    private readonly Dictionary<string, Conversation> _conversations = new();
    private readonly Dictionary<string, Message> _messages = new();
    private readonly Dictionary<string, Notification> _notifications = new();
    private readonly List<Follow> _follows = new();
    private readonly HashSet<string> _likes = new(); // "postId:userId"

    public SocialService(ILogger<SocialService> logger)
    {
        _logger = logger;
        InitializeSampleData();
    }

    private void InitializeSampleData()
    {
        // Sample users
        var sampleUsers = new[]
        {
            ("satoshi_nakamoto", "Satoshi Nakamoto", "Creator of Bitcoin", true),
            ("vitalik_eth", "Vitalik Buterin", "Ethereum co-founder", true),
            ("ierahkwa_official", "IERAHKWA", "Sovereign Digital Nation 🏛️", true),
            ("crypto_queen", "Crypto Queen", "Trading & DeFi enthusiast", false),
            ("web3_dev", "Web3 Developer", "Building the future", false),
            ("nft_collector", "NFT Collector", "Digital art lover", false)
        };

        foreach (var (username, name, bio, verified) in sampleUsers)
        {
            var user = new SocialUser
            {
                Username = username,
                DisplayName = name,
                Bio = bio,
                Email = $"{username}@ierahkwa.io",
                IsVerified = verified,
                FollowersCount = _random.Next(1000, 1000000),
                FollowingCount = _random.Next(100, 5000),
                PostsCount = _random.Next(50, 500),
                Avatar = $"https://api.dicebear.com/7.x/avataaars/svg?seed={username}"
            };
            _users[user.Id] = user;
        }

        // Sample posts
        var contents = new[]
        {
            "🚀 Just deployed our new smart contract! The future is decentralized. #Web3 #Crypto",
            "Building in public: Day 47. The AI-powered trading bot is now live! 📈",
            "🏛️ IERAHKWA announces new sovereign digital identity system. Welcome to the future!",
            "The merge was just the beginning. What's next for Ethereum? 🔷",
            "Unpopular opinion: NFTs are more than just JPEGs. They're programmable ownership. 🎨",
            "Market dip? I see opportunity. Time to accumulate. 💎🙌",
            "Just crossed 1M users on our platform! Thank you for believing in us! 🎉"
        };

        var userList = _users.Values.ToList();
        foreach (var content in contents)
        {
            var user = userList[_random.Next(userList.Count)];
            var post = new Post
            {
                UserId = user.Id,
                Username = user.Username,
                DisplayName = user.DisplayName,
                Avatar = user.Avatar,
                IsVerified = user.IsVerified,
                Content = content,
                LikesCount = _random.Next(100, 50000),
                CommentsCount = _random.Next(10, 1000),
                SharesCount = _random.Next(5, 500),
                ViewsCount = _random.Next(1000, 100000),
                CreatedAt = DateTime.UtcNow.AddHours(-_random.Next(1, 72))
            };
            
            // Extract hashtags
            post.Hashtags = content.Split(' ')
                .Where(w => w.StartsWith("#"))
                .Select(w => w.TrimStart('#'))
                .ToList();
            
            _posts[post.Id] = post;
        }
    }

    // ============= USERS =============

    public async Task<SocialUser> GetUserAsync(string id)
    {
        await Task.Delay(10);
        if (_users.TryGetValue(id, out var user))
            return user;
        
        var suffix = id.Length >= 8 ? id[..8] : id;
        return await CreateUserAsync($"user_{suffix}", $"user_{suffix}@ierahkwa.io");
    }

    public async Task<SocialUser> GetUserByUsernameAsync(string username)
    {
        await Task.Delay(10);
        return _users.Values.FirstOrDefault(u => u.Username == username) ?? 
               await CreateUserAsync(username, $"{username}@ierahkwa.io");
    }

    public async Task<SocialUser> CreateUserAsync(string username, string email)
    {
        await Task.Delay(10);
        var user = new SocialUser
        {
            Username = username,
            DisplayName = username,
            Email = email,
            Avatar = $"https://api.dicebear.com/7.x/avataaars/svg?seed={username}"
        };
        _users[user.Id] = user;
        _logger.LogInformation("Created social user: {Username}", username);
        return user;
    }

    public async Task<SocialUser> UpdateUserAsync(string id, SocialUser updates)
    {
        await Task.Delay(10);
        var user = await GetUserAsync(id);
        
        if (!string.IsNullOrEmpty(updates.DisplayName)) user.DisplayName = updates.DisplayName;
        if (!string.IsNullOrEmpty(updates.Bio)) user.Bio = updates.Bio;
        if (!string.IsNullOrEmpty(updates.Avatar)) user.Avatar = updates.Avatar;
        if (!string.IsNullOrEmpty(updates.Website)) user.Website = updates.Website;
        if (!string.IsNullOrEmpty(updates.Location)) user.Location = updates.Location;
        
        return user;
    }

    // ============= POSTS =============

    public async Task<Post> CreatePostAsync(CreatePostRequest request)
    {
        await Task.Delay(50);
        
        var user = await GetUserAsync(request.UserId);
        
        var post = new Post
        {
            UserId = user.Id,
            Username = user.Username,
            DisplayName = user.DisplayName,
            Avatar = user.Avatar,
            IsVerified = user.IsVerified,
            Content = request.Content,
            Media = request.Media ?? new List<string>(),
            MediaType = request.Media?.Count > 0 ? "image" : "none",
            ReplyToId = request.ReplyToId,
            Visibility = request.Visibility
        };

        // Extract hashtags and mentions
        post.Hashtags = request.Content.Split(' ')
            .Where(w => w.StartsWith("#"))
            .Select(w => w.TrimStart('#').TrimEnd(',', '.', '!', '?'))
            .ToList();
        
        post.Mentions = request.Content.Split(' ')
            .Where(w => w.StartsWith("@"))
            .Select(w => w.TrimStart('@').TrimEnd(',', '.', '!', '?'))
            .ToList();

        _posts[post.Id] = post;
        user.PostsCount++;

        // Create notifications for mentions
        foreach (var mention in post.Mentions)
        {
            var mentionedUser = _users.Values.FirstOrDefault(u => u.Username == mention);
            if (mentionedUser != null)
            {
                await CreateNotificationAsync(mentionedUser.Id, "mention", user, post.Id, 
                    $"@{user.Username} mentioned you in a post");
            }
        }

        _logger.LogInformation("Post created by {Username}: {Content}", user.Username, post.Content[..Math.Min(50, post.Content.Length)]);

        return post;
    }

    public async Task<Post?> GetPostAsync(string id)
    {
        await Task.Delay(10);
        return _posts.GetValueOrDefault(id);
    }

    public async Task<bool> DeletePostAsync(string id)
    {
        await Task.Delay(10);
        return _posts.Remove(id);
    }

    public async Task<bool> LikePostAsync(string postId, string oderId)
    {
        await Task.Delay(10);
        
        var key = $"{postId}:{oderId}";
        if (_likes.Contains(key)) return false;
        
        _likes.Add(key);
        
        if (_posts.TryGetValue(postId, out var post))
        {
            post.LikesCount++;
            post.IsLiked = true;
            
            var liker = await GetUserAsync(oderId);
            await CreateNotificationAsync(post.UserId, "like", liker, postId, 
                $"@{liker.Username} liked your post");
        }
        
        return true;
    }

    public async Task<bool> UnlikePostAsync(string postId, string oderId)
    {
        await Task.Delay(10);
        
        var key = $"{postId}:{oderId}";
        if (!_likes.Remove(key)) return false;
        
        if (_posts.TryGetValue(postId, out var post))
        {
            post.LikesCount = Math.Max(0, post.LikesCount - 1);
            post.IsLiked = false;
        }
        
        return true;
    }

    public async Task<bool> BookmarkPostAsync(string postId, string oderId)
    {
        await Task.Delay(10);
        if (_posts.TryGetValue(postId, out var post))
        {
            post.IsBookmarked = !post.IsBookmarked;
            return true;
        }
        return false;
    }

    // ============= COMMENTS =============

    public async Task<Comment> AddCommentAsync(string postId, string oderId, string content)
    {
        await Task.Delay(50);
        
        var user = await GetUserAsync(oderId);
        
        var comment = new Comment
        {
            PostId = postId,
            UserId = user.Id,
            Username = user.Username,
            Avatar = user.Avatar,
            Content = content
        };
        
        _comments[comment.Id] = comment;
        
        if (_posts.TryGetValue(postId, out var post))
        {
            post.CommentsCount++;
            
            await CreateNotificationAsync(post.UserId, "comment", user, postId,
                $"@{user.Username} commented on your post");
        }
        
        return comment;
    }

    public async Task<List<Comment>> GetCommentsAsync(string postId)
    {
        await Task.Delay(10);
        return _comments.Values.Where(c => c.PostId == postId).OrderByDescending(c => c.CreatedAt).ToList();
    }

    // ============= FEED =============

    public async Task<FeedResponse> GetFeedAsync(FeedRequest request)
    {
        await Task.Delay(50);
        
        var query = _posts.Values.AsEnumerable();
        
        switch (request.Type)
        {
            case "following":
                var following = _follows.Where(f => f.FollowerId == request.UserId).Select(f => f.FollowingId);
                query = query.Where(p => following.Contains(p.UserId));
                break;
            case "trending":
                query = query.OrderByDescending(p => p.LikesCount + p.CommentsCount * 2 + p.SharesCount * 3);
                break;
            case "explore":
                query = query.OrderByDescending(p => p.ViewsCount);
                break;
            default:
                query = query.OrderByDescending(p => p.CreatedAt);
                break;
        }
        
        var total = query.Count();
        var posts = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();
        
        return new FeedResponse
        {
            Posts = posts,
            TotalCount = total,
            Page = request.Page,
            HasMore = request.Page * request.PageSize < total
        };
    }

    public async Task<List<Post>> GetUserPostsAsync(string userId)
    {
        await Task.Delay(10);
        return _posts.Values.Where(p => p.UserId == userId).OrderByDescending(p => p.CreatedAt).ToList();
    }

    // ============= FOLLOW =============

    public async Task<bool> FollowUserAsync(FollowRequest request)
    {
        await Task.Delay(10);
        
        if (_follows.Any(f => f.FollowerId == request.FollowerId && f.FollowingId == request.FollowingId))
            return false;
        
        _follows.Add(new Follow
        {
            FollowerId = request.FollowerId,
            FollowingId = request.FollowingId
        });
        
        var follower = await GetUserAsync(request.FollowerId);
        var following = await GetUserAsync(request.FollowingId);
        
        follower.FollowingCount++;
        following.FollowersCount++;
        
        await CreateNotificationAsync(request.FollowingId, "follow", follower, null,
            $"@{follower.Username} started following you");
        
        return true;
    }

    public async Task<bool> UnfollowUserAsync(FollowRequest request)
    {
        await Task.Delay(10);
        
        var follow = _follows.FirstOrDefault(f => f.FollowerId == request.FollowerId && f.FollowingId == request.FollowingId);
        if (follow == null) return false;
        
        _follows.Remove(follow);
        
        var follower = await GetUserAsync(request.FollowerId);
        var following = await GetUserAsync(request.FollowingId);
        
        follower.FollowingCount = Math.Max(0, follower.FollowingCount - 1);
        following.FollowersCount = Math.Max(0, following.FollowersCount - 1);
        
        return true;
    }

    public async Task<List<SocialUser>> GetFollowersAsync(string userId)
    {
        await Task.Delay(10);
        var followerIds = _follows.Where(f => f.FollowingId == userId).Select(f => f.FollowerId);
        return _users.Values.Where(u => followerIds.Contains(u.Id)).ToList();
    }

    public async Task<List<SocialUser>> GetFollowingAsync(string userId)
    {
        await Task.Delay(10);
        var followingIds = _follows.Where(f => f.FollowerId == userId).Select(f => f.FollowingId);
        return _users.Values.Where(u => followingIds.Contains(u.Id)).ToList();
    }

    // ============= STORIES =============

    public async Task<Story> CreateStoryAsync(string userId, string mediaUrl, string mediaType, string? caption)
    {
        await Task.Delay(50);
        
        var user = await GetUserAsync(userId);
        
        var story = new Story
        {
            UserId = user.Id,
            Username = user.Username,
            Avatar = user.Avatar,
            MediaUrl = mediaUrl,
            MediaType = mediaType,
            Caption = caption,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
        
        _stories[story.Id] = story;
        
        return story;
    }

    public async Task<List<Story>> GetStoriesAsync(string userId)
    {
        await Task.Delay(10);
        return _stories.Values
            .Where(s => s.UserId == userId && s.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(s => s.CreatedAt)
            .ToList();
    }

    public async Task<List<Story>> GetFollowingStoriesAsync(string userId)
    {
        await Task.Delay(10);
        var followingIds = _follows.Where(f => f.FollowerId == userId).Select(f => f.FollowingId).ToHashSet();
        followingIds.Add(userId); // Include own stories
        
        return _stories.Values
            .Where(s => followingIds.Contains(s.UserId) && s.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(s => s.CreatedAt)
            .ToList();
    }

    // ============= MESSAGES =============

    public async Task<Conversation> GetOrCreateConversationAsync(string user1Id, string user2Id)
    {
        await Task.Delay(10);
        
        var existing = _conversations.Values.FirstOrDefault(c => 
            !c.IsGroup && c.Participants.Contains(user1Id) && c.Participants.Contains(user2Id));
        
        if (existing != null) return existing;
        
        var conversation = new Conversation
        {
            Participants = new List<string> { user1Id, user2Id }
        };
        
        _conversations[conversation.Id] = conversation;
        
        return conversation;
    }

    public async Task<List<Conversation>> GetConversationsAsync(string userId)
    {
        await Task.Delay(10);
        return _conversations.Values
            .Where(c => c.Participants.Contains(userId))
            .OrderByDescending(c => c.UpdatedAt)
            .ToList();
    }

    public async Task<Message> SendMessageAsync(SendMessageRequest request)
    {
        await Task.Delay(50);
        
        var sender = await GetUserAsync(request.SenderId);
        
        var message = new Message
        {
            ConversationId = request.ConversationId,
            SenderId = sender.Id,
            SenderName = sender.Username,
            SenderAvatar = sender.Avatar,
            Content = request.Content,
            Type = request.Type,
            MediaUrl = request.MediaUrl
        };
        
        _messages[message.Id] = message;
        
        if (_conversations.TryGetValue(request.ConversationId, out var conversation))
        {
            conversation.LastMessage = message;
            conversation.UpdatedAt = DateTime.UtcNow;
            
            // Notify other participants
            foreach (var participantId in conversation.Participants.Where(p => p != request.SenderId))
            {
                conversation.UnreadCount++;
                await CreateNotificationAsync(participantId, "message", sender, null,
                    $"@{sender.Username} sent you a message");
            }
        }
        
        return message;
    }

    public async Task<List<Message>> GetMessagesAsync(string conversationId)
    {
        await Task.Delay(10);
        return _messages.Values
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.CreatedAt)
            .ToList();
    }

    // ============= NOTIFICATIONS =============

    private async Task CreateNotificationAsync(string userId, string type, SocialUser fromUser, string? postId, string message)
    {
        await Task.Delay(10);
        
        var notification = new Notification
        {
            UserId = userId,
            Type = type,
            FromUserId = fromUser.Id,
            FromUsername = fromUser.Username,
            FromAvatar = fromUser.Avatar,
            PostId = postId,
            Message = message
        };
        
        _notifications[notification.Id] = notification;
    }

    public async Task<List<Notification>> GetNotificationsAsync(string userId)
    {
        await Task.Delay(10);
        return _notifications.Values
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .ToList();
    }

    public async Task<bool> MarkNotificationReadAsync(string notificationId)
    {
        await Task.Delay(10);
        if (_notifications.TryGetValue(notificationId, out var notification))
        {
            notification.IsRead = true;
            return true;
        }
        return false;
    }

    // ============= SEARCH & TRENDING =============

    public async Task<SearchResult> SearchAsync(SearchRequest request)
    {
        await Task.Delay(50);
        
        var query = request.Query.ToLower();
        
        var users = request.Type is "all" or "users"
            ? _users.Values.Where(u => 
                u.Username.ToLower().Contains(query) || 
                (u.DisplayName?.ToLower().Contains(query) ?? false))
                .Take(10).ToList()
            : new List<SocialUser>();
        
        var posts = request.Type is "all" or "posts"
            ? _posts.Values.Where(p => p.Content.ToLower().Contains(query))
                .OrderByDescending(p => p.LikesCount)
                .Take(20).ToList()
            : new List<Post>();
        
        var hashtags = request.Type is "all" or "hashtags"
            ? (await GetTrendingAsync()).Where(t => t.Hashtag.ToLower().Contains(query)).ToList()
            : new List<TrendingTopic>();
        
        return new SearchResult
        {
            Users = users,
            Posts = posts,
            Hashtags = hashtags
        };
    }

    public async Task<List<TrendingTopic>> GetTrendingAsync()
    {
        await Task.Delay(10);
        
        var hashtags = _posts.Values
            .SelectMany(p => p.Hashtags)
            .GroupBy(h => h.ToLower())
            .Select(g => new TrendingTopic
            {
                Hashtag = g.Key,
                PostsCount = g.Count() * _random.Next(100, 10000)
            })
            .OrderByDescending(t => t.PostsCount)
            .Take(10)
            .ToList();
        
        // Add some default trending if needed
        if (hashtags.Count < 5)
        {
            hashtags.AddRange(new[]
            {
                new TrendingTopic { Hashtag = "crypto", PostsCount = 125000, Category = "tech" },
                new TrendingTopic { Hashtag = "web3", PostsCount = 98000, Category = "tech" },
                new TrendingTopic { Hashtag = "IERAHKWA", PostsCount = 75000, Category = "general" },
                new TrendingTopic { Hashtag = "nft", PostsCount = 65000, Category = "tech" },
                new TrendingTopic { Hashtag = "defi", PostsCount = 45000, Category = "tech" }
            });
        }
        
        for (int i = 0; i < hashtags.Count; i++)
            hashtags[i].Rank = i + 1;
        
        return hashtags;
    }

    // ============= LIVE & SPACES =============

    public async Task<LiveStream> StartLiveStreamAsync(string userId, string title)
    {
        await Task.Delay(50);
        
        var user = await GetUserAsync(userId);
        
        return new LiveStream
        {
            UserId = user.Id,
            Username = user.Username,
            Avatar = user.Avatar,
            Title = title,
            StreamUrl = $"rtmp://live.ierahkwa.io/{Guid.NewGuid():N}",
            ThumbnailUrl = user.Avatar ?? ""
        };
    }

    public async Task<List<LiveStream>> GetLiveStreamsAsync()
    {
        await Task.Delay(10);
        
        var users = _users.Values.Take(5).ToList();
        return users.Select(u => new LiveStream
        {
            UserId = u.Id,
            Username = u.Username,
            Avatar = u.Avatar,
            Title = $"Live with {u.DisplayName}",
            ViewersCount = _random.Next(100, 10000),
            LikesCount = _random.Next(500, 50000)
        }).ToList();
    }

    public async Task<Space> CreateSpaceAsync(string hostId, string title)
    {
        await Task.Delay(50);
        
        var host = await GetUserAsync(hostId);
        
        return new Space
        {
            HostId = host.Id,
            HostUsername = host.Username,
            Title = title,
            Speakers = new List<SpaceParticipant>
            {
                new() { UserId = host.Id, Username = host.Username, Avatar = host.Avatar, IsSpeaker = true, IsMuted = false }
            }
        };
    }

    public async Task<List<Space>> GetSpacesAsync()
    {
        await Task.Delay(10);
        
        var users = _users.Values.Take(3).ToList();
        return users.Select(u => new Space
        {
            HostId = u.Id,
            HostUsername = u.Username,
            Title = $"Space: {u.DisplayName} talks crypto",
            ListenersCount = _random.Next(50, 5000),
            Speakers = new List<SpaceParticipant>
            {
                new() { UserId = u.Id, Username = u.Username, Avatar = u.Avatar, IsSpeaker = true }
            }
        }).ToList();
    }
}
