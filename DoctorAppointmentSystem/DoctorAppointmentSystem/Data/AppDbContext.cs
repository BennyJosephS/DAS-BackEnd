using DoctorAppointmentSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentSystem.Data;

public class AppDbContext:DbContext
{
    public DbSet<Doctors> __Doctors { get; set; }
    public DbSet<TimeSlot> __TimeSlot { get; set; }
    public DbSet<User> __User { get; set; }
    public DbSet<Appointment> __Appointment { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> option) : base(option)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // USER
    modelBuilder.Entity<User>(entity =>
    {
        entity.HasKey(u => u.Id);
        entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
        entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
        entity.HasIndex(u => u.Email).IsUnique();
        entity.Property(u => u.Gender).HasMaxLength(10);
        entity.Property(u => u.PasswordHash).IsRequired();
    });

    // DOCTORS
    modelBuilder.Entity<Doctors>(entity =>
    {
        entity.HasKey(d => d.Id);
        entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
        entity.Property(d => d.Email).IsRequired().HasMaxLength(100);
        entity.HasIndex(d => d.Email).IsUnique();
        entity.Property(d => d.Speciality).IsRequired().HasMaxLength(100);
        entity.Property(d => d.Gender).HasMaxLength(10);
    });

    // TIMESLOT
    modelBuilder.Entity<TimeSlot>(entity =>
    {
        entity.HasKey(t => t.Id);
        entity.Property(t => t.Date).IsRequired();
        entity.Property(t => t.StartTime).IsRequired();
        entity.Property(t => t.EndTime).IsRequired();
        entity.Property(t => t.Isbooked).HasDefaultValue(false);

        // FK → Doctor
        entity.HasOne<Doctors>()
              .WithMany()
              .HasForeignKey(t => t.DoctorId)
              .OnDelete(DeleteBehavior.Cascade);
    });

    // APPOINTMENT
    modelBuilder.Entity<Appointment>(entity =>
    {
        entity.HasKey(a => a.Id);
        entity.Property(a => a.Email).IsRequired().HasMaxLength(100);
        entity.Property(a => a.Description).HasMaxLength(500);
        entity.Property(a => a.Status).IsRequired().HasMaxLength(20);

        // FK → Patient (User)
        entity.HasOne<User>()
              .WithMany()
              .HasForeignKey(a => a.PatientId)
              .OnDelete(DeleteBehavior.Restrict);

        // FK → Doctor
        entity.HasOne<Doctors>()
              .WithMany()
              .HasForeignKey(a => a.DocterId)
              .OnDelete(DeleteBehavior.Restrict);

        // FK → TimeSlot (owned navigation)
        entity.HasOne(a => a.TimeSlot)
              .WithOne()
              .HasForeignKey<Appointment>(a => a.Id)
              .OnDelete(DeleteBehavior.Restrict);
    });
}

}