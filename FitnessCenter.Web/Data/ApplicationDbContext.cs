using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FitnessCenter.Web.Models;

namespace FitnessCenter.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Gym> Gyms { get; set; } = null!;
        public DbSet<GymService> GymServices { get; set; } = null!;
        public DbSet<Trainer> Trainers { get; set; } = null!;
        public DbSet<TrainerService> TrainerServices { get; set; } = null!;
        public DbSet<TrainerAvailability> TrainerAvailabilities { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<GymService>()
           .Property(x => x.Price)
           .HasPrecision(18, 2);

            builder.Entity<Appointment>()
                .Property(x => x.Price)
                .HasPrecision(18, 2);

            builder.Entity<TrainerService>()
                .HasKey(ts => new { ts.TrainerId, ts.GymServiceId });

            builder.Entity<TrainerService>()
                .HasOne(ts => ts.Trainer)
                .WithMany(t => t.TrainerServices)
                .HasForeignKey(ts => ts.TrainerId);

            builder.Entity<TrainerService>()
                .HasOne(ts => ts.GymService)
                .WithMany(gs => gs.TrainerServices)
                .HasForeignKey(ts => ts.GymServiceId);
        }

    }
}
