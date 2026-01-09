# Resumen del Sistema de Clínica

## Ejemplos de Uso - Flujo por Capas

### Ejemplo 1: Crear un Paciente

**Request:** `POST /api/patients` con datos del paciente

```
Controller (PatientsController)
  ↓ Recibe el request HTTP
  ↓ Valida formato básico
  ↓ Llama a PatientService.CreateAsync()
  
Service (PatientService)
  ↓ Valida que el seguro exista y esté activo
  ↓ Crea objeto Patient con los datos
  ↓ Llama a PatientRepository.AddAsync()
  
Repository (PatientRepository)
  ↓ Agrega el paciente al contexto de EF
  ↓ Llama a SaveChangesAsync()
  
Database (SQL Server)
  ↓ Ejecuta INSERT INTO Patients
  ↓ Valida constraint de Document único
  ↓ Retorna el ID generado
  
  ↑ Retorna PatientDTO con datos completos
  ↑ Controller devuelve HTTP 201 Created
```

---

### Ejemplo 2: Crear una Cita

**Request:** `POST /api/appointments` con doctorId, patientId, date, time

```
Controller (AppointmentsController)
  ↓ Recibe el request HTTP
  ↓ Llama a AppointmentService.CreateAsync()
  
Service (AppointmentService)
  ↓ Valida que el médico exista y esté activo
  ↓ Valida que la hora esté entre 08:00-17:00
  ↓ Valida que la fecha no sea pasada
  ↓ INICIA TRANSACCIÓN (REPEATABLE READ)
  ↓ Cuenta citas existentes en esa hora
  ↓ Si hay < 2 cupos:
     → Crea objeto Appointment
     → Llama a AppointmentRepository.AddAsync()
     → Guarda cambios
     → CONFIRMA TRANSACCIÓN
  ↓ Si hay >= 2 cupos:
     → CANCELA TRANSACCIÓN
     → Lanza excepción "No hay cupos"
  
Repository (AppointmentRepository)
  ↓ Agrega la cita al contexto
  ↓ SaveChangesAsync() ejecuta el INSERT
  
Database (SQL Server)
  ↓ Ejecuta INSERT INTO Appointments
  ↓ Valida constraints (hora válida, etc.)
  ↓ Retorna el ID generado
  
  ↑ Retorna AppointmentDTO
  ↑ Controller devuelve HTTP 201 Created
  ↑ O HTTP 409 Conflict si no hay cupos
```

---

### Ejemplo 3: Consultar Disponibilidad de Médicos

**Request:** `GET /api/doctors/available?serviceId=2&date=2024-01-15`

```
Controller (DoctorsController)
  ↓ Recibe parámetros de query
  ↓ Llama a AppointmentService.GetAvailableDoctorsAsync()
  
Service (AppointmentService)
  ↓ Llama a DoctorRepository.GetDoctorsByServiceAsync()
  ↓ Para cada médico:
     → Cuenta citas del día con AppointmentRepository
     → Calcula horas ocupadas vs horas totales (10 horas)
     → Si tiene al menos 1 hora disponible → lo agrega a la lista
  
Repository (DoctorRepository)
  ↓ Busca médicos que ofrecen el servicio
  ↓ Incluye relaciones DoctorService
  
Repository (AppointmentRepository)
  ↓ Cuenta citas por médico y fecha
  ↓ Filtra solo citas programadas (Status = Scheduled)
  
Database (SQL Server)
  ↓ Ejecuta JOIN entre Doctors, DoctorServices, Services
  ↓ Ejecuta COUNT en Appointments con filtros
  ↓ Retorna resultados
  
  ↑ Retorna lista de AvailableDoctorDTO
  ↑ Controller devuelve HTTP 200 OK con JSON
```

---

## Arquitectura en Capas

```
┌─────────────┐
│  Controller │  → Recibe requests HTTP, valida formato, devuelve respuestas
└──────┬──────┘
       │
┌──────▼──────┐
│   Service   │  → Lógica de negocio, validaciones, transacciones
└──────┬──────┘
       │
┌──────▼──────┐
│  Repository │  → Acceso a datos, consultas a base de datos
└──────┬──────┘
       │
┌──────▼──────┐
│  Database   │  → Almacenamiento persistente (SQL Server)
└─────────────┘
```

**Responsabilidades:**
- **Controller**: HTTP, códigos de estado, formato de respuestas
- **Service**: Reglas de negocio, validaciones complejas, orquestación
- **Repository**: Consultas SQL, operaciones CRUD, abstracción de datos
- **Database**: Persistencia, constraints, índices, integridad

---

## Glosario

- **Repository**: Clase que maneja el acceso a datos
- **Service**: Clase con lógica de negocio
- **DTO**: Objeto para transferir datos (sin lógica)
- **Entity**: Clase que representa una tabla de BD
- **DbContext**: Puente entre código C# y base de datos
- **Migration**: Script para actualizar estructura de BD
- **Seed**: Datos iniciales de prueba
- **Transaction**: Grupo de operaciones que se ejecutan juntas
- **Async/Await**: Operaciones asincrónicas (no bloquean)
- **LINQ**: Lenguaje de consultas integrado en C#
- **Dependency Injection**: Patrón para gestionar dependencias
- **Race Condition**: Error cuando 2+ usuarios modifican lo mismo simultáneamente

┌─────────────┐
│   CLIENT    │  POST /api/patients + JSON
└──────┬──────┘
       │
       ↓
┌─────────────┐
│ CONTROLLER  │  Recibe CreatePatientDTO → valida → llama Service
└──────┬──────┘
       │
       ↓
┌─────────────┐
│  SERVICE    │  Valida seguro → crea Patient → llama Repository
└──────┬──────┘
       │
       ↓
┌─────────────┐
│ REPOSITORY  │  AddAsync() → SaveChangesAsync()
└──────┬──────┘
       │
       ↓
┌─────────────┐
│  DbContext  │  INSERT INTO Patients...
└──────┬──────┘
       │
       ↓
┌─────────────┐
│  DATABASE   │  Guarda registro → retorna ID
└──────┬──────┘
       │
       ↑ (Retorno)
       │
┌──────┴──────┐
│  DbContext  │  Recibe ID generado
└──────┬──────┘
       │
       ↓
┌─────────────┐
│ REPOSITORY  │  Retorna Patient con ID
└──────┬──────┘
       │
       ↓
┌─────────────┐
│  SERVICE    │  Convierte Patient → PatientDTO (incluye nombre del seguro)
└──────┬──────┘
       │
       ↓
┌─────────────┐
│ CONTROLLER  │  Retorna 201 Created + PatientDTO en JSON
└──────┬──────┘
       │
       ↓
┌─────────────┐
│   CLIENT    │  Recibe: { id: 1, firstName: "Pedro", insuranceName: "Seguro A" }
└─────────────┘
