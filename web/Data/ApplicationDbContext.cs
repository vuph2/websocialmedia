using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using web.Models;

namespace web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            var currentDir = System.IO.Directory.GetCurrentDirectory();
            var dir = new System.IO.DirectoryInfo(currentDir);
            string? dataDir = null;
            while (dir != null)
            {
                var candidate = System.IO.Path.Combine(dir.FullName, "Databases");
                if (System.IO.Directory.Exists(candidate))
                {
                    dataDir = candidate;
                    break;
                }
                var candidateWeb = System.IO.Path.Combine(dir.FullName, "web", "Databases");
                if (System.IO.Directory.Exists(candidateWeb))
                {
                    dataDir = candidateWeb;
                    break;
                }
                dir = dir.Parent;
            }

            if (dataDir != null)
            {
                AppDomain.CurrentDomain.SetData("DataDirectory", dataDir);
            }
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PollOption> PollOptions { get; set; }
        public DbSet<PollVote> PollVotes { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<PostMedia> PostMedia { get; set; }

        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Story> Stories { get; set; }
        public DbSet<StoryLike> StoryLikes { get; set; }
        public DbSet<StoryView> StoryViews { get; set; }

        // New tables for moderation & blocking
        public DbSet<Report> Reports { get; set; }
        public DbSet<BlockedUser> BlockedUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Singular Table Mappings
            builder.Entity<Post>().ToTable("Post");
            builder.Entity<PostLike>().ToTable("PostLike");
            builder.Entity<Comment>().ToTable("Comment");
            builder.Entity<PollOption>().ToTable("PollOption");
            builder.Entity<PollVote>().ToTable("PollVote");
            builder.Entity<Follow>().ToTable("Follow");
            builder.Entity<Conversation>().ToTable("Conversation");
            builder.Entity<ChatMessage>().ToTable("ChatMessage");
            builder.Entity<Notification>().ToTable("Notification");
            builder.Entity<PostMedia>().ToTable("PostMedia");
            builder.Entity<Friendship>().ToTable("Friendship");
            builder.Entity<Story>().ToTable("Story");
            builder.Entity<StoryLike>().ToTable("StoryLike");
            builder.Entity<StoryView>().ToTable("StoryView");
            builder.Entity<Report>().ToTable("Report");
            builder.Entity<BlockedUser>().ToTable("BlockedUser");

            // Identity Tables Singular Mappings
            builder.Entity<ApplicationUser>().ToTable("AspNetUser");
            builder.Entity<IdentityRole>().ToTable("AspNetRole");
            builder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRole");
            builder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaim");
            builder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogin");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaim");
            builder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserToken");

            // Unique like per user per post
            builder.Entity<PostLike>()
                .HasIndex(l => new { l.PostId, l.UserId })
                .IsUnique();

            // Unique vote per user per option
            builder.Entity<PollVote>()
                .HasIndex(v => new { v.PollOptionId, v.UserId })
                .IsUnique();

            // Prevent cascade delete conflicts
            builder.Entity<PostLike>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PollVote>()
                .HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Follow Relationship
            builder.Entity<Follow>()
                .HasIndex(f => new { f.FollowerId, f.FollowedId })
                .IsUnique();

            builder.Entity<Follow>()
                .HasOne(f => f.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Follow>()
                .HasOne(f => f.Followed)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FollowedId)
                .OnDelete(DeleteBehavior.NoAction);

            // ── Chat / Direct Messaging ───────────────────────────────
            builder.Entity<Conversation>()
                .HasIndex(c => new { c.User1Id, c.User2Id })
                .IsUnique();

            builder.Entity<Conversation>()
                .HasOne(c => c.User1)
                .WithMany()
                .HasForeignKey(c => c.User1Id)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Conversation>()
                .HasOne(c => c.User2)
                .WithMany()
                .HasForeignKey(c => c.User2Id)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ChatMessage>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ChatMessage>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            // Notifications Relationship
            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // PostMedia Relationship
            builder.Entity<PostMedia>()
                .HasOne(m => m.Post)
                .WithMany(p => p.Media)
                .HasForeignKey(m => m.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Friendship Relationship
            builder.Entity<Friendship>()
                .HasIndex(f => new { f.RequesterId, f.ReceiverId })
                .IsUnique();

            builder.Entity<Friendship>()
                .HasOne(f => f.Requester)
                .WithMany(u => u.FriendshipsRequested)
                .HasForeignKey(f => f.RequesterId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Friendship>()
                .HasOne(f => f.Receiver)
                .WithMany(u => u.FriendshipsReceived)
                .HasForeignKey(f => f.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);

            // Story Relationship
            builder.Entity<Story>()
                .HasOne(s => s.User)
                .WithMany(u => u.Stories)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // StoryLike Relationship
            builder.Entity<StoryLike>()
                .HasIndex(s => new { s.StoryId, s.UserId })
                .IsUnique();

            builder.Entity<StoryLike>()
                .HasOne(s => s.Story)
                .WithMany(st => st.StoryLikes)
                .HasForeignKey(s => s.StoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<StoryLike>()
                .HasOne(s => s.User)
                .WithMany(u => u.StoryLikes)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // StoryView Relationship
            builder.Entity<StoryView>()
                .HasIndex(s => new { s.StoryId, s.UserId })
                .IsUnique();

            builder.Entity<StoryView>()
                .HasOne(s => s.Story)
                .WithMany(st => st.StoryViews)
                .HasForeignKey(s => s.StoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<StoryView>()
                .HasOne(s => s.User)
                .WithMany(u => u.StoryViews)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // ── Reports ─────────────────────────────────────────────────
            builder.Entity<Report>()
                .HasOne(r => r.Reporter)
                .WithMany()
                .HasForeignKey(r => r.ReporterId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Report>()
                .HasOne(r => r.TargetPost)
                .WithMany()
                .HasForeignKey(r => r.TargetPostId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Report>()
                .HasOne(r => r.TargetUser)
                .WithMany()
                .HasForeignKey(r => r.TargetUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Report>()
                .HasOne(r => r.ResolvedBy)
                .WithMany()
                .HasForeignKey(r => r.ResolvedById)
                .OnDelete(DeleteBehavior.NoAction);

            // ── BlockedUsers ─────────────────────────────────────────────
            builder.Entity<BlockedUser>()
                .HasIndex(b => new { b.BlockerId, b.BlockedUserId })
                .IsUnique();

            builder.Entity<BlockedUser>()
                .HasOne(b => b.Blocker)
                .WithMany(u => u.BlockedByMe)
                .HasForeignKey(b => b.BlockerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<BlockedUser>()
                .HasOne(b => b.Blocked)
                .WithMany(u => u.BlockedMe)
                .HasForeignKey(b => b.BlockedUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
