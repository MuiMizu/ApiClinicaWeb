using System.ComponentModel.DataAnnotations;

namespace APIClinica.Models
{
    public class Medico
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;
        [StringLength(50)]
        public string? Especialidad { get; set; }
        [StringLength(20)]
        public string? Telefono { get; set; }
        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }
        public bool Activo { get; set; } = true;

        public ICollection<MedicoServicio> MedicoServicios { get; set; } = new List<MedicoServicio>();
        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    }
}