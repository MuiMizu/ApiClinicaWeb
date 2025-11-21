using System.ComponentModel.DataAnnotations;

namespace APIClinica.Models
{
    public class Seguro
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;
        [StringLength(500)]
        public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;
        public ICollection<Paciente> Pacientes { get; set; } = new List<Paciente>();
    }
}