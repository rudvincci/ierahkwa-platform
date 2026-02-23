namespace IERAHKWA.Platform.Models;

// ============= SOCIAL MEDIA MODELS =============

public class SocialUser
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Username { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Bio { get; set; }
    public string? Avatar { get; set; }
    public string? Cover { get; set; }
    public string? Website { get; set; }
    public string? Location { get; set; }
    public bool IsVerified { get; set; } = false;
    public bool IsPrivate { get; set; } = false;
    public int FollowersCount { get; set; } = 0;
    public int FollowingCount { get; set; } = 0;
    public int PostsCount { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
}

// POSTS
public class Post
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public string Username { get; set; } = "";
    public string? DisplayName { get; set; }
    public string? Avatar { get; set; }
    public bool IsVerified { get; set; }
    public string Content { get; set; } = "";
    public List<string> Media { get; set; } = new(); // URLs to images/videos
    public string MediaType { get; set; } = "none"; // none, image, video, gallery
    public List<string> Hashtags { get; set; } = new();
    public List<string> Mentions { get; set; } = new();
    public int LikesCount { get; set; } = 0;
    public int CommentsCount { get; set; } = 0;
    public int SharesCount { get; set; } = 0;
    public int ViewsCount { get; set; } = 0;
    public bool IsLiked { get; set; } = false;
    public bool IsBookmarked { get; set; } = false;
    public string? ReplyToId { get; set; } // If it's a reply
    public string? RepostId { get; set; } // If it's a repost
    public string Visibility { get; set; } = "public"; // public, followers, private
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class CreatePostRequest
{
    public string UserId { get; set; } = "";
    public string Content { get; set; } = "";
    public List<string>? Media { get; set; }
    public string? ReplyToId { get; set; }
    public string Visibility { get; set; } = "public";
}

// COMMENTS
public class Comment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PostId { get; set; } = "";
    public string UserId { get; set; } = "";
    public string Username { get; set; } = "";
    public string? Avatar { get; set; }
    public string Content { get; set; } = "";
    public int LikesCount { get; set; } = 0;
    public int RepliesCount { get; set; } = 0;
    public bool IsLiked { get; set; } = false;
    public string? ParentId { get; set; } // For nested replies
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// STORIES
public class Story
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public string Username { get; set; } = "";
    public string? Avatar { get; set; }
    public string MediaUrl { get; set; } = "";
    public string MediaType { get; set; } = "image"; // image, video
    public string? Caption { get; set; }
    public List<string> ViewedBy { get; set; } = new();
    public int ViewsCount { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
}

// MESSAGES
public class Conversation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public List<string> Participants { get; set; } = new();
    public string? GroupName { get; set; }
    public string? GroupAvatar { get; set; }
    public bool IsGroup { get; set; } = false;
    public Message? LastMessage { get; set; }
    public int UnreadCount { get; set; } = 0;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class Message
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ConversationId { get; set; } = "";
    public string SenderId { get; set; } = "";
    public string SenderName { get; set; } = "";
    public string? SenderAvatar { get; set; }
    public string Content { get; set; } = "";
    public string Type { get; set; } = "text"; // text, image, video, audio, file, emoji
    public string? MediaUrl { get; set; }
    public bool IsRead { get; set; } = false;
    public List<string> ReadBy { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class SendMessageRequest
{
    public string ConversationId { get; set; } = "";
    public string SenderId { get; set; } = "";
    public string Content { get; set; } = "";
    public string Type { get; set; } = "text";
    public string? MediaUrl { get; set; }
}

// NOTIFICATIONS
public class Notification
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public string Type { get; set; } = ""; // like, comment, follow, mention, repost, message
    public string FromUserId { get; set; } = "";
    public string FromUsername { get; set; } = "";
    public string? FromAvatar { get; set; }
    public string? PostId { get; set; }
    public string Message { get; set; } = "";
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// FOLLOW
public class Follow
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FollowerId { get; set; } = "";
    public string FollowingId { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class FollowRequest
{
    public string FollowerId { get; set; } = "";
    public string FollowingId { get; set; } = "";
}

// TRENDING
public class TrendingTopic
{
    public int Rank { get; set; }
    public string Hashtag { get; set; } = "";
    public int PostsCount { get; set; }
    public string Category { get; set; } = ""; // general, news, sports, entertainment, tech
}

// FEED
public class FeedRequest
{
    public string UserId { get; set; } = "";
    public string Type { get; set; } = "home"; // home, following, explore, trending
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class FeedResponse
{
    public List<Post> Posts { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public bool HasMore { get; set; }
}

// SEARCH
public class SearchRequest
{
    public string Query { get; set; } = "";
    public string Type { get; set; } = "all"; // all, users, posts, hashtags
    public int Page { get; set; } = 1;
}

public class SearchResult
{
    public List<SocialUser> Users { get; set; } = new();
    public List<Post> Posts { get; set; } = new();
    public List<TrendingTopic> Hashtags { get; set; } = new();
}

// LIVE STREAMING
public class LiveStream
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public string Username { get; set; } = "";
    public string? Avatar { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string StreamUrl { get; set; } = "";
    public string ThumbnailUrl { get; set; } = "";
    public int ViewersCount { get; set; } = 0;
    public int LikesCount { get; set; } = 0;
    public string Status { get; set; } = "live"; // live, ended
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }
}

// POLLS
public class Poll
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PostId { get; set; } = "";
    public string Question { get; set; } = "";
    public List<PollOption> Options { get; set; } = new();
    public int TotalVotes { get; set; } = 0;
    public DateTime ExpiresAt { get; set; }
    public bool HasVoted { get; set; } = false;
    public int? VotedOptionIndex { get; set; }
}

public class PollOption
{
    public int Index { get; set; }
    public string Text { get; set; } = "";
    public int Votes { get; set; } = 0;
    public decimal Percentage { get; set; } = 0;
}

// SPACES (Audio rooms)
public class Space
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string HostId { get; set; } = "";
    public string HostUsername { get; set; } = "";
    public string Title { get; set; } = "";
    public List<SpaceParticipant> Speakers { get; set; } = new();
    public int ListenersCount { get; set; } = 0;
    public string Status { get; set; } = "live"; // scheduled, live, ended
    public DateTime? ScheduledAt { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
}

public class SpaceParticipant
{
    public string UserId { get; set; } = "";
    public string Username { get; set; } = "";
    public string? Avatar { get; set; }
    public bool IsSpeaker { get; set; }
    public bool IsMuted { get; set; } = true;
}
