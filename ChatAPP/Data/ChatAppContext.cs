using Microsoft.EntityFrameworkCore;
using ChatApp.Models;

namespace ChatApp.Data
{
    public class ChatAppContext : DbContext
    {
        public ChatAppContext(DbContextOptions<ChatAppContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User and ChatRoom many-to-many relationship
            modelBuilder.Entity<User>()
                .HasMany(u => u.ChatRooms)
                .WithMany(r => r.Users)
                .UsingEntity(j => j.ToTable("UserChatRooms"));

            // Configure ChatRoom and ChatMessage one-to-many relationship
            modelBuilder.Entity<ChatRoom>()
                .HasMany(r => r.Messages)
                .WithOne(m => m.ChatRoom)
                .HasForeignKey(m => m.ChatRoomId);

            // Configure User and ChatMessage one-to-many relationship
            modelBuilder.Entity<User>()
                .HasMany(u => u.Messages)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserId);
        }
    }
}
