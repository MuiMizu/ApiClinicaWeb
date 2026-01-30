# 📢 ACTUALIZACIÓN DE BASE DE DATOS (AUDITORÍA)

Ejecutar el siguiente script para agregar campos de auditoría (`CreateDate`, `CreateBy`, `UpdateAt`, `UpdateBy`) a las tablas `Patients`, `Doctors` y `Appointments`.
> **Nota:** La tabla `Appointments` ya cuenta con fecha de creación, por lo que solo se agregan los campos faltantes.

```sql
USE ClinicaDB;
GO

-- =============================================
-- 1. Tabla: Patients
-- =============================================
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'CreateDate' AND Object_ID = Object_ID(N'Patients'))
    ALTER TABLE Patients ADD CreateDate DATETIME2 DEFAULT GETDATE();

IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'CreateBy' AND Object_ID = Object_ID(N'Patients'))
    ALTER TABLE Patients ADD CreateBy NVARCHAR(100) NULL;

IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'UpdateAt' AND Object_ID = Object_ID(N'Patients'))
    ALTER TABLE Patients ADD UpdateAt DATETIME2 NULL;

IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'UpdateBy' AND Object_ID = Object_ID(N'Patients'))
    ALTER TABLE Patients ADD UpdateBy NVARCHAR(100) NULL;
GO

-- =============================================
-- 2. Tabla: Doctors
-- =============================================
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'CreateDate' AND Object_ID = Object_ID(N'Doctors'))
    ALTER TABLE Doctors ADD CreateDate DATETIME2 DEFAULT GETDATE();

IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'CreateBy' AND Object_ID = Object_ID(N'Doctors'))
    ALTER TABLE Doctors ADD CreateBy NVARCHAR(100) NULL;

IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'UpdateAt' AND Object_ID = Object_ID(N'Doctors'))
    ALTER TABLE Doctors ADD UpdateAt DATETIME2 NULL;

IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'UpdateBy' AND Object_ID = Object_ID(N'Doctors'))
    ALTER TABLE Doctors ADD UpdateBy NVARCHAR(100) NULL;
GO

-- =============================================
-- 3. Tabla: Appointments
-- =============================================
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'CreateBy' AND Object_ID = Object_ID(N'Appointments'))
    ALTER TABLE Appointments ADD CreateBy NVARCHAR(100) NULL;

IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'UpdateAt' AND Object_ID = Object_ID(N'Appointments'))
    ALTER TABLE Appointments ADD UpdateAt DATETIME2 NULL;

IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'UpdateBy' AND Object_ID = Object_ID(N'Appointments'))
    ALTER TABLE Appointments ADD UpdateBy NVARCHAR(100) NULL;
GO
```

---


# Sistema de Clínica - Backend (C# .NET 8 + SQL Server)

Guía completa para construir el backend desde cero con todas las especificaciones requeridas.

## 📋 Tabla de Contenidos

