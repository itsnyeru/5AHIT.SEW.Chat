using Microsoft.EntityFrameworkCore;
using EFCAT.Model.Configuration;
using Model.Entity;

namespace Model.Configuration {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public class ChatDbContext : DatabaseContext {
        public DbSet<Chat> Chats { get; set;}
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageAttachment> MessageAttachments { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<Groupchat> Groupchats { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Singlechat> UserChats { get; set; }
        public DbSet<UserCode> UserCodes { get; set; }
        public DbSet<Friend> UserFriends { get; set; }

        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<User>().Property(p => p.ChatMode).HasConversion<string>();
            modelBuilder.Entity<UserCode>().Property(c => c.Type).HasConversion<string>();
            base.OnModelCreating(modelBuilder);
        }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
