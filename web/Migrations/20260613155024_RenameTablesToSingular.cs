using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations
{
    /// <inheritdoc />
    public partial class RenameTablesToSingular : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_BlockedUsers_AspNetUsers_BlockedUserId",
                table: "BlockedUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_BlockedUsers_AspNetUsers_BlockerId",
                table: "BlockedUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_AspNetUsers_SenderId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Conversations_ConversationId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Posts_PostId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_AspNetUsers_User1Id",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_AspNetUsers_User2Id",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Follows_AspNetUsers_FollowedId",
                table: "Follows");

            migrationBuilder.DropForeignKey(
                name: "FK_Follows_AspNetUsers_FollowerId",
                table: "Follows");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_ReceiverId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_RequesterId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_UserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_PollOptions_Posts_PostId",
                table: "PollOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_PollVotes_AspNetUsers_UserId",
                table: "PollVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_PollVotes_PollOptions_PollOptionId",
                table: "PollVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_PostLikes_AspNetUsers_UserId",
                table: "PostLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_PostLikes_Posts_PostId",
                table: "PostLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_PostMedia_Posts_PostId",
                table: "PostMedia");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_UserId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_ReporterId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_ResolvedById",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_TargetUserId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Posts_TargetPostId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Stories_AspNetUsers_UserId",
                table: "Stories");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryLikes_AspNetUsers_UserId",
                table: "StoryLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryLikes_Stories_StoryId",
                table: "StoryLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryViews_AspNetUsers_UserId",
                table: "StoryViews");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryViews_Stories_StoryId",
                table: "StoryViews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoryViews",
                table: "StoryViews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoryLikes",
                table: "StoryLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stories",
                table: "Stories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reports",
                table: "Reports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Posts",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostLikes",
                table: "PostLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PollVotes",
                table: "PollVotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PollOptions",
                table: "PollOptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Friendships",
                table: "Friendships");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Follows",
                table: "Follows");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Conversations",
                table: "Conversations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatMessages",
                table: "ChatMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlockedUsers",
                table: "BlockedUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "StoryViews",
                newName: "StoryView");

            migrationBuilder.RenameTable(
                name: "StoryLikes",
                newName: "StoryLike");

            migrationBuilder.RenameTable(
                name: "Stories",
                newName: "Story");

            migrationBuilder.RenameTable(
                name: "Reports",
                newName: "Report");

            migrationBuilder.RenameTable(
                name: "Posts",
                newName: "Post");

            migrationBuilder.RenameTable(
                name: "PostLikes",
                newName: "PostLike");

            migrationBuilder.RenameTable(
                name: "PollVotes",
                newName: "PollVote");

            migrationBuilder.RenameTable(
                name: "PollOptions",
                newName: "PollOption");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "Notification");

            migrationBuilder.RenameTable(
                name: "Friendships",
                newName: "Friendship");

            migrationBuilder.RenameTable(
                name: "Follows",
                newName: "Follow");

            migrationBuilder.RenameTable(
                name: "Conversations",
                newName: "Conversation");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "Comment");

            migrationBuilder.RenameTable(
                name: "ChatMessages",
                newName: "ChatMessage");

            migrationBuilder.RenameTable(
                name: "BlockedUsers",
                newName: "BlockedUser");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "AspNetUserToken");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "AspNetUser");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "AspNetUserRole");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "AspNetUserLogin");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "AspNetUserClaim");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "AspNetRole");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "AspNetRoleClaim");

            migrationBuilder.RenameIndex(
                name: "IX_StoryViews_UserId",
                table: "StoryView",
                newName: "IX_StoryView_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_StoryViews_StoryId_UserId",
                table: "StoryView",
                newName: "IX_StoryView_StoryId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_StoryLikes_UserId",
                table: "StoryLike",
                newName: "IX_StoryLike_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_StoryLikes_StoryId_UserId",
                table: "StoryLike",
                newName: "IX_StoryLike_StoryId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Stories_UserId",
                table: "Story",
                newName: "IX_Story_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_TargetUserId",
                table: "Report",
                newName: "IX_Report_TargetUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_TargetPostId",
                table: "Report",
                newName: "IX_Report_TargetPostId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_ResolvedById",
                table: "Report",
                newName: "IX_Report_ResolvedById");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_ReporterId",
                table: "Report",
                newName: "IX_Report_ReporterId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_UserId",
                table: "Post",
                newName: "IX_Post_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PostLikes_UserId",
                table: "PostLike",
                newName: "IX_PostLike_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PostLikes_PostId_UserId",
                table: "PostLike",
                newName: "IX_PostLike_PostId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PollVotes_UserId",
                table: "PollVote",
                newName: "IX_PollVote_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PollVotes_PollOptionId_UserId",
                table: "PollVote",
                newName: "IX_PollVote_PollOptionId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PollOptions_PostId",
                table: "PollOption",
                newName: "IX_PollOption_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_UserId",
                table: "Notification",
                newName: "IX_Notification_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Friendships_RequesterId_ReceiverId",
                table: "Friendship",
                newName: "IX_Friendship_RequesterId_ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_Friendships_ReceiverId",
                table: "Friendship",
                newName: "IX_Friendship_ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_Follows_FollowerId_FollowedId",
                table: "Follow",
                newName: "IX_Follow_FollowerId_FollowedId");

            migrationBuilder.RenameIndex(
                name: "IX_Follows_FollowedId",
                table: "Follow",
                newName: "IX_Follow_FollowedId");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_User2Id",
                table: "Conversation",
                newName: "IX_Conversation_User2Id");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_User1Id_User2Id",
                table: "Conversation",
                newName: "IX_Conversation_User1Id_User2Id");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_UserId",
                table: "Comment",
                newName: "IX_Comment_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_PostId",
                table: "Comment",
                newName: "IX_Comment_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMessages_SenderId",
                table: "ChatMessage",
                newName: "IX_ChatMessage_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMessages_ConversationId",
                table: "ChatMessage",
                newName: "IX_ChatMessage_ConversationId");

            migrationBuilder.RenameIndex(
                name: "IX_BlockedUsers_BlockerId_BlockedUserId",
                table: "BlockedUser",
                newName: "IX_BlockedUser_BlockerId_BlockedUserId");

            migrationBuilder.RenameIndex(
                name: "IX_BlockedUsers_BlockedUserId",
                table: "BlockedUser",
                newName: "IX_BlockedUser_BlockedUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRole",
                newName: "IX_AspNetUserRole_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogin",
                newName: "IX_AspNetUserLogin_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaim",
                newName: "IX_AspNetUserClaim_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaim",
                newName: "IX_AspNetRoleClaim_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoryView",
                table: "StoryView",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoryLike",
                table: "StoryLike",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Story",
                table: "Story",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Report",
                table: "Report",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Post",
                table: "Post",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostLike",
                table: "PostLike",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PollVote",
                table: "PollVote",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PollOption",
                table: "PollOption",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notification",
                table: "Notification",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Friendship",
                table: "Friendship",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Follow",
                table: "Follow",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Conversation",
                table: "Conversation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatMessage",
                table: "ChatMessage",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlockedUser",
                table: "BlockedUser",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserToken",
                table: "AspNetUserToken",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUser",
                table: "AspNetUser",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRole",
                table: "AspNetUserRole",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserLogin",
                table: "AspNetUserLogin",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserClaim",
                table: "AspNetUserClaim",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRole",
                table: "AspNetRole",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoleClaim",
                table: "AspNetRoleClaim",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaim_AspNetRole_RoleId",
                table: "AspNetRoleClaim",
                column: "RoleId",
                principalTable: "AspNetRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaim_AspNetUser_UserId",
                table: "AspNetUserClaim",
                column: "UserId",
                principalTable: "AspNetUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogin_AspNetUser_UserId",
                table: "AspNetUserLogin",
                column: "UserId",
                principalTable: "AspNetUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRole_AspNetRole_RoleId",
                table: "AspNetUserRole",
                column: "RoleId",
                principalTable: "AspNetRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRole_AspNetUser_UserId",
                table: "AspNetUserRole",
                column: "UserId",
                principalTable: "AspNetUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserToken_AspNetUser_UserId",
                table: "AspNetUserToken",
                column: "UserId",
                principalTable: "AspNetUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlockedUser_AspNetUser_BlockedUserId",
                table: "BlockedUser",
                column: "BlockedUserId",
                principalTable: "AspNetUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlockedUser_AspNetUser_BlockerId",
                table: "BlockedUser",
                column: "BlockerId",
                principalTable: "AspNetUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessage_AspNetUser_SenderId",
                table: "ChatMessage",
                column: "SenderId",
                principalTable: "AspNetUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessage_Conversation_ConversationId",
                table: "ChatMessage",
                column: "ConversationId",
                principalTable: "Conversation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUser_UserId",
                table: "Comment",
                column: "UserId",
                principalTable: "AspNetUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Post_PostId",
                table: "Comment",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversation_AspNetUser_User1Id",
                table: "Conversation",
                column: "User1Id",
                principalTable: "AspNetUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversation_AspNetUser_User2Id",
                table: "Conversation",
                column: "User2Id",
                principalTable: "AspNetUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Follow_AspNetUser_FollowedId",
                table: "Follow",
                column: "FollowedId",
                principalTable: "AspNetUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Follow_AspNetUser_FollowerId",
                table: "Follow",
                column: "FollowerId",
                principalTable: "AspNetUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_AspNetUser_ReceiverId",
                table: "Friendship",
                column: "ReceiverId",
                principalTable: "AspNetUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_AspNetUser_RequesterId",
                table: "Friendship",
                column: "RequesterId",
                principalTable: "AspNetUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_AspNetUser_UserId",
                table: "Notification",
                column: "UserId",
                principalTable: "AspNetUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PollOption_Post_PostId",
                table: "PollOption",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PollVote_AspNetUser_UserId",
                table: "PollVote",
                column: "UserId",
                principalTable: "AspNetUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PollVote_PollOption_PollOptionId",
                table: "PollVote",
                column: "PollOptionId",
                principalTable: "PollOption",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_AspNetUser_UserId",
                table: "Post",
                column: "UserId",
                principalTable: "AspNetUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostLike_AspNetUser_UserId",
                table: "PostLike",
                column: "UserId",
                principalTable: "AspNetUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostLike_Post_PostId",
                table: "PostLike",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostMedia_Post_PostId",
                table: "PostMedia",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_AspNetUser_ReporterId",
                table: "Report",
                column: "ReporterId",
                principalTable: "AspNetUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_AspNetUser_ResolvedById",
                table: "Report",
                column: "ResolvedById",
                principalTable: "AspNetUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_AspNetUser_TargetUserId",
                table: "Report",
                column: "TargetUserId",
                principalTable: "AspNetUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Post_TargetPostId",
                table: "Report",
                column: "TargetPostId",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Story_AspNetUser_UserId",
                table: "Story",
                column: "UserId",
                principalTable: "AspNetUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryLike_AspNetUser_UserId",
                table: "StoryLike",
                column: "UserId",
                principalTable: "AspNetUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StoryLike_Story_StoryId",
                table: "StoryLike",
                column: "StoryId",
                principalTable: "Story",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryView_AspNetUser_UserId",
                table: "StoryView",
                column: "UserId",
                principalTable: "AspNetUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StoryView_Story_StoryId",
                table: "StoryView",
                column: "StoryId",
                principalTable: "Story",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaim_AspNetRole_RoleId",
                table: "AspNetRoleClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaim_AspNetUser_UserId",
                table: "AspNetUserClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogin_AspNetUser_UserId",
                table: "AspNetUserLogin");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRole_AspNetRole_RoleId",
                table: "AspNetUserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRole_AspNetUser_UserId",
                table: "AspNetUserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserToken_AspNetUser_UserId",
                table: "AspNetUserToken");

            migrationBuilder.DropForeignKey(
                name: "FK_BlockedUser_AspNetUser_BlockedUserId",
                table: "BlockedUser");

            migrationBuilder.DropForeignKey(
                name: "FK_BlockedUser_AspNetUser_BlockerId",
                table: "BlockedUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessage_AspNetUser_SenderId",
                table: "ChatMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessage_Conversation_ConversationId",
                table: "ChatMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUser_UserId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Post_PostId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversation_AspNetUser_User1Id",
                table: "Conversation");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversation_AspNetUser_User2Id",
                table: "Conversation");

            migrationBuilder.DropForeignKey(
                name: "FK_Follow_AspNetUser_FollowedId",
                table: "Follow");

            migrationBuilder.DropForeignKey(
                name: "FK_Follow_AspNetUser_FollowerId",
                table: "Follow");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_AspNetUser_ReceiverId",
                table: "Friendship");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_AspNetUser_RequesterId",
                table: "Friendship");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_AspNetUser_UserId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_PollOption_Post_PostId",
                table: "PollOption");

            migrationBuilder.DropForeignKey(
                name: "FK_PollVote_AspNetUser_UserId",
                table: "PollVote");

            migrationBuilder.DropForeignKey(
                name: "FK_PollVote_PollOption_PollOptionId",
                table: "PollVote");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_AspNetUser_UserId",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_PostLike_AspNetUser_UserId",
                table: "PostLike");

            migrationBuilder.DropForeignKey(
                name: "FK_PostLike_Post_PostId",
                table: "PostLike");

            migrationBuilder.DropForeignKey(
                name: "FK_PostMedia_Post_PostId",
                table: "PostMedia");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_AspNetUser_ReporterId",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_AspNetUser_ResolvedById",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_AspNetUser_TargetUserId",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Post_TargetPostId",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Story_AspNetUser_UserId",
                table: "Story");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryLike_AspNetUser_UserId",
                table: "StoryLike");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryLike_Story_StoryId",
                table: "StoryLike");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryView_AspNetUser_UserId",
                table: "StoryView");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryView_Story_StoryId",
                table: "StoryView");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoryView",
                table: "StoryView");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoryLike",
                table: "StoryLike");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Story",
                table: "Story");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Report",
                table: "Report");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostLike",
                table: "PostLike");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Post",
                table: "Post");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PollVote",
                table: "PollVote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PollOption",
                table: "PollOption");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notification",
                table: "Notification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Friendship",
                table: "Friendship");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Follow",
                table: "Follow");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Conversation",
                table: "Conversation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatMessage",
                table: "ChatMessage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlockedUser",
                table: "BlockedUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserToken",
                table: "AspNetUserToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRole",
                table: "AspNetUserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserLogin",
                table: "AspNetUserLogin");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserClaim",
                table: "AspNetUserClaim");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUser",
                table: "AspNetUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoleClaim",
                table: "AspNetRoleClaim");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRole",
                table: "AspNetRole");

            migrationBuilder.RenameTable(
                name: "StoryView",
                newName: "StoryViews");

            migrationBuilder.RenameTable(
                name: "StoryLike",
                newName: "StoryLikes");

            migrationBuilder.RenameTable(
                name: "Story",
                newName: "Stories");

            migrationBuilder.RenameTable(
                name: "Report",
                newName: "Reports");

            migrationBuilder.RenameTable(
                name: "PostLike",
                newName: "PostLikes");

            migrationBuilder.RenameTable(
                name: "Post",
                newName: "Posts");

            migrationBuilder.RenameTable(
                name: "PollVote",
                newName: "PollVotes");

            migrationBuilder.RenameTable(
                name: "PollOption",
                newName: "PollOptions");

            migrationBuilder.RenameTable(
                name: "Notification",
                newName: "Notifications");

            migrationBuilder.RenameTable(
                name: "Friendship",
                newName: "Friendships");

            migrationBuilder.RenameTable(
                name: "Follow",
                newName: "Follows");

            migrationBuilder.RenameTable(
                name: "Conversation",
                newName: "Conversations");

            migrationBuilder.RenameTable(
                name: "Comment",
                newName: "Comments");

            migrationBuilder.RenameTable(
                name: "ChatMessage",
                newName: "ChatMessages");

            migrationBuilder.RenameTable(
                name: "BlockedUser",
                newName: "BlockedUsers");

            migrationBuilder.RenameTable(
                name: "AspNetUserToken",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "AspNetUserRole",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogin",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaim",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "AspNetUser",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaim",
                newName: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "AspNetRole",
                newName: "AspNetRoles");

            migrationBuilder.RenameIndex(
                name: "IX_StoryView_UserId",
                table: "StoryViews",
                newName: "IX_StoryViews_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_StoryView_StoryId_UserId",
                table: "StoryViews",
                newName: "IX_StoryViews_StoryId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_StoryLike_UserId",
                table: "StoryLikes",
                newName: "IX_StoryLikes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_StoryLike_StoryId_UserId",
                table: "StoryLikes",
                newName: "IX_StoryLikes_StoryId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Story_UserId",
                table: "Stories",
                newName: "IX_Stories_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Report_TargetUserId",
                table: "Reports",
                newName: "IX_Reports_TargetUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Report_TargetPostId",
                table: "Reports",
                newName: "IX_Reports_TargetPostId");

            migrationBuilder.RenameIndex(
                name: "IX_Report_ResolvedById",
                table: "Reports",
                newName: "IX_Reports_ResolvedById");

            migrationBuilder.RenameIndex(
                name: "IX_Report_ReporterId",
                table: "Reports",
                newName: "IX_Reports_ReporterId");

            migrationBuilder.RenameIndex(
                name: "IX_PostLike_UserId",
                table: "PostLikes",
                newName: "IX_PostLikes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PostLike_PostId_UserId",
                table: "PostLikes",
                newName: "IX_PostLikes_PostId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Post_UserId",
                table: "Posts",
                newName: "IX_Posts_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PollVote_UserId",
                table: "PollVotes",
                newName: "IX_PollVotes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PollVote_PollOptionId_UserId",
                table: "PollVotes",
                newName: "IX_PollVotes_PollOptionId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PollOption_PostId",
                table: "PollOptions",
                newName: "IX_PollOptions_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_UserId",
                table: "Notifications",
                newName: "IX_Notifications_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Friendship_RequesterId_ReceiverId",
                table: "Friendships",
                newName: "IX_Friendships_RequesterId_ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_Friendship_ReceiverId",
                table: "Friendships",
                newName: "IX_Friendships_ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_Follow_FollowerId_FollowedId",
                table: "Follows",
                newName: "IX_Follows_FollowerId_FollowedId");

            migrationBuilder.RenameIndex(
                name: "IX_Follow_FollowedId",
                table: "Follows",
                newName: "IX_Follows_FollowedId");

            migrationBuilder.RenameIndex(
                name: "IX_Conversation_User2Id",
                table: "Conversations",
                newName: "IX_Conversations_User2Id");

            migrationBuilder.RenameIndex(
                name: "IX_Conversation_User1Id_User2Id",
                table: "Conversations",
                newName: "IX_Conversations_User1Id_User2Id");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_UserId",
                table: "Comments",
                newName: "IX_Comments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_PostId",
                table: "Comments",
                newName: "IX_Comments_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMessage_SenderId",
                table: "ChatMessages",
                newName: "IX_ChatMessages_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMessage_ConversationId",
                table: "ChatMessages",
                newName: "IX_ChatMessages_ConversationId");

            migrationBuilder.RenameIndex(
                name: "IX_BlockedUser_BlockerId_BlockedUserId",
                table: "BlockedUsers",
                newName: "IX_BlockedUsers_BlockerId_BlockedUserId");

            migrationBuilder.RenameIndex(
                name: "IX_BlockedUser_BlockedUserId",
                table: "BlockedUsers",
                newName: "IX_BlockedUsers_BlockedUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserRole_RoleId",
                table: "AspNetUserRoles",
                newName: "IX_AspNetUserRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserLogin_UserId",
                table: "AspNetUserLogins",
                newName: "IX_AspNetUserLogins_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserClaim_UserId",
                table: "AspNetUserClaims",
                newName: "IX_AspNetUserClaims_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetRoleClaim_RoleId",
                table: "AspNetRoleClaims",
                newName: "IX_AspNetRoleClaims_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoryViews",
                table: "StoryViews",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoryLikes",
                table: "StoryLikes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stories",
                table: "Stories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reports",
                table: "Reports",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostLikes",
                table: "PostLikes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Posts",
                table: "Posts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PollVotes",
                table: "PollVotes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PollOptions",
                table: "PollOptions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Friendships",
                table: "Friendships",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Follows",
                table: "Follows",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Conversations",
                table: "Conversations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatMessages",
                table: "ChatMessages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlockedUsers",
                table: "BlockedUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlockedUsers_AspNetUsers_BlockedUserId",
                table: "BlockedUsers",
                column: "BlockedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlockedUsers_AspNetUsers_BlockerId",
                table: "BlockedUsers",
                column: "BlockerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_AspNetUsers_SenderId",
                table: "ChatMessages",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Conversations_ConversationId",
                table: "ChatMessages",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Posts_PostId",
                table: "Comments",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_AspNetUsers_User1Id",
                table: "Conversations",
                column: "User1Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_AspNetUsers_User2Id",
                table: "Conversations",
                column: "User2Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_AspNetUsers_FollowedId",
                table: "Follows",
                column: "FollowedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_AspNetUsers_FollowerId",
                table: "Follows",
                column: "FollowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_ReceiverId",
                table: "Friendships",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_RequesterId",
                table: "Friendships",
                column: "RequesterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PollOptions_Posts_PostId",
                table: "PollOptions",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PollVotes_AspNetUsers_UserId",
                table: "PollVotes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PollVotes_PollOptions_PollOptionId",
                table: "PollVotes",
                column: "PollOptionId",
                principalTable: "PollOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostLikes_AspNetUsers_UserId",
                table: "PostLikes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostLikes_Posts_PostId",
                table: "PostLikes",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostMedia_Posts_PostId",
                table: "PostMedia",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_UserId",
                table: "Posts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_ReporterId",
                table: "Reports",
                column: "ReporterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_ResolvedById",
                table: "Reports",
                column: "ResolvedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_TargetUserId",
                table: "Reports",
                column: "TargetUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Posts_TargetPostId",
                table: "Reports",
                column: "TargetPostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_AspNetUsers_UserId",
                table: "Stories",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryLikes_AspNetUsers_UserId",
                table: "StoryLikes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StoryLikes_Stories_StoryId",
                table: "StoryLikes",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryViews_AspNetUsers_UserId",
                table: "StoryViews",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StoryViews_Stories_StoryId",
                table: "StoryViews",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
