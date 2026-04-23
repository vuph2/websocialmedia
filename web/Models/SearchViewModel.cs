using System.Collections.Generic;
using web.Models;

namespace web.Models
{
    public class SearchUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public string FriendshipStatus { get; set; } = "None"; // None, Friends, PendingSent, PendingReceived
        public int FriendshipId { get; set; }
        public bool IsFollowing { get; set; }
    }

    public class SearchViewModel
    {
        public string Query { get; set; } = string.Empty;
        public List<SearchUserViewModel> Users { get; set; } = new();
        public List<Post> Posts { get; set; } = new();
    }
}