1. [Requisitos](#requisitos)
2. [Estructura del Proyecto](#estructura-del-proyecto)
3. [Configuración de Base de Datos](#configuración-de-base-de-datos)
4. [Entidades](#entidades)
5. [DbContext y Configuración](#dbcontext-y-configuración)
6. [Validaciones](#validaciones)
7. [Controladores](#controladores)
8. [Lógica de Negocio](#lógica-de-negocio)
9. [Stored Procedures](#stored-procedures)
10. [Semillas (Seed Data)](#semillas-seed-data)
11. [Configuración Final](#configuración-final)

---

## Requisitos

- .NET 8 SDK
- SQL Server (2019 o superior)
- Visual Studio 2022 o VS Code
- SQL Server Management Studio (SSMS) o Azure Data Studio

---

## Estructura del Proyecto

```
APIClinica/
├── Controllers/
│   ├── PatientsController.cs
│   ├── InsurancesController.cs
│   ├── ServicesController.cs
│   ├── DoctorsController.cs
│   └── AppointmentsController.cs
├── Models/
│   ├── Insurance.cs
│   ├── Patient.cs
│   ├── Service.cs
│   ├── Doctor.cs
│   ├── DoctorService.cs
│   ├── Appointment.cs
│   └── Enums/
│       └── AppointmentStatus.cs
├── Data/
│   ├── ClinicaDbContext.cs
│   └── DbInitializer.cs
├── DTOs/
│   ├── PatientDTO.cs
│   ├── AppointmentDTO.cs
│   ├── AvailableDoctorDTO.cs
│   └── AvailableTimeDTO.cs
├── Repositories/
│   ├── IRepository.cs
│   ├── Repository.cs
│   ├── IPatientRepository.cs
│   ├── PatientRepository.cs
│   ├── IAppointmentRepository.cs
│   ├── AppointmentRepository.cs
│   ├── IDoctorRepository.cs
│   └── DoctorRepository.cs
├── Services/
│   ├── IPatientService.cs
│   ├── PatientService.cs
│   ├── IAppointmentService.cs
│   └── AppointmentService.cs
├── Program.cs
├── appsettings.json
└── APIClinica.csproj
```

---

## Configuración de Base de Datos

### 1. Crear Base de Datos

```sql
CREATE DATABASE ClinicaDB;
GO

USE ClinicaDB;
GO
```

### 2. Instalar Paquetes NuGet

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package AspNetCore.HealthChecks.SqlServer
dotnet add package Microsoft.Data.SqlClient
```

**Nota:** El paquete `Microsoft.Data.SqlClient` es necesario para manejar correctamente las excepciones SQL Server (códigos de error 2601 y 2627) en el manejo de concurrencia.

---

## Entidades

### AppointmentStatus.cs

```csharp
namespace APIClinica.Models.Enums
{
    public enum AppointmentStatus
    {
        Scheduled = 0,
        Attended = 1,
        Cancelled = 2
    }
}
```

### Insurance.cs

```csharp
using System.ComponentModel.DataAnnotations;

namespace APIClinica.Models
{
    public class Insurance
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public bool Active { get; set; } = true;
        
        // Navigation
        public ICollection<Patient> Patients { get; set; } = new List<Patient>();
    }
}
```

### Patient.cs

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIClinica.Models
{
    public class Patient
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string Document { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }
        
        [Required]
        public int InsuranceId { get; set; }
        
        // Navigation
        [ForeignKey(nameof(InsuranceId))]
        public Insurance Insurance { get; set; } = null!;
        
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
```

### Service.cs

```csharp
using System.ComponentModel.DataAnnotations;

namespace APIClinica.Models
{
    public class Service
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public bool Active { get; set; } = true;
        
        // Navigation
        public ICollection<DoctorService> DoctorServices { get; set; } = new List<DoctorService>();
    }
}
```

### Doctor.cs

```csharp
using System.ComponentModel.DataAnnotations;

namespace APIClinica.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string? Specialty { get; set; }
        
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }
        
        public bool Active { get; set; } = true;
        
        public ICollection<DoctorService> DoctorServices { get; set; } = new List<DoctorService>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
```

### DoctorService.cs

```csharp
namespace APIClinica.Models
{
    public class DoctorService
    {
        public int Id { get; set; }
        
        public int DoctorId { get; set; }
        public int ServiceId { get; set; }
        
        // Navigation
        public Doctor Doctor { get; set; } = null!;
        public Service Service { get; set; } = null!;
    }
}
```

### Appointment.cs

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using APIClinica.Models.Enums;

namespace APIClinica.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        
        [Required]
        public int PatientId { get; set; }
        
        [Required]
        public int DoctorId { get; set; }
        
        [Required]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
        
        [Required]
        [RegularExpression(@"^(0[8-9]|1[0-7]):00$", ErrorMessage = "La hora debe estar entre 08:00 y 17:00")]
        public string Time { get; set; } = string.Empty;
        
        [Required]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        // Navigation
        [ForeignKey(nameof(PatientId))]
        public Patient Patient { get; set; } = null!;
        
        [ForeignKey(nameof(DoctorId))]
        public Doctor Doctor { get; set; } = null!;
    }
}
```

---

## DbContext y Configuración

### ClinicaDbContext.cs

```csharp
using Microsoft.EntityFrameworkCore;
using APIClinica.Models;
using APIClinica.Models.Enums;

namespace APIClinica.Data
{
    public class ClinicaDbContext : DbContext
    {
        public ClinicaDbContext(DbContextOptions<ClinicaDbContext> options) : base(options)
        {
        }

        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorService> DoctorServices { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Insurance
            modelBuilder.Entity<Insurance>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            // Configuración de Patient
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Document).IsUnique();
                entity.HasOne(e => e.Insurance)
                      .WithMany(s => s.Patients)
                      .HasForeignKey(e => e.InsuranceId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Service
            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            // Configuración de Doctor
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            // Configuración de DoctorService (Many-to-Many)
            modelBuilder.Entity<DoctorService>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.DoctorId, e.ServiceId }).IsUnique();
                entity.HasOne(e => e.Doctor)
                      .WithMany(m => m.DoctorServices)
                      .HasForeignKey(e => e.DoctorId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Service)
                      .WithMany(s => s.DoctorServices)
                      .HasForeignKey(e => e.ServiceId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de Appointment
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // Índice NO único para mejorar consultas de disponibilidad
                // La validación de máximo 2 cupos por hora se hace en la aplicación con transacciones
                entity.HasIndex(e => new { e.DoctorId, e.Date, e.Time, e.Status })
                      .HasDatabaseName("IX_Appointments_SlotPerHour")
                      .HasFilter("[Status] = 0"); // Solo para citas programadas
                
                entity.HasOne(e => e.Patient)
                      .WithMany(p => p.Appointments)
                      .HasForeignKey(e => e.PatientId)
                      .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.Doctor)
                      .WithMany(m => m.Appointments)
                      .HasForeignKey(e => e.DoctorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed Data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Insurances
            modelBuilder.Entity<Insurance>().HasData(
                new Insurance { Id = 1, Name = "Seguro A", Description = "Seguro básico", Active = true },
                new Insurance { Id = 2, Name = "Seguro B", Description = "Seguro premium", Active = true },
                new Insurance { Id = 3, Name = "Seguro C", Description = "Seguro familiar", Active = true }
            );

            // Services
            modelBuilder.Entity<Service>().HasData(
                new Service { Id = 1, Name = "Consulta General", Description = "Consulta médica general", Active = true },
                new Service { Id = 2, Name = "Cardiología", Description = "Especialidad en cardiología", Active = true },
                new Service { Id = 3, Name = "Pediatría", Description = "Atención pediátrica", Active = true },
                new Service { Id = 4, Name = "Dermatología", Description = "Especialidad en dermatología", Active = true },
                new Service { Id = 5, Name = "Neurología", Description = "Especialidad en neurología", Active = true }
            );

            // Doctors
            modelBuilder.Entity<Doctor>().HasData(
                new Doctor { Id = 1, FirstName = "Juan", LastName = "Pérez", Specialty = "Medicina General", Active = true },
                new Doctor { Id = 2, FirstName = "María", LastName = "González", Specialty = "Cardiología", Active = true },
                new Doctor { Id = 3, FirstName = "Carlos", LastName = "Rodríguez", Specialty = "Pediatría", Active = true },
                new Doctor { Id = 4, FirstName = "Ana", LastName = "Martínez", Specialty = "Dermatología", Active = true }
            );

            // DoctorServices (relaciones)
            modelBuilder.Entity<DoctorService>().HasData(
                new DoctorService { Id = 1, DoctorId = 1, ServiceId = 1 }, // Juan - Consulta General
                new DoctorService { Id = 2, DoctorId = 2, ServiceId = 2 }, // María - Cardiología
                new DoctorService { Id = 3, DoctorId = 2, ServiceId = 1 }, // María - Consulta General
                new DoctorService { Id = 4, DoctorId = 3, ServiceId = 3 }, // Carlos - Pediatría
                new DoctorService { Id = 5, DoctorId = 3, ServiceId = 1 }, // Carlos - Consulta General
                new DoctorService { Id = 6, DoctorId = 4, ServiceId = 4 }, // Ana - Dermatología
                new DoctorService { Id = 7, DoctorId = 4, ServiceId = 1 }  // Ana - Consulta General
            );
        }
    }
}
```

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ClinicaDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

### Program.cs

```csharp
using Microsoft.EntityFrameworkCore;
using APIClinica.Data;
using APIClinica.Services;
using APIClinica.Repositories;
using APIClinica.Models;
using Microsoft.Data.SqlClient;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<ClinicaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// IMPORTANTE: El orden de registro es importante
// Primero se registran los repositorios genéricos, luego los específicos, y finalmente los servicios

// Repositories - Generic Repository Pattern (debe ir primero)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Specific Repositories (deben ir antes de los servicios que los usan)
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();

// Services (deben ir después de los repositorios que dependen)
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

// Health Checks
builder.Services.AddHealthChecks()
    .AddSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ?? "",
        name: "sqlserver",
        timeout: TimeSpan.FromSeconds(5));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

// Health Check endpoint (usa el sistema integrado de health checks)
app.MapHealthChecks("/health");

// Health endpoint personalizado que verifica la conexión real
app.MapGet("/health/detailed", async (ClinicaDbContext db) =>
{
    try
    {
        var connection = db.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1";
        command.CommandTimeout = 5;
        await command.ExecuteScalarAsync();

        return Results.Ok(new
        {
            status = "healthy",
            db = "connected",
            timestamp = DateTime.UtcNow
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: $"Database connection failed: {ex.Message}",
            statusCode: StatusCodes.Status503ServiceUnavailable);
    }
})
.WithName("HealthDetailed")
.WithTags("Health");

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ClinicaDbContext>();
    context.Database.EnsureCreated();
    
    // Seed patients if they don't exist
    if (!context.Patients.Any())
    {
        DbInitializer.SeedPatients(context);
    }
}

app.Run();
```

---

## Validaciones

### CHECK Constraint en Base de Datos

Ejecutar después de crear las migraciones:

```sql
USE ClinicaDB;
GO

-- Si existe el índice único antiguo, eliminarlo primero
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'UX_Appointments_SlotPerHour' AND object_id = OBJECT_ID('Appointments'))
BEGIN
    DROP INDEX UX_Appointments_SlotPerHour ON Appointments;
END
GO

-- Constraint para validar hora entre 8-17
ALTER TABLE Appointments
ADD CONSTRAINT CK_Appointments_Time_Valid 
CHECK (Time IN ('08:00', '09:00', '10:00', '11:00', '12:00', '13:00', '14:00', '15:00', '16:00', '17:00'));
GO
```

**Nota Importante:** Si ya tienes una base de datos creada con el índice único `UX_Appointments_SlotPerHour`, debes eliminarlo primero ejecutando el script SQL anterior. El índice único impedía tener 2 citas en la misma hora. Ahora usamos un índice normal (`IX_Appointments_SlotPerHour`) y la validación de máximo 2 cupos se hace en la aplicación con transacciones.

---

## DTOs

### PatientDTO.cs

```csharp
namespace APIClinica.DTOs
{
    public class PatientDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int InsuranceId { get; set; }
        public string? InsuranceName { get; set; }
    }

    public class CreatePatientDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int InsuranceId { get; set; }
    }
}
```

### AppointmentDTO.cs

```csharp
using APIClinica.Models.Enums;

namespace APIClinica.DTOs
{
    public class AppointmentDTO
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public int DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; } = string.Empty;
        public AppointmentStatus Status { get; set; }
        public string? ServiceName { get; set; }
    }

    public class CreateAppointmentDTO
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; } = string.Empty;
    }
}
```

### AvailableDoctorDTO.cs

```csharp
namespace APIClinica.DTOs
{
    public class AvailableDoctorDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Specialty { get; set; }
    }
}
```

### AvailableTimeDTO.cs

```csharp
namespace APIClinica.DTOs
{
    public class AvailableTimeDTO
    {
        public string Time { get; set; } = string.Empty;
        public int AvailableSlots { get; set; }
    }
}   
```

---

## Repositories (Repositorios)

### Generic Repository Pattern (Patrón de Repositorio Genérico)

#### IRepository.cs

```csharp
using System.Linq.Expressions;
using System.Linq;

