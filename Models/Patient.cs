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

        [ForeignKey(nameof(InsuranceId))]
        public Insurance Insurance { get; set; } = null!;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}