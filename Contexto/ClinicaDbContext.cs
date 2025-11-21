using Microsoft.EntityFrameworkCore;
using APIClinica.Models;
using APIClinica.Models.Enums;

namespace APIClinica.Data
{
    public class ClinicaDbContext : DbContext
    {
        public ClinicaDbContext(DbContextOptions<ClinicaDbContext> options) : base(options) { }

        public DbSet<Seguro> Seguros { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<MedicoServicio> MedicoServicios { get; set; }
        public DbSet<Cita> Citas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Paciente
            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Documento).IsUnique();
                entity.HasOne(e => e.Seguro)
                      .WithMany(s => s.Pacientes)
                      .HasForeignKey(e => e.SeguroId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de MedicoServicio (Many-to-Many)
            modelBuilder.Entity<MedicoServicio>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.MedicoId, e.ServicioId }).IsUnique();
            });

            // Configuración de Cita - Control de cupos (máximo 2 por hora)
            modelBuilder.Entity<Cita>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.MedicoId, e.Fecha, e.Hora, e.Estado })
                      .HasDatabaseName("UX_Citas_CupoPorHora")
                      .IsUnique()
                      .HasFilter("[Estado] = 0"); // Solo para citas programadas
            });

            // Seed Data (datos iniciales)
            modelBuilder.Entity<Seguro>().HasData(
                new Seguro { Id = 1, Nombre = "Seguro A", Descripcion = "Seguro básico", Activo = true },
                new Seguro { Id = 2, Nombre = "Seguro B", Descripcion = "Seguro premium", Activo = true },
                new Seguro { Id = 3, Nombre = "Seguro C", Descripcion = "Seguro familiar", Activo = true }
            );

            modelBuilder.Entity<Servicio>().HasData(
                new Servicio { Id = 1, Nombre = "Consulta General", Descripcion = "Consulta médica general", Activo = true },
                new Servicio { Id = 2, Nombre = "Cardiología", Descripcion = "Especialidad en cardiología", Activo = true },
                new Servicio { Id = 3, Nombre = "Pediatría", Descripcion = "Atención pediátrica", Activo = true },
                new Servicio { Id = 4, Nombre = "Dermatología", Descripcion = "Especialidad en dermatología", Activo = true },
                new Servicio { Id = 5, Nombre = "Neurología", Descripcion = "Especialidad en neurología", Activo = true }
            );

            modelBuilder.Entity<Medico>().HasData(
                new Medico { Id = 1, Nombre = "Juan", Apellido = "Pérez", Especialidad = "Medicina General", Activo = true },
                new Medico { Id = 2, Nombre = "María", Apellido = "González", Especialidad = "Cardiología", Activo = true },
                new Medico { Id = 3, Nombre = "Carlos", Apellido = "Rodríguez", Especialidad = "Pediatría", Activo = true },
                new Medico { Id = 4, Nombre = "Ana", Apellido = "Martínez", Especialidad = "Dermatología", Activo = true }
            );

            modelBuilder.Entity<MedicoServicio>().HasData(
                new MedicoServicio { Id = 1, MedicoId = 1, ServicioId = 1 },
                new MedicoServicio { Id = 2, MedicoId = 2, ServicioId = 2 },
                new MedicoServicio { Id = 3, MedicoId = 2, ServicioId = 1 },
                new MedicoServicio { Id = 4, MedicoId = 3, ServicioId = 3 },
                new MedicoServicio { Id = 5, MedicoId = 3, ServicioId = 1 },
                new MedicoServicio { Id = 6, MedicoId = 4, ServicioId = 4 },
                new MedicoServicio { Id = 7, MedicoId = 4, ServicioId = 1 }
            );
        }
    }
}