namespace APIClinica.Repositories
{
    public interface IRepository<T> where T : class
    {
        // Query methods
        IQueryable<T> GetQueryable();
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        
        // Command methods (without SaveChanges)
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        
        // Save changes
        Task<int> SaveChangesAsync();
    }
}
```

#### Repository.cs

```csharp
using Microsoft.EntityFrameworkCore;
using APIClinica.Data;
using System.Linq.Expressions;
using System.Linq;

namespace APIClinica.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ClinicaDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ClinicaDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // Query methods
        public virtual IQueryable<T> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate == null 
                ? await _dbSet.CountAsync() 
                : await _dbSet.CountAsync(predicate);
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        // Command methods (track changes but don't save)
        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public virtual void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        // Save changes
        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
```

#### IPatientRepository.cs

```csharp
using APIClinica.Models;

namespace APIClinica.Repositories
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<IEnumerable<Patient>> GetPatientsWithInsuranceAsync();
        Task<Patient?> GetPatientWithInsuranceByIdAsync(int id);
        Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm);
    }
}
```

#### PatientRepository.cs

```csharp
using Microsoft.EntityFrameworkCore;
using APIClinica.Data;
using APIClinica.Models;

namespace APIClinica.Repositories
{
    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        public PatientRepository(ClinicaDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Patient>> GetPatientsWithInsuranceAsync()
        {
            return await _dbSet
                .Include(p => p.Insurance)
                .ToListAsync();
        }

        public async Task<Patient?> GetPatientWithInsuranceByIdAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Insurance)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm)
        {
            return await _dbSet
                .Include(p => p.Insurance)
                .Where(p => p.FirstName.Contains(searchTerm) ||
                           p.LastName.Contains(searchTerm) ||
                           p.Document.Contains(searchTerm))
                .ToListAsync();
        }
    }
}
```

#### IAppointmentRepository.cs

```csharp
using APIClinica.Models;
using APIClinica.Models.Enums;

namespace APIClinica.Repositories
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetAppointmentsWithDetailsAsync();
        Task<Appointment?> GetAppointmentWithDetailsByIdAsync(int id);
        Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date);
        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(int patientId);
        Task<IEnumerable<Appointment>> GetAppointmentsByStatusAsync(AppointmentStatus status);
        Task<int> CountAppointmentsByDoctorAndTimeAsync(int doctorId, DateTime date, string time);
    }
}
``` 

#### AppointmentRepository.cs

```csharp
using Microsoft.EntityFrameworkCore;
using APIClinica.Data;
using APIClinica.Models;
using APIClinica.Models.Enums;

namespace APIClinica.Repositories
{
    public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(ClinicaDbContext context) : base(context)
        {
        }

        private IQueryable<Appointment> WithDetails() => _dbSet
            .Include(a => a.Patient)
            .Include(a => a.Doctor);

        public async Task<IEnumerable<Appointment>> GetAppointmentsWithDetailsAsync()
        {
            return await WithDetails().ToListAsync();
        }

