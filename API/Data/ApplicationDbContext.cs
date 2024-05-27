using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<User, IdentityRole<int>, int>(options)
    {
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Message>()
                    .HasOne(u => u.Recipient)
                    .WithMany(m => m.MessagesRecieved)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                    .HasOne(u => u.Sender)
                    .WithMany(m => m.MessagesSent)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}