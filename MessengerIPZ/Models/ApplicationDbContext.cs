
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Channel> Channels { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChannelMember> ChannelMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.Property(u => u.UserName).IsRequired().HasMaxLength(50);
            });

            builder.Entity<ChannelMember>(entity =>
            {
                entity.HasKey(cm => new { cm.UserId, cm.ChannelId });

                entity.HasOne(cm => cm.User)
                    .WithMany(u => u.SubscribedChannels)
                    .HasForeignKey(cm => cm.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cm => cm.Channel)
                    .WithMany(c => c.Members)
                    .HasForeignKey(cm => cm.ChannelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Message>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.HasOne(m => m.User)
                    .WithMany()
                    .HasForeignKey(m => m.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Channel)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(m => m.ChannelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}