        public async Task<Appointment?> GetAppointmentWithDetailsByIdAsync(int id)
        {
            return await WithDetails().FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date)
        {
            return await WithDetails().Where(a => a.Date.Date == date.Date).ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId)
        {
            return await WithDetails().Where(a => a.DoctorId == doctorId).ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(int patientId)
        {
            return await WithDetails().Where(a => a.PatientId == patientId).ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByStatusAsync(AppointmentStatus status)
        {
            return await WithDetails().Where(a => a.Status == status).ToListAsync();
        }

        public async Task<int> CountAppointmentsByDoctorAndTimeAsync(int doctorId, DateTime date, string time)
        {
            return await _dbSet
                .CountAsync(a => a.DoctorId == doctorId &&
                                a.Date.Date == date.Date &&
                                a.Time == time &&
                                a.Status == AppointmentStatus.Scheduled);
        }
    }
}
```

#### IDoctorRepository.cs

```csharp
using APIClinica.Models;

namespace APIClinica.Repositories
{
    public interface IDoctorRepository : IRepository<Doctor>
    {
        Task<IEnumerable<Doctor>> GetActiveDoctorsAsync();
        Task<Doctor?> GetDoctorWithServicesByIdAsync(int id);
        Task<IEnumerable<Doctor>> GetDoctorsByServiceAsync(int serviceId);
    }
}
```

#### DoctorRepository.cs

```csharp
using Microsoft.EntityFrameworkCore;
using APIClinica.Data;
using APIClinica.Models;

namespace APIClinica.Repositories
{
    public class DoctorRepository : Repository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(ClinicaDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Doctor>> GetActiveDoctorsAsync()
        {
            return await _dbSet
                .Where(d => d.Active)
                .ToListAsync();
        }

        public async Task<Doctor?> GetDoctorWithServicesByIdAsync(int id)
        {
            return await _dbSet
                .Include(d => d.DoctorServices)
                    .ThenInclude(ds => ds.Service)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsByServiceAsync(int serviceId)
        {
            return await _dbSet
                .Include(d => d.DoctorServices)
                .Where(d => d.Active && d.DoctorServices.Any(ds => ds.ServiceId == serviceId))
                .ToListAsync();
        }
    }
}
```

---

## Services (Servicios)

### IPatientService.cs

```csharp
using APIClinica.DTOs;

namespace APIClinica.Services
{
    public interface IPatientService
    {
        Task<(List<PatientDTO> Items, int TotalItems, int CurrentPage, int TotalPages)> 
            GetAllAsync(int page = 1, int pageSize = 10, string? search = null);
        Task<PatientDTO?> GetByIdAsync(int id);
        Task<PatientDTO> CreateAsync(CreatePatientDTO dto);
        Task<PatientDTO> UpdateAsync(int id, CreatePatientDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
```

### PatientService.cs

```csharp
using APIClinica.DTOs;
using APIClinica.Models;
using APIClinica.Repositories;

namespace APIClinica.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IRepository<Insurance> _insuranceRepository;

        public PatientService(
            IPatientRepository patientRepository,
            IRepository<Insurance> insuranceRepository)
        {
            _patientRepository = patientRepository;
            _insuranceRepository = insuranceRepository;
        }

        public async Task<(List<PatientDTO> Items, int TotalItems, int CurrentPage, int TotalPages)> 
            GetAllAsync(int page = 1, int pageSize = 10, string? search = null)
        {
            IEnumerable<Patient> patients;

            if (!string.IsNullOrWhiteSpace(search))
            {
                patients = await _patientRepository.SearchPatientsAsync(search);
            }
            else
            {
                patients = await _patientRepository.GetPatientsWithInsuranceAsync();
            }

            var totalItems = patients.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = patients
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PatientDTO
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Document = p.Document,
                    Phone = p.Phone,
                    Email = p.Email,
                    InsuranceId = p.InsuranceId,
                    InsuranceName = p.Insurance?.Name ?? string.Empty
                })
                .ToList();

            return (items, totalItems, page, totalPages);
        }

        public async Task<PatientDTO?> GetByIdAsync(int id)
        {
            var patient = await _patientRepository.GetPatientWithInsuranceByIdAsync(id);
            if (patient == null)
                return null;

            return new PatientDTO
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Document = patient.Document,
                Phone = patient.Phone,
                Email = patient.Email,
                InsuranceId = patient.InsuranceId,
                InsuranceName = patient.Insurance?.Name ?? string.Empty
            };
        }

        public async Task<PatientDTO> CreateAsync(CreatePatientDTO dto)
        {
            // Validate insurance exists
            var insurance = await _insuranceRepository.GetByIdAsync(dto.InsuranceId);
            if (insurance == null || !insurance.Active)
                throw new ArgumentException("Insurance not found or inactive");

            var patient = new Patient
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Document = dto.Document,
                Phone = dto.Phone,
                Email = dto.Email,
                InsuranceId = dto.InsuranceId
            };

            await _patientRepository.AddAsync(patient);
            await _patientRepository.SaveChangesAsync();

            return await GetByIdAsync(patient.Id) ?? throw new Exception("Error creating patient");
        }

        public async Task<PatientDTO> UpdateAsync(int id, CreatePatientDTO dto)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                throw new KeyNotFoundException("Patient not found");

            // Validate insurance exists
            var insurance = await _insuranceRepository.GetByIdAsync(dto.InsuranceId);
            if (insurance == null || !insurance.Active)
                throw new ArgumentException("Insurance not found or inactive");

            patient.FirstName = dto.FirstName;
            patient.LastName = dto.LastName;
            patient.Document = dto.Document;
            patient.Phone = dto.Phone;
            patient.Email = dto.Email;
            patient.InsuranceId = dto.InsuranceId;

            _patientRepository.Update(patient);
            await _patientRepository.SaveChangesAsync();

            return await GetByIdAsync(id) ?? throw new Exception("Error updating patient");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                return false;

            _patientRepository.Remove(patient);
            await _patientRepository.SaveChangesAsync();
            return true;
        }
    }
}
```

### IAppointmentService.cs

```csharp
using APIClinica.DTOs;
using APIClinica.Models.Enums;

