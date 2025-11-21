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

        public ICollection<DoctorService> DoctorServices { get; set; } = new List<DoctorService>();
    }
}