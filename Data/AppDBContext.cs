using Microsoft.EntityFrameworkCore;
using BlogApp_SharedModels.Models;

namespace Blog.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<User> users { get; set; }
        public DbSet<Post> posts { get; set; }
        public DbSet<Coment> coments { get; set; }
        public DbSet<Like> likes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Likes)
                .WithOne(l => l.Post)
                .HasForeignKey(l => l.postid)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