namespace APIClinica.Services
{
    public interface IAppointmentService
    {
        Task<List<AppointmentDTO>> GetAllAsync(DateTime? date = null, int? doctorId = null, int? patientId = null, AppointmentStatus? status = null);
        Task<AppointmentDTO?> GetByIdAsync(int id);
        Task<AppointmentDTO> CreateAsync(CreateAppointmentDTO dto);
        Task<bool> UpdateStatusAsync(int id, AppointmentStatus status);
        Task<List<AvailableDoctorDTO>> GetAvailableDoctorsAsync(int serviceId, DateTime date);
        Task<List<AvailableTimeDTO>> GetAvailableTimesAsync(int doctorId, DateTime date);
    }
}
```

### AppointmentService.cs

```csharp
using APIClinica.DTOs;
using APIClinica.Models;
using APIClinica.Models.Enums;
using APIClinica.Repositories;
using APIClinica.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;

namespace APIClinica.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IRepository<Service> _serviceRepository;
        private readonly ClinicaDbContext _context;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IDoctorRepository doctorRepository,
            IRepository<Service> serviceRepository,
            ClinicaDbContext context)
        {
            _appointmentRepository = appointmentRepository;
            _doctorRepository = doctorRepository;
            _serviceRepository = serviceRepository;
            _context = context;
        }

        public async Task<List<AppointmentDTO>> GetAllAsync(DateTime? date = null, int? doctorId = null, int? patientId = null, AppointmentStatus? status = null)
        {
            IEnumerable<Appointment> appointments;

            if (date.HasValue)
                appointments = await _appointmentRepository.GetAppointmentsByDateAsync(date.Value);
            else if (doctorId.HasValue)
                appointments = await _appointmentRepository.GetAppointmentsByDoctorAsync(doctorId.Value);
            else if (patientId.HasValue)
                appointments = await _appointmentRepository.GetAppointmentsByPatientAsync(patientId.Value);
            else if (status.HasValue)
                appointments = await _appointmentRepository.GetAppointmentsByStatusAsync(status.Value);
            else
                appointments = await _appointmentRepository.GetAppointmentsWithDetailsAsync();

            // Apply additional filters
            if (date.HasValue && appointments.Any())
                appointments = appointments.Where(a => a.Date.Date == date.Value.Date);

            if (doctorId.HasValue && appointments.Any())
                appointments = appointments.Where(a => a.DoctorId == doctorId.Value);

            if (patientId.HasValue && appointments.Any())
                appointments = appointments.Where(a => a.PatientId == patientId.Value);

            if (status.HasValue && appointments.Any())
                appointments = appointments.Where(a => a.Status == status.Value);

            return appointments
                .OrderBy(a => a.Date)
                .ThenBy(a => a.Time)
                .Select(a => new AppointmentDTO
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = $"{a.Patient.FirstName} {a.Patient.LastName}",
                    DoctorId = a.DoctorId,
                    DoctorName = $"{a.Doctor.FirstName} {a.Doctor.LastName}",
                    Date = a.Date,
                    Time = a.Time,
                    Status = a.Status,
                    ServiceName = a.Doctor.DoctorServices
                        .Select(ds => ds.Service.Name)
                        .FirstOrDefault() ?? string.Empty
                })
                .ToList();
        }

        public async Task<AppointmentDTO?> GetByIdAsync(int id)
        {
            var appointment = await _appointmentRepository.GetAppointmentWithDetailsByIdAsync(id);
            if (appointment == null)
                return null;

            return new AppointmentDTO
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                PatientName = $"{appointment.Patient.FirstName} {appointment.Patient.LastName}",
                DoctorId = appointment.DoctorId,
                DoctorName = $"{appointment.Doctor.FirstName} {appointment.Doctor.LastName}",
                Date = appointment.Date,
                Time = appointment.Time,
                Status = appointment.Status,
                ServiceName = appointment.Doctor.DoctorServices
                    .Select(ds => ds.Service.Name)
                    .FirstOrDefault() ?? string.Empty
            };
        }

        public async Task<AppointmentDTO> CreateAsync(CreateAppointmentDTO dto)
        {
            // Validate doctor exists and is active
            var doctor = await _doctorRepository.GetDoctorWithServicesByIdAsync(dto.DoctorId);
            if (doctor == null || !doctor.Active)
                throw new InvalidOperationException("Doctor not found or inactive");

            // Validate time
            var validTimes = new[] { "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00" };
            if (!validTimes.Contains(dto.Time))
                throw new ArgumentException("Invalid time. Must be between 08:00 and 17:00");

            // Validate date is not in the past
            if (dto.Date.Date < DateTime.Today)
                throw new ArgumentException("Cannot create appointments in the past");

            // Use transaction with REPEATABLE READ isolation level to prevent race conditions
            // This ensures that the count and insert happen atomically
            using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.RepeatableRead);
            try
            {
                // Check availability within the transaction
                var existingAppointments = await _appointmentRepository
                    .CountAppointmentsByDoctorAndTimeAsync(dto.DoctorId, dto.Date, dto.Time);

                if (existingAppointments >= 2)
                {
                    await transaction.RollbackAsync();
                    throw new InvalidOperationException("No available slots at this time");
                }

                var appointment = new Appointment
                {
                    PatientId = dto.PatientId,
                    DoctorId = dto.DoctorId,
                    Date = dto.Date.Date,
                    Time = dto.Time,
                    Status = AppointmentStatus.Scheduled
                };

                await _appointmentRepository.AddAsync(appointment);
                await _appointmentRepository.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetByIdAsync(appointment.Id) ?? throw new Exception("Error creating appointment");
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                
                // Re-throw database exceptions (la validación de cupos se hace antes de guardar)
                throw;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateStatusAsync(int id, AppointmentStatus status)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
                return false;

            appointment.Status = status;
            _appointmentRepository.Update(appointment);
            await _appointmentRepository.SaveChangesAsync();
            return true;
        }

        public async Task<List<AvailableDoctorDTO>> GetAvailableDoctorsAsync(int serviceId, DateTime date)
        {
            // Get doctors that provide the service
            var doctors = await _doctorRepository.GetDoctorsByServiceAsync(serviceId);

            var availableDoctors = new List<AvailableDoctorDTO>();

            foreach (var doctor in doctors)
            {
                var appointmentsOfDay = await _appointmentRepository
                    .FindAsync(a => a.DoctorId == doctor.Id &&
                                   a.Date.Date == date.Date &&
                                   a.Status == AppointmentStatus.Scheduled);

                var appointmentsByTime = appointmentsOfDay
                    .GroupBy(a => a.Time)
                    .Select(g => new { Time = g.Key, Count = g.Count() })
                    .ToList();

                // Check if there's at least one available hour
                var occupiedHours = appointmentsByTime.Where(a => a.Count >= 2).Count();
                var totalHours = 10; // 8:00 to 17:00

                if (occupiedHours < totalHours)
                {
                    availableDoctors.Add(new AvailableDoctorDTO
                    {
                        Id = doctor.Id,
                        FirstName = doctor.FirstName,
                        LastName = doctor.LastName,
                        Specialty = doctor.Specialty
                    });
                }
            }

            return availableDoctors;
        }

        public async Task<List<AvailableTimeDTO>> GetAvailableTimesAsync(int doctorId, DateTime date)
        {
            var times = new[] { "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00" };
            var availability = new List<AvailableTimeDTO>();

            var appointmentsOfDay = await _appointmentRepository
                .FindAsync(a => a.DoctorId == doctorId &&
                              a.Date.Date == date.Date &&
                              a.Status == AppointmentStatus.Scheduled);

            var appointmentsByTime = appointmentsOfDay
                .GroupBy(a => a.Time)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var time in times)
            {
                var occupiedSlots = appointmentsByTime.GetValueOrDefault(time, 0);
                var availableSlots = Math.Max(0, 2 - occupiedSlots);

                availability.Add(new AvailableTimeDTO
                {
                    Time = time,
                    AvailableSlots = availableSlots
                });
            }

            return availability;
        }
    }
}
```

---

## Controllers (Controladores)

### PatientsController.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using APIClinica.Services;
using APIClinica.DTOs;

namespace APIClinica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _service;

        public PatientsController(IPatientService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            var result = await _service.GetAllAsync(page, pageSize, search);
            return Ok(new
            {
                items = result.Items,
                currentPage = result.CurrentPage,
                totalPages = result.TotalPages,
                totalItems = result.TotalItems
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDTO>> GetById(int id)
        {
            var patient = await _service.GetByIdAsync(id);
            if (patient == null)
                return NotFound();
            return Ok(patient);
        }

        [HttpPost]
        public async Task<ActionResult<PatientDTO>> Create([FromBody] CreatePatientDTO dto)
        {
            try
            {
                var patient = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PatientDTO>> Update(int id, [FromBody] CreatePatientDTO dto)
        {
            try
            {
                var patient = await _service.UpdateAsync(id, dto);
                return Ok(patient);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
```

