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

        public ICollection<Patient> Patients { get; set; } = new List<Patient>();
    }
}