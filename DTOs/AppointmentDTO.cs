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