### InsurancesController.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using APIClinica.Repositories;
using APIClinica.Models;

namespace APIClinica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsurancesController : ControllerBase
    {
        private readonly IRepository<Insurance> _insuranceRepository;

        public InsurancesController(IRepository<Insurance> insuranceRepository)
        {
            _insuranceRepository = insuranceRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var insurances = await _insuranceRepository.FindAsync(i => i.Active);
            var result = insurances.Select(i => new { i.Id, i.Name, i.Description });
            return Ok(result);
        }
    }
}
```

### ServicesController.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using APIClinica.Repositories;
using APIClinica.Models;

namespace APIClinica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly IRepository<Service> _serviceRepository;

        public ServicesController(IRepository<Service> serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var services = await _serviceRepository.FindAsync(s => s.Active);
            var result = services.Select(s => new { s.Id, s.Name, s.Description });
            return Ok(result);
        }
    }
}
```

### DoctorsController.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using APIClinica.Services;

namespace APIClinica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public DoctorsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet("available")]
        public async Task<ActionResult> GetAvailable([FromQuery] int serviceId, [FromQuery] DateTime date)
        {
            var doctors = await _appointmentService.GetAvailableDoctorsAsync(serviceId, date);
            return Ok(doctors);
        }
    }
}
```

### AppointmentsController.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using APIClinica.Services;
using APIClinica.DTOs;
using APIClinica.Models.Enums;

namespace APIClinica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _service;

        public AppointmentsController(IAppointmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll(
            [FromQuery] DateTime? date = null,
            [FromQuery] int? doctorId = null,
            [FromQuery] int? patientId = null,
            [FromQuery] AppointmentStatus? status = null)
        {
            var appointments = await _service.GetAllAsync(date, doctorId, patientId, status);
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDTO>> GetById(int id)
        {
            var appointment = await _service.GetByIdAsync(id);
            if (appointment == null)
                return NotFound();
            return Ok(appointment);
        }

        [HttpPost]
        public async Task<ActionResult<AppointmentDTO>> Create([FromBody] CreateAppointmentDTO dto)
        {
            try
            {
                var appointment = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDTO dto)
        {
            var result = await _service.UpdateStatusAsync(id, dto.Status);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpGet("availability")]
        public async Task<ActionResult> GetAvailability([FromQuery] int doctorId, [FromQuery] DateTime date)
        {
            var availability = await _service.GetAvailableTimesAsync(doctorId, date);
            return Ok(availability);
        }
    }

    public class UpdateStatusDTO
    {
        public AppointmentStatus Status { get; set; }
    }
}
```

---

## Stored Procedures

### sp_DoctorAvailability

```sql
USE ClinicaDB;
GO

CREATE PROCEDURE sp_DoctorAvailability
    @DoctorId INT,
    @Date DATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Generate the hourly range (08:00 to 17:00)
    WITH Hours AS (
        SELECT '08:00' AS Hour
        UNION ALL SELECT '09:00'
        UNION ALL SELECT '10:00'
        UNION ALL SELECT '11:00'
        UNION ALL SELECT '12:00'
        UNION ALL SELECT '13:00'
        UNION ALL SELECT '14:00'
        UNION ALL SELECT '15:00'
        UNION ALL SELECT '16:00'
        UNION ALL SELECT '17:00'
    ),
    AppointmentsPerHour AS (
        SELECT 
            Time AS Hour,
            COUNT(*) AS OccupiedSlots
        FROM Appointments
        WHERE DoctorId = @DoctorId
          AND Date = @Date
          AND Status = 0  -- Scheduled
        GROUP BY Time
    )
    SELECT 
        h.Hour,
        ISNULL(2 - a.OccupiedSlots, 2) AS AvailableSlots
    FROM Hours h
    LEFT JOIN AppointmentsPerHour a ON h.Hour = a.Hour
    ORDER BY h.Hour;
END
GO
```

### Health Endpoint

Agregar en `Program.cs` (revisa estado real de la conexión a la BD):

```csharp
    app.MapGet("/health", async (ClinicaDbContext db) =>
    {
        try
        {
            // Consulta mínima para validar conexión
            await db.Database.ExecuteSqlRawAsync("SELECT 1");

            return Results.Ok(new
            {
                status = "healthy",
                db = "ok",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            // Si la conexión falla, responde 503
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }
    })
    .WithName("Health")
    .WithTags("Health");
```


