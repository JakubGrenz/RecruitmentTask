using Microsoft.EntityFrameworkCore;

namespace Guestline.Models.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<RoomTypeAmenity> RoomTypeAmenities { get; set; }
        public DbSet<RoomTypeFeature> RoomTypeFeatures { get; set; }
        public DbSet<RoomAmenity> RoomAmenities { get; set; }
        public DbSet<RoomFeature> RoomFeature { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("TestDatabase");
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoomAmenity>()
                .HasIndex(rt => rt.Name)
                .IsUnique();

            modelBuilder.Entity<RoomFeature>()
                .HasIndex(rt => rt.Name)
                .IsUnique();
        }
    }
}
