using dating_app_backend.src.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace dating_app_backend.src.DB
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<PostModel> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(x => x.Id);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Username).IsUnique();
            });

            modelBuilder.Entity<PostModel>(entity =>
            {
                entity.ToTable("Posts");
                entity.HasOne(p => p.User)
                      .WithMany(b => b.Posts)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    } 
}