---

## Seed Data (Semillas)

### DbInitializer.cs

```csharp
using APIClinica.Data;
using APIClinica.Models;

namespace APIClinica.Data
{
    public static class DbInitializer
    {
        public static void SeedPatients(ClinicaDbContext context)
        {
            if (context.Patients.Any())
                return;

            var patients = new List<Patient>
            {
                new Patient { FirstName = "Pedro", LastName = "López", Document = "12345678", Phone = "555-0001", Email = "pedro@email.com", InsuranceId = 1 },
                new Patient { FirstName = "Laura", LastName = "Sánchez", Document = "23456789", Phone = "555-0002", Email = "laura@email.com", InsuranceId = 2 },
                new Patient { FirstName = "Roberto", LastName = "García", Document = "34567890", Phone = "555-0003", Email = "roberto@email.com", InsuranceId = 1 },
                new Patient { FirstName = "Carmen", LastName = "Fernández", Document = "45678901", Phone = "555-0004", Email = "carmen@email.com", InsuranceId = 3 },
                new Patient { FirstName = "Diego", LastName = "Martínez", Document = "56789012", Phone = "555-0005", Email = "diego@email.com", InsuranceId = 2 },
                new Patient { FirstName = "Sofía", LastName = "Ruiz", Document = "67890123", Phone = "555-0006", Email = "sofia@email.com", InsuranceId = 1 },
                new Patient { FirstName = "Miguel", LastName = "Torres", Document = "78901234", Phone = "555-0007", Email = "miguel@email.com", InsuranceId = 3 },
                new Patient { FirstName = "Elena", LastName = "Díaz", Document = "89012345", Phone = "555-0008", Email = "elena@email.com", InsuranceId = 2 },
                new Patient { FirstName = "Javier", LastName = "Moreno", Document = "90123456", Phone = "555-0009", Email = "javier@email.com", InsuranceId = 1 },
                new Patient { FirstName = "Isabel", LastName = "Jiménez", Document = "01234567", Phone = "555-0010", Email = "isabel@email.com", InsuranceId = 3 }
            };

            context.Patients.AddRange(patients);
            context.SaveChanges();
        }
    }
}
```

---

## Configuración Final

### APIClinica.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
    <PackageReference Include="AspNetCore.HealthChecks.SqlServer" Version="8.0.1" />
  </ItemGroup>

</Project>
```

---

## Pasos para Ejecutar

1. **Crear Base de Datos:**
   ```sql
   CREATE DATABASE ClinicaDB;
   ```

2. **Configurar Connection String** en `appsettings.json`

3. **Ejecutar Migraciones** (si usas migraciones) o usar `EnsureCreated()`:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

4. **Ejecutar Stored Procedure:**
   ```sql
   -- Ejecutar el script de sp_DisponibilidadMedico
   ```

5. **Ejecutar CHECK Constraint:**
   ```sql
   -- Ejecutar el script de CK_Appointments_Time_Valid
   ```

6. **Ejecutar la aplicación:**
   ```bash
   dotnet run
   ```

---

## Endpoints

- `GET /health` - Health check (sistema integrado de ASP.NET Core)
- `GET /health/detailed` - Health check detallado (verifica conexión real a BD)
- `GET /api/patients` - List patients (paginated)
- `GET /api/patients/{id}` - Get patient
- `POST /api/patients` - Create patient
- `PUT /api/patients/{id}` - Update patient
- `DELETE /api/patients/{id}` - Delete patient
- `GET /api/insurances` - List insurances
- `GET /api/services` - List services
- `GET /api/doctors/available?serviceId={id}&date={date}` - Available doctors
- `GET /api/appointments` - List appointments (with filters)
- `GET /api/appointments/{id}` - Get appointment
- `POST /api/appointments` - Create appointment
- `PATCH /api/appointments/{id}/status` - Update status
- `GET /api/appointments/availability?doctorId={id}&date={date}` - Available times

---

## Notas Importantes

1. **Repository Pattern (Patrón de Repositorio Genérico):** 
   - Se implementa un repositorio genérico (`IRepository<T>`) que proporciona operaciones CRUD básicas
   - **Separación de operaciones:** Los métodos `Add`, `Update`, `Remove` solo marcan las entidades para cambios, no guardan automáticamente
   - **Control de transacciones:** Se debe llamar explícitamente a `SaveChangesAsync()` después de las operaciones, permitiendo múltiples operaciones en una sola transacción
   - **Flexibilidad:** El método `GetQueryable()` permite construir consultas complejas usando LINQ
   - Los repositorios específicos extienden el repositorio genérico para agregar métodos personalizados
   - Los servicios utilizan repositorios en lugar de acceder directamente al `DbContext`
   - Esto mejora la separación de responsabilidades y facilita las pruebas unitarias

2. **Dependency Injection (Inyección de Dependencias):**
   - Todos los repositorios y servicios se registran en `Program.cs` usando `AddScoped`
   - Los controladores reciben las dependencias a través del constructor
   - El repositorio genérico se registra usando `AddScoped(typeof(IRepository<>), typeof(Repository<>))`

3. **Control de Cupos:** La validación de máximo 2 cupos por hora se realiza en la aplicación mediante transacciones con nivel de aislamiento `REPEATABLE READ`. Se usa un índice NO único (`IX_Appointments_SlotPerHour`) para optimizar las consultas de disponibilidad.

4. **Manejo de Concurrencia:** El servicio de citas usa transacciones con nivel de aislamiento `REPEATABLE READ` para prevenir condiciones de carrera. Esto asegura que el conteo de citas y la inserción se ejecuten de forma atómica, evitando que dos usuarios reserven el último cupo simultáneamente.

5. **Validaciones:** 
   - Hora entre 08:00 y 17:00 (Data Annotation + CHECK constraint)
   - El médico debe prestar el servicio seleccionado
   - No se pueden crear citas en fechas pasadas

6. **CORS:** Configurado para permitir requests desde `http://localhost:3000`

---

## Problema de Concurrencia en Creación de Citas - SOLUCIONADO

### El Problema

Cuando dos usuarios intentaban crear una cita para la misma hora al mismo tiempo, ocurría una **condición de carrera (race condition)**:

