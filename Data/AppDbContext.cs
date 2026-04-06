using Microsoft.EntityFrameworkCore;
using HealthyBites.Api.Models;

namespace HealthyBites.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<AppUser> AppUsers { get; set; } = null!;
        public DbSet<SugarReading> SugarReadings { get; set; } = null!;
        public DbSet<Prediction> Predictions { get; set; } = null!;
        public DbSet<Restaurant> Restaurants { get; set; } = null!;
        public DbSet<UserRestaurant> UserRestaurants { get; set; } = null!;
        public DbSet<RestaurantReview> RestaurantReviews { get; set; } = null!;
        public DbSet<DailyCalory> DailyCalories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Prediction ↔ SugarReading one-to-one
            modelBuilder.Entity<Prediction>()
                .HasOne(p => p.SugarReading)
                .WithOne(s => s.Prediction)
                .HasForeignKey<Prediction>(p => p.SugarReadingId)
                .OnDelete(DeleteBehavior.Restrict); // لتجنب multiple cascade paths

            // SugarReading ↔ AppUser
            modelBuilder.Entity<SugarReading>()
                .HasOne(s => s.User)
                .WithMany(u => u.SugarReadings)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Prediction ↔ AppUser
            modelBuilder.Entity<Prediction>()
                .HasOne(p => p.User)
                .WithMany(u => u.Predictions)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserRestaurant relationships
            modelBuilder.Entity<UserRestaurant>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRestaurants)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRestaurant>()
                .HasOne(ur => ur.Restaurant)
                .WithMany(r => r.UserRestaurants)
                .HasForeignKey(ur => ur.RestaurantId);

            // RestaurantReview relationships
            modelBuilder.Entity<RestaurantReview>()
                .HasOne(rr => rr.User)
                .WithMany(u => u.RestaurantReviews)
                .HasForeignKey(rr => rr.UserId);

            modelBuilder.Entity<RestaurantReview>()
                .HasOne(rr => rr.Restaurant)
                .WithMany(r => r.Reviews)
                .HasForeignKey(rr => rr.RestaurantId);

            // DailyCalory ↔ AppUser
            modelBuilder.Entity<DailyCalory>()
                .HasOne(d => d.User)
                .WithMany(u => u.DailyCalories)
                .HasForeignKey(d => d.UserId);
        }
    }
}
