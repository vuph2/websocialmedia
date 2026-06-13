using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using web.Data;
using web.Hubs;
using web.Models;

namespace web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IHubContext<NexusHub> _hubContext;

        public HomeController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env,
            IHubContext<NexusHub> hubContext)
        {
            _db = db;
            _userManager = userManager;
            _env = env;
            _hubContext = hubContext;
        }

        // GET /Home/Index
        public async Task<IActionResult> Index()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vm = new FeedPageViewModel();
            try
            {
                var followingIds = await _db.Follows
                    .Where(f => f.FollowerId == currentUserId)
                    .Select(f => f.FollowedId)
                    .ToListAsync();

                var blockedUserIds = await _db.BlockedUsers
                    .Where(b => b.BlockerId == currentUserId || b.BlockedUserId == currentUserId)
                    .Select(b => b.BlockerId == currentUserId ? b.BlockedUserId : b.BlockerId)
                    .ToListAsync();

                var activeStories = await _db.Stories
                    .Include(s => s.User)
                    .Include(s => s.StoryLikes).ThenInclude(l => l.User)
                    .Include(s => s.StoryViews).ThenInclude(v => v.User)
                    .Where(s => s.ExpiresAt > DateTime.UtcNow 
                             && !blockedUserIds.Contains(s.UserId)
                             && (s.UserId == currentUserId || followingIds.Contains(s.UserId)))
                    .OrderByDescending(s => s.CreatedAt)
                    .ToListAsync();

                vm.UserStories = activeStories
                    .GroupBy(s => s.UserId)
                    .Select(g => new UserStoriesViewModel
                    {
                        UserId = g.Key,
                        AuthorName = $"{g.First().User.FirstName} {g.First().User.LastName}".Trim(),
                        AuthorAvatar = g.First().User.ProfilePictureUrl,
                        Stories = g.Select(s => new StoryFeedViewModel
                        {
                            Id = s.Id,
                            UserId = s.UserId,
                            AuthorName = $"{s.User.FirstName} {s.User.LastName}".Trim(),
                            AuthorAvatar = s.User.ProfilePictureUrl,
                            MediaUrl = s.MediaUrl,
                            MediaType = s.MediaType,
                            CreatedAt = s.CreatedAt,
                            IsOwn = s.UserId == currentUserId,
                            ViewCount = s.StoryViews.Count,
                            Reactions = s.StoryLikes.Select(l => new StoryStatViewModel {
                                UserId = l.UserId,
                                UserName = $"{l.User.FirstName} {l.User.LastName}".Trim(),
                                UserAvatar = l.User.ProfilePictureUrl,
                                ReactionType = l.ReactionType
                            }).ToList(),
                            Viewers = s.StoryViews.Select(v => new StoryStatViewModel {
                                UserId = v.UserId,
                                UserName = $"{v.User.FirstName} {v.User.LastName}".Trim(),
                                UserAvatar = v.User.ProfilePictureUrl
                            }).ToList()
                        }).OrderBy(s => s.CreatedAt).ToList()
                    })
                    .OrderByDescending(u => u.UserId == currentUserId)
                    .ThenByDescending(u => u.Stories.Any() ? u.Stories.Max(s => s.CreatedAt) : DateTime.MinValue)
                    .ToList();

                var posts = await _db.Posts
                    .Include(p => p.User)
                    .Include(p => p.Likes)
                    .Include(p => p.Media)
                    .Include(p => p.Comments).ThenInclude(c => c.User)
                    .Include(p => p.PollOptions).ThenInclude(o => o.Votes)
                    .Where(p => !blockedUserIds.Contains(p.UserId) &&
                                (p.Privacy == "public" || 
                                 p.UserId == currentUserId || 
                                 (p.Privacy == "followers" && followingIds.Contains(p.UserId))))
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(20)
                    .ToListAsync();

                vm.Posts = posts.Select(p =>
                {
                    var totalVotes = p.PollOptions.Sum(o => o.Votes.Count);
                    var userLike = p.Likes.FirstOrDefault(l => l.UserId == currentUserId);
                    return new PostFeedViewModel
                    {
                        Id            = p.Id,
                        Content       = p.Content,
                        ImageUrl      = p.ImageUrl,
                        PostType      = p.PostType,
                        Feeling       = p.Feeling,
                        CreatedAt     = p.CreatedAt,
                        UpdatedAt     = p.UpdatedAt,
                        AuthorId      = p.UserId,
                        AuthorName    = $"{p.User.FirstName} {p.User.LastName}".Trim(),
                        AuthorUsername = p.User.UserName ?? p.User.Email ?? "user",
                        AuthorAvatar  = p.User.ProfilePictureUrl,
                        ImageUrls     = p.Media.Select(m => m.ImageUrl).ToList(),
                        LikeCount     = p.Likes.Count,
                        CommentCount  = p.Comments.Count,
                        IsLikedByCurrentUser = userLike != null,
                        CurrentUserReaction  = userLike?.ReactionType,
                        IsOwnPost     = p.UserId == currentUserId,
                        IsFollowedByAuthor = _db.Follows.Any(f => f.FollowerId == currentUserId && f.FollowedId == p.UserId),
                        HasVoted      = p.PollOptions.Any(o => o.Votes.Any(v => v.UserId == currentUserId)),
                        PollOptions   = p.PollOptions.Select(o => new PollOptionViewModel
                        {
                            Id         = o.Id,
                            OptionText = o.OptionText,
                            VoteCount  = o.Votes.Count,
                            VotedByCurrentUser = o.Votes.Any(v => v.UserId == currentUserId),
                            Percentage = totalVotes == 0 ? 0 : Math.Round((double)o.Votes.Count / totalVotes * 100, 1)
                        }).ToList(),
                        RecentComments = p.Comments
                            .OrderByDescending(c => c.CreatedAt)
                            .Take(2)
                            .Select(c => new CommentViewModel
                            {
                                Id           = c.Id,
                                Content      = c.Content,
                                AuthorName   = $"{c.User.FirstName} {c.User.LastName}".Trim(),
                                AuthorAvatar = c.User.ProfilePictureUrl,
                                CreatedAt    = c.CreatedAt,
                                IsOwnComment = c.UserId == currentUserId
                            }).ToList()
                    };
                }).ToList();

                vm.SuggestedUsers = await _db.Users
                    .Where(u => u.Id != currentUserId && !_db.Follows.Any(f => f.FollowerId == currentUserId && f.FollowedId == u.Id))
                    .OrderBy(u => Guid.NewGuid()) 
                    .Take(3)
                    .Select(u => new SuggestUserViewModel
                    {
                        Id     = u.Id,
                        Name   = $"{u.FirstName} {u.LastName}".Trim(),
                        Avatar = u.ProfilePictureUrl,
                        Bio    = u.Bio,
                        IsFollowed = false
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                TempData["FeedError"] = $"Could not load posts: {ex.Message}";
            }
            return View(vm);
        }

        // GET /Home/CreatePost
        public IActionResult CreatePost() => View(new CreatePostViewModel());

        // POST /Home/CreatePost
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(CreatePostViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var post = new Post
            {
                Content   = model.Content ?? string.Empty,
                PostType  = model.PostType,
                Feeling   = model.Feeling,
                Privacy   = model.Privacy,
                UserId    = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            _db.Posts.Add(post);
            await _db.SaveChangesAsync();

            // Save multiple media files
            if (model.MediaFiles != null && model.MediaFiles.Any())
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsDir);

                foreach (var file in model.MediaFiles)
                {
                    if (file.Length > 0)
                    {
                        var cleanFileName = Path.GetFileName(file.FileName).Replace(" ", "_");
                        var fileName = $"{Guid.NewGuid()}_{cleanFileName}";
                        using var stream = new FileStream(Path.Combine(uploadsDir, fileName), FileMode.Create);
                        await file.CopyToAsync(stream);
                        
                        var media = new PostMedia
                        {
                            PostId = post.Id,
                            ImageUrl = $"/uploads/{fileName}"
                        };
                        _db.PostMedia.Add(media);

                        // Set the first image as the main Thumbnail ImageUrl for backward compatibility
                        if (string.IsNullOrEmpty(post.ImageUrl))
                        {
                            post.ImageUrl = media.ImageUrl;
                        }
                    }
                }
                await _db.SaveChangesAsync();
            }

            // Save poll options if poll type
            if (model.PostType == "poll" && model.PollOptions.Any())
            {
                foreach (var opt in model.PollOptions.Where(o => !string.IsNullOrWhiteSpace(o)))
                {
                    _db.PollOptions.Add(new PollOption
                    {
                        PostId     = post.Id,
                        OptionText = opt.Trim()
                    });
                }
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // POST /Home/DeletePost
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var userId = _userManager.GetUserId(User);
            var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId);
            if (post == null)
                return Json(new { success = false, message = "Post not found or not yours." });

            // Fetch and delete all associated media files
            var postMedia = await _db.PostMedia.Where(m => m.PostId == postId).ToListAsync();
            foreach (var m in postMedia)
            {
                var filePath = Path.Combine(_env.WebRootPath, m.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            _db.Posts.Remove(post);
            await _db.SaveChangesAsync();

            return Json(new { success = true });
        }

        // POST /Home/ToggleLike
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLike(int postId, string type = "like")
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var existing = await _db.PostLikes
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

            bool liked = true;
            if (existing != null)
            {
                if (existing.ReactionType == type)
                {
                    _db.PostLikes.Remove(existing);
                    liked = false;
                }
                else
                {
                    existing.ReactionType = type;
                    _db.PostLikes.Update(existing);
                    liked = true;
                }
            }
            else
            {
                _db.PostLikes.Add(new PostLike { PostId = postId, UserId = userId, ReactionType = type });
                liked = true;
            }

            await _db.SaveChangesAsync();
            var likeCount = await _db.PostLikes.CountAsync(l => l.PostId == postId);

            // Notify only on NEW or CHANGED reaction
            if (liked)
            {
                var post = await _db.Posts.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == postId);
                if (post != null && post.UserId != userId)
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    var name = $"{currentUser?.FirstName} {currentUser?.LastName}".Trim();
                    
                    var icon = type switch {
                        "love" => "bi-heart-fill",
                        "haha" => "bi-emoji-laughing-fill",
                        "wow"  => "bi-emoji-astonished-fill",
                        "sad"  => "bi-emoji-frown-fill",
                        "angry"=> "bi-emoji-angry-fill",
                        _      => "bi-hand-thumbs-up-fill"
                    };

                    _db.Notifications.Add(new Notification { 
                        UserId = post.UserId, 
                        Content = $"{name} reacted with {type}", 
                        Type = "like", 
                        RelatedUrl = $"/Home/Profile/{post.UserId}" 
                    });
                    await _db.SaveChangesAsync();

                    await _hubContext.Clients.Group(post.UserId).SendAsync("ReceiveNotification", new
                    {
                        type = "like",
                        message = $"{name} reacted with {type}",
                        icon = icon,
                        fromUserId = userId,
                        postId
                    });
                }
            }

            return Json(new { liked, likeCount, reactionType = type });
        }

        // POST /Home/AddComment
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int postId, string content)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            if (string.IsNullOrWhiteSpace(content)) return BadRequest();

            var comment = new Comment
            {
                PostId    = postId,
                UserId    = user.Id,
                Content   = content.Trim(),
                CreatedAt = DateTime.UtcNow
            };
            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();

            // ── SignalR: Notify post author about the comment ──
            var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post != null && post.UserId != user.Id)
            {
                var name = $"{user.FirstName} {user.LastName}".Trim();

                _db.Notifications.Add(new Notification { UserId = post.UserId, Content = $"{name} commented on your post", Type = "comment", RelatedUrl = $"/Home/Profile/{post.UserId}" });
                await _db.SaveChangesAsync();

                await _hubContext.Clients.Group(post.UserId).SendAsync("ReceiveNotification", new
                {
                    type = "comment",
                    message = $"{name} commented on your post",
                    icon = "bi-chat-text-fill",
                    fromUserId = user.Id,
                    postId
                });
            }

            return Json(new
            {
                commentId    = comment.Id,
                authorName   = $"{user.FirstName} {user.LastName}".Trim(),
                authorAvatar = user.ProfilePictureUrl,
                content      = comment.Content,
                createdAt    = comment.CreatedAt.ToString("MMM d")
            });
        }

        // GET /Home/GetPost/{id}
        [HttpGet]
        public async Task<IActionResult> GetPost(int id)
        {
            var post = await _db.Posts.FindAsync(id);
            if (post == null) return NotFound();
            return Json(new { content = post.Content, privacy = post.Privacy });
        }

        // POST /Home/EditPost
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int Id, string Content, string Privacy = "public")
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var post = await _db.Posts.FindAsync(Id);
            if (post == null) return NotFound();
            if (post.UserId != userId) return Forbid();

            post.Content = Content.Trim();
            post.Privacy = Privacy;
            post.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            var formatted = web.Helpers.HtmlFormatter.FormatPostContent(post.Content).ToString();
            return Json(new { success = true, content = formatted });
        }

        // POST /Home/DeleteComment
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var comment = await _db.Comments.Include(c => c.Post).FirstOrDefaultAsync(c => c.Id == commentId);
            if (comment == null) return NotFound();

            // Allow deletion if user owns the comment OR owns the post
            if (comment.UserId != userId && comment.Post.UserId != userId)
                return Forbid();

            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        // GET /Home/AllComments?postId=X
        [HttpGet]
        public async Task<IActionResult> AllComments(int postId)
        {
            var userId = _userManager.GetUserId(User);
            var comments = await _db.Comments
                .Where(c => c.PostId == postId)
                .Include(c => c.User)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new
                {
                    id = c.Id,
                    content = c.Content,
                    authorName = $"{c.User.FirstName} {c.User.LastName}".Trim(),
                    authorAvatar = c.User.ProfilePictureUrl,
                    isOwnComment = c.UserId == userId
                })
                .ToListAsync();
            return Json(comments);
        }

        // POST /Home/VotePoll
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> VotePoll(int optionId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var option = await _db.PollOptions
                .Include(o => o.Votes)
                .Include(o => o.Post).ThenInclude(p => p.PollOptions).ThenInclude(o => o.Votes)
                .FirstOrDefaultAsync(o => o.Id == optionId);

            if (option == null) return NotFound();

            // Check if already voted on any option in this poll
            var postId       = option.PostId;
            var allOptions   = option.Post.PollOptions.Select(o => o.Id).ToList();
            var existingVote = await _db.PollVotes
                .FirstOrDefaultAsync(v => allOptions.Contains(v.PollOptionId) && v.UserId == userId);

            if (existingVote != null)
                return Json(new { success = false, message = "Already voted." });

            _db.PollVotes.Add(new PollVote { PollOptionId = optionId, UserId = userId });
            await _db.SaveChangesAsync();

            // Return updated results
            var updatedPost = await _db.Posts
                .Include(p => p.PollOptions).ThenInclude(o => o.Votes)
                .FirstOrDefaultAsync(p => p.Id == postId);

            var totalVotes = updatedPost!.PollOptions.Sum(o => o.Votes.Count);
            var results    = updatedPost.PollOptions.Select(o => new
            {
                id         = o.Id,
                voteCount  = o.Votes.Count,
                percentage = totalVotes == 0 ? 0 : Math.Round((double)o.Votes.Count / totalVotes * 100, 1),
            }).ToList();

            return Json(new { success = true, totalVotes, results });
        }

        // POST /Home/ToggleFollow
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFollow(string userId)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null || currentUserId == userId) return BadRequest();

            var existing = await _db.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == currentUserId && f.FollowedId == userId);

            if (existing != null)
                _db.Follows.Remove(existing);
            else
                _db.Follows.Add(new Follow { FollowerId = currentUserId, FollowedId = userId });

            await _db.SaveChangesAsync();

            // Get updated counts for the target user
            var followerCount = await _db.Follows.CountAsync(f => f.FollowedId == userId);
            var followingCount = await _db.Follows.CountAsync(f => f.FollowerId == userId);

            // ── SignalR: Notify user about the follow ──
            if (existing == null) // Only notify on follow, not unfollow
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var name = $"{currentUser?.FirstName} {currentUser?.LastName}".Trim();

                _db.Notifications.Add(new Notification { UserId = userId, Content = $"{name} started following you", Type = "follow", RelatedUrl = $"/Home/Profile/{currentUserId}" });
                await _db.SaveChangesAsync();

                await _hubContext.Clients.Group(userId).SendAsync("ReceiveNotification", new
                {
                    type = "follow",
                    message = $"{name} started following you",
                    icon = "bi-person-plus-fill",
                    fromUserId = currentUserId
                });
            }

            return Json(new { followed = existing == null, followerCount, followingCount });
        }

        // GET /Home/Profile/{id}
        public async Task<IActionResult> Profile(string id)
        {
            var user = await _userManager.Users
                .Include(u => u.Followers)
                .Include(u => u.Following)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);

            var isBlockedByTarget = await _db.BlockedUsers.AnyAsync(b => b.BlockerId == id && b.BlockedUserId == currentUserId);
            if (isBlockedByTarget && !User.IsInRole("Admin") && !User.IsInRole("Moderator"))
            {
                return NotFound();
            }
            
            var posts = await _db.Posts
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .Include(p => p.Media)
                .Where(u => u.UserId == id)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var friendship = await _db.Friendships
                .FirstOrDefaultAsync(f => (f.RequesterId == currentUserId && f.ReceiverId == id) || (f.RequesterId == id && f.ReceiverId == currentUserId));

            string friendshipStatus = "None";
            int friendshipId = 0;
            if (friendship != null)
            {
                friendshipId = friendship.Id;
                if (friendship.Status == "Accepted") 
                    friendshipStatus = "Friends";
                else if (friendship.RequesterId == currentUserId)
                    friendshipStatus = "PendingSent";
                else 
                    friendshipStatus = "PendingReceived";
            }

            var vm = new ProfileViewModel
            {
                UserId            = user.Id,
                FirstName         = user.FirstName,
                LastName          = user.LastName,
                Email             = user.Email!,
                Bio               = user.Bio,
                ProfilePictureUrl = user.ProfilePictureUrl,
                CreatedAt         = user.CreatedAt,
                PostCount         = posts.Count,
                FriendshipStatus  = friendshipStatus,
                FriendshipId      = friendshipId,
                Posts             = posts.Select(p => new PostFeedViewModel
                {
                    Id            = p.Id,
                    Content       = p.Content,
                    ImageUrl      = p.ImageUrl,
                    PostType      = p.PostType,
                    CreatedAt     = p.CreatedAt,
                    AuthorName    = $"{user.FirstName} {user.LastName}".Trim(),
                    AuthorAvatar  = user.ProfilePictureUrl,
                    ImageUrls     = p.Media.Select(m => m.ImageUrl).ToList(),
                    LikeCount     = p.Likes.Count,
                    CommentCount  = p.Comments.Count,
                    IsLikedByCurrentUser = p.Likes.Any(l => l.UserId == currentUserId),
                    IsOwnPost     = p.UserId == currentUserId
                }).ToList()
            };

            ViewBag.IsFollowing   = _db.Follows.Any(f => f.FollowerId == currentUserId && f.FollowedId == id);
            ViewBag.FollowerCount  = user.Followers.Count;
            ViewBag.FollowingCount = user.Following.Count;
            ViewBag.IsBlockedByMe  = _db.BlockedUsers.Any(b => b.BlockerId == currentUserId && b.BlockedUserId == id);

            return View(vm);
        }

        // ── GET /Home/GetFollowers ────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetFollowers(string userId)
        {
            var currentUserId = _userManager.GetUserId(User);

            var followers = await _db.Follows
                .Where(f => f.FollowedId == userId)
                .Include(f => f.Follower)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new
                {
                    id     = f.Follower.Id,
                    name   = (f.Follower.FirstName + " " + f.Follower.LastName).Trim(),
                    avatar = f.Follower.ProfilePictureUrl,
                    bio    = f.Follower.Bio ?? "Nexus user",
                    isFollowedByMe = _db.Follows.Any(x => x.FollowerId == currentUserId && x.FollowedId == f.Follower.Id),
                    isMe   = f.Follower.Id == currentUserId
                })
                .ToListAsync();

            return Json(followers);
        }

        // ── GET /Home/GetFollowing ────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetFollowing(string userId)
        {
            var currentUserId = _userManager.GetUserId(User);

            var following = await _db.Follows
                .Where(f => f.FollowerId == userId)
                .Include(f => f.Followed)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new
                {
                    id     = f.Followed.Id,
                    name   = (f.Followed.FirstName + " " + f.Followed.LastName).Trim(),
                    avatar = f.Followed.ProfilePictureUrl,
                    bio    = f.Followed.Bio ?? "Nexus user",
                    isFollowedByMe = _db.Follows.Any(x => x.FollowerId == currentUserId && x.FollowedId == f.Followed.Id),
                    isMe   = f.Followed.Id == currentUserId
                })
                .ToListAsync();

            return Json(following);
        }

        // GET /Home/GetMorePosts
        public async Task<IActionResult> GetMorePosts(int page = 1)
        {
            int pageSize = 10;
            var currentUserId = _userManager.GetUserId(User);

            var followingIds = await _db.Follows
                .Where(f => f.FollowerId == currentUserId)
                .Select(f => f.FollowedId)
                .ToListAsync();

            var postsQuery = _db.Posts
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Media)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .Include(p => p.PollOptions).ThenInclude(o => o.Votes)
                .Where(p => p.Privacy == "public" || 
                            p.UserId == currentUserId || 
                            (p.Privacy == "followers" && followingIds.Contains(p.UserId)))
                .OrderByDescending(p => p.CreatedAt)
                .Skip(page * pageSize)
                .Take(pageSize);

            var posts = await postsQuery.ToListAsync();

            if (!posts.Any()) return NoContent();

            var viewModels = posts.Select(p =>
            {
                var totalVotes = p.PollOptions.Sum(o => o.Votes.Count);
                var userLike = p.Likes.FirstOrDefault(l => l.UserId == currentUserId);
                return new PostFeedViewModel
                {
                    Id            = p.Id,
                    Content       = p.Content,
                    ImageUrl      = p.ImageUrl,
                    PostType      = p.PostType,
                    Feeling       = p.Feeling,
                    CreatedAt     = p.CreatedAt,
                    AuthorId      = p.UserId,
                    AuthorName    = $"{p.User.FirstName} {p.User.LastName}".Trim(),
                    AuthorUsername = p.User.UserName ?? p.User.Email ?? "user",
                    AuthorAvatar  = p.User.ProfilePictureUrl,
                    ImageUrls     = p.Media.Select(m => m.ImageUrl).ToList(),
                    LikeCount     = p.Likes.Count,
                    CommentCount  = p.Comments.Count,
                    IsLikedByCurrentUser = userLike != null,
                    CurrentUserReaction  = userLike?.ReactionType,
                    IsOwnPost     = p.UserId == currentUserId,
                    HasVoted      = p.PollOptions.Any(o => o.Votes.Any(v => v.UserId == currentUserId)),
                    PollOptions   = p.PollOptions.Select(o => new PollOptionViewModel
                    {
                        Id         = o.Id,
                        OptionText = o.OptionText,
                        VoteCount  = o.Votes.Count,
                        VotedByCurrentUser = o.Votes.Any(v => v.UserId == currentUserId),
                        Percentage = totalVotes == 0 ? 0 : Math.Round((double)o.Votes.Count / totalVotes * 100, 1)
                    }).ToList(),
                    RecentComments = p.Comments
                        .OrderByDescending(c => c.CreatedAt)
                        .Take(2)
                        .Select(c => new CommentViewModel
                        {
                            Id           = c.Id,
                            Content      = c.Content,
                            AuthorName   = $"{c.User.FirstName} {c.User.LastName}".Trim(),
                            AuthorAvatar = c.User.ProfilePictureUrl,
                            CreatedAt    = c.CreatedAt
                        }).ToList()
                };
            }).ToList();

            return PartialView("_PostList", viewModels);
        }

        // GET /Home/Search
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return RedirectToAction("Index");

            var queryLower = query.ToLower().Trim();
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var blockedUserIds = currentUserId != null
                ? await _db.BlockedUsers
                    .Where(b => b.BlockerId == currentUserId || b.BlockedUserId == currentUserId)
                    .Select(b => b.BlockerId == currentUserId ? b.BlockedUserId : b.BlockerId)
                    .ToListAsync()
                : new List<string>();

            var users = await _userManager.Users
                .Where(u => !blockedUserIds.Contains(u.Id) &&
                            ((u.FirstName + " " + u.LastName).ToLower().Contains(queryLower) 
                          || u.UserName.ToLower().Contains(queryLower) 
                          || u.Email.ToLower().Contains(queryLower)))
                .Take(20)
                .ToListAsync();

            var friendships = currentUserId != null 
                ? await _db.Friendships.Where(f => f.RequesterId == currentUserId || f.ReceiverId == currentUserId).ToListAsync()
                : new List<Friendship>();
            var follows = currentUserId != null 
                ? await _db.Follows.Where(f => f.FollowerId == currentUserId).Select(f => f.FollowedId).ToListAsync() 
                : new List<string>();

            var searchUsers = users.Select(u => 
            {
                var friendship = friendships.FirstOrDefault(f => f.RequesterId == u.Id || f.ReceiverId == u.Id);
                string status = "None";
                int friendshipId = 0;
                
                if (friendship != null)
                {
                    friendshipId = friendship.Id;
                    if (friendship.Status == "Accepted") 
                        status = "Friends";
                    else if (friendship.RequesterId == currentUserId)
                        status = "PendingSent";
                    else 
                        status = "PendingReceived";
                }

                return new SearchUserViewModel
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email!,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                    FriendshipStatus = status,
                    FriendshipId = friendshipId,
                    IsFollowing = follows.Contains(u.Id)
                };
            }).ToList();

            var posts = await _db.Posts
                .Include(p => p.User)
                .Where(p => p.Content.ToLower().Contains(queryLower))
                .OrderByDescending(p => p.CreatedAt)
                .Take(20)
                .ToListAsync();

            var vm = new SearchViewModel
            {
                Query = query,
                Users = searchUsers,
                Posts = posts
            };

            return View(vm);
        }

        // ── GET /Home/Settings ────────────────────────────────────
        public async Task<IActionResult> Settings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var vm = new SettingsViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Bio = user.Bio,
                ProfilePictureUrl = user.ProfilePictureUrl
            };
            return View(vm);
        }

        // ── POST /Home/Settings ───────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(SettingsViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (model.ProfilePictureFile != null && model.ProfilePictureFile.Length > 0)
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsDir);
                var fileName = $"avatar_{user.Id}_{Guid.NewGuid()}{Path.GetExtension(model.ProfilePictureFile.FileName)}";
                using var stream = new FileStream(Path.Combine(uploadsDir, fileName), FileMode.Create);
                await model.ProfilePictureFile.CopyToAsync(stream);
                
                if (!string.IsNullOrEmpty(user.ProfilePictureUrl) && user.ProfilePictureUrl.StartsWith("/uploads/"))
                {
                    var oldFilePath = Path.Combine(_env.WebRootPath, user.ProfilePictureUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);
                }

                user.ProfilePictureUrl = $"/uploads/{fileName}";
            }

            user.FirstName = model.FirstName.Trim();
            user.LastName = model.LastName.Trim();
            user.Bio = model.Bio?.Trim();

            await _userManager.UpdateAsync(user);
            TempData["SettingsSuccess"] = "Profile updated successfully!";
            return RedirectToAction("Settings");
        }

        // ── GET /Home/Notifications ───────────────────────────────
        public async Task<IActionResult> Notifications()
        {
            var currentUserId = _userManager.GetUserId(User);
            var notifs = await _db.Notifications
                .Where(n => n.UserId == currentUserId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(50)
                .ToListAsync();

            var vm = notifs.Select(n => new NotificationViewModel
            {
                Id = n.Id,
                Content = n.Content,
                Type = n.Type,
                RelatedUrl = n.RelatedUrl,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList();

            // Mark unread as read
            var unread = notifs.Where(n => !n.IsRead).ToList();
            if (unread.Any())
            {
                foreach (var u in unread) u.IsRead = true;
                await _db.SaveChangesAsync();
            }

            return View(vm);
        }

        // ── POST /Home/MarkNotificationsRead ─────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkNotificationsRead()
        {
            var currentUserId = _userManager.GetUserId(User);
            var unread = await _db.Notifications
                .Where(n => n.UserId == currentUserId && !n.IsRead)
                .ToListAsync();
            foreach (var n in unread) n.IsRead = true;
            if (unread.Any()) await _db.SaveChangesAsync();
            return Ok();
        }

        // ── POST /Home/CreateStory ────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStory(IFormFile mediaFile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (mediaFile != null && mediaFile.Length > 0)
            {
                // Validate size (max 100MB)
                if (mediaFile.Length > 100 * 1024 * 1024)
                {
                    TempData["FeedError"] = "File size cannot exceed 100MB.";
                    return RedirectToAction("Index");
                }

                var isVideo = mediaFile.ContentType.StartsWith("video/");
                var isImage = mediaFile.ContentType.StartsWith("image/");

                if (!isVideo && !isImage)
                {
                    TempData["FeedError"] = "Invalid file type. Only images and videos are allowed.";
                    return RedirectToAction("Index");
                }

                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "stories");
                Directory.CreateDirectory(uploadsDir);

                var fileName = $"story_{Guid.NewGuid()}{Path.GetExtension(mediaFile.FileName)}";
                using var stream = new FileStream(Path.Combine(uploadsDir, fileName), FileMode.Create);
                await mediaFile.CopyToAsync(stream);

                var story = new Story
                {
                    UserId = user.Id,
                    MediaUrl = $"/uploads/stories/{fileName}",
                    MediaType = isVideo ? "video" : "image",
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                };

                _db.Stories.Add(story);
                await _db.SaveChangesAsync();
                TempData["CreatePostSuccess"] = "Story posted successfully!";
            }
            else
            {
                TempData["FeedError"] = "Please select a file to upload.";
            }

            return RedirectToAction("Index");
        }

        // ── POST /Home/DeleteStory ────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStory(int storyId)
        {
            var userId = _userManager.GetUserId(User);
            var story = await _db.Stories.FirstOrDefaultAsync(s => s.Id == storyId && s.UserId == userId);
            if (story == null)
                return Json(new { success = false, message = "Story not found or not yours." });

            var filePath = Path.Combine(_env.WebRootPath, story.MediaUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                try { System.IO.File.Delete(filePath); } catch { /* ignore */ }
            }

            _db.Stories.Remove(story);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        // ── POST /Home/DeleteAllStories ────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAllStories()
        {
            var userId = _userManager.GetUserId(User);
            var stories = await _db.Stories.Where(s => s.UserId == userId).ToListAsync();
            
            if (!stories.Any()) return Json(new { success = false, message = "No stories found." });

            foreach (var story in stories)
            {
                var filePath = Path.Combine(_env.WebRootPath, story.MediaUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    try { System.IO.File.Delete(filePath); } catch { /* ignore */ }
                }
            }

            _db.Stories.RemoveRange(stories);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        // ── POST /Home/AddFriend ──────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFriend(string targetUserId)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null || currentUserId == targetUserId) return BadRequest();

            var existing = await _db.Friendships.FirstOrDefaultAsync(f =>
                (f.RequesterId == currentUserId && f.ReceiverId == targetUserId) ||
                (f.RequesterId == targetUserId && f.ReceiverId == currentUserId));

            if (existing != null) 
                return Json(new { success = false, message = "Friendship already exists or pending." });

            var friendship = new Friendship
            {
                RequesterId = currentUserId,
                ReceiverId = targetUserId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _db.Friendships.Add(friendship);

            // Auto follow when adding friend
            var existingFollow = await _db.Follows.FirstOrDefaultAsync(f => f.FollowerId == currentUserId && f.FollowedId == targetUserId);
            if (existingFollow == null)
            {
                _db.Follows.Add(new Follow { FollowerId = currentUserId, FollowedId = targetUserId });
            }

            await _db.SaveChangesAsync();

            // Notify Target User
            var currentUser = await _userManager.GetUserAsync(User);
            var name = $"{currentUser?.FirstName} {currentUser?.LastName}".Trim();
            
            _db.Notifications.Add(new Notification { UserId = targetUserId, Content = $"{name} sent you a friend request", Type = "friend", RelatedUrl = $"/Home/Profile/{currentUserId}" });
            await _db.SaveChangesAsync();

            // SignalR: Update UI for target user
            await _hubContext.Clients.Group(targetUserId).SendAsync("FriendshipUpdated", new
            {
                status = "PendingReceived",
                fromUserId = currentUserId,
                friendshipId = friendship.Id
            });

            // SignalR: Notify target user (Toast)
            await _hubContext.Clients.Group(targetUserId).SendAsync("ReceiveNotification", new
            {
                type = "friend",
                message = $"{name} sent you a friend request",
                icon = "bi-person-add",
                fromUserId = currentUserId
            });

            return Json(new { success = true, status = "PendingSent", friendshipId = friendship.Id });
        }

        // ── POST /Home/AcceptFriend ───────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptFriend(int friendshipId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var friendship = await _db.Friendships.FindAsync(friendshipId);

            if (friendship == null)
                return Json(new { success = false, message = "Friend request no longer exists." });

            if (friendship.ReceiverId != currentUserId || friendship.Status != "Pending")
                return BadRequest();

            friendship.Status = "Accepted";
            friendship.AcceptedAt = DateTime.UtcNow;

            // Auto-follow two-way
            var f1 = await _db.Follows.FirstOrDefaultAsync(f => f.FollowerId == friendship.RequesterId && f.FollowedId == friendship.ReceiverId);
            if (f1 == null) _db.Follows.Add(new Follow { FollowerId = friendship.RequesterId, FollowedId = friendship.ReceiverId });
            
            var f2 = await _db.Follows.FirstOrDefaultAsync(f => f.FollowerId == friendship.ReceiverId && f.FollowedId == friendship.RequesterId);
            if (f2 == null) _db.Follows.Add(new Follow { FollowerId = friendship.ReceiverId, FollowedId = friendship.RequesterId });

            await _db.SaveChangesAsync();

            // Notify Requester
            var currentUser = await _userManager.GetUserAsync(User);
            var name = $"{currentUser?.FirstName} {currentUser?.LastName}".Trim();
            
            _db.Notifications.Add(new Notification { UserId = friendship.RequesterId, Content = $"{name} accepted your friend request", Type = "friend", RelatedUrl = $"/Home/Profile/{currentUserId}" });
            await _db.SaveChangesAsync();

            // SignalR: Update UI for requester
            await _hubContext.Clients.Group(friendship.RequesterId).SendAsync("FriendshipUpdated", new
            {
                status = "Friends",
                fromUserId = currentUserId,
                friendshipId = friendship.Id
            });

            // SignalR: Notify requester (Toast)
            await _hubContext.Clients.Group(friendship.RequesterId).SendAsync("ReceiveNotification", new
            {
                type = "friend",
                message = $"{name} accepted your friend request",
                icon = "bi-person-check-fill",
                fromUserId = currentUserId
            });

            return Json(new { success = true, status = "Friends", friendshipId = friendship.Id });
        }

        // ── POST /Home/DeclineFriend ──────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeclineFriend(int friendshipId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var friendship = await _db.Friendships.FindAsync(friendshipId);

            if (friendship == null)
                return Json(new { success = true, status = "None" });

            if (friendship.ReceiverId != currentUserId && friendship.RequesterId != currentUserId)
                return BadRequest();

            var otherUserId = friendship.RequesterId == currentUserId ? friendship.ReceiverId : friendship.RequesterId;

            _db.Friendships.Remove(friendship);
            await _db.SaveChangesAsync();

            // SignalR: Update UI for other user
            await _hubContext.Clients.Group(otherUserId).SendAsync("FriendshipUpdated", new
            {
                status = "None",
                fromUserId = currentUserId
            });

            return Json(new { success = true, status = "None" });
        }

        // ── POST /Home/RemoveFriend ───────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFriend(int friendshipId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var friendship = await _db.Friendships.FindAsync(friendshipId);

            if (friendship == null)
                return Json(new { success = true, status = "None" });

            if (friendship.ReceiverId != currentUserId && friendship.RequesterId != currentUserId)
                return BadRequest();

            var otherUserId = friendship.RequesterId == currentUserId ? friendship.ReceiverId : friendship.RequesterId;

            _db.Friendships.Remove(friendship);
            
            // Remove 2-way follow explicitly
            var f1 = await _db.Follows.FirstOrDefaultAsync(f => f.FollowerId == friendship.RequesterId && f.FollowedId == friendship.ReceiverId);
            if (f1 != null) _db.Follows.Remove(f1);

            var f2 = await _db.Follows.FirstOrDefaultAsync(f => f.FollowerId == friendship.ReceiverId && f.FollowedId == friendship.RequesterId);
            if (f2 != null) _db.Follows.Remove(f2);

            await _db.SaveChangesAsync();

            // SignalR: Update UI for other user
            await _hubContext.Clients.Group(otherUserId).SendAsync("FriendshipUpdated", new
            {
                status = "None",
                fromUserId = currentUserId
            });

            return Json(new { success = true, status = "None" });
        }

        // ── POST /Home/ToggleStoryLike ────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStoryLike(int storyId, string type = "like")
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var story = await _db.Stories.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == storyId);
            if (story == null) return NotFound();

            var existingLike = await _db.StoryLikes.FirstOrDefaultAsync(l => l.StoryId == storyId && l.UserId == userId);
            
            bool liked = false;
            string finalType = type;

            if (existingLike != null)
            {
                if (existingLike.ReactionType == type)
                {
                    _db.StoryLikes.Remove(existingLike);
                    finalType = "";
                }
                else
                {
                    existingLike.ReactionType = type;
                    existingLike.LikedAt = DateTime.UtcNow;
                    liked = true;
                }
            }
            else
            {
                _db.StoryLikes.Add(new StoryLike { StoryId = storyId, UserId = userId, ReactionType = type });
                liked = true;

                if (story.UserId != userId)
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    var name = $"{currentUser?.FirstName} {currentUser?.LastName}".Trim();
                    
                    _db.Notifications.Add(new Notification
                    {
                        UserId = story.UserId,
                        Content = $"{name} reacted to your story",
                        Type = "story_like",
                        RelatedUrl = "/"
                    });

                    await _hubContext.Clients.Group(story.UserId).SendAsync("ReceiveNotification", new
                    {
                        type = "story_like",
                        message = $"{name} reacted to your story",
                        icon = "bi-heart-fill"
                    });
                }
            }

            await _db.SaveChangesAsync();
            return Json(new { success = true, liked = liked, reactionType = finalType });
        }

        // ── POST /Home/RecordStoryView ────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordStoryView(int storyId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var story = await _db.Stories.FindAsync(storyId);
            if (story == null) return NotFound();

            // Don't record own view
            if (story.UserId == userId) return Ok();

            var existingView = await _db.StoryViews.FirstOrDefaultAsync(v => v.StoryId == storyId && v.UserId == userId);
            if (existingView == null)
            {
                _db.StoryViews.Add(new StoryView { StoryId = storyId, UserId = userId });
                await _db.SaveChangesAsync();
            }

            return Ok();
        }

        // ── GET /Home/Friends ─────────────────────────────────────────
        public async Task<IActionResult> Friends()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var friendships = await _db.Friendships
                .Include(f => f.Requester)
                .Include(f => f.Receiver)
                .Where(f => (f.RequesterId == userId || f.ReceiverId == userId))
                .ToListAsync();

            var model = new FriendsPageViewModel();

            foreach (var f in friendships)
            {
                var isRequester = f.RequesterId == userId;
                var otherUser = isRequester ? f.Receiver : f.Requester;

                var vm = new FriendUserViewModel
                {
                    Id = otherUser.Id,
                    Name = $"{otherUser.FirstName} {otherUser.LastName}".Trim(),
                    Avatar = otherUser.ProfilePictureUrl,
                    Bio = otherUser.Bio,
                    FriendshipId = f.Id,
                    FriendsSince = f.AcceptedAt
                };

                if (f.Status == "Accepted")
                {
                    model.Friends.Add(vm);
                }
                else if (f.Status == "Pending" && !isRequester)
                {
                    model.PendingRequests.Add(vm);
                }
            }

            return View(model);
        }

        // ── Report a Post (FR12) ──────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportPost(int postId, string reason)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var post = await _db.Posts.FindAsync(postId);
            if (post == null) return Json(new { success = false, message = "Bài viết không tồn tại." });

            // Prevent duplicate reports from same user
            var existing = await _db.Reports
                .AnyAsync(r => r.ReporterId == currentUserId && r.TargetPostId == postId && r.Status == "Pending");
            if (existing)
                return Json(new { success = false, message = "Bạn đã báo cáo bài viết này rồi." });

            _db.Reports.Add(new Report
            {
                ReporterId  = currentUserId,
                TargetPostId = postId,
                TargetUserId = post.UserId,
                Reason      = reason,
                Status      = "Pending",
                CreatedAt   = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Báo cáo đã được gửi. Cảm ơn bạn!" });
        }

        // ── Block User (BR04) ─────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> BlockUser(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (userId == currentUserId)
                return Json(new { success = false, message = "Không thể tự chặn bản thân." });

            var already = await _db.BlockedUsers
                .AnyAsync(b => b.BlockerId == currentUserId && b.BlockedUserId == userId);
            if (!already)
            {
                _db.BlockedUsers.Add(new BlockedUser
                {
                    BlockerId     = currentUserId,
                    BlockedUserId = userId,
                    CreatedAt     = DateTime.UtcNow
                });
                await _db.SaveChangesAsync();
            }
            return Json(new { success = true, message = "Đã chặn người dùng này." });
        }

        // ── Unblock User ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UnblockUser(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var block = await _db.BlockedUsers
                .FirstOrDefaultAsync(b => b.BlockerId == currentUserId && b.BlockedUserId == userId);
            if (block != null)
            {
                _db.BlockedUsers.Remove(block);
                await _db.SaveChangesAsync();
            }
            return Json(new { success = true, message = "Đã bỏ chặn người dùng này." });
        }
    }
}
