using APIClinica.Models;
using APIClinica.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace APIClinica.Data
{
    public class ClinicaDbContext : DbContext
    {
        public ClinicaDbContext(DbContextOptions<ClinicaDbContext> options) : base(options) { }

        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorService> DoctorServices { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Document).IsUnique();
                entity.HasOne(e => e.Insurance)
                      .WithMany(s => s.Patients)
                      .HasForeignKey(e => e.InsuranceId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DoctorService>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.DoctorId, e.ServiceId }).IsUnique();
            });

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.DoctorId, e.Date, e.Time, e.Status })
                      .HasDatabaseName("UX_Appointments_SlotPerHour")
                      .IsUnique()
                      .HasFilter("[Status] = 0");
            });

            modelBuilder.Entity<Insurance>().HasData(
                new Insurance { Id = 1, Name = "Insurance A", Description = "Basic insurance", Active = true },
                new Insurance { Id = 2, Name = "Insurance B", Description = "Premium insurance", Active = true },
                new Insurance { Id = 3, Name = "Insurance C", Description = "Family insurance", Active = true }
            );

            modelBuilder.Entity<Service>().HasData(
                new Service { Id = 1, Name = "General Consultation", Description = "General medical consultation", Active = true },
                new Service { Id = 2, Name = "Cardiology", Description = "Cardiology specialty", Active = true },
                new Service { Id = 3, Name = "Pediatrics", Description = "Pediatric care", Active = true },
                new Service { Id = 4, Name = "Dermatology", Description = "Dermatology specialty", Active = true },
                new Service { Id = 5, Name = "Neurology", Description = "Neurology specialty", Active = true }
            );

            modelBuilder.Entity<Doctor>().HasData(
                new Doctor { Id = 1, FirstName = "Juan", LastName = "Pérez", Specialty = "General Medicine", Active = true },
                new Doctor { Id = 2, FirstName = "María", LastName = "González", Specialty = "Cardiology", Active = true },
                new Doctor { Id = 3, FirstName = "Carlos", LastName = "Rodríguez", Specialty = "Pediatrics", Active = true },
                new Doctor { Id = 4, FirstName = "Ana", LastName = "Martínez", Specialty = "Dermatology", Active = true }
            );

            modelBuilder.Entity<DoctorService>().HasData(
                new DoctorService { Id = 1, DoctorId = 1, ServiceId = 1 },
                new DoctorService { Id = 2, DoctorId = 2, ServiceId = 2 },
                new DoctorService { Id = 3, DoctorId = 2, ServiceId = 1 },
                new DoctorService { Id = 4, DoctorId = 3, ServiceId = 3 },
                new DoctorService { Id = 5, DoctorId = 3, ServiceId = 1 },
                new DoctorService { Id = 6, DoctorId = 4, ServiceId = 4 },
                new DoctorService { Id = 7, DoctorId = 4, ServiceId = 1 }
            );
        }
    }
}