1. **Usuario A** consulta disponibilidad → Ve 1 cupo disponible (hay 1 cita existente)
2. **Usuario B** consulta disponibilidad al mismo tiempo → También ve 1 cupo disponible
3. **Usuario A** crea la cita → Ahora hay 2 citas (cupo lleno)
4. **Usuario B** intenta crear la cita → El sistema rechaza porque ya hay 2 citas, pero el error no era claro

El problema era que entre el **conteo de citas** y la **inserción**, otra transacción podía insertar una cita, causando que:
- El conteo inicial mostraba 1 cupo disponible
- Pero al intentar guardar, otra transacción ya había ocupado el segundo cupo
- Esto causaba inconsistencias en la disponibilidad mostrada

### La Solución

Se implementó una **transacción con nivel de aislamiento `REPEATABLE READ`** que:

1. **Asegura atomicidad**: El conteo y la inserción se ejecutan dentro de la misma transacción
2. **Previene lecturas sucias**: Otros usuarios no pueden leer datos modificados hasta que la transacción termine
3. **Manejo mejorado de errores**: Captura específicamente los errores de violación de índice único (códigos SQL 2601 y 2627)
4. **Mensajes claros**: Informa al usuario que el cupo fue tomado por otro usuario

### Cambios Realizados

1. **Eliminación del índice único**: Se cambió de índice único a índice normal para permitir hasta 2 citas por hora
2. **Inyección de `ClinicaDbContext`** en `AppointmentService` para poder usar transacciones
3. **Uso de `BeginTransactionAsync`** con `IsolationLevel.RepeatableRead` para asegurar atomicidad
4. **Validación en aplicación**: El conteo y la inserción se hacen dentro de la misma transacción
5. **Rollback automático** en caso de error para mantener la consistencia

### Código Clave

```csharp
// Transacción con REPEATABLE READ previene condiciones de carrera
using var transaction = await _context.Database.BeginTransactionAsync(
    System.Data.IsolationLevel.RepeatableRead);

// El conteo y la inserción ocurren dentro de la misma transacción
var existingAppointments = await _appointmentRepository
    .CountAppointmentsByDoctorAndTimeAsync(dto.DoctorId, dto.Date, dto.Time);

if (existingAppointments >= 2)
{
    await transaction.RollbackAsync();
    throw new InvalidOperationException("No available slots at this time");
}

// Si llega aquí, hay cupo disponible y lo reservamos
await _appointmentRepository.AddAsync(appointment);
await _appointmentRepository.SaveChangesAsync();
await transaction.CommitAsync();
```

---

## Solución de Problemas Comunes

### Error: "Unable to resolve service for type 'APIClinica.Services.IPatientService'"

Este error ocurre cuando un servicio no está registrado en el contenedor de Dependency Injection. Para solucionarlo:

1. **Verifica que el servicio esté registrado en `Program.cs`:**
   
   Asegúrate de que tu `Program.cs` tenga exactamente estas líneas en este orden:
   
   ```csharp
   // Repositories - Generic Repository Pattern (debe ir primero)
   builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
   
   // Specific Repositories (deben ir antes de los servicios que los usan)
   builder.Services.AddScoped<IPatientRepository, PatientRepository>();
   builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
   builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
   
   // Services (deben ir después de los repositorios que dependen)
   builder.Services.AddScoped<IPatientService, PatientService>();
   builder.Services.AddScoped<IAppointmentService, AppointmentService>();
   ```

2. **Verifica que todos los using statements estén presentes en `Program.cs`:**
   ```csharp
   using Microsoft.EntityFrameworkCore;
   using APIClinica.Data;
   using APIClinica.Services;
   using APIClinica.Repositories;
   using APIClinica.Models;
   ```

3. **Verifica que todas las dependencias del servicio estén registradas:**
   - `PatientService` requiere `IPatientRepository` y `IRepository<Insurance>`
   - `IRepository<Insurance>` se registra automáticamente con `AddScoped(typeof(IRepository<>), typeof(Repository<>))`
   - `IPatientRepository` debe estar registrado explícitamente

4. **Verifica los namespaces en los archivos de servicio:**
   - `IPatientService.cs` debe tener: `namespace APIClinica.Services;`
   - `PatientService.cs` debe tener: `namespace APIClinica.Services;`
   - `IPatientRepository.cs` debe tener: `namespace APIClinica.Repositories;`
   - `PatientRepository.cs` debe tener: `namespace APIClinica.Repositories;`

5. **Verifica que los archivos existan y compilen correctamente:**
   ```bash
   dotnet build
   ```
   
   Si hay errores de compilación, corrígelos primero. Los errores comunes incluyen:
   - Namespaces incorrectos
   - Usings faltantes
   - Nombres de clases/propiedades incorrectos (verifica que uses los nombres en inglés: `Patient`, `Insurance`, etc.)

6. **Si el problema persiste después de verificar todo lo anterior:**
   
   a. **Limpia y reconstruye el proyecto:**
   ```bash
   dotnet clean
   dotnet build
   ```
   
   b. **Verifica que no haya múltiples definiciones del mismo servicio:**
   - Busca en tu `Program.cs` si hay registros duplicados o conflictivos
   
   c. **Verifica que el proyecto esté usando .NET 8:**
   - Revisa el archivo `.csproj` y asegúrate de que tenga `<TargetFramework>net8.0</TargetFramework>`
   
   d. **Reinicia el servidor de desarrollo:**
   - Detén la aplicación completamente
   - Vuelve a ejecutar `dotnet run`

7. **Verificación rápida - Código completo de Program.cs:**
   
   Asegúrate de que tu `Program.cs` tenga esta estructura completa:
   
   ```csharp
   using Microsoft.EntityFrameworkCore;
   using APIClinica.Data;
   using APIClinica.Services;
   using APIClinica.Repositories;
   using APIClinica.Models;
   using Microsoft.Data.SqlClient;
   using System.Data;
   
   var builder = WebApplication.CreateBuilder(args);
   
   builder.Services.AddControllers();
   builder.Services.AddEndpointsApiExplorer();
   builder.Services.AddSwaggerGen();
   
   // Database
   builder.Services.AddDbContext<ClinicaDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
   
   // Repositories - Generic Repository Pattern (debe ir primero)
   builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
   
   // Specific Repositories
   builder.Services.AddScoped<IPatientRepository, PatientRepository>();
   builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
   builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
   
   // Services
   builder.Services.AddScoped<IPatientService, PatientService>();
   builder.Services.AddScoped<IAppointmentService, AppointmentService>();
   
   // ... resto del código
   ```

---

¡Listo! Con esta guía puedes construir el backend completo desde cero.

