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