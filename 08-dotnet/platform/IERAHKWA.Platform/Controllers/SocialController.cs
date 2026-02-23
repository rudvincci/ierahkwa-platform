using Microsoft.AspNetCore.Mvc;
using IERAHKWA.Platform.Services;
using IERAHKWA.Platform.Models;

namespace IERAHKWA.Platform.Controllers;

[ApiController]
[Route("api/social")]
public class SocialController : ControllerBase
{
    private readonly ISocialService _social;
    private readonly ILogger<SocialController> _logger;

    public SocialController(ISocialService social, ILogger<SocialController> logger)
    {
        _social = social;
        _logger = logger;
    }

    // ============= USERS =============

    [HttpGet("user/{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _social.GetUserAsync(id);
        return Ok(new { success = true, data = user });
    }

    [HttpGet("user/username/{username}")]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        var user = await _social.GetUserByUsernameAsync(username);
        return Ok(new { success = true, data = user });
    }

    [HttpPost("user")]
    public async Task<IActionResult> CreateUser([FromBody] CreateSocialUserRequest request)
    {
        var user = await _social.CreateUserAsync(request.Username, request.Email);
        return Ok(new { success = true, data = user });
    }

    [HttpPut("user/{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] Models.SocialUser updates)
    {
        var user = await _social.UpdateUserAsync(id, updates);
        return Ok(new { success = true, data = user });
    }

    // ============= POSTS =============

    [HttpPost("post")]
    public async Task<IActionResult> CreatePost([FromBody] Models.CreatePostRequest request)
    {
        var post = await _social.CreatePostAsync(request);
        return Ok(new { success = true, data = post });
    }

    [HttpGet("post/{id}")]
    public async Task<IActionResult> GetPost(string id)
    {
        var post = await _social.GetPostAsync(id);
        return post != null 
            ? Ok(new { success = true, data = post })
            : NotFound(new { success = false, error = "Post not found" });
    }

    [HttpDelete("post/{id}")]
    public async Task<IActionResult> DeletePost(string id)
    {
        var result = await _social.DeletePostAsync(id);
        return Ok(new { success = result });
    }

    [HttpPost("post/{id}/like")]
    public async Task<IActionResult> LikePost(string id, [FromBody] UserIdRequest request)
    {
        var result = await _social.LikePostAsync(id, request.UserId);
        return Ok(new { success = result });
    }

    [HttpDelete("post/{id}/like")]
    public async Task<IActionResult> UnlikePost(string id, [FromBody] UserIdRequest request)
    {
        var result = await _social.UnlikePostAsync(id, request.UserId);
        return Ok(new { success = result });
    }

    [HttpPost("post/{id}/bookmark")]
    public async Task<IActionResult> BookmarkPost(string id, [FromBody] UserIdRequest request)
    {
        var result = await _social.BookmarkPostAsync(id, request.UserId);
        return Ok(new { success = result });
    }

    // ============= COMMENTS =============

    [HttpPost("post/{id}/comment")]
    public async Task<IActionResult> AddComment(string id, [FromBody] AddCommentRequest request)
    {
        var comment = await _social.AddCommentAsync(id, request.UserId, request.Content);
        return Ok(new { success = true, data = comment });
    }

    [HttpGet("post/{id}/comments")]
    public async Task<IActionResult> GetComments(string id)
    {
        var comments = await _social.GetCommentsAsync(id);
        return Ok(new { success = true, data = comments });
    }

    // ============= FEED =============

    [HttpPost("feed")]
    public async Task<IActionResult> GetFeed([FromBody] Models.FeedRequest request)
    {
        var feed = await _social.GetFeedAsync(request);
        return Ok(new { success = true, data = feed });
    }

    [HttpGet("user/{id}/posts")]
    public async Task<IActionResult> GetUserPosts(string id)
    {
        var posts = await _social.GetUserPostsAsync(id);
        return Ok(new { success = true, data = posts });
    }

    // ============= FOLLOW =============

    [HttpPost("follow")]
    public async Task<IActionResult> Follow([FromBody] Models.FollowRequest request)
    {
        var result = await _social.FollowUserAsync(request);
        return Ok(new { success = result });
    }

    [HttpDelete("follow")]
    public async Task<IActionResult> Unfollow([FromBody] Models.FollowRequest request)
    {
        var result = await _social.UnfollowUserAsync(request);
        return Ok(new { success = result });
    }

    [HttpGet("user/{id}/followers")]
    public async Task<IActionResult> GetFollowers(string id)
    {
        var followers = await _social.GetFollowersAsync(id);
        return Ok(new { success = true, data = followers });
    }

    [HttpGet("user/{id}/following")]
    public async Task<IActionResult> GetFollowing(string id)
    {
        var following = await _social.GetFollowingAsync(id);
        return Ok(new { success = true, data = following });
    }

    // ============= STORIES =============

    [HttpPost("story")]
    public async Task<IActionResult> CreateStory([FromBody] CreateStoryRequest request)
    {
        var story = await _social.CreateStoryAsync(request.UserId, request.MediaUrl, request.MediaType, request.Caption);
        return Ok(new { success = true, data = story });
    }

    [HttpGet("user/{id}/stories")]
    public async Task<IActionResult> GetStories(string id)
    {
        var stories = await _social.GetStoriesAsync(id);
        return Ok(new { success = true, data = stories });
    }

    [HttpGet("stories/following/{userId}")]
    public async Task<IActionResult> GetFollowingStories(string userId)
    {
        var stories = await _social.GetFollowingStoriesAsync(userId);
        return Ok(new { success = true, data = stories });
    }

    // ============= MESSAGES =============

    [HttpPost("conversation")]
    public async Task<IActionResult> GetOrCreateConversation([FromBody] ConversationRequest request)
    {
        var conversation = await _social.GetOrCreateConversationAsync(request.User1Id, request.User2Id);
        return Ok(new { success = true, data = conversation });
    }

    [HttpGet("conversations/{userId}")]
    public async Task<IActionResult> GetConversations(string userId)
    {
        var conversations = await _social.GetConversationsAsync(userId);
        return Ok(new { success = true, data = conversations });
    }

    [HttpPost("message")]
    public async Task<IActionResult> SendMessage([FromBody] Models.SendMessageRequest request)
    {
        var message = await _social.SendMessageAsync(request);
        return Ok(new { success = true, data = message });
    }

    [HttpGet("conversation/{id}/messages")]
    public async Task<IActionResult> GetMessages(string id)
    {
        var messages = await _social.GetMessagesAsync(id);
        return Ok(new { success = true, data = messages });
    }

    // ============= NOTIFICATIONS =============

    [HttpGet("notifications/{userId}")]
    public async Task<IActionResult> GetNotifications(string userId)
    {
        var notifications = await _social.GetNotificationsAsync(userId);
        return Ok(new { success = true, data = notifications });
    }

    [HttpPost("notification/{id}/read")]
    public async Task<IActionResult> MarkNotificationRead(string id)
    {
        var result = await _social.MarkNotificationReadAsync(id);
        return Ok(new { success = result });
    }

    // ============= SEARCH & TRENDING =============

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] Models.SearchRequest request)
    {
        var results = await _social.SearchAsync(request);
        return Ok(new { success = true, data = results });
    }

