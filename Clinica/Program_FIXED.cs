using APIClinica.Data;
using APIClinica.Services;
using APIClinica.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