    [HttpGet("trending")]
    public async Task<IActionResult> GetTrending()
    {
        var trending = await _social.GetTrendingAsync();
        return Ok(new { success = true, data = trending });
    }

    // ============= LIVE & SPACES =============

    [HttpPost("live")]
    public async Task<IActionResult> StartLiveStream([FromBody] StartLiveRequest request)
    {
        var stream = await _social.StartLiveStreamAsync(request.UserId, request.Title);
        return Ok(new { success = true, data = stream });
    }

    [HttpGet("live")]
    public async Task<IActionResult> GetLiveStreams()
    {
        var streams = await _social.GetLiveStreamsAsync();
        return Ok(new { success = true, data = streams });
    }

    [HttpPost("space")]
    public async Task<IActionResult> CreateSpace([FromBody] CreateSpaceRequest request)
    {
        var space = await _social.CreateSpaceAsync(request.HostId, request.Title);
        return Ok(new { success = true, data = space });
    }

    [HttpGet("spaces")]
    public async Task<IActionResult> GetSpaces()
    {
        var spaces = await _social.GetSpacesAsync();
        return Ok(new { success = true, data = spaces });
    }
}

// Request DTOs
public class CreateSocialUserRequest
{
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
}

public class UserIdRequest
{
    public string UserId { get; set; } = "";
}

public class AddCommentRequest
{
    public string UserId { get; set; } = "";
    public string Content { get; set; } = "";
}

public class CreateStoryRequest
{
    public string UserId { get; set; } = "";
    public string MediaUrl { get; set; } = "";
    public string MediaType { get; set; } = "image";
    public string? Caption { get; set; }
}

public class ConversationRequest
{
    public string User1Id { get; set; } = "";
    public string User2Id { get; set; } = "";
}

public class StartLiveRequest
{
    public string UserId { get; set; } = "";
    public string Title { get; set; } = "";
}

public class CreateSpaceRequest
{
    public string HostId { get; set; } = "";
    public string Title { get; set; } = "";
}
